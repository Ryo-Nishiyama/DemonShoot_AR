using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_testStartCheck : MonoBehaviour
{
    TextMeshProUGUI txt;
    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game_manager.startCheck)
        {
            txt.text = "START!";
        }
    }
}
