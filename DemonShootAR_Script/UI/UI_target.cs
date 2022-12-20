using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_target : MonoBehaviour
{
    [SerializeField] Image targetImg;
    [SerializeField] Sprite[] targetSprites;
    bool imgChangeCheck = false;
    int _targetCounter = -1;
    int _targetNumNow = 0;
    // Start is called before the first frame update
    void Start()
    {
        _targetNumNow = Generate_obj.targetNumNow;
    }

    // Update is called once per frame
    void Update()
    {
        //クリアしておらずカウントが更新されたとき
        if (!Click_target.gameClear_flag && _targetNumNow != Generate_obj.targetNumNow)
        {
            Debug.Log("rand image start");
            _targetNumNow = Generate_obj.targetNumNow;
            StartCoroutine(ChangeImg());
        }
        _targetCounter = Click_target.targetCounter;

        
    }
    /// <summary>
    /// 次のターゲットの画像をランダムに決めている演出
    /// </summary>
    /// <param name="randTime">画像切り替えをする時間</param>
    /// <param name="randInterval">切り替える間隔</param>
    /// <param name="flashNum">切り替え完了後点滅させる回数</param>
    /// <param name="flashInterval">切り替え完了後点滅させる間隔</param>
    /// <returns></returns>
    IEnumerator ChangeImg(float randTime = 1.0f , float randInterval = 0.1f, int flashNum = 3, float flashInterval = 0.25f)
    {
        imgChangeCheck = true;
        int targetRandNum = Random.Range(0, targetSprites.Length);
        int targetRandNum_pre = 999;
        float randTimer = 0;
        //WAITでクリアしていないとき
        while (randTimer<randTime && !Click_target.gameClear_flag)
        {
            randTimer += randInterval;
            //前回と同じ画像になったとき再抽選
            while (targetRandNum == targetRandNum_pre)
            {
                if (targetSprites.Length == 1)
                {
                    Debug.Log("スプライトを2枚以上挿入してください。");
                    break;
                }
                targetRandNum = Random.Range(0, targetSprites.Length);
            }
            //値を保存しておく
            targetRandNum_pre = targetRandNum;
            targetImg.sprite = targetSprites[Random.Range(0,targetSprites.Length)];
            yield return new WaitForSeconds(randInterval);
        }
        //決った最新のtargetの画像を入れる
        targetImg.sprite = targetSprites[Generate_obj.targetNumNow];
        imgChangeCheck = false;

        //ターゲット画像を点滅させる
        for(int i = 0; i < flashNum; i++)
        {
            yield return new WaitForSeconds(flashInterval);
            targetImg.enabled = true;
            yield return new WaitForSeconds(flashInterval);
            targetImg.enabled = false;
        }
        targetImg.enabled = true;
        yield break;
    }
}
