import os
import re

TARGET_FONT_GUID = "2c31971a758c934789077706967acc0d"
TARGET_FONT_FILEID = "11400000"
TARGET_MATERIAL_FILEID = "-2551621216232197282"

def find_all_files(directory, extensions):
    files = []
    for root, dirs, filenames in os.walk(directory):
        for filename in filenames:
            for ext in extensions:
                if filename.endswith(ext):
                    files.append(os.path.join(root, filename))
    return files

def replace_font_references(file_path):
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        original_content = content
        
        content = re.sub(
            r'm_fontAsset:\s*\{fileID:\s*\d+,\s*guid:\s*[a-f0-9]+,\s*type:\s*2\}',
            f'm_fontAsset: {{fileID: {TARGET_FONT_FILEID}, guid: {TARGET_FONT_GUID}, type: 2}}',
            content
        )
        
        content = re.sub(
            r'm_sharedMaterial:\s*\{fileID:\s*-?\d+,\s*guid:\s*[a-f0-9]+,\s*type:\s*2\}',
            f'm_sharedMaterial: {{fileID: {TARGET_MATERIAL_FILEID}, guid: {TARGET_FONT_GUID}, type: 2}}',
            content
        )
        
        if content != original_content:
            with open(file_path, 'w', encoding='utf-8') as f:
                f.write(content)
            return True
        return False
    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        return False

def main():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    assets_dir = os.path.join(script_dir, 'Assets')
    
    print("=" * 60)
    print("Replacing font references in .prefab and .unity files...")
    print(f"Target font GUID: {TARGET_FONT_GUID}")
    print("=" * 60)
    
    files = find_all_files(assets_dir, ['.prefab', '.unity'])
    
    modified_count = 0
    for file_path in files:
        if replace_font_references(file_path):
            print(f"[MODIFIED] {file_path}")
            modified_count += 1
    
    print("\n" + "=" * 60)
    print(f"Done! Modified {modified_count} files.")
    print("=" * 60)

if __name__ == '__main__':
    main()
