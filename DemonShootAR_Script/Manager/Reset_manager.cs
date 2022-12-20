using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Game_manager.startCheck = false;
        AR_test_scrippt.GENERATE_POSITION = new Pose();
        Click_target.gameClear_flag = false;
        Click_target.gamePlay_flag = false;
        Click_target.targetCounter = 0;
    }
}
