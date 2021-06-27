import os
import cv2
import imutils
import numpy as np
import matplotlib.pyplot as plt
import pytesseract


class ImgProcess:
    def __init__(self, filepath):
        self.filepath = filepath
        self.filename = os.path.basename(filepath)
        self.filename_without_ext = os.path.splitext(self.filename)[0]
        self.img = None
        self.img_pre_processed = None
        data_folder_name = "imgs"
        self.img_pre_processed_filename = os.path.join(data_folder_name, "pre", f"{self.filename_without_ext}.png")
        self.img_region_dir = os.path.join(data_folder_name, "regions")
        self.img_region_list = []
        self.img_region_marked_dir = os.path.join(data_folder_name, "region_marked")

        self.img_table_lined_list = []
        self.img_table_lined_dir = os.path.join(data_folder_name, "table_line")

        self.img_item_dir = os.path.join(data_folder_name, "items")
        self.img_item_list = []
        self.img_region_item_marked_dir = os.path.join(data_folder_name, "item_marked")

    def load_source(self):
        self.img = cv2.imread(self.filepath)

    def pre_process(self, thresh_mode):
        # get rid of the color
        pre = cv2.cvtColor(self.img, cv2.COLOR_BGR2GRAY)
        # Otsu threshold
        pre = cv2.threshold(pre, 100, 255, thresh_mode | cv2.THRESH_OTSU)[1]
        pre = ~pre
        cv2.imwrite(self.img_pre_processed_filename, pre)
        self.img_pre_processed = pre

    def get_regions(self, save_in_file=False):
        self.img_region_list.clear()
        low_threshold = 50
        high_threshold = 150
        edges = cv2.Canny(self.img_pre_processed, low_threshold, high_threshold)
        img_region_marked = np.copy(self.img)
        img_region_marked_view = np.copy(self.img)
        contours, hierarchy = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        hull_list = []
        min_region = self.img.size // 100
        for cnt in range(len(contours)):
            epsilon = 0.01 * cv2.arcLength(contours[cnt], True)
            approx = cv2.approxPolyDP(contours[cnt], epsilon, True)
            corners = len(approx)
            if corners in range(4, 10):
                area = cv2.contourArea(contours[cnt])
                if area > min_region:
                    hull = cv2.convexHull(contours[cnt])
                    hull_list.append(hull)

        for cnt in range(len(hull_list)):
            cv2.drawContours(img_region_marked, hull_list, cnt, (0, 0, 0), 5)
            cv2.drawContours(img_region_marked_view, hull_list, cnt, (0, 0, 255), 10)
            x, y, w, h = cv2.boundingRect(hull_list[cnt])
            region = img_region_marked[y:y + h, x:x + w]
            self.img_region_list.append(np.copy(region))

        if save_in_file:
            # save regions file
            for i in range(len(self.img_region_list)):
                cv2.imwrite(os.path.join(self.img_region_dir, f"{self.filename_without_ext}_{i}.png"),
                            self.img_region_list[i])
            # save region marked file
            path = os.path.join(self.img_region_marked_dir, f"{self.filename_without_ext}.png")
            cv2.imwrite(path, img_region_marked_view)
        plt.imshow(img_region_marked_view, cmap='gray')
        plt.show()
        return len(self.img_region_list)

    def get_table(self):
        def pre_process_region(region):
            # get rid of the color
            pre = cv2.cvtColor(region, cv2.COLOR_BGR2GRAY)
            # Otsu threshold
            pre = cv2.threshold(pre, 50, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)[1]
            pre = ~pre
            return pre

        for img_region in self.img_region_list:
            pre = pre_process_region(img_region)
            ver_kernel_len = np.array(img_region).shape[0] // 30
            hor_kernel_len = np.array(img_region).shape[1] // 70
            # Defining a vertical kernel to detect all vertical lines of image
            ver_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1, ver_kernel_len))
            # Defining a horizontal kernel to detect all horizontal lines of image
            hor_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (hor_kernel_len, 1))
            # A kernel of 2x2
            kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (2, 2))

            # Use vertical kernel to detect and save the vertical lines in a jpg
            image_1 = cv2.erode(pre, ver_kernel, iterations=3)
            vertical_lines = cv2.dilate(image_1, ver_kernel, iterations=3)

            # Use horizontal kernel to detect and save the horizontal lines in a jpg
            image_2 = cv2.erode(pre, hor_kernel, iterations=3)
            horizontal_lines = cv2.dilate(image_2, hor_kernel, iterations=3)

            # Combine horizontal and vertical lines in a new third image, with both having same weight.
            img_vh = cv2.addWeighted(vertical_lines, 0.5, horizontal_lines, 0.5, 0.0)
            # Eroding and thesholding the image
            img_vh = cv2.erode(~img_vh, kernel, iterations=2)
            thresh, img_vh = cv2.threshold(img_vh, 128, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

            # connect broken line
            kernel = np.ones((1, 5), np.uint8)
            erosion = cv2.erode(img_vh, kernel, iterations=10)
            dilate = cv2.dilate(erosion, kernel, iterations=10)

            kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (4, 1))
            morph_img = cv2.morphologyEx(dilate, cv2.MORPH_CLOSE, kernel)

            self.img_table_lined_list.append(morph_img)
            plt.imshow(dilate, cmap='gray')
            plt.show()

        # save table line file
        for i in range(len(self.img_table_lined_list)):
            cv2.imwrite(os.path.join(self.img_table_lined_dir, f"{self.filename_without_ext}_{i}.png"),
                        self.img_table_lined_list[i])

    def crop_by_table_line(self):
        for table_img_index in range(len(self.img_table_lined_list)):
            img_table_lined = self.img_table_lined_list[table_img_index]
            low_threshold = 100
            high_threshold = 255
            img_region_marked = np.copy(self.img_region_list[table_img_index])
            edges = cv2.Canny(~img_table_lined, low_threshold, high_threshold)

            contours, hierarchy = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            for cnt in range(len(contours)):
                # 轮廓逼近
                epsilon = 0.01 * cv2.arcLength(contours[cnt], True)
                approx = cv2.approxPolyDP(contours[cnt], epsilon, True)
                corners = len(approx)
                # if corners in range(4, 10):
                ar = cv2.contourArea(contours[cnt])
                if ar > (img_table_lined.size // 100):
                    cv2.drawContours(img_region_marked, contours, cnt, (255, 0, 0), 5)
                    hull = cv2.convexHull(contours[cnt])

                    x, y, w, h = cv2.boundingRect(hull)
                    c = self.img_region_list[table_img_index][y:y + h, x:x + w]
                    self.img_item_list.append(c)

            self.img_item_list.reverse()
            path = os.path.join(self.img_region_item_marked_dir,
                                f"{self.filename_without_ext}_{table_img_index}.png")
            plt.imshow(img_region_marked, cmap='gray')
            plt.show()
            cv2.imwrite(path, img_region_marked)

        for index in range(len(self.img_item_list)):
            item = self.img_item_list[index]
            path = os.path.join(self.img_item_dir, f"{self.filename_without_ext}_{index}.png")
            cv2.imwrite(path, item)

    def ocr(self):
        s = ""
        # item_test_list = []
        # for i in range(len(self.img_item_list)):
        #     item = self.img_item_list[i]
        #     out = pytesseract.image_to_string(item)
        #     out_t = out.strip()
        #     if len(out_t) != 0:
        #         item_test_list.append(out_t)
        #         s = s + f"\nitem {i}:\n" + out_t
        print(f"{self.filename_without_ext}============")
        print(s)
        print("============")


if __name__ == "__main__":
    source_dir = os.path.join("imgs", "source")
    dir_list = os.listdir(source_dir)

    thresh_modes = [cv2.THRESH_BINARY_INV, cv2.THRESH_BINARY, cv2.THRESH_TRUNC]
    for cur_file in dir_list:
        source_file = os.path.join(source_dir, cur_file)
        in_file = source_file
        img = ImgProcess(source_file)
        img.load_source()
        region_count = 0
        max_index = -1
        max_region_count = 0
        for thresh_mode_index in range(len(thresh_modes)):
            thresh_mode = thresh_modes[thresh_mode_index]
            img.pre_process(thresh_mode)
            region_count = img.get_regions()
            if region_count > max_region_count:
                max_index = thresh_mode_index
                max_region_count = region_count
        if max_region_count == 0:
            continue
        img.pre_process(thresh_modes[max_index])
        img.get_regions(True)
        img.get_table()
        img.crop_by_table_line()
        img.ocr()
        print(f"Done {cur_file}")
