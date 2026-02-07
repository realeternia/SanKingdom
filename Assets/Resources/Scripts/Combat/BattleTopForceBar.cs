using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleForceBar : MonoBehaviour
{  
    public RectTransform cover;
    public RectTransform bg;
    public Image forceImg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRate(float v)
    {
        v = 1-v;
        cover.sizeDelta = new Vector2(Math.Max(1, bg.sizeDelta.x * v), bg.sizeDelta.y);
        UnityEngine.Debug.Log($"SetRate {v}");
    }
}
