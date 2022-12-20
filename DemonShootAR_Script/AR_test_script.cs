using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AR_test_script : MonoBehaviour
{
    ARRaycastManager raycastManager;
    ARPlaneManager planeManager;
    [SerializeField] GameObject sphere;

    public static Pose GENERATE_POSITION = new Pose(new Vector3(0, 0, 0), Quaternion.identity);

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Ended || sphere == null)
        {
            return;
        }

        var hits = new List<ARRaycastHit>();
        // TrackableType.PlaneWithinPolygonを指定することによって検出した平面を対象にできる
        if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon) && !Game_manager.startCheck)
        {
            Pose hitPose = hits[0].pose;
            GENERATE_POSITION = hitPose;
            Game_manager.startCheck = true;
            // インスタンス化
            Instantiate(sphere, hitPose.position, hitPose.rotation);
        }
    }
}
