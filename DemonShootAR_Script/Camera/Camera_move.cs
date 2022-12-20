using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    Vector3 cameraPos;
    // Start is called before the first frame update
    void Start()
    {
        cameraPos = this.transform.position;
    }

    IEnumerator Vibration(float duration, float magnitude)
    {

        float elapsed = 0;
        while (elapsed < duration)
        {
            float xPos = cameraPos.x + Random.Range(-1f, 1f) * magnitude;
            float yPos = cameraPos.y + Random.Range(-1f, 1f) * magnitude;
            //this.transform.position=
            transform.localPosition = new Vector3(xPos, yPos, cameraPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        this.transform.localPosition = cameraPos;
    }
}
