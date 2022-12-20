using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class Click_target : MonoBehaviour
{
    enum STATE_TYPE
    {
        WAIT,
        PLAY,
        ATTACK,
    }
    [SerializeField] STATE_TYPE _STATE_TYPE = STATE_TYPE.WAIT;
    STATE_TYPE _NEXT_STATE_TYPE = STATE_TYPE.WAIT;

    Coroutine _WaitTimeSet;
    [SerializeField] GameObject _Generate_Obj;
    List<GameObject> childList = new List<GameObject>();

    List<Material> _nomal_material = new List<Material>();
    Material _target_material = null;
    CPU_move _target_CPU_Move = null;

    float roundTime = 0;
    [SerializeField] float roundLimitTime = 60;
    [SerializeField] int scoreAddition = 100;
    [SerializeField] int scoreSubtraction = 10;

    bool targetCheck = false;
    bool firstGenerate = true;
    float waitTimer = 0;
    [SerializeField] float waitTime = 0.5f;
    [SerializeField] float waitTargetTime = 0.5f;
    public static bool gamePlay_flag = false;
    public static bool gameAttack_flag = false;
    public static bool gameKill_flag = true;
    public static bool gameClear_flag = false;
    [SerializeField] int generateNum = 10;

    public static int targetCounter = 0;
    public static int targetClearNum = 5;
    [SerializeField] int targetClearNum_temp=5;

    [SerializeField] LayerMask layerMask;
    [SerializeField] Vector3 generateRange = new Vector3(5, 0, 5);

    [SerializeField] GameObject AttackObj;
    [SerializeField] GameObject Eff_Hit;
    [SerializeField] GameObject Eff_Damage;
    [SerializeField] GameObject Eff_nomalHit;
    [SerializeField] GameObject Eff_smoke;
    List<GameObject> AttackObjList = new List<GameObject>();
    List<Attack_obj> _attackObjList = new List<Attack_obj>();
    GameObject camera_player;

    [SerializeField] TextMeshProUGUI textDebug;
    [SerializeField] TextMeshProUGUI textDebug2;

    public static float attackCoolTime = 1.0f;
    public static float attackCoolTimer = 0;

    bool changeRotate_flag = false;

    private void Awake()
    {
        targetClearNum = targetClearNum_temp;
    }
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
        switch (_STATE_TYPE)
        {
            case STATE_TYPE.WAIT:
                WaitUpdate();
                break;
            case STATE_TYPE.PLAY:
                PlayUpdate();
                break;
            case STATE_TYPE.ATTACK:
                AttackUpdate();
                break;
        }
        if (_STATE_TYPE != _NEXT_STATE_TYPE)
        {
            //終了処理
            switch (_STATE_TYPE)
            {
                case STATE_TYPE.ATTACK:
                    AttackEnd();
                    break;
            }

            //開始処理
            switch (_NEXT_STATE_TYPE)
            {
                case STATE_TYPE.ATTACK:
                    AttackStart();
                    break;
            }
            _STATE_TYPE = _NEXT_STATE_TYPE;
        }
    }

    void WaitUpdate()
    {
        if (Game_manager.startCheck && waitTimer <= 0 && !gameClear_flag)
        {
            _NEXT_STATE_TYPE = STATE_TYPE.PLAY;
            
            //プレイ中のフラグを立てる
            gamePlay_flag = true;
            gameKill_flag = false;
            //コルーチンが走っているときは止める
            if (_WaitTimeSet != null)
            {
                StopCoroutine(_WaitTimeSet);
                _WaitTimeSet = null;
            }

            //残っている全ての子オブジェクトを破棄
            foreach (GameObject childObj in childList)
            {
                Destroy(childObj);
            }
            if (firstGenerate)
            {
                Generate_wall.instance.GenerateFloor();
                firstGenerate = false;
            }
            //新たなオブジェクトを生成
            //向いている方向に生成(90度間隔)
            if((camera_player.transform.localEulerAngles.y>=45&& camera_player.transform.localEulerAngles.y <= 135) || (camera_player.transform.localEulerAngles.y >= 225 && camera_player.transform.localEulerAngles.y <= 315))
            {
                Vector3 newPos = new Vector3(generateRange.z, generateRange.y, generateRange.x);
                Generate_wall.instance.GenerateWall(newPos);
                changeRotate_flag = true;
            }
            else
            {
                Generate_wall.instance.GenerateWall(generateRange);
                changeRotate_flag = false;
            }
            _Generate_Obj.GetComponent<Generate_obj>().Generate(AR_test_scrippt.GENERATE_POSITION.position, changeRotate_flag, generateNum);
            


            //変数を初期化する
            childList = new List<GameObject>();
            _target_material = null;
            _nomal_material = new List<Material>();

            //タイマーをリセット
            roundTime = 0;
        }
    }
    /// <summary>
    /// ターゲットオブジェクトを触ったとき、ゲームの処理を行う
    /// </summary>
    void PlayUpdate()
    {
        roundTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedGameObject = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            Vector3 hitPos = new Vector3(0,0,0);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // オブジェクトを取得する
                clickedGameObject = hit.collider.gameObject;
                hitPos = hit.point;
            }
            else
            {
                return;
            }

            if (clickedGameObject.tag == "target")
            {
                //生成したオブジェクトを取得
                for (int i = 0; i < _Generate_Obj.transform.childCount; i++)
                {
                    childList.Add(_Generate_Obj.transform.GetChild(i).gameObject);
                    if (childList[i].transform.tag == "target")
                    {
                        //2にdemonのモデルを置く
                        _target_material = childList[i].transform.GetChild(2).GetComponent<Renderer>().material;
                        _target_CPU_Move= childList[i].transform.GetComponent<CPU_move>();
                    }
                    else
                    {
                        _nomal_material.Add(childList[i].transform.GetChild(0).GetComponent<Renderer>().material);
                    }
                }
                //正解オブジェトをタップしたときエフェクト生成
                if (Eff_Hit)
                {
                    Instantiate(Eff_Hit, hitPos, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Eff_Hitが見つかりません");
                }
                //waitにステートを遷移
                _NEXT_STATE_TYPE = STATE_TYPE.WAIT;

                _target_CPU_Move.TapChangeModel(0.25f, 0.5f, Eff_smoke);

                //待ってからATTACKに変更
                gamePlay_flag = false;
                Invoke("AttackChangeState", 3);
                
                //人を消す
                _WaitTimeSet = StartCoroutine(WaitTimeSet(waitTime));
            }
            else if (clickedGameObject.tag == "mob")
            {
                //減点する
                if (Eff_nomalHit)
                {
                    Instantiate(Eff_nomalHit, hitPos, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Eff_nomalHitが見つかりません");
                }
                Game_manager.instance.ScoreSubtraction(targetCounter);
            }
        }
    }
    void AttackChangeState()
    {
        _NEXT_STATE_TYPE = STATE_TYPE.ATTACK;
        gameAttack_flag = true;
    }
    void AttackStart()
    {
        Game_manager.instance.AttackInfoEnable();
        //壁を破棄して追いかける
        Generate_wall.instance.DestructionWall();
        ChangeRenderingType(_target_material, 1.0f);
    }
    void AttackUpdate()
    {
        roundTime += Time.deltaTime;
        
        //クールタイムで連射を防ぐ
        if (Input.GetMouseButtonDown(0) && attackCoolTimer <= 0)
        {
            //カメラ中央から前に向かって発射
            GameObject attackObj_new = Instantiate(AttackObj, camera_player.transform.position + camera_player.transform.forward , Quaternion.Euler(camera_player.transform.localEulerAngles));
            //オブジェクト、クラスを取得
            AttackObjList.Add(attackObj_new);
            _attackObjList.Add(attackObj_new.GetComponent<Attack_obj>());
            Audio_manager.instance.PlaySE("shot");
            attackCoolTimer = attackCoolTime;
        }
        if (attackCoolTimer > 0)
        {
            attackCoolTimer -= Time.deltaTime;
        }
        for(int i = 0; i < AttackObjList.Count; i++)
        {
            if (_attackObjList[i].killTarget_flag)
            {
                //attackObj系を初期化する
                for (int j = 0; j < AttackObjList.Count; j++)
                {
                    Destroy(AttackObjList[j]);
                }
                _attackObjList = new List<Attack_obj>();
                AttackObjList = new List<GameObject>();

                //ターゲットを倒したらスコア加算
                targetCounter += 1;
                Game_manager.instance.GameCounter(targetCounter);
                Game_manager.instance.ScoreAdditional((int)((roundLimitTime - roundTime) * targetCounter));

                //waitにステートを遷移
                _NEXT_STATE_TYPE = STATE_TYPE.WAIT;
                gamePlay_flag = false;
                gameAttack_flag = false;

                //ゲームのクリア処理
                if (targetCounter == targetClearNum)
                {
                    gameClear_flag = true;
                    //ターゲットが消える時間と同じだけ遅延させる
                    Game_manager.instance.GameClear(waitTime * 2);
                }
                StartCoroutine(WaitTimeTargetSet(waitTime * 2));

                return;
            }
            //当たったらオブジェクトを破棄
            if (_attackObjList[i].hitAny_flag)
            {
                //ヒット音を鳴らす
                Audio_manager.instance.PlaySE("hit");
                Destroy(AttackObjList[i]);
                _attackObjList.RemoveAt(i);
                AttackObjList.RemoveAt(i);
            }
        }
    }
    void AttackEnd()
    {
        Game_manager.instance.AttackInfoUnable();
    }
    /// <summary>
    /// 徐々にalpha値を下げる
    /// </summary>
    /// <param name="material">値を変えるマテリアル</param>
    /// <param name="nowTime">次までの時間</param>
    /// <param name="finishTime">全体時間</param>
    void ChangeMaterial_transparent(Material material, float nowTime, float finishTime)
    {
        
        float alphaNow = nowTime / finishTime;
        if (alphaNow >= 0)
        {
            Color32 newColor = new Color32((byte)(material.color.r * 255f), (byte)(material.color.g * 255f), (byte)(material.color.b * 255f), (byte)(alphaNow * 255f));
            material.SetColor("_Color", newColor);
        }
        
    }

    /// <summary>
    /// マテリアルのレンダリングタイプを変更する
    /// </summary>
    /// <param name="material">適応するマテリアル</param>
    /// <param name="RenderingNum">どのタイプにするかの番号</param>
    void ChangeRenderingType(Material material, float alphaValue = 0.5f, int RenderingNum = 2)
    {
        //0:Opaque,1:Cutout,2:Fade,3:Transparent
        material.SetFloat("_Mode", RenderingNum);
        material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        material.SetColor("_Color", new Color(1, 1, 1, alphaValue));
    }
    /// <summary>
    /// 次のオブジェクトに触れるまでのクールタイム
    /// <param name="wait">止めておく時間(sec)</param>
    /// <param name="waitTransparentRate">オブジェクトを透明にしてからの待ち時間倍率</param>
    /// </summary>
    IEnumerator WaitTimeSet(float wait,float waitTransparentRate=2f)
    {
        //waitTimerが0にならないようにする,使いまわしやめたほうがいいかも
        waitTimer = wait + 0.5f;
        float waitTimer_transparent = wait;
        foreach (Material nomalMaterial in _nomal_material)
        {
            ChangeRenderingType(nomalMaterial);
        }
        while (waitTimer > 0.5f)
        {
            //ターゲット以外を徐々に透明にする
            foreach(Material nomalMaterial in _nomal_material)
            {
                ChangeMaterial_transparent(nomalMaterial, waitTimer_transparent, wait);
            }
            waitTimer -= Time.deltaTime;
            waitTimer_transparent -= Time.deltaTime * waitTransparentRate;
            yield return null;
        }
        foreach (GameObject childObj in childList)
        {
            //透明にしたらターゲット以外を破棄
            if (childObj.transform.tag != "target")
            {
                Destroy(childObj);
            }
        }
    }
    IEnumerator WaitTimeTargetSet(float wait)
    {
        waitTimer = wait;
        float waitTimer_transparent = wait;
        bool typeCheck = false;
        while (waitTimer > 0)
        {
            //waitTimeが半分経過したら透明化を始める
            if (waitTimer < wait * 0.5f)
            {
                if (!typeCheck)
                {
                    ChangeRenderingType(_target_material);
                }
                waitTimer_transparent -= Time.deltaTime * 2f;
                ChangeMaterial_transparent(_target_material, waitTimer_transparent, wait);
            }

            waitTimer -= Time.deltaTime;
            yield return null;
        }
        gameKill_flag = true;
        foreach (GameObject childObj in childList)
        {
            Destroy(childObj);
        }
        yield break;
    }
    IEnumerator testBall()
    {
        while (true)
        {
            Instantiate(AttackObj, transform.forward, transform.rotation);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
