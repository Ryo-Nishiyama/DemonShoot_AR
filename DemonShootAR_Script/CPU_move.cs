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

    //CPU�̍s������
    [SerializeField, Range(0.0f, 1.0f)] float actionRateSet_Wait;
    [SerializeField, Range(0.0f, 1.0f)] float actionRateSet_Walk;
    [SerializeField, Range(0.0f, 1.0f)] float actionRateSet_Run;
    List<float> actionRate = new List<float>();
    //true�ɂ���ƃ����_���Ŋ��������肷��
    [SerializeField] bool actionSetRandCheck = false;

    //walk,run��Ԃ̑��x
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
        //���v�̒l��1�ɕ␳����
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
                //�F�����ɖ߂�
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
    /// �_���[�W����
    /// </summary>
    /// <param name="coolTime">�U���㖳�G����</param>
    /// <param name="damage">�_���[�W��</param>
    public bool TapDamage(GameObject eff = null, float coolTime = 0.5f, int damage=10)
    {
        if (damageCoolTime == 0)
        {
            if (eff != null)
            {
                GenerateObj(eff);
            }
            
            cpuHP -= damage;
            Debug.Log("�c��HP : " + cpuHP.ToString());

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

            //HP���c���Ă���Ƃ��U���N�[���^�C�����Z�b�g����
            if (cpuHP > 0)
            {
                damageCoolTime = coolTime;
            }
        }
        else
        {
            Debug.Log("�N�[���^�C�����ł��B �c�� : " + damageCoolTime.ToString() + "(s)");
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
    /// �҂����Ԃ̊Ǘ�
    /// </summary>
    /// <param name="playCheck">�v���C�������m�F����t���O</param>
    void ChangeWaitState(bool attackCheck ,bool playCheck)
    {
        if (attackCheck)
        {
            if(col_startMove != null)
            {
                StopCoroutine(col_startMove);
                col_startMove = null;
            }
            //�����Ȃ�CHASE
            if (demonCheck)
            {
                _NEXT_STATE_CPU = STATE_CPU.CHASE;
            }
            //�l�Ȃ�WAIT�ɑJ��
            else
            {
                _NEXT_STATE_CPU = STATE_CPU.WAIT;
            }
        }
        else
        {
            //�҂����Ԓ���WAIT�ɂ���
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
            //WAIT��Ԃő҂����������ꂽ�Ƃ�������
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
        //�^�b�v����Ă��班�����Ԃ�u��
        yield return new WaitForSeconds(tapInterval);
        if (eff_smoke != null)
        {
            Vector3 effPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Instantiate(eff_smoke, effPos, Quaternion.Euler(eff_smoke.transform.localEulerAngles));
        }
        //���G�t�F�N�g����ɗ���܂ő҂�
        yield return new WaitForSeconds(smokeInterval);
        //���������I�u�W�F�N�g���擾
        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            childList.Add(this.transform.GetChild(i).gameObject);
            //�l�̃��f���A�{�[�����\��
            if (i <= 1)
            {
                childList[i].SetActive(false);
            }
            //demon�̃��f���A�{�[���AHP��\��
            else
            {
                childList[i].SetActive(true);
            }
        }
        yield break;
    }
    /// <summary>
    /// �X�e�[�g�̐؂�ւ�
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
            //�I������


            //�J�n����
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
            //�X�e�[�g��ύX
            _STATE_CPU = _NEXT_STATE_CPU;
        }
    }
    /// <summary>
    /// ���G���Ԃɓ_�ł�����
    /// </summary>
    /// <param name="flashInterval">�_�ł̊Ԋu(s)</param>
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
        //���G���ԏI�����A�F�����Ƃɖ߂�
        if (damageCoolTime == 0)
        {
            _renderer.material.color = new Color32((byte)(_renderer.material.color.r * 255), (byte)(_renderer.material.color.g * 255), (byte)(_renderer.material.color.b * 255), 255);
        }
        yield break;
    }
    #region �e�X�e�[�g�̓���
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
        //�v���C���[�J�����܂ł̊p�x�����擾
        Vector3 diffPos = camera_player.transform.position - this.transform.position;
        float rad = Mathf.Atan2(diffPos.x, diffPos.z);
        float degree = rad * Mathf.Rad2Deg - this.transform.localEulerAngles.y;
        if (degree < 0)
        {
            degree += 360;
        }

        //�p�x��ύX
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

        //CHASE����0�ɂȂ���������
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
            //90~270��������ς���
            float addRotate = Random.Range(90, 270);
            Vector3 nextRotate = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + addRotate, transform.localEulerAngles.z);
            transform.localEulerAngles = nextRotate;
        }
    }
    #endregion
    /// <summary>
    /// �X�e�[�g���X�V����
    /// </summary>
    /// <param name="waitTime">���̃X�e�[�g�ɉf��܂ł̎���</param>
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
