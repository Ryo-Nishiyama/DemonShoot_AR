using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_flashing : MonoBehaviour
{
    MeshRenderer meshRenderer;
    [ColorUsage(false, true)] [SerializeField] Color COLOR_FLASH = new Color(1, 1, 0);
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        meshRenderer.material.SetColor("_EmissionColor", COLOR_FLASH);
        StartCoroutine(flash());
    }

    /// <summary>
    /// ターゲットを点滅させる
    /// </summary>
    /// <param name="delayTime">点滅させるまでの待ち時間</param>
    /// <param name="flashTime">点灯時間</param>
    /// <param name="flashInterval">点滅の間隔</param>
    /// <param name="flashCount">点滅の回数</param>
    /// <returns></returns>
    IEnumerator flash(float delayTime = 5.0f, float flashTime = 0.3f, float flashInterval = 0.3f, int flashCount = 2)
    {
        while (!Click_target.gameClear_flag)
        {
            yield return new WaitForSeconds(delayTime);
            for(int i = 0; i < flashCount; i++)
            {
                if (Click_target.gameClear_flag)
                {
                    break;
                }
                meshRenderer.material.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(flashTime);
                meshRenderer.material.DisableKeyword("_EMISSION");
                yield return new WaitForSeconds(flashInterval);
            }
        }
        //クリアしたら点滅をやめる
        meshRenderer.material.DisableKeyword("_EMISSION");
        yield break;
    }
}
