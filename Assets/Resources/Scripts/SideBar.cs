using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBar : MonoBehaviour
{
    public GameObject scrollItem;
    public Image scrollImg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShow()
    {
    }
    public void OnHide()
    {
        scrollItem.SetActive(false);
    }

}
