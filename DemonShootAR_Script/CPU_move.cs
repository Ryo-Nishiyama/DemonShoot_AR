using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CPU_move : MonoBehaviour
{
    public enum STATE_CPU
    {
        WAIT,
        WALK,
        RUN,
        CHASE,
        DEAD,
    }
    public STATE_CPU _STATE_CPU = STATE_CPU.WAIT;
    private STATE_CPU _NEXT_STATE_CPU = STATE_CPU.WAIT;
    Coroutine col_flashCPU = null;
    Coroutine col_startMove = null;
    GameObject camera_player = null;
    Material _material;
    Renderer _renderer;

    Color colorOriginal;
    float alphaValue = 150;

    Color32 color_HP_half = new Color32(255, 255, 0, 255);
    Color32 color_HP_quarter = new Color32(255, 0, 0, 255);

    bool col_startMoveCheck = false;
    bool wallCheck = false;
    [SerializeField] Image HP_bar,HP_bar_back;
    [SerializeField] bool demonCheck = false;
    [SerializeField] float changeStateTime = 10;
    [SerializeField] int cpuHP = 100;
    int cpuHP_max;

    //CPUの行動割合
    [SerializeField, Range(0.0f, 1.0f)] float actionRateSet_Wait;
    [SerializeField, Range(0.0f, 1.0f)] float actionRateSet_Walk;
    [SerializeField, Range(0.0f, 1.0f)] float actionRateSet_Run;
    List<float> actionRate = new List<float>();
    //trueにするとランダムで割合を決定する
    [SerializeField] bool actionSetRandCheck = false;

    //walk,run状態の速度
    [SerializeField] float speed_walk = 0.005f;
    [SerializeField] float speed_run = 0.0075f;
    [SerializeField] float speed_chase = 0.0075f;
    [SerializeField] float rotate_chase = 1.0f;

    [SerializeField] GameObject Eff_dead;

    public float damageCoolTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        actionRate.Add(actionRateSet_Wait);
        actionRate.Add(actionRateSet_Walk);
        actionRate.Add(actionRateSet_Run);
        if (actionSetRandCheck)
        {
            for(int i = 0; i < actionRate.Count; i++)
            {
                actionRate[i] = Random.Range(0.0f, 1.0f);
            }
        }

        float actionRateSum = actionRate.Sum();
        //合計の値を1に補正する
        for (int i = 0; i < actionRate.Count; i++)
        {
            actionRate[i] /= actionRateSum;
        }
        camera_player = GameObject.Find("AR Session Origin").transform.GetChild(0).gameObject;
        if (camera_player == null)
        {
            camera_player = GameObject.Find("Main Camera");
        }
        if (transform.tag == "target")
        {
            _renderer = this.gameObject.transform.GetChild(2).gameObject.GetComponent<Renderer>();
        }
        else
        {
            _renderer = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        }
        colorOriginal = _renderer.material.color;

        cpuHP_max = cpuHP;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWaitState(Click_target.gameAttack_flag,Click_target.gamePlay_flag);
        ChangeState();

        if (damageCoolTime > 0)
        {
            damageCoolTime -= Time.deltaTime;
            if (damageCoolTime <= 0)
            {
                damageCoolTime = 0;
                //色を元に戻す
                if (col_flashCPU != null)
                {
                    StopCoroutine(col_flashCPU);
                    col_flashCPU = null;
                }
                _renderer.material.color = new Color32((byte)(_renderer.material.color.r * 255), (byte)(_renderer.material.color.g * 255), (byte)(_renderer.material.color.b * 255), 255);
                return;
            }
            if (col_flashCPU == null)
            {
                col_flashCPU = StartCoroutine(FlashCPU());
            }
        }
    }
    void GenerateObj(GameObject obj)
    {
        GameObject eff = Instantiate(obj, transform.position, Quaternion.identity);
        eff.transform.parent = this.transform;
    }
    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="coolTime">攻撃後無敵時間</param>
    /// <param name="damage">ダメージ量</param>
    public bool TapDamage(GameObject eff = null, float coolTime = 0.5f, int damage=10)
    {
        if (damageCoolTime == 0)
        {
            if (eff != null)
            {
                GenerateObj(eff);
            }
            
            cpuHP -= damage;
            Debug.Log("残りHP : " + cpuHP.ToString());

            float HP_ratio = (float)cpuHP / (float)cpuHP_max;
            HP_bar.fillAmount = HP_ratio;
            if (HP_ratio <= 0.25f)
            {
                HP_bar.color = color_HP_quarter;
            }
            else if (HP_ratio <= 0.5f)
            {
                HP_bar.color = color_HP_half;
            }

            //HPが残っているとき攻撃クールタイムをセットする
            if (cpuHP > 0)
            {
                damageCoolTime = coolTime;
            }
        }
        else
        {
            Debug.Log("クールタイム中です。 残り : " + damageCoolTime.ToString() + "(s)");
        }
        if (cpuHP <= 0)
        {
            _NEXT_STATE_CPU = STATE_CPU.DEAD;
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 待ち時間の管理
    /// </summary>
    /// <param name="playCheck">プレイ中かを確認するフラグ</param>
    void ChangeWaitState(bool attackCheck ,bool playCheck)
    {
        if (attackCheck)
        {
            if(col_startMove != null)
            {
                StopCoroutine(col_startMove);
                col_startMove = null;
            }
            //悪魔ならCHASE
            if (demonCheck)
            {
                _NEXT_STATE_CPU = STATE_CPU.CHASE;
            }
            //人ならWAITに遷移
            else
            {
                _NEXT_STATE_CPU = STATE_CPU.WAIT;
            }
        }
        else
        {
            //待ち時間中はWAITにする
            if (col_startMoveCheck && !playCheck)
            {
                _NEXT_STATE_CPU = STATE_CPU.WAIT;
                if (col_startMove != null)
                {
                    StopCoroutine(col_startMove);
                    col_startMove = null;
                }
                col_startMoveCheck = false;
            }
            //WAIT状態で待ちが解除されたとき動かす
            else if (!col_startMoveCheck && playCheck)
            {
                col_startMove = StartCoroutine(ChangeMoveState(changeStateTime));
                col_startMoveCheck = true;
            }
        }
        
    }
    public void TapChangeModel(float tapInterval, float smokeInterval, GameObject eff_smoke = null)
    {
        StartCoroutine(ChangeModel(tapInterval, smokeInterval, eff_smoke));
    }
    IEnumerator ChangeModel(float tapInterval,float smokeInterval, GameObject eff_smoke=null)
    {
        //タップされてから少し時間を置く
        yield return new WaitForSeconds(tapInterval);
        if (eff_smoke != null)
        {
            Vector3 effPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Instantiate(eff_smoke, effPos, Quaternion.Euler(eff_smoke.transform.localEulerAngles));
        }
        //煙エフェクトが上に来るまで待つ
        yield return new WaitForSeconds(smokeInterval);
        //生成したオブジェクトを取得
        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            childList.Add(this.transform.GetChild(i).gameObject);
            //人のモデル、ボーンを非表示
            if (i <= 1)
            {
                childList[i].SetActive(false);
            }
            //demonのモデル、ボーン、HPを表示
            else
            {
                childList[i].SetActive(true);
            }
        }
        yield break;
    }
    /// <summary>
    /// ステートの切り替え
    /// </summary>
    void ChangeState()
    {
        switch (_STATE_CPU)
        {
            case STATE_CPU.WAIT:
                WaitUpdate();
                break;
            case STATE_CPU.WALK:
                WalkUpdate(speed_walk);
                break;
            case STATE_CPU.RUN:
                RunUpdate(speed_run);
                break;
            case STATE_CPU.CHASE:
                ChaseUpdate(speed_chase);
                break;
            case STATE_CPU.DEAD:
                DeadUpdate();
                break;
        }
        if (_NEXT_STATE_CPU != _STATE_CPU)
        {
            //終了処理


            //開始処理
            switch (_NEXT_STATE_CPU)
            {
                case STATE_CPU.WAIT:
                    WaitStart();
                    break;
                case STATE_CPU.WALK:
                    WalkStart();
                    break;
                case STATE_CPU.RUN:
                    RunStart();
                    break;
                case STATE_CPU.CHASE:
                    ChaseStart();
                    break;
                case STATE_CPU.DEAD:
                    DeadStart();
                    break;
            }
            //ステートを変更
            _STATE_CPU = _NEXT_STATE_CPU;
        }
    }
    /// <summary>
    /// 無敵時間に点滅させる
    /// </summary>
    /// <param name="flashInterval">点滅の間隔(s)</param>
    /// <returns></returns>
    IEnumerator FlashCPU(float flashInterval=0.15f)
    {
        bool trans_flag = false;
        float _alphaValue = 255;
        while (damageCoolTime > 0)
        {
            if (!trans_flag)
            {
                _alphaValue = alphaValue;
            }
            else
            {
                _alphaValue = 255;
            }
            trans_flag = !trans_flag;
            Color32 nowColor = new Color32((byte)(_renderer.material.color.r * 255), (byte)(_renderer.material.color.g * 255), (byte)(_renderer.material.color.b * 255), (byte)_alphaValue);
            _renderer.material.color = nowColor;
            yield return new WaitForSeconds(flashInterval);
        }
        //無敵時間終了時、色をもとに戻す
        if (damageCoolTime == 0)
        {
            _renderer.material.color = new Color32((byte)(_renderer.material.color.r * 255), (byte)(_renderer.material.color.g * 255), (byte)(_renderer.material.color.b * 255), 255);
        }
        yield break;
    }
    #region 各ステートの動作
    void WaitStart()
    {
        //Debug.Log("Change State : WAIT");
    }
    void WaitUpdate()
    {
        
    }
    void WalkStart()
    {
        //Debug.Log("Change State : WALK");
    }
    void WalkUpdate(float walkSpeed)
    {
        ChangeRotate();
        Vector3 nextPos = transform.localPosition + transform.forward * walkSpeed;
        transform.localPosition = nextPos;
    }
    void RunStart()
    {
        //Debug.Log("Change State : RUN");
    }
    void RunUpdate(float runSpeed)
    {
        ChangeRotate();
        Vector3 nextPos = transform.localPosition + transform.forward * runSpeed;
        transform.localPosition = nextPos;
    }
    void ChaseStart()
    {
        Debug.Log("Change State : CHASE");
    }
    void ChaseUpdate(float chaseSpeed)
    {
        //プレイヤーカメラまでの角度差を取得
        Vector3 diffPos = camera_player.transform.position - this.transform.position;
        float rad = Mathf.Atan2(diffPos.x, diffPos.z);
        float degree = rad * Mathf.Rad2Deg - this.transform.localEulerAngles.y;
        if (degree < 0)
        {
            degree += 360;
        }

        //角度を変更
        if (degree > 3 && degree < 180)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rotate_chase, transform.localEulerAngles.z);
        }
        else if(degree<357)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - rotate_chase, transform.localEulerAngles.z);
        }
        else
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, degree, transform.localEulerAngles.z);
        }
        
        Vector3 nextPos = transform.localPosition + transform.forward * chaseSpeed;
        transform.localPosition = nextPos;

        //CHASE中に0になったらやられる
        if (cpuHP <= 0)
        {
            _NEXT_STATE_CPU = STATE_CPU.DEAD;
        }
    }
    void DeadStart()
    {
        Debug.Log("Change State : DEAD");
        Vector3 effPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Instantiate(Eff_dead, effPos, Quaternion.Euler(Eff_dead.transform.localEulerAngles));
    }
    void DeadUpdate()
    {

    }
    void ChangeRotate()
    {
        if (wallCheck)
        {
            wallCheck = false;
            //90~270°向きを変える
            float addRotate = Random.Range(90, 270);
            Vector3 nextRotate = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + addRotate, transform.localEulerAngles.z);
            transform.localEulerAngles = nextRotate;
        }
    }
    #endregion
    /// <summary>
    /// ステートを更新する
    /// </summary>
    /// <param name="waitTime">次のステートに映るまでの時間</param>
    /// <returns></returns>
    IEnumerator ChangeMoveState(float waitTime = 10)
    {
        while (true)
        {
            float stateNum = Random.Range(0.0f, 1.0f);
            if (stateNum < actionRate[0])
            {
                _NEXT_STATE_CPU = STATE_CPU.WAIT;
            }
            else if (stateNum < actionRate[1])
            {
                _NEXT_STATE_CPU = STATE_CPU.WALK;
            }
            else
            {
                _NEXT_STATE_CPU = STATE_CPU.RUN;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "wall" || collision.gameObject.tag == "mob" || collision.gameObject.tag == "target")
        {
            wallCheck = true;
        }
    }
}
