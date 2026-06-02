using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    public GameObject WallNode;
    public GameObject RegionNode;

    public int Mapid;
    // Start is called before the first frame update
    void Start()
    {

        foreach(Transform wall in WallNode.transform)
        {
            wall.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
