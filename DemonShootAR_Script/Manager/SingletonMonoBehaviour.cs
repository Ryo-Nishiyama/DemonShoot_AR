using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// �V���O���g���p�̊��N���X
/// </summary>
/// <typeparam name="T">�e�N���X��������</typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //�p����N���X�̌^�ɃL���X�g
                Type t = typeof(T);
                //�����^�����I�u�W�F�N�g��T��
                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError(t + " �̃C���X�^���X�͂���܂���");
                }
            }
            return instance;
        }
    }
    //public static bool IsValid() => Instance != null;

    virtual protected void Awake()
    {
        CheckInstance();
    }
    protected bool CheckInstance()
    {
        //���������Ă��Ȃ��Ƃ��V���������
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        //���g�������Ă������͉������Ȃ�
        else if (Instance == this)
        {
            return true;
        }
        //�ǂ���ł��Ȃ��Ƃ��j������
        Destroy(this);
        return false;
    }
}
