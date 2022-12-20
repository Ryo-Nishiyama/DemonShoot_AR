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
        //�N���A���Ă��炸�J�E���g���X�V���ꂽ�Ƃ�
        if (!Click_target.gameClear_flag && _targetNumNow != Generate_obj.targetNumNow)
        {
            Debug.Log("rand image start");
            _targetNumNow = Generate_obj.targetNumNow;
            StartCoroutine(ChangeImg());
        }
        _targetCounter = Click_target.targetCounter;

        
    }
    /// <summary>
    /// ���̃^�[�Q�b�g�̉摜�������_���Ɍ��߂Ă��鉉�o
    /// </summary>
    /// <param name="randTime">�摜�؂�ւ������鎞��</param>
    /// <param name="randInterval">�؂�ւ���Ԋu</param>
    /// <param name="flashNum">�؂�ւ�������_�ł������</param>
    /// <param name="flashInterval">�؂�ւ�������_�ł�����Ԋu</param>
    /// <returns></returns>
    IEnumerator ChangeImg(float randTime = 1.0f , float randInterval = 0.1f, int flashNum = 3, float flashInterval = 0.25f)
    {
        imgChangeCheck = true;
        int targetRandNum = Random.Range(0, targetSprites.Length);
        int targetRandNum_pre = 999;
        float randTimer = 0;
        //WAIT�ŃN���A���Ă��Ȃ��Ƃ�
        while (randTimer<randTime && !Click_target.gameClear_flag)
        {
            randTimer += randInterval;
            //�O��Ɠ����摜�ɂȂ����Ƃ��Ē��I
            while (targetRandNum == targetRandNum_pre)
            {
                if (targetSprites.Length == 1)
                {
                    Debug.Log("�X�v���C�g��2���ȏ�}�����Ă��������B");
                    break;
                }
                targetRandNum = Random.Range(0, targetSprites.Length);
            }
            //�l��ۑ����Ă���
            targetRandNum_pre = targetRandNum;
            targetImg.sprite = targetSprites[Random.Range(0,targetSprites.Length)];
            yield return new WaitForSeconds(randInterval);
        }
        //�������ŐV��target�̉摜������
        targetImg.sprite = targetSprites[Generate_obj.targetNumNow];
        imgChangeCheck = false;

        //�^�[�Q�b�g�摜��_�ł�����
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
