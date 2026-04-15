import os
import re
import json

def is_invalid_guid(guid_str):
    if len(guid_str) > 32 or '+' in guid_str or '/' in guid_str or '=' in guid_str:
        return True
    return False

def find_all_files(directory, extensions):
    files = []
    for root, dirs, filenames in os.walk(directory):
        for filename in filenames:
            for ext in extensions:
                if filename.endswith(ext):
                    files.append(os.path.join(root, filename))
    return files

def get_guid_from_meta(meta_file):
    try:
        with open(meta_file, 'r', encoding='utf-8') as f:
            content = f.read()
        guid_match = re.search(r'guid:\s*([a-zA-Z0-9+/=]+)', content)
        if guid_match:
            return guid_match.group(1)
    except Exception as e:
        print(f"Error reading {meta_file}: {e}")
    return None

def replace_guid_in_meta(meta_file, new_guid):
    try:
        with open(meta_file, 'r', encoding='utf-8') as f:
            content = f.read()
        
        old_content = content
        content = re.sub(r'guid:\s*[a-zA-Z0-9+/=]+', f'guid: {new_guid}', content)
        
        if content != old_content:
            with open(meta_file, 'w', encoding='utf-8') as f:
                f.write(content)
            return True
    except Exception as e:
        print(f"Error writing {meta_file}: {e}")
    return False

def process_with_json_mapping(assets_dir, json_file):
    print(f"Loading GUID mapping from: {json_file}")
    
    with open(json_file, 'r', encoding='utf-8') as f:
        guid_map = json.load(f)
    
    print(f"Found {len(guid_map)} asset mappings")
    
    old_to_new_guid = {}
    
    print("\n" + "=" * 60)
    print("Step 1: Replacing GUIDs in .meta files...")
    print("=" * 60)
    
    for asset_path, new_guid in guid_map.items():
        meta_file = os.path.join(assets_dir, "..", asset_path + ".meta")
        meta_file = os.path.normpath(meta_file)
        
        if not os.path.exists(meta_file):
            print(f"[SKIP] Meta file not found: {meta_file}")
            continue
        
        old_guid = get_guid_from_meta(meta_file)
        if old_guid:
            if old_guid != new_guid:
                old_to_new_guid[old_guid] = new_guid
                if replace_guid_in_meta(meta_file, new_guid):
                    print(f"[META] {meta_file}")
                    print(f"  {old_guid} -> {new_guid}")
            else:
                pass
        else:
            print(f"[WARN] No GUID found in: {meta_file}")
    
    return old_to_new_guid

def process_prefab_files(assets_dir, guid_mapping):
    print("\n" + "=" * 60)
    print(f"Step 2: Replacing GUIDs in prefab/unity/asset files...")
    print("=" * 60)
    
    prefab_files = find_all_files(assets_dir, ['.prefab', '.unity', '.asset'])
    
    total_replaced = 0
    for prefab_file in prefab_files:
        try:
            with open(prefab_file, 'r', encoding='utf-8') as f:
                content = f.read()
            
            modified = False
            for old_guid, new_guid in guid_mapping.items():
                if old_guid in content:
                    content = content.replace(old_guid, new_guid)
                    modified = True
                    total_replaced += 1
            
            if modified:
                with open(prefab_file, 'w', encoding='utf-8') as f:
                    f.write(content)
                print(f"[PREFAB] {prefab_file}")
        except Exception as e:
            print(f"Error processing {prefab_file}: {e}")
    
    print(f"\nTotal GUID references replaced: {total_replaced}")

def main():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    assets_dir = os.path.join(script_dir, 'Assets')
    json_file = os.path.join(assets_dir, 'guid_assetpath_map.json')
    
    if not os.path.exists(assets_dir):
        print(f"Assets directory not found: {assets_dir}")
        return
    
    if os.path.exists(json_file):
        print("=" * 60)
        print("Found guid_assetpath_map.json - Using JSON mapping mode")
        print("=" * 60)
        
        old_to_new_guid = process_with_json_mapping(assets_dir, json_file)
        
        if old_to_new_guid:
            process_prefab_files(assets_dir, old_to_new_guid)
        else:
            print("\nNo GUID changes needed.")
    else:
        print("=" * 60)
        print("guid_assetpath_map.json not found!")
        print("=" * 60)
        print("""
To fix Tuanjie GUIDs, you need to:

1. Open the project in Tuanjie engine
2. Create a script 'GuidToJsonExporter.cs' in Assets/Editor folder
3. Run menu: Tools -> Export Filename-GUID Mapping
4. This will generate 'Assets/guid_assetpath_map.json'
5. Run this Python script again

The C# script code:

using Newtonsoft.Json;  
using System.Collections.Generic; 
using System.IO;  
using UnityEditor; 
using UnityEngine;  
 
public class GuidToJsonExporter  
{ 
    [MenuItem("Tools/Export Filename-GUID Mapping")] 
    private static void ExportGuidFilenameMap() 
    { 
        Dictionary<string, string> guidMap = new Dictionary<string, string>(); 
        string outputPath = Path.Combine(Application.dataPath, "guid_assetpath_map.json");  
        string[] allAssets = AssetDatabase.GetAllAssetPaths(); 
         
        foreach (string assetPath in allAssets) 
        { 
            if (assetPath.EndsWith(".meta")) 
                continue; 
 
            string guid = AssetDatabase.AssetPathToGUID(assetPath);  
            if (string.IsNullOrEmpty(guid)) 
                continue;  
            guidMap[assetPath] = guid; 
        } 
 
        string jsonData = JsonConvert.SerializeObject(guidMap); 
        File.WriteAllText(outputPath, jsonData); 
 
        AssetDatabase.Refresh(); 
        Debug.Log($"GUID-Filename mapping saved to: {outputPath}"); 
    } 
}
""")
    
    print("\n" + "=" * 60)
    print("Done!")
    print("=" * 60)

if __name__ == '__main__':
    main()
