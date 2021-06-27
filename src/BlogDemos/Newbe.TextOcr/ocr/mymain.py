import os
import cv2
import imutils
import numpy as np
import matplotlib.pyplot as plt


def pre_process_image(img, save_in_file=None, morph_size=(8, 8)):
    # get rid of the color
    pre = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    # Otsu threshold
    pre = cv2.threshold(pre, 250, 255, cv2.THRESH_TOZERO | cv2.THRESH_OTSU)[1]
    # pre = cv2.threshold(pre, 250, 255, cv2.THRESH_TRUNC | cv2.THRESH_OTSU)[1]
    # pre = cv2.threshold(pre, 250, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)[1]
    pre = ~pre

    if save_in_file is not None:
        cv2.imwrite(save_in_file, pre)
    return pre


def pre_process_image_crop(img, save_in_file=None, morph_size=(8, 8)):
    # get rid of the color
    pre = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    # Otsu threshold
    pre = cv2.threshold(pre, 100, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)[1]
    # # dilate the text to make it solid spot
    # cpy = pre.copy()
    # struct = cv2.getStructuringElement(cv2.MORPH_RECT, morph_size)
    # cpy = cv2.dilate(~cpy, struct, anchor=(-1, -1), iterations=1)
    pre = ~pre

    if save_in_file is not None:
        cv2.imwrite(save_in_file, pre)
    return pre


def crop_range(source_img, source_file_name, pre_processed_img, mark_in_file, crop_dir):
    low_threshold = 50
    high_threshold = 150
    edges = cv2.Canny(pre_processed_img, low_threshold, high_threshold)
    result = np.copy(source_img)
    contours, hierarchy = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    crop_index = 0
    copped_images = []
    hull_list = []
    for cnt in range(len(contours)):

        # 轮廓逼近
        epsilon = 0.01 * cv2.arcLength(contours[cnt], True)
        approx = cv2.approxPolyDP(contours[cnt], epsilon, True)
        corners = len(approx)
        if corners == 4:
            area = cv2.contourArea(contours[cnt])
            if area > 10000:
                # 提取与绘制轮廓
                # cv2.drawContours(result, contours, cnt, (0, 0, 255), 10)
                # plt.imshow(result, cmap='gray')
                # plt.show()
                hull = cv2.convexHull(contours[cnt])
                hull_list.append(hull)
                cv2.drawContours(result, hull_list, len(hull_list) - 1, (0, 0, 0), 10)

                x, y, w, h = cv2.boundingRect(hull)
                cropped = result[y:y + h, x:x + w]
                cv2.imwrite(os.path.join(crop_dir, f"{source_file_name}_{crop_index}.png"), cropped)
                plt.imshow(cropped, cmap='gray')
                plt.show()
                copped_images.append(np.copy(cropped))
                crop_index += 1

    for cnt in range(len(hull_list)):
        cv2.drawContours(result, hull_list, cnt, (0, 0, 255), 10)
        plt.imshow(result, cmap='gray')
        plt.show()

    cv2.imwrite(mark_in_file, result)
    return copped_images


def get_vh(source_img):
    ver_kernel_len = np.array(source_img).shape[0] // 30
    hor_kernel_len = np.array(source_img).shape[1] // 100
    # Defining a vertical kernel to detect all vertical lines of image
    ver_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (1, ver_kernel_len))
    # Defining a horizontal kernel to detect all horizontal lines of image
    hor_kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (hor_kernel_len, 1))
    # A kernel of 2x2
    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (2, 2))

    # Use vertical kernel to detect and save the vertical lines in a jpg
    image_1 = cv2.erode(source_img, ver_kernel, iterations=3)
    vertical_lines = cv2.dilate(image_1, ver_kernel, iterations=3)

    # Use horizontal kernel to detect and save the horizontal lines in a jpg
    image_2 = cv2.erode(source_img, hor_kernel, iterations=3)
    horizontal_lines = cv2.dilate(image_2, hor_kernel, iterations=3)

    # Combine horizontal and vertical lines in a new third image, with both having same weight.
    img_vh = cv2.addWeighted(vertical_lines, 0.5, horizontal_lines, 0.5, 0.0)
    # Eroding and thesholding the image
    img_vh = cv2.erode(~img_vh, kernel, iterations=2)
    thresh, img_vh = cv2.threshold(img_vh, 128, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)
    return img_vh


if __name__ == "__main__":
    source_dir = os.path.join("data", "source")
    dir_list = os.listdir(source_dir)
    for cur_file in dir_list:
        source_file = os.path.join(source_dir, cur_file)
        in_file = source_file

        pre_file = os.path.join("data", "pre", cur_file)
        out_file = os.path.join("data", "out", cur_file)
        cropped_dir = os.path.join("data", "cropped_region")
        cropped_sub_dir = os.path.join("data", "cropped_sub")

        img = cv2.imread(os.path.join(in_file))

        pre_processed = pre_process_image(img, pre_file)
        copped_images = crop_range(img, cur_file, pre_processed, out_file, cropped_dir)

        cropped_index = 0
        for cropped in copped_images:
            cropped_pre_file = os.path.join("data", "cropped_pre", f"{cur_file}_{cropped_index}.png")
            pre_processed_cropped = pre_process_image_crop(cropped, cropped_pre_file)
            plt.imshow(pre_processed_cropped, cmap='gray')
            plt.show()
            cropped_index += 1

            img_vh = get_vh(pre_processed_cropped)
            cv2.imwrite("./img_vh.jpg", img_vh)
            plt.imshow(~img_vh, cmap='gray')
            plt.show()

            kernel = np.ones((1, 5), np.uint8)
            erosion = cv2.erode(img_vh, kernel, iterations=10)
            dilate = cv2.dilate(erosion, kernel, iterations=10)
            cv2.imwrite("./img_vh_dilate.jpg", dilate)
            plt.imshow(dilate, cmap='gray')
            plt.show()

            low_threshold = 100
            high_threshold = 255
            result = np.copy(cropped)
            edges = cv2.Canny(~dilate, low_threshold, high_threshold)

            contours, hierarchy = cv2.findContours(edges, cv2.RETR_CCOMP, cv2.CHAIN_APPROX_SIMPLE)
            index = 0
            for cnt in range(len(contours)):
                # 轮廓逼近
                epsilon = 0.01 * cv2.arcLength(contours[cnt], True)
                approx = cv2.approxPolyDP(contours[cnt], epsilon, True)
                corners = len(approx)
                # if corners == 4 or corners == 5 or corners == 6 or corners == 7 or corners == 8:
                ar = cv2.contourArea(contours[cnt])
                if ar > (pre_processed_cropped.size // 300):
                    cv2.drawContours(result, contours, cnt, (0, 0, 255), 5)
                    hull = cv2.convexHull(contours[cnt])

                    x, y, w, h = cv2.boundingRect(hull)
                    c = cropped[y:y + h, x:x + w]
                    cv2.imwrite(os.path.join(cropped_sub_dir, f"{cur_file}_{index}.png"), c)
                    index += 1
            cv2.imwrite("./img_vh_result.jpg", result)
            plt.imshow(result)
            plt.show()
