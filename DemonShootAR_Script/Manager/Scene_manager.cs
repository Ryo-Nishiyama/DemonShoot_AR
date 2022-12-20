using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// シーン遷移を管理するクラス。シングルトン。
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
        //既にあるとき削除
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        //ないときはずっと保持
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
        Debug.Log("遷移開始");
        fadeOutCheck = true;
        loadUI.SetActive(true);
        _CanvasGroup = loadUI.GetComponent<CanvasGroup>();
        while (true)
        {
            //0.25秒でフェードアウト完了
            if (loadUItime < 0.25f)
            {
                //timeScale0でも動く
                loadUItime += Time.unscaledDeltaTime;
                float loadUItransparent = Mathf.Lerp(0, 255, loadUItime / 0.25f);
                //loadUI.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)loadUItransparent);
                _CanvasGroup.alpha = ((float)loadUItransparent) / 255;
            }
            else if (!oneFade)
            {
                //フェード完了後遷移開始
                NextScene();
                fadeOutCheck = false;
                //複数回処理を防止
                oneFade = true;
                break;
            }
            yield return null;
        }
    }
    public void SE_enter()
    {
        //se_enterを鳴らす
        //AudioManager.Instance.PlaySE("");
    }
    //シーン遷移ではloadXXXの後にstartFadeOut()を実行してください。
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
            //遷移状態を初期化
            oneFade = false;
            loadUItime = 0;
            Click_target.gameClear_flag = false;
            //ロード用UIの読み込み
            loadUI = GameObject.Find("loadObj");
            loadUI.SetActive(false);
        }
    }
    #region 公開メソッド
    public void SceneChange(SCENE_TYPE type,bool useFade = true)
    {
        if (type == SCENE_TYPE.INVILED)
        {
            Debug.Log("無効なシーンタイプです");
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
