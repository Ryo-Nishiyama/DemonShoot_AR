using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate_wall : MonoBehaviour
{
    [SerializeField] GameObject wall;
    [SerializeField] GameObject floor;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;

    List<GameObject> wallObjList = new List<GameObject>();
    GameObject floorObj;

    public static Generate_wall instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        
    }
    /// <summary>
    /// 壁生成呼び出し用関数
    /// </summary>
    /// <param name="posRange">中心の座標</param>
    public void GenerateWall(Vector3 posRange)
    {
        Vector3 _posNew = new Vector3(0, -1, 0);
        //位置はそれぞれの中心、サイズは位置の差+1(サイズ補正)
        GenerateWallScale(new Vector3(_posNew.x-posRange.x, _posNew.y, _posNew.z), new Vector3(1, 50, 100));
        GenerateWallScale(new Vector3(_posNew.x + posRange.x, _posNew.y, _posNew.z), new Vector3(1, 50, 100));
        GenerateWallScale(new Vector3(_posNew.x , _posNew.y, _posNew.z - posRange.z), new Vector3(100, 50, 1));
        GenerateWallScale(new Vector3(_posNew.x, _posNew.y, _posNew.z + posRange.z), new Vector3(100, 50, 1));
    }
    /// <summary>
    /// 壁の生成関数
    /// </summary>
    /// <param name="_pos">中心となる座標</param>
    /// <param name="_scale">壁のサイズ</param>
    private void GenerateWallScale(Vector3 _pos,Vector3 _scale)
    {
        Vector3 PosNew = _pos;
        GameObject wallNew = Instantiate(wall, PosNew, Quaternion.identity);
        wallNew.transform.localScale = _scale;
        wallObjList.Add(wallNew);
    }
    /// <summary>
    /// 生成した壁を全て破棄する
    /// </summary>
    public void DestructionWall()
    {
        foreach(GameObject childObj in wallObjList)
        {
            Destroy(childObj);
        }
    }

    /// <summary>
    /// 2つの座標をもとに床を生成する
    /// </summary>
    /// <param name="startPos">始まりの角の座標</param>
    /// <param name="endPos">終わりの角の座標</param>
    public void GenerateFloor(/*Vector3 startPos, Vector3 endPos*/)
    {
        Vector3 _posNew = new Vector3(0,-1,0);
        floorObj = Instantiate(floor, new Vector3(_posNew.x, _posNew.y - 1, _posNew.z), Quaternion.identity);
        
        //動いても敵が落ちないように十分な大きさを持たせる
        floorObj.transform.localScale = new Vector3(1000, 1, 1000);
    }
    public void DestructionFloor()
    {
        Destroy(floorObj);
    }
}
