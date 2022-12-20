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
    /// �^�[�Q�b�g��_�ł�����
    /// </summary>
    /// <param name="delayTime">�_�ł�����܂ł̑҂�����</param>
    /// <param name="flashTime">�_������</param>
    /// <param name="flashInterval">�_�ł̊Ԋu</param>
    /// <param name="flashCount">�_�ł̉�</param>
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
        //�N���A������_�ł���߂�
        meshRenderer.material.DisableKeyword("_EMISSION");
        yield break;
    }
}
