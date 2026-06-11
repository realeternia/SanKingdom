"""
OBJ 模型批量处理脚本
功能：
  1. 减面：保证顶点数 < 2000
  2. 增加UV：圆柱体投影（已有UV则跳过）
  3. 调整朝向：使圆柱体投影效果更好（长轴对齐Y轴）
"""

import os
import glob
import trimesh
import numpy as np

TARGET_DIR = os.path.dirname(os.path.abspath(__file__))
MAX_VERTICES = 2000


def adjust_orientation(mesh: trimesh.Trimesh):
    """
    调整模型朝向：用 PCA 找到最长轴，旋转使其对齐 Y 轴。
    这样圆柱体投影的 UV 展开效果最好（长轴做圆柱轴心）。
    """
    vertices = mesh.vertices
    center = vertices.mean(axis=0)
    centered = vertices - center

    # PCA：协方差矩阵的特征向量即为主轴
    cov = np.cov(centered.T)
    eigenvalues, eigenvectors = np.linalg.eigh(cov)

    # 按特征值升序排列，最后一列是最大特征值对应的主轴（最长轴）
    longest_axis = eigenvectors[:, -1]

    # 将最长轴对齐到 Y 轴 (0, 1, 0)
    y_axis = np.array([0.0, 1.0, 0.0])
    rotation = rotation_from_to(longest_axis, y_axis)

    mesh.vertices = centered @ rotation.T + center
    return mesh


def rotation_from_to(src: np.ndarray, dst: np.ndarray):
    """
    计算从 src 方向旋转到 dst 方向的最小旋转矩阵（Rodrigues 公式）。
    """
    src = src / np.linalg.norm(src)
    dst = dst / np.linalg.norm(dst)

    if np.allclose(src, dst):
        return np.eye(3)
    if np.allclose(src, -dst):
        # 180度旋转，找一个垂直于 src 的轴
        perp = np.array([1.0, 0.0, 0.0])
        if np.abs(np.dot(src, perp)) > 0.99:
            perp = np.array([0.0, 1.0, 0.0])
        axis = np.cross(src, perp)
        axis = axis / np.linalg.norm(axis)
        return rotation_matrix(axis, np.pi)

    axis = np.cross(src, dst)
    axis = axis / np.linalg.norm(axis)
    angle = np.arccos(np.clip(np.dot(src, dst), -1.0, 1.0))
    return rotation_matrix(axis, angle)


def rotation_matrix(axis: np.ndarray, angle: float):
    """Rodrigues 旋转公式。"""
    K = np.array([
        [0, -axis[2], axis[1]],
        [axis[2], 0, -axis[0]],
        [-axis[1], axis[0], 0]
    ])
    return np.eye(3) + np.sin(angle) * K + (1 - np.cos(angle)) * (K @ K)


def has_uv(mesh: trimesh.Trimesh):
    """检查 mesh 是否已有 UV 坐标（实际数据，非默认值）。"""
    if hasattr(mesh.visual, 'uv') and mesh.visual.uv is not None:
        uvs = mesh.visual.uv
        if len(uvs) > 0 and not np.allclose(uvs, 0.0):
            return True
    return False


def generate_cylindrical_uv(mesh: trimesh.Trimesh):
    """
    为 mesh 生成圆柱体投影 UV。
    u = atan2(x, z) / (2*pi) + 0.5
    v = (y - y_min) / (y_max - y_min)
    """
    vertices = mesh.vertices
    y_min, y_max = vertices[:, 1].min(), vertices[:, 1].max()
    y_range = y_max - y_min
    if y_range < 1e-6:
        y_range = 1.0

    uv = np.zeros((len(vertices), 2), dtype=np.float32)
    uv[:, 0] = np.arctan2(vertices[:, 0], vertices[:, 2]) / (2.0 * np.pi) + 0.5
    uv[:, 1] = (vertices[:, 1] - y_min) / y_range

    mesh.visual = trimesh.visual.TextureVisuals(uv=uv)
    return mesh


def decimate_to_target(mesh: trimesh.Trimesh, max_vertices: int):
    """
    减面直到顶点数 < max_vertices。
    使用 trimesh 的 simplify_quadratic_decimation 逐步减少面数。
    """
    current_verts = len(mesh.vertices)
    if current_verts <= max_vertices:
        return mesh, False

    # 估算需要减少的比例
    target_face_count = int(len(mesh.faces) * (max_vertices / current_verts) * 0.85)

    try:
        mesh = mesh.simplify_quadratic_decimation(target_face_count)
    except Exception:
        # 如果一次减面不够，逐步减
        while len(mesh.vertices) > max_vertices and len(mesh.faces) > 0:
            target = max(int(len(mesh.faces) * 0.8), 1)
            try:
                mesh = mesh.simplify_quadratic_decimation(target)
            except Exception:
                break

    return mesh, True


def process_obj(filepath: str):
    """处理单个 OBJ 文件。"""
    filename = os.path.basename(filepath)
    print(f"\n{'='*60}")
    print(f"处理: {filename}")

    try:
        mesh = trimesh.load(filepath, force='mesh')
    except Exception as e:
        print(f"  错误: 无法加载 - {e}")
        return

    if not isinstance(mesh, trimesh.Trimesh):
        print(f"  跳过: 不是单一 mesh（可能是多物体场景）")
        return

    if len(mesh.vertices) == 0 or len(mesh.faces) == 0:
        print(f"  跳过: 空 mesh")
        return

    print(f"  原始顶点数: {len(mesh.vertices)}, 面数: {len(mesh.faces)}")

    modified = False

    # Step 1: 减面
    if len(mesh.vertices) > MAX_VERTICES:
        print(f"  减面中... (目标 < {MAX_VERTICES} 顶点)")
        mesh, did_decimate = decimate_to_target(mesh, MAX_VERTICES)
        if did_decimate:
            modified = True
            print(f"  减面后顶点数: {len(mesh.vertices)}, 面数: {len(mesh.faces)}")
        else:
            print(f"  减面未执行或无效")
    else:
        print(f"  顶点数已达标，跳过减面")

    # Step 3: 调整朝向（先调朝向，再生成UV，效果更好）
    if modified or not has_uv(mesh):
        print(f"  调整朝向中...")
        mesh = adjust_orientation(mesh)
        print(f"  朝向已调整（长轴对齐Y轴）")

    # Step 2: 加UV（圆柱体投影）
    if has_uv(mesh):
        print(f"  已有UV，跳过UV生成")
    else:
        print(f"  生成圆柱体投影UV中...")
        mesh = generate_cylindrical_uv(mesh)
        modified = True
        print(f"  UV已生成（圆柱体投影）")

    # 保存
    if modified:
        # 备份原文件
        backup_path = filepath.replace('.obj', '_backup.obj')
        if not os.path.exists(backup_path):
            os.rename(filepath, backup_path)
            print(f"  原文件已备份至: {os.path.basename(backup_path)}")

        # 导出时写入 UV 坐标
        try:
            export_obj_with_uv(mesh, filepath)
            print(f"  已保存: {filename}")
        except Exception as e:
            print(f"  保存失败: {e}")
            # 恢复原文件
            if os.path.exists(backup_path) and not os.path.exists(filepath):
                os.rename(backup_path, filepath)
    else:
        print(f"  无需修改，跳过保存")


def export_obj_with_uv(mesh: trimesh.Trimesh, filepath: str):
    """
    导出 OBJ 文件并包含 UV 坐标。
    手动写入以确保障 UV 正确输出。
    """
    vertices = mesh.vertices
    faces = mesh.faces

    # 获取 UV
    if hasattr(mesh.visual, 'uv') and mesh.visual.uv is not None:
        uvs = mesh.visual.uv
    else:
        uvs = None

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write("# Processed by process_objs.py\n")
        f.write(f"# Vertices: {len(vertices)}, Faces: {len(faces)}\n")

        # 写入顶点
        for v in vertices:
            f.write(f"v {v[0]:.6f} {v[1]:.6f} {v[2]:.6f}\n")

        # 写入 UV
        if uvs is not None:
            for uv in uvs:
                f.write(f"vt {uv[0]:.6f} {uv[1]:.6f}\n")

        # 写入面
        if uvs is not None:
            # 带 UV 的面格式: f v1/vt1 v2/vt2 v3/vt3
            for face in faces:
                i0, i1, i2 = face + 1  # OBJ 索引从 1 开始
                f.write(f"f {i0}/{i0} {i1}/{i1} {i2}/{i2}\n")
        else:
            for face in faces:
                i0, i1, i2 = face + 1
                f.write(f"f {i0} {i1} {i2}\n")


def main():
    obj_files = glob.glob(os.path.join(TARGET_DIR, "*.obj"))
    # 排除备份文件
    obj_files = [f for f in obj_files if '_backup' not in os.path.basename(f)]

    if not obj_files:
        print("未找到 .obj 文件")
        return

    print(f"找到 {len(obj_files)} 个 .obj 文件\n")

    for filepath in obj_files:
        process_obj(filepath)

    print(f"\n{'='*60}")
    print("所有文件处理完毕!")


if __name__ == "__main__":
    main()
