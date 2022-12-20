using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// �V�[���J�ڂ��Ǘ�����N���X�B�V���O���g���B
/// </summary>
public class Scene_manager : SingletonMonoBehaviour<Scene_manager>
{
    public enum SCENE_TYPE
    {
        INVILED,
        MASTER_TITLE,
        MASTER_MAIN,
        RESULT,
    }
    [SerializeField] SCENE_TYPE _SCENE_TYPE;
    Dictionary<SCENE_TYPE, string> SceneTypeToName = new Dictionary<SCENE_TYPE, string>() { [SCENE_TYPE.INVILED]="",
                                                                                            [SCENE_TYPE.MASTER_MAIN]= "Master_title",
                                                                                            [SCENE_TYPE.MASTER_TITLE]= "Master_main"};
    [SerializeField] GameObject loadUI;
    CanvasGroup _CanvasGroup;
    float loadUItime = 0;
    bool fadeOutCheck = false;
    bool oneFade = false;

    Coroutine fadeOutSave;

    protected override void Awake()
    {
        //���ɂ���Ƃ��폜
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        //�Ȃ��Ƃ��͂����ƕێ�
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        loadObjSet();
    }
    public IEnumerator fadeOut()
    {
        Debug.Log("�J�ڊJ�n");
        fadeOutCheck = true;
        loadUI.SetActive(true);
        _CanvasGroup = loadUI.GetComponent<CanvasGroup>();
        while (true)
        {
            //0.25�b�Ńt�F�[�h�A�E�g����
            if (loadUItime < 0.25f)
            {
                //timeScale0�ł�����
                loadUItime += Time.unscaledDeltaTime;
                float loadUItransparent = Mathf.Lerp(0, 255, loadUItime / 0.25f);
                //loadUI.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)loadUItransparent);
                _CanvasGroup.alpha = ((float)loadUItransparent) / 255;
            }
            else if (!oneFade)
            {
                //�t�F�[�h������J�ڊJ�n
                NextScene();
                fadeOutCheck = false;
                //�����񏈗���h�~
                oneFade = true;
                break;
            }
            yield return null;
        }
    }
    public void SE_enter()
    {
        //se_enter��炷
        //AudioManager.Instance.PlaySE("");
    }
    //�V�[���J�ڂł�loadXXX�̌��startFadeOut()�����s���Ă��������B
    public void startFadeOut()
    {
        fadeOutSave = StartCoroutine(fadeOut());
    }
    public void loadTitle()
    {
        _SCENE_TYPE = SCENE_TYPE.MASTER_TITLE;
    }
    public void loadGame()
    {
        _SCENE_TYPE = SCENE_TYPE.MASTER_MAIN;
    }
    private string GetSceneName(SCENE_TYPE type)
    {
        switch (type)
        {
            case SCENE_TYPE.MASTER_TITLE:
                return "Master_title";
            case SCENE_TYPE.MASTER_MAIN:
                return "Master_main";
        }
        return "";
    }
    void NextScene()
    {
        StopCoroutine(fadeOutSave);
        SceneManager.LoadScene(GetSceneName(_SCENE_TYPE));
    }
    void loadObjSet()
    {
        if (!loadUI)
        {
            //�J�ڏ�Ԃ�������
            oneFade = false;
            loadUItime = 0;
            Click_target.gameClear_flag = false;
            //���[�h�pUI�̓ǂݍ���
            loadUI = GameObject.Find("loadObj");
            loadUI.SetActive(false);
        }
    }
    #region ���J���\�b�h
    public void SceneChange(SCENE_TYPE type,bool useFade = true)
    {
        if (type == SCENE_TYPE.INVILED)
        {
            Debug.Log("�����ȃV�[���^�C�v�ł�");
            return;
        }

        if (useFade)
        {
            //FadeManager.Instance.LoadScene(GetSceneName(type), 0.5f);
        }
        else
        {
            SceneManager.LoadScene(GetSceneName(type));
        }
    }
    #endregion
}
