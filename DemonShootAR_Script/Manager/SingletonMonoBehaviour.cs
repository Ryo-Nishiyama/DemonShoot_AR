using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// シングルトン用の基底クラス
/// </summary>
/// <typeparam name="T">各クラス名を入れる</typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //継承先クラスの型にキャスト
                Type t = typeof(T);
                //同じ型を持つオブジェクトを探す
                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError(t + " のインスタンスはありません");
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
        //何も入っていないとき新しく入れる
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        //自身が入っていた時は何もしない
        else if (Instance == this)
        {
            return true;
        }
        //どちらでもないとき破棄する
        Destroy(this);
        return false;
    }
}
