using UnityEngine;
using UnityEngine.UI;

public class UIColorChanger : MonoBehaviour
{
    public Image targetImage;
    
    // 将黄色UI变为绿色
    public void ChangeYellowToGreen()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
            
        // 方法1：直接设置颜色（乘法混合）
        targetImage.color = new Color(0.5f, 1f, 0.5f, 1f); // 偏绿色调
        
        // 或使用方法2：使用预设颜色
        targetImage.color = Color.green;
    }
    
    // 还原颜色
    public void ResetColor()
    {
        targetImage.color = Color.white;
    }
    
    // 动态颜色过渡
    public void ChangeColorWithTransition(Color targetColor, float duration = 1f)
    {
        StartCoroutine(ColorTransition(targetColor, duration));
    }
    
    private System.Collections.IEnumerator ColorTransition(Color targetColor, float duration)
    {
        Color startColor = targetImage.color;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            targetImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        
        targetImage.color = targetColor;
    }
}