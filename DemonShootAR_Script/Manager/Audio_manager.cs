using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// BGM��SE���Ǘ�����N���X
/// </summary>
public class Audio_manager : SingletonMonoBehaviour<Audio_manager>
{
    //�{�����[���̐ݒ�
    private const string BGM_VOLUME_KEY = "bgm_volume";
    private const string SE_VOLUME_KEY = "se_volume";
    private const float BGM_VOLUME_DEFULT = 0.5f;
    private const float SE_VOLUME_DEFULT = 0.5f;

    //�p�X�ݒ�
    private const string BGM_PATH = "Sounds/BGM";
    private const string SE_PATH = "Sounds/SE";

    //BGM���t�F�[�h����̂ɂ����鎞��
    public const float BGM_FADE_SPEED_RATE_HIGH = 0.9f;
    public const float BGM_FADE_SPEED_RATE_LOW = 0.3f;
    private float _bgmFadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH;

    //BGM���t�F�[�h�A�E�g����
    private bool _isFadeOut = false;

    //������BGM���ASE��
    private string _nextBGMName;
    private string _nextSEName;

    //BGM�p�ASE�p�ɕ����ăI�[�f�B�I�\�[�X������
    private AudioSource _bgmSource;
    private List<AudioSource> _seSourceList;
    private const int SE_SOURCE_NUM = 20;

    //�SAudioClip�������ŕێ�
    private Dictionary<string, AudioClip> _bgmDic, _seDic;

    // Start is called before the first frame update
    void Start()
    {
        //���\�[�X�t�H���_����SSE&BGM�̃t�@�C����ǂݍ��݃Z�b�g
        _bgmDic = new Dictionary<string, AudioClip>();
        _seDic = new Dictionary<string, AudioClip>();

        object[] bgmList = Resources.LoadAll(BGM_PATH);
        object[] seList = Resources.LoadAll(SE_PATH);

        foreach (AudioClip bgm in bgmList)
        {
            _bgmDic[bgm.name] = bgm;
        }
        foreach (AudioClip se in seList)
        {
            _seDic[se.name] = se;
            Debug.Log(se.name);
        }

        //�I�[�f�B�I�\�[�X��SE+1(BGM�̕�)�쐬
        //gameObject.AddComponent<AudioListener>();
        for (int i = 0; i < _seDic.Count + 1; i++)
        {
            gameObject.AddComponent<AudioSource>();
        }

        //�쐬�����I�[�f�B�I�\�[�X���擾���Ċe�ϐ��ɐݒ�A�{�����[�����ݒ�
        AudioSource[] audioSourceArray = GetComponents<AudioSource>();
        _seSourceList = new List<AudioSource>();

        //���O�ƃ{�����[����ۑ�����
        for (int i = 0; i < audioSourceArray.Length; i++)
        {
            audioSourceArray[i].playOnAwake = false;
            if (i == 0)
            {
                audioSourceArray[i].loop = true;
                _bgmSource = audioSourceArray[i];
                _bgmSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
            }
            else
            {
                _seSourceList.Add(audioSourceArray[i]);
                audioSourceArray[i].volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFULT);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isFadeOut)
        {
            return;
        }

        //���X�Ƀ{�����[���������Ă����A�{�����[����0�ɂȂ�����{�����[����߂����̋Ȃ𗬂�
        _bgmSource.volume -= Time.deltaTime * _bgmFadeSpeedRate;
        if (_bgmSource.volume <= 0)
        {
            _bgmSource.Stop();
            _bgmSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
            _isFadeOut = false;

            if (!string.IsNullOrEmpty(_nextBGMName))
            {
                PlayBGM(_nextBGMName);
            }
        }
    }

    //=================================================================================
    //SE
    //=================================================================================

    /// <summary>
    /// �w�肵���t�@�C������SE�𗬂��B��������delay�Ɏw�肵�����Ԃ����Đ��܂ł̊Ԋu���󂯂�
    /// </summary>
    public void PlaySE(string seName, float delay = 0.0f)
    {
        if (!_seDic.ContainsKey(seName))
        {
            Debug.Log(seName + "�Ƃ������O��SE������܂���");
            return;
        }

        _nextSEName = seName;

        //delay���x�������ē�����
        Invoke("DelayPlaySE", delay);
    }

    private void DelayPlaySE()
    {
        foreach (AudioSource seSource in _seSourceList)
        {
            //�d�����Ė炷�̂�h��
            if (!seSource.isPlaying)
            {
                seSource.PlayOneShot(_seDic[_nextSEName] as AudioClip);
                return;
            }
        }
    }

    //=================================================================================
    //BGM
    //=================================================================================

    /// <summary>
    /// �w�肵���t�@�C������BGM�𗬂��B���������ɗ���Ă���ꍇ�͑O�̋Ȃ��t�F�[�h�A�E�g�����Ă���B
    /// ��������fadeSpeedRate�Ɏw�肵�������Ńt�F�[�h�A�E�g����X�s�[�h���ς��
    /// </summary>
    public void PlayBGM(string bgmName, float fadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH)
    {
        if (!_bgmDic.ContainsKey(bgmName))
        {
            Debug.Log(bgmName + "�Ƃ������O��BGM������܂���");
            return;
        }

        //����BGM������Ă��Ȃ����͂��̂܂ܗ���
        if (!_bgmSource.isPlaying)
        {
            _nextBGMName = "";
            _bgmSource.clip = _bgmDic[bgmName] as AudioClip;
            _bgmSource.Play();
        }
        //�ႤBGM������Ă��鎞�́A����Ă���BGM���t�F�[�h�A�E�g�����Ă��玟�𗬂��B����BGM������Ă��鎞�̓X���[
        else if (_bgmSource.clip.name != bgmName)
        {
            _nextBGMName = bgmName;
            FadeOutBGM(fadeSpeedRate);
        }
    }

    /// <summary>
    /// BGM�������Ɏ~�߂�
    /// </summary>
    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    /// <summary>
    /// ���ݗ���Ă���Ȃ��t�F�[�h�A�E�g������
    /// fadeSpeedRate�Ɏw�肵�������Ńt�F�[�h�A�E�g����X�s�[�h���ς��
    /// </summary>
    public void FadeOutBGM(float fadeSpeedRate = BGM_FADE_SPEED_RATE_LOW)
    {
        _bgmFadeSpeedRate = fadeSpeedRate;
        _isFadeOut = true;
    }
}
