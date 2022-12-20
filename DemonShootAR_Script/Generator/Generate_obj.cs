using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate_obj : MonoBehaviour
{
    [SerializeField] GameObject[] nomal_obj;
    [SerializeField] GameObject[] target_obj;
    [SerializeField] GameObject Eff_smoke;
    [SerializeField] Vector2 generatePosStart, generatePosFinish;
    public List<Material> _materials_nomal = new List<Material>();
    public Material _material_target;
    GameObject camera_obj;
    Vector3 cameraPos;
    [SerializeField] Vector3 generateAreaRange = new Vector3(1, 0, 1);
    static public int targetNumNow = 0;

    // Start is called before the first frame update
    void Start()
    {
        camera_obj = GameObject.Find("AR Session Origin").transform.GetChild(0).gameObject;
        if (!camera_obj)
        {
            camera_obj = GameObject.Find("Main Camera");
        }
    }

    public void Generate(Vector3 generatePos,bool changeRotate_flag, int generateNum = 10, float delayTime = 0.3f)
    {
        cameraPos = camera_obj.transform.position;

        //SEとエフェクトを発生させる
        Audio_manager.instance.PlaySE("generate_bomb");
        if (Eff_smoke)
        {
            GameObject Eff_smoke_instance = Instantiate(Eff_smoke, cameraPos + camera_obj.transform.forward * 2, Quaternion.Euler(camera_obj.transform.localEulerAngles));
            //エフェクトとカメラの位置を同期させる
            Eff_smoke_instance.transform.parent = camera_obj.transform;
        }
        else
        {
            Debug.Log("Eff_smokeが見つかりません。");
        }

        //生成をエフェクト分遅らせる
        StartCoroutine(DelayGenerate(generateNum,changeRotate_flag, delayTime,generatePos));
    }
    void RandGenerate(GameObject generateObj, bool changeRotate_flag ,Vector3 generatePos)
    {
        Vector3 generatePosNow = new Vector3(Random.Range(generatePos.x+generateAreaRange.x, generatePos.x - generateAreaRange.x), 0f, Random.Range(generatePos.z + generateAreaRange.z, generatePos.z - generateAreaRange.z));
        if (changeRotate_flag)
        {
            generatePosNow = new Vector3(generatePosNow.z, generatePosNow.y, generatePosNow.x);
        }
        Quaternion generateRotateNow = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        
        GameObject nomalObj = Instantiate(generateObj, generatePosNow, generateRotateNow);
        nomalObj.transform.parent = this.transform;
    }
    IEnumerator DelayGenerate(int generateNum, bool changeRotate_flag, float delayTime, Vector3 generatePos)
    {
        //次のターゲットオブジェクトを決定
        int tempTargetNumNow = Random.Range(0, target_obj.Length);
        if (tempTargetNumNow != targetNumNow)
        {
            targetNumNow = tempTargetNumNow;
        }
        else
        {
            while(targetNumNow == tempTargetNumNow)
            {
                tempTargetNumNow = Random.Range(0, target_obj.Length);
            }
            Debug.Log("変えた");
            targetNumNow = tempTargetNumNow;
        }
        targetNumNow = Random.Range(0, target_obj.Length);
        yield return new WaitForSeconds(delayTime);
        for (int i = 0; i < generateNum; i++)
        {
            int nomal_num = Random.Range(0, nomal_obj.Length);
            RandGenerate(nomal_obj[nomal_num],changeRotate_flag,generatePos);
        }
        RandGenerate(target_obj[targetNumNow], changeRotate_flag, generatePos);
        Debug.Log("target num : " + targetNumNow.ToString());
        yield break;
    }
}
