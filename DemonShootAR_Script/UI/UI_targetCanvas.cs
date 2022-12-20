using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_targetCanvas : MonoBehaviour
{
    GameObject camera_player = null;
    // Start is called before the first frame update
    void Start()
    {
        camera_player = GameObject.Find("AR Session Origin").transform.GetChild(0).gameObject;
        if (camera_player == null)
        {
            camera_player = GameObject.Find("Main Camera");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera_player.transform.rotation;
    }
}
