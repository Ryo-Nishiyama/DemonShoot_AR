using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game_manager : MonoBehaviour
{
    public static Game_manager instance;

    [SerializeField] GameObject clearTextObj;
    RectTransform _clearRect;
    //書き換えるテキスト
    [SerializeField] TextMeshProUGUI clearText;
    [SerializeField] TextMeshProUGUI counterText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI infoText;

    [SerializeField] GameObject attackInfo;
    [SerializeField] GameObject resetButton;
    [SerializeField] GameObject Eff_firework;

    [SerializeField] Image bar_coolTime;

    //スコアの変更値
    [SerializeField] int scoreAdditionalValue=100;
    [SerializeField] int scoreSubtractionValue=100;

    CanvasGroup _attackInfo_canvasGroup;

    bool clearCheck = false;
    bool resetCheck = false;
    int scoreTotal = 0;
    static public bool startCheck = false;
    //static public bool startCheck = true;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _clearRect = clearTextObj.GetComponent<RectTransform>();
        //値を初期化して表示
        GameCounter(0);
        GameScoreCounter(0);
        _attackInfo_canvasGroup = attackInfo.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        //startCheckの切り替えを書いたらそこに移す
        if (startCheck)
        {
            infoText.text = "";
        }
        if (Click_target.attackCoolTimer >= 0)
        {
            bar_coolTime.fillAmount = Mathf.Lerp(1, 0, Click_target.attackCoolTimer / Click_target.attackCoolTime);
        }
    }
    /// <summary>
    /// クリア後処理
    /// </summary>
    /// <param name="delayTime">処理の遅延時間</param>
    public void GameClear(float delayTime = 1.0f)
    {
        Invoke("GameClearProcess", delayTime);
    }
    private void GameClearProcess()
    {
        //生成位置は後で決める
        if (Eff_firework)
        {
            Instantiate(Eff_firework, new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(Eff_firework, new Vector3(5, 0, 0), Quaternion.identity);
            Instantiate(Eff_firework, new Vector3(0, 0, 5), Quaternion.identity);
            Instantiate(Eff_firework, new Vector3(5, 0, 5), Quaternion.identity);
        }
        else
        {
            Debug.Log("Eff_fireworkが見つかりません");
        }
        
        clearCheck = true;
        clearText.text = "Clear!!";
        resetButton.SetActive(true);
        Audio_manager.instance.PlaySE("clear");
        StartCoroutine(GameClearAnimUP(0.5f, new Vector2(1, 1)));
    }
    IEnumerator GameClearAnimUP(float changeTime, Vector2 scale)
    {
        float changeTimer = 0;
        while (changeTimer < changeTime)
        {
            changeTimer += Time.deltaTime;
            _clearRect.localScale = Vector2.Lerp(new Vector2(0, 0), scale * 1.1f, changeTimer / changeTime);
            yield return null;
        }
        StartCoroutine(GameClearAnimDown(changeTime * 0.2f, scale));
        yield break;
    }
    IEnumerator GameClearAnimDown(float changeTime, Vector2 scale)
    {
        float changeTimer = 0;
        while (changeTimer < changeTime)
        {
            changeTimer += Time.deltaTime;
            _clearRect.localScale = Vector2.Lerp(scale * 1.1f, scale, changeTimer / changeTime);
            yield return null;
        }
        yield break;
    }
    public void GameReset()
    {
        Click_target.gameClear_flag = false;
        clearCheck = false;
        clearText.text = "";
        resetButton.SetActive(false);
    }
    public void GameCounter(int nowCount)
    {
        Debug.Log(nowCount);
        counterText.text = "Round : " + nowCount.ToString("N0") + "/" + Click_target.targetClearNum.ToString("N0");
    }
    //=================================================================================
    //Score
    //=================================================================================
    /// <summary>
    /// 正解をタップしたときに呼ぶ
    /// </summary>
    /// <param name="scoreRate">追加で増やす分</param>
    public void ScoreAdditional(int scoreRate)
    {
        Audio_manager.instance.PlaySE("tap_target");
        int _scoreAdd = scoreAdditionalValue;
        if (scoreRate > 1)
        {
            _scoreAdd = scoreAdditionalValue * scoreRate;
        }
        GameScoreCounter(_scoreAdd);
    }
    /// <summary>
    /// 不正解をタップしたときに呼ぶ
    /// </summary>
    public void ScoreSubtraction(int scoreRate = 1)
    {
        Audio_manager.instance.PlaySE("tap_nomal");
        GameScoreCounter(-scoreSubtractionValue * scoreRate);
    }
    void GameScoreCounter(int nowScore)
    {
        scoreTotal += nowScore;
        if (scoreTotal < 0)
        {
            scoreTotal = 0;
        }
        scoreText.text = "Score : " + scoreTotal.ToString("N0");
    }

    public void AttackInfoEnable()
    {
        _attackInfo_canvasGroup.alpha = 1;
    }
    public void AttackInfoUnable()
    {
        _attackInfo_canvasGroup.alpha = 0;
    }
}
