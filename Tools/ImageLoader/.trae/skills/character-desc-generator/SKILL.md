---
name: "character-desc-generator"
description: "分析 Task 目录下的人物立绘任务，生成国风写实数字立绘描述并写入 result.txt。Invoke when user asks to process Task directory, generate character descriptions, or handle des.txt/des.jpg tasks."
---

# Character Description Generator

## Purpose

扫描项目 `Task/` 目录下的子任务，根据 `des.txt`（文生文）或 `des.jpg/des.png`（图生文）生成角色外貌描写，最终写入 `result.txt`，并拼接固定的国风写实数字立绘画风后缀。

## Directory Rules

- 工作根目录：`d:\U3dPrj\SanKingdom\Tools\ImageLoader`
- 任务目录：`Task/<sub-dir>/`
- 已完成目录：以 `done` 开头的子目录跳过。
- 每个子目录应包含：
  - `des.txt`：文字提示（可选，图生文时作为参考）。
  - `des.jpg` / `des.png`：参考图片（可选）。
- 输出文件：`Task/<sub-dir>/result.txt`

## Workflow

1. **Scan Task directory**
   - 列出 `Task/` 下所有子目录。
   - 跳过名称以 `done` 开头的目录。
   - 跳过已存在 `result.txt` 的目录（避免重复生成）。

2. **Determine task type**
   - **Image-to-text**: 存在 `des.jpg` 或 `des.png`。
   - **Text-to-text**: 只有 `des.txt`，没有图片。
   - 如果两者都没有，跳过。

3. **Read inputs**
   - `des.txt`: 仅作为生成提示使用，**不要把它的原文写入 `result.txt`**。
   - `des.jpg` / `des.png`: 如果你是多模态模型，使用 `Read` 工具读取图片，结合 `des.txt` 内容生成描述；否则降级为 text-to-text（仅使用 `des.txt`）。

4. **Generate description**
   - 描述角色的五官、表情、发型、衣着、姿态等外貌细节。
   - 可以适当发挥，使形象更鲜活立体。
   - 必须结合 `des.txt` 中的角色设定（如“三国时期美女武将”）。
   - **不要出现画风描述**（不要写“油画、水墨、CG、厚涂、光影、构图”等词）。
   - 输出为连贯生动的中文段落。

5. **Append fixed style suffix**

   在生成的人物描述之后，追加以下内容（固定不变）：

   ```
   经典的国风写实数字立绘，神似《三国志》游戏人物画风。人物面部写实生动，神态各异；服饰纹饰精美，明暗褶皱呈现出真实的布料质感。光影柔和细腻，色彩沉稳内敛，局部有华丽点缀。融合了东方古典神韵与现代CG厚涂技法，背景适度虚化，整体展现出典雅、端庄且极具历史厚重感的艺术气质。背景不要纯色，半身搭配虚化的背景。
   ```

6. **Write result.txt**
   - 写入格式：`[人物外貌描述]\n\n[固定画风后缀]`
   - 不要写入 `des.txt` 原文。

7. **Rename directory after success**
   - 成功写入 `result.txt` 后，将子目录重命名为 `done<原名称>`。
   - 如果目标目录已存在，则依次尝试 `done<原名称>_1`、`done<原名称>_2` 等。

## External API Fallback (for text-to-text)

如果当前环境不是多模态模型，且用户没有明确要求手动处理，可以调用本地脚本 `task_analyzer.py` 中的 DeepSeek 文生文逻辑处理纯文本任务。该脚本依赖 `api_key.txt` 中的 DeepSeek API Key（已加入 `.gitignore`）。

注意：DeepSeek 的 OpenAI 兼容接口目前不支持图片输入，因此图生文任务必须由多模态模型直接识图完成；否则只能降级为文生文。

## Important Constraints

- 只描写人物外貌本身，不写画风、摄影、构图等技术性描述。
- `des.txt` 的提示词原文不得出现在 `result.txt` 中。
- 固定画风后缀必须原样保留。
- 处理完成后目录必须重命名为 `done` 前缀。
