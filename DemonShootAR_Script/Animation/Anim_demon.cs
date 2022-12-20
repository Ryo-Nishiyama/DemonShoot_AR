using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_demon : MonoBehaviour
{
    Animator _animator;
    GameObject _parent;
    CPU_move _CPU_move = null;
    CPU_move.STATE_CPU _PRE_STATE_CPU=CPU_move.STATE_CPU.WAIT;

    Coroutine waitCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _CPU_move = GetComponent<CPU_move>();
        //自身についていないとき親オブジェクトから取得
        if (_CPU_move == null)
        {
            _parent = transform.parent.gameObject;
            _CPU_move = _parent.GetComponent<CPU_move>();
        }
        waitCoroutine = StartCoroutine(WaitAnimRandSet());
    }

    // Update is called once per frame
    void Update()
    {
        //ステートが切り替わったときアクションを変更
        if (_CPU_move._STATE_CPU != _PRE_STATE_CPU)
        {
            //前回のアニメーションを停止
            switch (_PRE_STATE_CPU)
            {
                case CPU_move.STATE_CPU.WAIT:
                    _animator.SetBool("Idle",false);
                    if (waitCoroutine != null)
                    {
                        StopCoroutine(waitCoroutine);
                        waitCoroutine = null;
                    }
                    break;
                case CPU_move.STATE_CPU.WALK:
                    _animator.SetBool("Walk", false);
                    break;
                case CPU_move.STATE_CPU.RUN:
                    _animator.SetBool("Run", false);
                    break;
                case CPU_move.STATE_CPU.CHASE:
                    _animator.SetBool("Run", false);
                    break;
                case CPU_move.STATE_CPU.DEAD:
                    _animator.SetBool("Dead", false);
                    break;
            }
            //現在のアニメーションを起動
            switch (_CPU_move._STATE_CPU)
            {
                case CPU_move.STATE_CPU.WAIT:
                    _animator.SetBool("Idle", true);
                    waitCoroutine = StartCoroutine(WaitAnimRandSet());
                    break;
                case CPU_move.STATE_CPU.WALK:
                    _animator.SetBool("Walk", true);
                    break;
                case CPU_move.STATE_CPU.RUN:
                    _animator.SetBool("Run", true);
                    break;
                case CPU_move.STATE_CPU.CHASE:
                    _animator.SetBool("Run", true);
                    break;
                case CPU_move.STATE_CPU.DEAD:
                    _animator.SetBool("Dead", true);
                    break;
            }
            _PRE_STATE_CPU = _CPU_move._STATE_CPU;
        }
    }
    /// <summary>
    /// Idleステートでランダムにアニメーションを切り替える
    /// </summary>
    /// <param name="waitTime">抽選をするまでの時間</param>
    /// <returns></returns>
    IEnumerator WaitAnimRandSet(float waitTime=5)
    {
        while (true)
        {
            //0が出やすいようにする
            int setNum = Random.Range(0, 4);
            if (setNum >= 3)
            {
                setNum = 0;
            }
            _animator.SetInteger("Idle_randNum", setNum);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
