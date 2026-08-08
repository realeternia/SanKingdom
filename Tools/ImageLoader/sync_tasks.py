# -*- coding: utf-8 -*-
"""
扫描 F:\\BaiduNetdiskDownload\\sanguo\\out 下所有图片文件，
对每个文件名 xxx，在 Task 目录下递归查找是否存在 donexxx 文件夹。
若不存在，则在 Task 下创建 xxx 文件夹，把图片复制为 des.jpg，
并复制 donechendao/des.txt 作为提示词模板。
"""
import os
import shutil

# 配置区
SRC_DIR = r"F:\BaiduNetdiskDownload\sanguo\out"
TASK_DIR = r"D:\U3dPrj\SanKingdom\Tools\ImageLoader\Task"
DES_TXT_TEMPLATE = os.path.join(TASK_DIR, "donechendao", "des.txt")
IMAGE_EXTS = {".jpg", ".jpeg", ".png"}


def find_done_folder(task_dir, name):
    """递归查找是否存在 done + name 的文件夹。"""
    target = "done" + name
    for root, dirs, _ in os.walk(task_dir):
        if target in dirs:
            return os.path.join(root, target)
    return None


def main():
    if not os.path.isfile(DES_TXT_TEMPLATE):
        print(f"[错误] 模板文件不存在: {DES_TXT_TEMPLATE}")
        return
    if not os.path.isdir(SRC_DIR):
        print(f"[错误] 源目录不存在: {SRC_DIR}")
        return

    created = 0
    skipped_done = 0
    skipped_exists = 0

    for fname in sorted(os.listdir(SRC_DIR)):
        src_path = os.path.join(SRC_DIR, fname)
        if not os.path.isfile(src_path):
            continue
        name, ext = os.path.splitext(fname)
        if ext.lower() not in IMAGE_EXTS:
            continue

        # 递归查找 donexxx
        if find_done_folder(TASK_DIR, name):
            skipped_done += 1
            continue

        # 已存在未处理目录则跳过
        target_dir = os.path.join(TASK_DIR, name)
        if os.path.exists(target_dir):
            skipped_exists += 1
            continue

        # 创建目录并复制文件
        os.makedirs(target_dir, exist_ok=True)
        shutil.copy2(src_path, os.path.join(target_dir, "des.jpg"))
        shutil.copy2(DES_TXT_TEMPLATE, os.path.join(target_dir, "des.txt"))
        created += 1
        print(f"[创建] {name}")

    print(f"\n完成：新建 {created} 个，跳过(已完成) {skipped_done} 个，跳过(已存在) {skipped_exists} 个")


if __name__ == "__main__":
    main()
