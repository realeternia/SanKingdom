#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
图片处理脚本：
  1. 将输入图片等比例缩放为 600x800，高度溢出部分从底部裁掉，导出结果 a。
  2. 识别头像（OpenCV YuNet DNN 模型，含 5 关键点），提取 160x160 头像，
     以“眼睛中点与嘴巴中点 的中点”为头像中心，导出结果 head（结果 b）。

用法：
    python image_resize_and_head.py <图片路径>
    python image_resize_and_head.py <图片路径1> <图片路径2> ...

依赖：
    pip install pillow numpy "opencv-python<5"

需要 face_detection_yunet_2023mar.onnx 文件（与本脚本同目录）。
下载地址：
    https://github.com/opencv/opencv_zoo/raw/main/models/face_detection_yunet/face_detection_yunet_2023mar.onnx
注意：OpenCV 5.x 移除了 FaceDetectorYN/CascadeClassifier，需用 4.x（如 4.10.0）。
"""

import os
import sys

import cv2
import numpy as np
from PIL import Image

# ===== 配置 =====
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
YUNET_FILENAME = "face_detection_yunet_2023mar.onnx"
YUNET_SCORE_THRESHOLD = 0.5   # 置信度阈值，立绘可适当放低
YUNET_NMS_THRESHOLD = 0.3
YUNET_MIN_AREA_RATIO = 0.008  # 过滤面积小于图像 0.8% 的小框误检

TARGET_W = 600      # 结果 a 宽度
TARGET_H = 800      # 结果 a 高度
HEAD_SIZE = 160     # 头像尺寸（正方形）


def resize_and_crop(img):
    """
    等比例缩放并裁剪为 TARGET_W x TARGET_H。
    水平居中裁剪宽度溢出，垂直保留顶部、裁掉底部溢出（符合“把原图最下面去掉”）。
    返回 (裁剪后的 PIL 图, 左侧偏移 left, 顶部偏移 top, 缩放比例 scale)。
    """
    orig_w, orig_h = img.size
    # 取能“覆盖”目标尺寸的缩放比例
    scale = max(TARGET_W / orig_w, TARGET_H / orig_h)
    new_w = int(round(orig_w * scale))
    new_h = int(round(orig_h * scale))
    resized = img.resize((new_w, new_h), Image.LANCZOS)

    # 水平居中，垂直顶部对齐
    left = (new_w - TARGET_W) // 2
    top = 0
    cropped = resized.crop((left, top, left + TARGET_W, top + TARGET_H))
    return cropped, left, top, scale


def detect_face_center(img):
    """
    用 YuNet DNN 在原图上检测人脸，返回头像中心点 (cx, cy)。
    中心 = (双眼中点 与 双嘴角中点) 的中点，使眼睛在中心偏上、嘴巴在中心偏下。
    检测失败返回 None。
    """
    model_path = os.path.join(SCRIPT_DIR, YUNET_FILENAME)
    if not os.path.isfile(model_path):
        print(f"[警告] 找不到 YuNet 模型文件：{model_path}")
        return None

    arr = cv2.cvtColor(np.array(img.convert("RGB")), cv2.COLOR_RGB2BGR)
    h, w = arr.shape[:2]
    detector = cv2.FaceDetectorYN.create(
        model_path,
        "",
        (w, h),
        score_threshold=YUNET_SCORE_THRESHOLD,
        nms_threshold=YUNET_NMS_THRESHOLD,
        top_k=5000,
    )
    _, faces = detector.detect(arr)
    if faces is None or len(faces) == 0:
        return None

    # 过滤面积过小的小框误检，再选 score 最高的人脸
    # faces[i] = [x, y, w, h, re_x, re_y, le_x, le_y, nose_x, nose_y, rm_x, rm_y, lm_x, lm_y, score]
    img_area = w * h
    min_area = img_area * YUNET_MIN_AREA_RATIO
    candidates = [f for f in faces if float(f[2]) * float(f[3]) >= min_area]
    if not candidates:  # 极端情况：全部低于阈值，降级用全部
        candidates = list(faces)
    best = max(candidates, key=lambda f: float(f[14]))
    re_x, re_y = float(best[4]), float(best[5])     # 右眼
    le_x, le_y = float(best[6]), float(best[7])     # 左眼
    rm_x, rm_y = float(best[10]), float(best[11])   # 右嘴角
    lm_x, lm_y = float(best[12]), float(best[13])   # 左嘴角

    eye_cx = (re_x + le_x) / 2
    eye_cy = (re_y + le_y) / 2
    mouth_cx = (rm_x + lm_x) / 2
    mouth_cy = (rm_y + lm_y) / 2

    # 头像中心 = 眼睛中点 与 嘴巴中点 的中点
    cx = (eye_cx + mouth_cx) / 2
    cy = (eye_cy + mouth_cy) / 2
    print(f"[YuNet] score={float(best[14]):.3f}  眼({eye_cx:.0f},{eye_cy:.0f}) "
          f"嘴({mouth_cx:.0f},{mouth_cy:.0f})")
    return cx, cy


def crop_head(img_a, face_center, left, top, scale):
    """
    在 600x800 的图上裁剪 HEAD_SIZE x HEAD_SIZE 头像。
    face_center 为原图坐标系，需映射到 img_a 坐标系。
    检测失败时降级为顶部居中区域。
    """
    if face_center is None:
        print("[头像] 未检测到人脸，使用降级方案（顶部居中，略下移）")
        head_left = (TARGET_W - HEAD_SIZE) // 2
        head_top = max(0, int(TARGET_H * 0.05))
    else:
        orig_cx, orig_cy = face_center
        a_cx = orig_cx * scale - left
        a_cy = orig_cy * scale - top
        print(f"[头像] 检测到人脸，映射到 600x800 后中心=({a_cx:.1f}, {a_cy:.1f})")
        head_left = int(a_cx - HEAD_SIZE / 2)
        head_top = int(a_cy - HEAD_SIZE / 2)
        # 边界保护
        head_left = max(0, min(head_left, TARGET_W - HEAD_SIZE))
        head_top = max(0, min(head_top, TARGET_H - HEAD_SIZE))

    return img_a.crop((head_left, head_top, head_left + HEAD_SIZE, head_top + HEAD_SIZE))


def make_output_path(src_path, suffix):
    base, ext = os.path.splitext(src_path)
    if ext.lower() not in (".png", ".jpg", ".jpeg"):
        ext = ".png"
    return f"{base}_{suffix}{ext}"


def process_one(src_path):
    if not os.path.isfile(src_path):
        print(f"[错误] 文件不存在：{src_path}")
        return

    try:
        img = Image.open(src_path).convert("RGB")
    except Exception as e:
        print(f"[错误] 无法打开图片：{src_path}  ({e})")
        return

    print(f"\n处理：{os.path.basename(src_path)}  原图尺寸={img.size[0]}x{img.size[1]}")

    # 结果 a：600x800
    img_a, left, top, scale = resize_and_crop(img)
    out_a = make_output_path(src_path, "a")
    img_a.save(out_a, "PNG")
    print(f"[结果 a] {out_a}  ({img_a.size[0]}x{img_a.size[1]})")

    # 结果 b：160x160 头像
    face_center = detect_face_center(img)
    img_head = crop_head(img_a, face_center, left, top, scale)
    out_head = make_output_path(src_path, "head")
    img_head.save(out_head, "PNG")
    print(f"[结果 b] {out_head}  ({img_head.size[0]}x{img_head.size[1]})")


def main():
    if len(sys.argv) < 2:
        print(f"用法：python {os.path.basename(__file__)} <图片路径> [<图片路径2> ...]")
        sys.exit(1)

    for path in sys.argv[1:]:
        process_one(path)

    print("\n全部处理完毕。")


if __name__ == "__main__":
    main()
