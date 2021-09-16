using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class GameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainPanel;

    [SerializeField]
    private GameObject _coinsBar;

    [SerializeField]
    private Text _coins;

    [SerializeField]
    private Button _buy;

    [SerializeField]
    private GameObject _pricePanel;

    [SerializeField]
    private Text _price;

    [SerializeField]
    private Button _select;

    [SerializeField]
    private Button _left;

    [SerializeField]
    private Button _right;

    [SerializeField]
    private Button _play;

    [SerializeField]
    private GameObject[] _rockets;

    [SerializeField]
    private Text _score;

    [SerializeField]
    private GameObject _scoreBar;



    [SerializeField]
    private GameObject _gamePanel;

    [SerializeField]
    private UIButton[] _rotateLeft;

    [SerializeField]
    private UIButton[] _rotateRight;

    [SerializeField]
    private UIButton[] _fly;

    [SerializeField]
    private Button _pause;

    [SerializeField]
    private Image _pause_image;

    [SerializeField]
    private Sprite _pauseSprite;

    [SerializeField]
    private Sprite _playSprite;

    [SerializeField]
    private GameObject _drive_1;

    [SerializeField]
    private GameObject _drive_2;

    [SerializeField]
    private GameObject _drive_3;



    [SerializeField]
    private GameObject _gameOverPanel;

    [SerializeField]
    private Text _overScore;

    [SerializeField]
    private Text _overBestScore;

    [SerializeField]
    private Text _overGold;

    [SerializeField]
    private Text _overCollected;

    [SerializeField]
    private Button _next;


    [SerializeField]
    private GameObject _settings;

    [SerializeField]
    private Button _openSettings;

    [SerializeField]
    private Button _closeSettings;

    [SerializeField]
    private Slider _sfxSlider;

    [SerializeField]
    private Slider _musicSlider;

    [SerializeField]
    private Button _select_drive_1;

    [SerializeField]
    private Button _select_drive_2;

    [SerializeField]
    private Button _select_drive_3;


    [Header("Privacy Policy")]
    [SerializeField]
    private Button _privacy_policy;



    public event Action OnBuyClicked;
    public event Action OnSelectClicked;
    public event Action OnLeftClicked;
    public event Action OnRightClicked;
    public event Action OnPlayClicked;
    public event Action OnNextClicked;
    public event Action OnSetiingsClicked;
    public event Action OnCloseSettingsClicked;
    public event Action OnPauseClicked;

    public event Action OnPrivacyPolicyClicked;

    public event Action<int> OnDriveModeChanged;

    public event Action<float> OnSfxVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;

    public event Action<Rocket.EDriveParameter, bool> OnDrive;



    private void Awake()
    {
        _left.onClick.AddListener(() => OnLeftClicked.Invoke());
        _right.onClick.AddListener(() => OnRightClicked.Invoke());
        _buy.onClick.AddListener(() => OnBuyClicked.Invoke());
        _select.onClick.AddListener(() => OnSelectClicked.Invoke());
        _play.onClick.AddListener(() => OnPlayClicked.Invoke());
        _next.onClick.AddListener(() => OnNextClicked.Invoke());
        _openSettings.onClick.AddListener(() => OnSetiingsClicked.Invoke());
        _closeSettings.onClick.AddListener(() => OnCloseSettingsClicked.Invoke());
        _pause.onClick.AddListener(() => OnPauseClicked.Invoke());

        _privacy_policy.onClick.AddListener(() => OnPrivacyPolicyClicked.Invoke());

        _select_drive_1.onClick.AddListener(() => OnDriveModeChanged.Invoke(0));
        _select_drive_2.onClick.AddListener(() => OnDriveModeChanged.Invoke(1));
        _select_drive_3.onClick.AddListener(() => OnDriveModeChanged.Invoke(2));

        for (int i = 0; i < 3; i++)
        {
            _rotateLeft[i].OnMouseDown += () => OnDrive.Invoke(Rocket.EDriveParameter.left, true);
            _rotateLeft[i].OnMouseUp += () => OnDrive.Invoke(Rocket.EDriveParameter.left, false);

            _rotateRight[i].OnMouseDown += () => OnDrive.Invoke(Rocket.EDriveParameter.right, true);
            _rotateRight[i].OnMouseUp += () => OnDrive.Invoke(Rocket.EDriveParameter.right, false);

            _fly[i].OnMouseDown += () => OnDrive.Invoke(Rocket.EDriveParameter.fly, true);
            _fly[i].OnMouseUp += () => OnDrive.Invoke(Rocket.EDriveParameter.fly, false);
        }

        _sfxSlider.onValueChanged.AddListener((f) => OnSfxVolumeChanged(f));
        _musicSlider.onValueChanged.AddListener((f) => OnMusicVolumeChanged(f));
    }

    private void OnDestroy()
    {
        _left.onClick.RemoveAllListeners();
        _right.onClick.RemoveAllListeners();
        _buy.onClick.RemoveAllListeners();
        _select.onClick.RemoveAllListeners();
        _play.onClick.RemoveAllListeners();
        _next.onClick.RemoveAllListeners();
        _openSettings.onClick.RemoveAllListeners();
        _closeSettings.onClick.RemoveAllListeners();
        _pause.onClick.RemoveAllListeners();

        _privacy_policy.onClick.RemoveAllListeners();

        for (int i = 0; i < 3; i++)
        {
            _rotateLeft[i].OnMouseDown -= () => OnDrive.Invoke(Rocket.EDriveParameter.left, true);
            _rotateLeft[i].OnMouseUp -= () => OnDrive.Invoke(Rocket.EDriveParameter.left, false);

            _rotateRight[i].OnMouseDown -= () => OnDrive.Invoke(Rocket.EDriveParameter.right, true);
            _rotateRight[i].OnMouseUp -= () => OnDrive.Invoke(Rocket.EDriveParameter.right, false);

            _fly[i].OnMouseDown -= () => OnDrive.Invoke(Rocket.EDriveParameter.fly, true);
            _fly[i].OnMouseUp -= () => OnDrive.Invoke(Rocket.EDriveParameter.fly, false);
        }

        _sfxSlider.onValueChanged.RemoveAllListeners();
        _musicSlider.onValueChanged.RemoveAllListeners();

        _select_drive_1.onClick.RemoveAllListeners();
        _select_drive_2.onClick.RemoveAllListeners();
        _select_drive_3.onClick.RemoveAllListeners();
    }

    public void SetDriveMode(int mode)
    {
        _select_drive_1.image.color = Color.white;
        _select_drive_2.image.color = Color.white;
        _select_drive_3.image.color = Color.white;

        _drive_1.SetActive(false);
        _drive_2.SetActive(false);
        _drive_3.SetActive(false);

        if (mode == 1)
        {
            _select_drive_2.image.color = Color.green;
            _drive_2.SetActive(true);
        }
        else if(mode == 2)
        {
            _select_drive_3.image.color = Color.green;
            _drive_3.SetActive(true);
        }
        else
        {
            _select_drive_1.image.color = Color.green;
            _drive_1.SetActive(true);
        }
    }



    public void SetRocketID(int id)
    {
        for (int i = 0; i < _rockets.Length; i++)
            _rockets[i].SetActive(false);

        _rockets[id].SetActive(true);
    }

    public void SetRocket(bool bought, int price, bool isSelected, bool canBuy)
    {
        _buy.gameObject.SetActive(!bought && canBuy);
        _select.gameObject.SetActive(bought && !isSelected);

        _pricePanel.SetActive(!bought);
        _price.text = price.ToString();
    }

    public void SetPause(bool pause)
    {
        if(pause)
        {
            _pause_image.sprite = _playSprite;
        }
        else
        {
           _pause_image.sprite = _pauseSprite;
        }
    }

    public void SetCoins(int coins)
    {
        _coins.text = coins.ToString();
    }

    public void SetScore(int score)
    {
        _score.text = score.ToString();
    }

    public void SetSfxVolume(float volume)
    {
        _sfxSlider.value = volume;
    }

    public void SetMusicVolume(float volume)
    {
        _musicSlider.value = volume;
    }

    public void SetOver(int score, int bestScore, int gold, int collected)
    {
        _overScore.text = $"Score: {score}";
        _overBestScore.text = $"Best score: {bestScore}";
        _overGold.text = $"Gold: {gold}";
        _overCollected.text = $"Collected: {collected}";
    }

    public void OpenMain()
    {
        _mainPanel.SetActive(true);
        _gamePanel.SetActive(false);
        _gameOverPanel.SetActive(false);
        _settings.SetActive(false);
        _coinsBar.SetActive(true);
        _scoreBar.SetActive(true);
    }

    public void OpenGame()
    {
        _mainPanel.SetActive(false);
        _gamePanel.SetActive(true);
        _settings.SetActive(false);
        _gameOverPanel.SetActive(false);
        _coinsBar.SetActive(true);
        _scoreBar.SetActive(true);
    }

    public void OpenOver(bool is_ad_loaded)
    {
        _mainPanel.SetActive(false);
        _gamePanel.SetActive(false);
        _settings.SetActive(false);
        _gameOverPanel.SetActive(true);
        _coinsBar.SetActive(true);
        _scoreBar.SetActive(false);

        _next.gameObject.SetActive(!is_ad_loaded);
        if (is_ad_loaded)
            StartCoroutine(ShowNextButton());
    }

    private IEnumerator ShowNextButton()
    {
        yield return new WaitForSeconds(3f);

        _next.gameObject.SetActive(true);
    }

    public void OpenSettings()
    {
        _mainPanel.SetActive(false);
        _gamePanel.SetActive(false);
        _settings.SetActive(true);
        _gameOverPanel.SetActive(false);
        _coinsBar.SetActive(false);
        _scoreBar.SetActive(false);
    }
}