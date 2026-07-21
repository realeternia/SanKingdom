#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
分析 Task 目录下的任务，调用 DeepSeek API 生成人物外貌描述。

目录约定：
  - Task 下以 "done" 开头的子目录视为已完成，跳过。
  - 文生文：子目录有 des.txt，且没有 des.png/jpg/jpeg。
  - 图生文：子目录有 des.png 或 des.jpg/jpeg。
  - 结果写入各子目录的 result.txt，并拼接固定画风描述。

API 密钥从同目录下的 api_key.txt 读取（该文件已加入 .gitignore，不会进 git）。
仅使用 Python 标准库，无需安装第三方依赖。
"""

import os
import sys
import json
import base64
import urllib.request
import urllib.error

# ===== 配置区 =====
ROOT_DIR = os.path.dirname(os.path.abspath(__file__))
TASK_DIR = os.path.join(ROOT_DIR, "Task")
API_KEY_FILE = os.path.join(ROOT_DIR, "api_key.txt")  # 不进 git

BASE_URL = "https://api.deepseek.com/v1/chat/completions"
TEXT_MODEL = "deepseek-chat"          # 文生文模型
VISION_MODEL = "deepseek-v4-flash"   # 图生文视觉模型（可选 deepseek-v4-pro）
REQUEST_TIMEOUT = 120                # 秒

# 固定画风后缀，拼接到每份结果末尾
STYLE_SUFFIX = (
    "经典的国风写实数字立绘，神似《三国志》游戏人物画风。"
    "人物面部写实生动，神态各异；服饰纹饰精美，明暗褶皱呈现出真实的布料质感。"
    "光影柔和细腻，色彩沉稳内敛，局部有华丽点缀。"
    "融合了东方古典神韵与现代CG厚涂技法，背景适度虚化，"
    "整体展现出典雅、端庄且极具历史厚重感的艺术气质。"
    "背景不要纯色，半身搭配虚化的背景。"
)


def load_api_key():
    """从 api_key.txt 读取 API 密钥。"""
    if not os.path.exists(API_KEY_FILE):
        print(f"错误：找不到 API 密钥文件：{API_KEY_FILE}")
        print("请创建该文件并把 DeepSeek API Key 写入其中（单行即可）。")
        sys.exit(1)
    with open(API_KEY_FILE, "r", encoding="utf-8") as f:
        key = f.read().strip()
    if not key:
        print("错误：api_key.txt 为空。")
        sys.exit(1)
    return key


def image_to_data_url(image_path):
    """本地图片转为 base64 data URL。"""
    ext = os.path.splitext(image_path)[1].lower()
    if ext in (".jpg", ".jpeg"):
        mime = "image/jpeg"
    elif ext == ".png":
        mime = "image/png"
    else:
        mime = "image/jpeg"
    with open(image_path, "rb") as f:
        b64 = base64.b64encode(f.read()).decode("utf-8")
    return f"data:{mime};base64,{b64}"


def call_deepseek(api_key, messages, model, max_tokens=1024):
    """调用 DeepSeek chat/completions 接口，返回文本内容。"""
    headers = {
        "Authorization": f"Bearer {api_key}",
        "Content-Type": "application/json",
    }
    payload = {
        "model": model,
        "messages": messages,
        "temperature": 0.7,
        "max_tokens": max_tokens,
        "stream": False,
    }
    data_bytes = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(
        BASE_URL, data=data_bytes, headers=headers, method="POST"
    )
    try:
        with urllib.request.urlopen(req, timeout=REQUEST_TIMEOUT) as resp:
            raw = resp.read().decode("utf-8")
    except urllib.error.HTTPError as e:
        body = e.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"HTTP {e.code}: {body[:500]}") from None
    data = json.loads(raw)
    return data["choices"][0]["message"]["content"].strip()


def gen_text_to_text(api_key, des_text):
    """文生文：依据 des.txt 生成人物外貌描述。"""
    prompt = (
        "请根据下面给出的角色信息，描写该角色的五官、表情、发型、衣着、姿态等外貌细节，"
        "可以适当发挥想象补充合理细节，使形象更鲜活立体。"
        "只描写人物本身，不要描述任何画风、绘画风格、镜头、构图等技术性内容。"
        "输出一段连贯生动的中文描写。\n\n"
        f"角色信息：{des_text}"
    )
    messages = [{"role": "user", "content": prompt}]
    return call_deepseek(api_key, messages, TEXT_MODEL)


def gen_image_to_text(api_key, image_path, des_text):
    """图生文：依据图片生成人物外貌描述，并拼接 des.txt（若有）。"""
    prompt = (
        "请仔细观察图片中的人物，描写这个人的五官、表情、发型、衣着、姿态等外貌细节，"
        "可以适当发挥想象补充合理细节，使形象更鲜活立体。"
        "只描写人物本身，不要描述任何画风、绘画风格、镜头、构图等技术性内容。"
        "输出一段连贯生动的中文描写。"
    )
    if des_text:
        prompt += f"\n\n参考信息：{des_text}"

    img_url = image_to_data_url(image_path)
    messages = [{
        "role": "user",
        "content": [
            {"type": "text", "text": prompt},
            {"type": "image_url", "image_url": {"url": img_url}},
        ],
    }]
    return call_deepseek(api_key, messages, VISION_MODEL, max_tokens=1536)


def find_des_image(subdir):
    """在子目录中查找 des.png / des.jpg / des.jpeg，返回路径或 None。"""
    for ext in (".png", ".jpg", ".jpeg"):
        p = os.path.join(subdir, "des" + ext)
        if os.path.exists(p):
            return p
    return None


def process_task(subdir, api_key):
    """处理单个任务子目录。"""
    name = os.path.basename(subdir)

    # done 开头视为已完成
    if name.lower().startswith("done"):
        print(f"[跳过] 已完成：{name}")
        return

    result_path = os.path.join(subdir, "result.txt")
    # 已有结果则跳过，避免重复消耗 API
    if os.path.exists(result_path):
        print(f"[跳过] 已有 result.txt：{name}")
        return

    des_txt_path = os.path.join(subdir, "des.txt")
    des_text = ""
    if os.path.exists(des_txt_path):
        with open(des_txt_path, "r", encoding="utf-8") as f:
            des_text = f.read().strip()

    des_img = find_des_image(subdir)

    # 判定任务类型
    if des_img:
        task_type = "图生文"
    elif des_text:
        task_type = "文生文"
    else:
        print(f"[跳过] 无 des.txt 也无 des 图片：{name}")
        return

    try:
        if task_type == "图生文":
            print(f"[图生文] {name}  <-  {os.path.basename(des_img)}")
            try:
                desc = gen_image_to_text(api_key, des_img, des_text)
            except Exception as img_err:
                # DeepSeek 当前不支持图片输入，若有 des.txt 则降级为文生文
                if des_text:
                    print(f"  └ 视觉调用失败，改用 des.txt 文生文降级：{img_err}")
                    desc = gen_text_to_text(api_key, des_text)
                else:
                    raise
        else:
            print(f"[文生文] {name}")
            desc = gen_text_to_text(api_key, des_text)

        # 拼接固定画风描述后写入 result.txt
        final = desc + "\n\n" + STYLE_SUFFIX
        with open(result_path, "w", encoding="utf-8") as f:
            f.write(final)
        print(f"[完成] {name}  ->  result.txt")

        # 成功后重命名为 done 前缀
        parent = os.path.dirname(subdir)
        new_name = "done" + name
        new_path = os.path.join(parent, new_name)
        counter = 1
        while os.path.exists(new_path):
            new_path = os.path.join(parent, f"done{name}_{counter}")
            counter += 1
        os.rename(subdir, new_path)
        print(f"[重命名] {name} -> {os.path.basename(new_path)}")
    except Exception as e:
        print(f"[错误] {name}: {e}")


def main():
    if not os.path.isdir(TASK_DIR):
        print(f"错误：Task 目录不存在：{TASK_DIR}")
        sys.exit(1)

    api_key = load_api_key()

    entries = sorted(os.listdir(TASK_DIR))
    todo = [e for e in entries if os.path.isdir(os.path.join(TASK_DIR, e))]
    if not todo:
        print("Task 目录下没有子目录。")
        return

    print(f"共发现 {len(todo)} 个子目录，开始处理...\n")
    for name in todo:
        process_task(os.path.join(TASK_DIR, name), api_key)
    print("\n全部处理完毕。")


if __name__ == "__main__":
    main()
