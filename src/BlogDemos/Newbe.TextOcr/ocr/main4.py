import os
import cv2
import imutils
import numpy as np
import matplotlib.pyplot as plt

# This only works if there's only one table on a page
# Important parameters:
#  - morph_size
#  - min_text_height_limit
#  - max_text_height_limit
#  - cell_threshold
#  - min_columns


if __name__ == "__main__":
    in_file = os.path.join("data", "data/9.png")
    # in_file = os.path.join("cv_inverted.png")
    # in_file = os.path.join("edges-50-150.jpg")

    img = cv2.imread(in_file)
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    kernel_size = 5
    blur_gray = cv2.GaussianBlur(gray, (kernel_size, kernel_size), 0)
    low_threshold = 50
    high_threshold = 150
    edges = cv2.Canny(blur_gray, low_threshold, high_threshold)
    plt.imshow(blur_gray, cmap='gray')
    rho = 1  # distance resolution in pixels of the Hough grid
    theta = np.pi / 180  # angular resolution in radians of the Hough grid
    threshold = 500  # minimum number of votes (intersections in Hough grid cell)
    min_line_length = 50  # minimum number of pixels making up a line
    max_line_gap = 20  # maximum gap in pixels between connectable line segments
    line_image = np.copy(img) * 0  # creating a blank to draw lines on

    # Run Hough on edge detected image
    # Output "lines" is an array containing endpoints of detected line segments
    lines = cv2.HoughLinesP(edges, rho, theta, threshold, np.array([]),
                            min_line_length, max_line_gap)

    result = np.copy(img)
    contours, hierarchy = cv2.findContours(255 - blur_gray, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    for cnt in range(len(contours)):

        # 轮廓逼近
        epsilon = 0.01 * cv2.arcLength(contours[cnt], True)
        approx = cv2.approxPolyDP(contours[cnt], epsilon, True)
        corners = len(approx)
        if corners == 4:
            area = cv2.contourArea(contours[cnt])
            if area > 10000:
                # 提取与绘制轮廓
                cv2.drawContours(result, contours, cnt, (0, 0, 255), 2)

    cv2.imwrite("main4out.png", result)

    # for line in lines:
    #     for x1, y1, x2, y2 in line:
    #         cv2.line(line_image, (x1, y1), (x2, y2), (255, 0, 0), 5)
    # # Draw the lines on the  image
    # lines_edges = cv2.addWeighted(img, 0.8, line_image, 1, 0)
    # cv2.imwrite("main4out.png", line_image)
