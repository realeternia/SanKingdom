using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ObjUVGenerator : EditorWindow
{
    private string scanPath = "Assets/Resources/Models";
    private List<string> objFilesWithoutUV = new List<string>();
    private Vector2 scrollPosition;
    private UVProjectionType defaultProjection = UVProjectionType.Cylindrical;

    private enum UVProjectionType
    {
        Cylindrical,
        Cone,
        Planar
    }

    [MenuItem("Tools/UV Generator for OBJ")]
    public static void ShowWindow()
    {
        GetWindow<ObjUVGenerator>("OBJ UV Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("OBJ UV Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Scan Path:", EditorStyles.label);
        scanPath = EditorGUILayout.TextField(scanPath);

        if (GUILayout.Button("Select Folder"))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Models Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.StartsWith(Application.dataPath))
                {
                    scanPath = "Assets" + selected.Substring(Application.dataPath.Length);
                }
                else
                {
                    scanPath = selected;
                }
            }
        }

        GUILayout.Space(10);

        defaultProjection = (UVProjectionType)EditorGUILayout.EnumPopup("Default Projection:", defaultProjection);

        GUILayout.Space(10);

        if (GUILayout.Button("Scan for OBJ Files Without UV"))
        {
            ScanForObjFilesWithoutUV();
        }

        GUILayout.Space(10);

        if (objFilesWithoutUV.Count > 0)
        {
            GUILayout.Label($"Found {objFilesWithoutUV.Count} OBJ file(s) without UV:", EditorStyles.boldLabel);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            foreach (string file in objFilesWithoutUV)
            {
                GUILayout.Label(file);
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("Generate UV for All"))
            {
                GenerateUVForAllFiles();
            }
        }
        else
        {
            GUILayout.Label("Click 'Scan' to check for OBJ files without UV.", EditorStyles.helpBox);
        }
    }

    private void ScanForObjFilesWithoutUV()
    {
        objFilesWithoutUV.Clear();

        string fullPath = scanPath;
        if (scanPath.StartsWith("Assets"))
        {
            fullPath = Path.Combine(Application.dataPath, scanPath.Substring(7));
        }

        if (!Directory.Exists(fullPath))
        {
            EditorUtility.DisplayDialog("Error", $"Directory not found: {fullPath}", "OK");
            return;
        }

        string[] objFiles = Directory.GetFiles(fullPath, "*.obj", SearchOption.AllDirectories);

        foreach (string objFile in objFiles)
        {
            if (!HasUVCoordinates(objFile))
            {
                string relativePath = objFile;
                if (objFile.StartsWith(Application.dataPath))
                {
                    relativePath = "Assets" + objFile.Substring(Application.dataPath.Length);
                }
                objFilesWithoutUV.Add(relativePath);
            }
        }

        Debug.Log($"Scan complete. Found {objFilesWithoutUV.Count} OBJ file(s) without UV coordinates.");
    }

    private bool HasUVCoordinates(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (trimmed.StartsWith("vt "))
            {
                return true;
            }
        }
        return false;
    }

    private void GenerateUVForAllFiles()
    {
        int processed = 0;
        foreach (string relativePath in objFilesWithoutUV)
        {
            string fullPath = relativePath;
            if (relativePath.StartsWith("Assets"))
            {
                fullPath = Path.Combine(Application.dataPath, relativePath.Substring(7));
            }

            if (File.Exists(fullPath))
            {
                ProcessObjFile(fullPath);
                processed++;
                Debug.Log($"Generated UV for: {relativePath}");
            }
        }

        AssetDatabase.Refresh();
        objFilesWithoutUV.Clear();
        EditorUtility.DisplayDialog("Complete", $"Generated UV for {processed} file(s).", "OK");
    }

    private void ProcessObjFile(string filePath)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Face> faces = new List<Face>();
        List<string> headerLines = new List<string>();

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (trimmed.StartsWith("v "))
            {
                string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    vertices.Add(new Vector3(x, y, z));
                }
            }
            else if (trimmed.StartsWith("f "))
            {
                string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Face face = new Face();
                for (int i = 1; i < parts.Length && i <= 3; i++)
                {
                    string[] indices = parts[i].Split('/');
                    face.vertexIndices[i - 1] = int.Parse(indices[0]) - 1;
                }
                faces.Add(face);
            }
            else if (trimmed.StartsWith("#") || trimmed.StartsWith("o "))
            {
                headerLines.Add(trimmed);
            }
        }

        UVProjectionType projection = DetermineProjectionType(Path.GetFileNameWithoutExtension(filePath));
        CalculateUVs(vertices, uvs, projection);

        WriteObjFile(filePath, headerLines, vertices, uvs, faces);
    }

    private UVProjectionType DetermineProjectionType(string modelName)
    {
        string name = modelName.ToLower();

        if (name.Contains("hat") || name.Contains("cap") || name.Contains("cone"))
        {
            return UVProjectionType.Cone;
        }
        else if (name.Contains("bow") || name.Contains("flag") || name.Contains("banner"))
        {
            return UVProjectionType.Planar;
        }
        else
        {
            return defaultProjection;
        }
    }

    private void CalculateUVs(List<Vector3> vertices, List<Vector2> uvs, UVProjectionType projectionType)
    {
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (Vector3 v in vertices)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        Vector3 center = (min + max) * 0.5f;

        uvs.Clear();
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            Vector2 uv;

            switch (projectionType)
            {
                case UVProjectionType.Cone:
                    uv = CalculateConeUV(v, min, max, center);
                    break;
                case UVProjectionType.Planar:
                    uv = CalculatePlanarUV(v, min, max);
                    break;
                default:
                    uv = CalculateCylindricalUV(v, min, max, center);
                    break;
            }

            uvs.Add(uv);
        }
    }

    private Vector2 CalculateConeUV(Vector3 v, Vector3 min, Vector3 max, Vector3 center)
    {
        float heightRange = max.y - min.y;
        float normalizedHeight = (v.y - min.y) / (heightRange > 0.001f ? heightRange : 0.001f);

        float angle = Mathf.Atan2(v.z - center.z, v.x - center.x);
        float u = (angle + Mathf.PI) / (2 * Mathf.PI);

        return new Vector2(u, normalizedHeight);
    }

    private Vector2 CalculatePlanarUV(Vector3 v, Vector3 min, Vector3 max)
    {
        float u = (v.x - min.x) / (max.x - min.x > 0.001f ? max.x - min.x : 0.001f);
        float vCoord = (v.y - min.y) / (max.y - min.y > 0.001f ? max.y - min.y : 0.001f);

        return new Vector2(u, vCoord);
    }

    private Vector2 CalculateCylindricalUV(Vector3 v, Vector3 min, Vector3 max, Vector3 center)
    {
        float angle = Mathf.Atan2(v.z - center.z, v.x - center.x);
        float u = (angle + Mathf.PI) / (2 * Mathf.PI);

        float heightRange = max.y - min.y;
        float vCoord = (v.y - min.y) / (heightRange > 0.001f ? heightRange : 0.001f);

        return new Vector2(u, vCoord);
    }

    private void WriteObjFile(string filePath, List<string> headerLines, List<Vector3> vertices, List<Vector2> uvs, List<Face> faces)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string header in headerLines)
        {
            sb.AppendLine(header);
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            sb.AppendLine($"v {v.x} {v.y} {v.z}");
        }

        sb.AppendLine();

        for (int i = 0; i < uvs.Count; i++)
        {
            Vector2 uv = uvs[i];
            sb.AppendLine($"vt {uv.x.ToString("F6")} {uv.y.ToString("F6")}");
        }

        sb.AppendLine();

        foreach (Face face in faces)
        {
            int v0 = face.vertexIndices[0] + 1;
            int v1 = face.vertexIndices[1] + 1;
            int v2 = face.vertexIndices[2] + 1;

            sb.AppendLine($"f {v0}/{v0} {v1}/{v1} {v2}/{v2}");
        }

        File.WriteAllText(filePath, sb.ToString());
    }

    private class Face
    {
        public int[] vertexIndices = new int[3];
    }
}
