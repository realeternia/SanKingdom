#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
批处理：遍历 Task/done 下所有含 result.png 的子目录，
  1. 等比例缩放裁剪为 600x800 大图 -> 覆盖 SkinsBig/<name>.jpg
  2. YuNet 识别头像裁剪 160x160    -> 覆盖 Skins/<name>.jpg
其中 name = 子目录名去掉 "done" 前缀。

用法：
    python batch_export_skins.py            # 实际执行（跳过已处理的）
    python batch_export_skins.py --force    # 强制重新处理所有
    python batch_export_skins.py --dry-run  # 仅打印映射，不写文件
    python batch_export_skins.py --only sunhuan,caocao  # 只处理指定 name
"""
import os
import sys

from PIL import Image

import image_resize_and_head as irh

# ===== 配置 =====
ROOT = os.path.dirname(os.path.abspath(__file__))
DONE_DIR = os.path.join(ROOT, "Task", "done")
SKINS_BIG = r"D:\U3dPrj\SanKingdom\Assets\Resources\Textures\SkinsBig"
SKINS = r"D:\U3dPrj\SanKingdom\Assets\Resources\Textures\Skins"
JPEG_QUALITY = 95


def strip_done(dirname):
    """去掉 done 前缀（大小写不敏感）。"""
    if dirname.lower().startswith("done"):
        return dirname[4:]
    return dirname


def collect_tasks(only_names=None):
    """收集 (result_png_path, name) 列表。"""
    tasks = []
    for name in sorted(os.listdir(DONE_DIR)):
        full = os.path.join(DONE_DIR, name)
        if not os.path.isdir(full):
            continue
        rp = os.path.join(full, "result.png")
        if not os.path.isfile(rp):
            continue
        short = strip_done(name)
        if not short:
            continue
        if only_names and short not in only_names:
            continue
        tasks.append((rp, short))
    return tasks


def main():
    dry = "--dry-run" in sys.argv
    force = "--force" in sys.argv
    only = None
    for a in sys.argv[1:]:
        if a.startswith("--only="):
            only = set(x.strip() for x in a[len("--only="):].split(",") if x.strip())
        elif a == "--only":
            # --only a,b,c
            pass

    tasks = collect_tasks(only)
    if not tasks:
        print("没有找到可处理的 result.png。")
        return

    # 默认跳过已处理的（大图和头像都已存在），--force 可强制重新处理
    if not force:
        tasks = [(rp, name) for rp, name in tasks
                 if not (os.path.isfile(os.path.join(SKINS_BIG, name + ".jpg"))
                         and os.path.isfile(os.path.join(SKINS, name + ".jpg")))]

    print(f"共 {len(tasks)} 个任务{'（dry-run，不写文件）' if dry else ''}"
          f"{'（强制重新处理）' if force and not dry else ''}：\n")

    if not tasks:
        print("没有需要处理的新任务（之前执行过的已跳过）。")
        return

    miss_big = miss_skin = 0
    for rp, name in tasks:
        big_path = os.path.join(SKINS_BIG, name + ".jpg")
        head_path = os.path.join(SKINS, name + ".jpg")
        big_ok = os.path.isfile(big_path)
        skin_ok = os.path.isfile(head_path)
        flag = ""
        if not big_ok:
            miss_big += 1
            flag += " [大图新增]"
        if not skin_ok:
            miss_skin += 1
            flag += " [头像新增]"

        if dry:
            print(f"  {name:20s} <- {os.path.basename(os.path.dirname(rp))}{flag}")
            continue

        # 实际处理
        try:
            img = Image.open(rp).convert("RGB")
            img_a, left, top, scale = irh.resize_and_crop(img)
            face_center = irh.detect_face_center(img)
            img_head = irh.crop_head(img_a, face_center, left, top, scale)
            img_a.save(big_path, "JPEG", quality=JPEG_QUALITY)
            img_head.save(head_path, "JPEG", quality=JPEG_QUALITY)
            status = "OK" if face_center else "降级"
            print(f"  {name:20s} {status:4s}  -> SkinsBig + Skins{flag}")
        except Exception as e:
            print(f"  {name:20s} 错误: {e}")

    if dry:
        print(f"\n映射检查：大图新增 {miss_big} 个，头像新增 {miss_skin} 个")
        print("确认无误后去掉 --dry-run 执行。")
    else:
        print("\n全部处理完毕。")


if __name__ == "__main__":
    main()
