using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectHeroControl : MonoBehaviour
{
    public Image heroHead1;
    public Image heroHead2;
    public Image heroHead3;
    public Button confirmButton;


    // Start is called before the first frame update
    void Start()
    {
        heroHead2.gameObject.SetActive(false);
        heroHead3.gameObject.SetActive(false);
        heroHead1.sprite = Resources.Load<Sprite>("Skins/moren");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
