using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneDetection : MonoBehaviour
{
    private bool m_PlaneVisible = true;
    ARPlaneManager m_ARPlaneManager;
    TrackableCollection<UnityEngine.XR.ARFoundation.ARPlane> m_ARPlaneManager_pre;

    [SerializeField] TextMeshProUGUI tm1;
    [SerializeField] TextMeshProUGUI tm2;

    // Start is called before the first frame update
    void Start()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        tm1.text = "start : "+Game_manager.startCheck.ToString();
        //設置した最初の一回だけ実行
        if (Game_manager.startCheck && m_PlaneVisible)
        {
            SetAllPlanesActive();
        }
        //前と内容が変わったとき表示状態を変更
        foreach (var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(m_PlaneVisible);
            tm2.text = "plane : " + m_PlaneVisible.ToString();
        }
        
        m_ARPlaneManager_pre = m_ARPlaneManager.trackables;
    }

    void SetAllPlanesActive()
    {
        m_PlaneVisible = !m_PlaneVisible;
    }

}