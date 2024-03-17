using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private TMP_Text _scoreText, _endScoreText, _bestScoreText;

    private int score;

    [SerializeField]
    private Animator _scoreAnimator;

    [SerializeField]
    private AnimationClip _scoreClip;

    [SerializeField]
    private Obstacle _targetPrefab;

    [SerializeField]
    private float _maxSpawnOffset;

    [SerializeField]
    private Vector3 _startTargetPos;

    [SerializeField]
    private GameObject _endPanel;

    [SerializeField]
    private Image _soundImage;

    [SerializeField]
    private Sprite _activeSoundSprite, _inactiveSoundSprite;

    private void Awake()
    {
        Instance = this;
    }
   




    private void Start()
    {
        _endPanel.SetActive(false);
        AudioManager.Instance.AddButtonSound();
        score = 0;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);
        SpawnObstacle();
        _bestScoreText.text = "RECORD " + PlayerPrefs.GetInt(Constants.DATA.HIGH_SCORE, 0);
    }

    private void SpawnObstacle()
    {
        Obstacle temp = Instantiate(_targetPrefab);
        Vector3 tempPos = _startTargetPos;
        _startTargetPos.x = Random.Range(-_maxSpawnOffset, _maxSpawnOffset);
        temp.MoveToPos(tempPos);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(Constants.DATA.MAIN_MENU_SCENE);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(Constants.DATA.GAMEPLAY_SCENE);
    }

    public void ToggleSound()
    {
        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND)
          : 1) == 1;
        sound = !sound;
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;
        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, sound ? 1 : 0);
        AudioManager.Instance.ToggleSound();
    }

    public void UpdateScore()
    {
        score++;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);
        SpawnObstacle();
        
    }

    public void EndGame()
    {
        _endPanel.SetActive(true);
        _endScoreText.text = score.ToString();

        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND)
         : 1) == 1;        
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;

        int highScore = PlayerPrefs.HasKey(Constants.DATA.HIGH_SCORE) ? PlayerPrefs.GetInt(Constants.DATA.HIGH_SCORE) : 0;
        if (score % 2 == 0)
        {
            CurrentColorId++;
        }
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(Constants.DATA.HIGH_SCORE, score);
            _bestScoreText.text = "NUEVO RECORD";
        }
        else
        {
            _bestScoreText.text = "RECORD " + PlayerPrefs.GetInt(Constants.DATA.HIGH_SCORE, 0);
        }
    }

    #region COLOR_CHANGE
    [SerializeField] private List<Color> _colors;

    [HideInInspector] public Color CurrentColor => _colors[CurrentColorId];

    [HideInInspector] public UnityAction<Color> ColorChanged1;

    private int _currentColorId;
    public int CurrentColorId
    {
        get
        {
            return _currentColorId;
        }

        set
        {
            _currentColorId = value % _colors.Count;
            ColorChanged1?.Invoke(CurrentColor);
        }
    }
    
    #endregion
    public void BorraRecord()
    {
        PlayerPrefs.DeleteAll();
    }
}
