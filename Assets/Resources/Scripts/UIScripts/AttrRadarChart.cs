using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttrRadarChart : MonoBehaviour
{
    [Header("五边形设置")]
    public int maxValue = 100;
    public float radius = 50f;
    public Color gridColor = new Color(1f, 1f, 1f, 0.3f);
    public float gridLineWidth = 2f;
    public Color fillColor = new Color(1f, 0.65f, 0f, 0.6f);
    public Color lineColor = new Color(1f, 0.8f, 0f, 1f);
    public float lineWidth = 2f;

    [Header("五维属性值")]
    public int leadShip;  // 统帅
    public int str;       // 武力
    public int inte;      // 智力
    public int fair;      // 政治
    public int charm;     // 魅力

    [Header("标签")]
    public Color labelColor = Color.white;
    public int fontSize = 12;

    private RectTransform rectTransform;
    private GameObject gridContainer;
    private GameObject fillContainer;
    private GameObject labelContainer;

    private string[] attrNames = { "统帅", "武力", "智力", "政治", "魅力" };

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetAttrValues(int leadShip, int str, int inte, int fair, int charm)
    {
        this.leadShip = Mathf.Clamp(leadShip, 0, maxValue);
        this.str = Mathf.Clamp(str, 0, maxValue);
        this.inte = Mathf.Clamp(inte, 0, maxValue);
        this.fair = Mathf.Clamp(fair, 0, maxValue);
        this.charm = Mathf.Clamp(charm, 0, maxValue);

        DrawRadarChart();
    }

    private void DrawRadarChart()
    {
        ClearChart();

        gridContainer = new GameObject("GridContainer");
        gridContainer.transform.SetParent(transform);
        gridContainer.transform.localPosition = Vector3.zero;
        gridContainer.transform.localScale = Vector3.one;

        fillContainer = new GameObject("FillContainer");
        fillContainer.transform.SetParent(transform);
        fillContainer.transform.localPosition = Vector3.zero;
        fillContainer.transform.localScale = Vector3.one;

        labelContainer = new GameObject("LabelContainer");
        labelContainer.transform.SetParent(transform);
        labelContainer.transform.localPosition = Vector3.zero;
        labelContainer.transform.localScale = Vector3.one;

        DrawGrid();
        DrawFill();
        DrawLabels();
    }

    private void ClearChart()
    {
        if (gridContainer != null)
            Destroy(gridContainer);
        if (fillContainer != null)
            Destroy(fillContainer);
        if (labelContainer != null)
            Destroy(labelContainer);
    }

    private void DrawGrid()
    {
        Vector3[] vertices = GetPentagonVertices(radius);

        for (int i = 0; i < 5; i++)
        {
            float ratio = (i + 1) / 5f;
            Vector3[] innerVertices = GetPentagonVertices(radius * ratio);
            for (int j = 0; j < 5; j++)
            {
                DrawLine(innerVertices[j], innerVertices[(j + 1) % 5], gridColor, gridLineWidth, gridContainer.transform);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            DrawLine(Vector3.zero, vertices[i], gridColor, gridLineWidth, gridContainer.transform);
        }
    }

    private void DrawFill()
    {
        float[] values = { leadShip, str, inte, fair, charm };
        Vector3[] pentagonVertices = GetPentagonVertices(radius);
        Vector3[] fillVertices = new Vector3[5];

        for (int i = 0; i < 5; i++)
        {
            float ratio = values[i] / (float)maxValue;
            fillVertices[i] = pentagonVertices[i] * ratio;
        }

        for (int i = 0; i < 5; i++)
        {
            DrawLine(fillVertices[i], fillVertices[(i + 1) % 5], lineColor, lineWidth, fillContainer.transform);
        }

        FillPolygon(fillVertices, fillColor);
    }

    private void DrawLabels()
    {
        Vector3[] vertices = GetPentagonVertices(radius);
        float[] values = { leadShip, str, inte, fair, charm };

        for (int i = 0; i < 5; i++)
        {
            GameObject labelObj = new GameObject("Label_" + attrNames[i]);
            labelObj.transform.SetParent(labelContainer.transform);
            labelObj.transform.localPosition = vertices[i] * 1.15f;
            labelObj.transform.localScale = Vector3.one;

            TMP_Text labelText = labelObj.AddComponent<TextMeshProUGUI>();
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>(ResPath.Font.HeiTiSDF());
            labelText.font = font != null ? font : TMP_Settings.defaultFontAsset;
            labelText.text = attrNames[i] + "\n" + values[i].ToString();
            labelText.fontSize = fontSize;
            labelText.color = labelColor;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.enableWordWrapping = false;

            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(60, 40);
        }
    }

    private Vector3[] GetPentagonVertices(float r)
    {
        Vector3[] vertices = new Vector3[5];
        float angle = -90f;

        for (int i = 0; i < 5; i++)
        {
            float rad = angle * Mathf.Deg2Rad;
            vertices[i] = new Vector3(Mathf.Cos(rad) * r, Mathf.Sin(rad) * r, 0);
            angle += 72f;
        }

        return vertices;
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, float width, Transform parent)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(parent);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localScale = Vector3.one;

        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = color;

        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        
        Vector3 direction = end - start;
        float length = direction.magnitude;
        
        lineRect.sizeDelta = new Vector2(length, width);
        
        Vector3 midPoint = (start + end) / 2;
        lineRect.localPosition = midPoint;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void FillPolygon(Vector3[] vertices, Color color)
    {
        if (vertices.Length < 3)
            return;

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillContainer.transform);
        fillObj.transform.localPosition = Vector3.zero;
        fillObj.transform.localScale = Vector3.one;

        CanvasRenderer canvasRenderer = fillObj.AddComponent<CanvasRenderer>();

        Mesh mesh = new Mesh();
        mesh.vertices = CreatePentagonMeshVertices(vertices);
        mesh.triangles = CreatePentagonTriangles(vertices.Length);
        mesh.colors = CreatePentagonColors(color, vertices.Length + 1);

        Material material = new Material(Shader.Find("UI/Default"));
        material.color = color;

        canvasRenderer.SetMesh(mesh);
        canvasRenderer.SetMaterial(material, null);
    }

    private Vector3[] CreatePentagonMeshVertices(Vector3[] vertices)
    {
        Vector3[] meshVertices = new Vector3[vertices.Length + 1];
        meshVertices[0] = Vector3.zero;
        for (int i = 0; i < vertices.Length; i++)
        {
            meshVertices[i + 1] = vertices[i];
        }
        return meshVertices;
    }

    private int[] CreatePentagonTriangles(int vertexCount)
    {
        int[] triangles = new int[vertexCount * 3];
        for (int i = 0; i < vertexCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = ((i + 1) % vertexCount) + 1;
        }
        return triangles;
    }

    private Color[] CreatePentagonColors(Color color, int colorCount)
    {
        Color[] colors = new Color[colorCount];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        return colors;
    }

    private void OnDestroy()
    {
        DestroyContainer(gridContainer);
        DestroyContainer(fillContainer);
        DestroyContainer(labelContainer);
    }

    private void DestroyContainer(GameObject container)
    {
        if (container == null) return;
        if (Application.isPlaying)
            Destroy(container);
        else
            DestroyImmediate(container);
    }
}

