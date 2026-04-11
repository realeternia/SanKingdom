using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    public float dissolveDuration = 2.5f;
    public Color edgeColor = new Color(1f, 0.5f, 0f, 1f);
    
    private Material dissolveMaterial;
    private Renderer[] renderers;
    private Material[] originalMaterials;
    private Coroutine dissolveCoroutine;

    private static Shader dissolveShader;
    private static Texture2D defaultDissolveTex;

    void Awake()
    {
        if (dissolveShader == null)
        {
            dissolveShader = Shader.Find("Custom/DissolveShader");
        }
        
        if (defaultDissolveTex == null)
        {
            defaultDissolveTex = GenerateNoiseTexture();
        }
    }

    public void StartDissolve()
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
        }
        dissolveCoroutine = StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        renderers = GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        originalMaterials = new Material[renderers.Length];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterial;
            
            Material newMat = new Material(dissolveShader);
            
            if (originalMaterials[i] != null && originalMaterials[i].mainTexture != null)
            {
                newMat.mainTexture = originalMaterials[i].mainTexture;
            }
            
            newMat.SetTexture("_DissolveTex", defaultDissolveTex);
            newMat.SetColor("_DissolveEdgeColor", edgeColor);
            newMat.SetFloat("_DissolveAmount", 0f);
            newMat.SetFloat("_DissolveEdgeWidth", 0.1f);
            
            if (originalMaterials[i] != null && originalMaterials[i].HasProperty("_Color"))
            {
                newMat.SetColor("_Color", originalMaterials[i].GetColor("_Color"));
            }
            
            renderers[i].material = newMat;
        }

        float elapsedTime = 0f;
        
        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            float dissolveAmount = Mathf.Clamp01(elapsedTime / dissolveDuration);
            
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && renderers[i].material != null)
                {
                    renderers[i].material.SetFloat("_DissolveAmount", dissolveAmount);
                }
            }
            
            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                Destroy(renderers[i].material);
            }
        }

        Destroy(gameObject);
    }

    private Texture2D GenerateNoiseTexture()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noise = 0f;
                float scale = 0.05f;
                
                noise += Mathf.PerlinNoise(x * scale, y * scale) * 0.5f;
                noise += Mathf.PerlinNoise(x * scale * 2f, y * scale * 2f) * 0.25f;
                noise += Mathf.PerlinNoise(x * scale * 4f, y * scale * 4f) * 0.125f;
                
                tex.SetPixel(x, y, new Color(noise, noise, noise, 1f));
            }
        }
        
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    void OnDestroy()
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
        }
    }
}
