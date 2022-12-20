using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_changeScene : MonoBehaviour
{
    public void Select_Title()
    {
        Scene_manager.instance.loadTitle();
    }
    public void Select_Main()
    {
        Scene_manager.instance.loadGame();
    }
    public void Load_Scene()
    {
        Scene_manager.instance.startFadeOut();
    }
}
