using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class GameController : MonoBehaviour
{
    private const string PRIVACY_POLICY = "https://games.limonadoent.com/privacy-policy.html";



    [SerializeField]
    private SoundController _soundController;

    [SerializeField]
    private GameUI _gameUI;


    private int _rocketID = 0;
    private int _selectedRocketID = 0;


    private bool _isStarted = false;
    private int _score = 0;
    private int _collectedGold = 0;

    [SerializeField]
    private Rocket[] _rocketsPrefab;
    [SerializeField]
    private GameObject _gameObjects;

    [SerializeField]
    private Planet[] _planets;
    private float _minYPos = 1f;
    private float _rocketMinYPos = -10f;

    private GameObject[] _coinPlanets;

    [SerializeField]
    private Planet[] _planetsPrefab;

    [SerializeField]
    private Rocket _player;

    [SerializeField]
    private GameObject _coin_1;
    [SerializeField]
    private GameObject _coin_2;

    [SerializeField]
    private GameObject[] _meteorits;

    private bool _pause = false;


    private SpaceShip _spaceShip = null;

    [SerializeField]
    private SpaceShip _spaceShipPrefab;

    [SerializeField]
    private float _timeRemainsToFindShip = 0f;

    [Header("Planets Balance")]
    [SerializeField]
    private Vector2 _gravity_radius = new Vector2(2f, 5f);

    [SerializeField]
    private Vector2 _force = new Vector2(0.2f, 0.55f);

    [SerializeField]
    private Vector2 _scale = new Vector2(0.4f, 0.8f);

    [SerializeField]
    private Vector2 _rotation_speed = new Vector2(-4.5f, 4.5f);



    private void Awake()
    {
        _gameUI.OnLeftClicked += () =>
        {
            _rocketID--;

            if (_rocketID < 0)
            {
                _rocketID = PlayerData.RocketsCount() - 1;
            }

            SetRocket();

            _soundController.PlaySfx(SoundController.ESfx.click);
        };

        _gameUI.OnRightClicked += () =>
        {
            _rocketID++;

            if (_rocketID >= PlayerData.RocketsCount())
            {
                _rocketID = 0;
            }

            SetRocket();

            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnBuyClicked += () =>
        {
            PlayerData data = SaveLoadManager.CurrentData;

            if(!data.rockets[_rocketID] && data.coins >= PlayerData.RocketsPrice()[_rocketID])
            {
                ChangeCoins(-PlayerData.RocketsPrice()[_rocketID]);
                data.rockets[_rocketID] = true;

               SaveLoadManager.Save();
            }

            _selectedRocketID = _rocketID;
            SetRocket();

            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnSelectClicked += () =>
        {
            if (SaveLoadManager.CurrentData.rockets[_rocketID])
            {
                SaveLoadManager.CurrentData.selectedRocketID = _selectedRocketID = _rocketID;
                SetRocket();
            }

            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnPlayClicked += () =>
        {
            if (!_isStarted)
                StartGame();

            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnNextClicked += () =>
        {
            _gameUI.OpenMain();
            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnSetiingsClicked += () =>
        {
            _gameUI.OpenSettings();
            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnCloseSettingsClicked += () =>
        {
            _gameUI.OpenMain();
            SaveLoadManager.Save();
            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnPauseClicked += () =>
        {
            if(_pause)
            {
                _pause = false;
                _soundController.Mute(false);
                Time.timeScale = 1f;
            }
            else
            {
                _pause = true;
                _soundController.Mute(true);
                Time.timeScale = 0f;
            }

            _gameUI.SetPause(_pause);
            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnSfxVolumeChanged += (f) =>
        {
            ChangeSfxVolume(f);
        };

        _gameUI.OnMusicVolumeChanged += (f) =>
        {
            ChangeMusicVolume(f);
        };

        _gameUI.OnDriveModeChanged += (i) =>
        {
            _gameUI.SetDriveMode(i);
            SaveLoadManager.CurrentData.SelectedDriveID = i;
            _soundController.PlaySfx(SoundController.ESfx.click);

        };

        _gameUI.OnPrivacyPolicyClicked += () =>
        {
            _soundController.PlaySfx(SoundController.ESfx.click);

            Application.OpenURL(PRIVACY_POLICY);
        };
    }



    private void Start()
    {
        _isStarted = false;
        _pause = false;

        PlayerData data = SaveLoadManager.CurrentData;
        _soundController.SetSfxVolume(data.sfxVolume);
        _soundController.SetMusicVolume(data.musicVolume);


        _rocketID = _selectedRocketID = SaveLoadManager.CurrentData.selectedRocketID;
        SetRocket();

        _gameUI.OpenMain();

        SetSfxVolume(SaveLoadManager.CurrentData.sfxVolume);
        SetMusicVolume(SaveLoadManager.CurrentData.musicVolume);

        ChangeCoins(0);
        _gameUI.SetDriveMode(SaveLoadManager.CurrentData.SelectedDriveID);
        _gameUI.SetScore(SaveLoadManager.CurrentData.bestScore);
    }

    private void OnDestroy()
    {

    }

    private void Update()
    {
        if (!_isStarted || _player == null)
            return;

        if (_planets.Length > 0 && _player.transform.position.y > _planets[0].transform.position.y + 5f)
        {
            RemoveFirstPlanet();
            SetScore(_score + 1);
            AddPlanets(1);
            _soundController.PlaySfx(SoundController.ESfx.addScore);
        }

        if(_player.transform.position.y > _rocketMinYPos + 15f)
        {
            _rocketMinYPos += Time.deltaTime;
        }

        if(_player.transform.position.y < _rocketMinYPos)
        {
            _player.transform.position = new Vector2(_player.transform.position.x, _rocketMinYPos);
        }

        transform.position = new Vector3(0f, _player.transform.position.y, transform.position.z);



        _timeRemainsToFindShip -= Time.deltaTime;

        if (_timeRemainsToFindShip <= 0f)
        {
            if (_spaceShip != null && !_spaceShip.IsPlayerFound)
            {
                DeleteSpaceShip();
            }

            _timeRemainsToFindShip = 0f;
        }
    }

    private Vector2 CalculatePlanetsForce()
    {
        Vector2 influence = Vector2.zero;

        for(int i = 0; i < _planets.Length; i++)
        { 
            if (_planets[i] != null)
                influence += _planets[i].Force(_player.transform.position);
        }

        return influence;
    }

    private void RemoveFirstPlanet()
    {
        Destroy(_planets[0].gameObject);


        Planet[] arr = new Planet[_planets.Length - 1];

        for (int i = 0; i < arr.Length; i++)
            arr[i] = _planets[i + 1];

        _planets = arr;
    }

    private void AddPlanets(int count)
    {
        for (int c = 0; c < count; c++)
        {
            _minYPos += UnityEngine.Random.Range(4f, 7f);

            Planet[] arr = new Planet[_planets.Length + 1];

            for (int i = 0; i < _planets.Length; i++)
                arr[i] = _planets[i];


            Vector2 position = new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), _minYPos);


            arr[_planets.Length] = Instantiate(_planetsPrefab[UnityEngine.Random.Range(0, _planetsPrefab.Length)],
                position, Quaternion.identity, _gameObjects.transform);

            float scale = UnityEngine.Random.Range(_scale.x, _scale.y);

            arr[_planets.Length].transform.localScale = new Vector3(scale, scale, 1f);

            float gravityRadius = UnityEngine.Random.Range(_gravity_radius.x, _gravity_radius.y);

            arr[_planets.Length].SetUp(gravityRadius, UnityEngine.Random.Range(_force.x, _force.y), _force.y, UnityEngine.Random.Range(_rotation_speed.x, _rotation_speed.y));


            int coins = UnityEngine.Random.Range(0, 17);

            Vector2[] points = CirclePoints(position, scale + 0.3f + UnityEngine.Random.Range(0.7f, 1f), coins);

            int meteorR = UnityEngine.Random.Range(0, 64 - coins);
            int id0 = -1;
            int id1 = -1;

            if (meteorR < 8)
            {
                id0 = UnityEngine.Random.Range(0, coins);

                if (coins > 1)
                {
                    do
                    {
                        id1 = UnityEngine.Random.Range(0, coins);
                    }
                    while (id0 == id1);
                }
            }
            else if (meteorR < 16)
            {
                id0 = UnityEngine.Random.Range(0, coins);
                id1 = -1;
            }


            for (int i = 0; i < points.Length; i++)
            {
                GameObject coin = null;

                if (i == id0 || i == id1)
                {
                    coin = Instantiate(_meteorits[UnityEngine.Random.Range(0, _meteorits.Length)], points[i], Quaternion.identity);
                }
                else
                {
                    int r = UnityEngine.Random.Range(0, 16);

                    if (r == 0)
                        coin = Instantiate(_coin_2, points[i], Quaternion.identity);
                    else
                        coin = Instantiate(_coin_1, points[i], Quaternion.identity);
                }


                coin.transform.SetParent(arr[_planets.Length].transform);
            }


            if(UnityEngine.Random.Range(0, 16) == 0)
            {
                points = CirclePoints(position, scale + 1.3f + UnityEngine.Random.Range(0.7f, 1f), coins);

                for (int i = 0; i < points.Length; i++)
                {
                    GameObject coin = null;

                        int r = UnityEngine.Random.Range(0, 4);

                    if (r == 0)
                        coin = Instantiate(_coin_2, points[i], Quaternion.identity);
                    else
                        coin = Instantiate(_coin_1, points[i], Quaternion.identity);

                    coin.transform.SetParent(arr[_planets.Length].transform);
                }

                if (UnityEngine.Random.Range(0, 16) == 0)
                {
                    points = CirclePoints(position, scale + 2.3f + UnityEngine.Random.Range(0.7f, 1f), coins);

                    for (int i = 0; i < points.Length; i++)
                    {
                        GameObject coin = null;

                        int r = UnityEngine.Random.Range(0, 2);

                        if (r == 0)
                            coin = Instantiate(_coin_2, points[i], Quaternion.identity);
                        else
                            coin = Instantiate(_coin_1, points[i], Quaternion.identity);

                        coin.transform.SetParent(arr[_planets.Length].transform);
                    }
                }
            }


            _planets = arr;
        }
    }

    private void RemoveFirstCoinPlanet()
    {
        Destroy(_coinPlanets[0].gameObject);


        GameObject[] arr = new GameObject[_coinPlanets.Length - 1];

        for (int i = 0; i < arr.Length; i++)
            arr[i] = _coinPlanets[i + 1];

        _coinPlanets = arr;
    }

    private void AddCoinPlanets(int count)
    {
        for (int c = 0; c < count; c++)
        {
            _minYPos += UnityEngine.Random.Range(4f, 7f);

            GameObject[] arr = new GameObject[_coinPlanets.Length + 1];

            for (int i = 0; i < _coinPlanets.Length; i++)
                arr[i] = _coinPlanets[i];


            Vector2 position = new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), _minYPos);

            arr[_coinPlanets.Length] = Instantiate(_meteorits[UnityEngine.Random.Range(0, _meteorits.Length)],
                position, Quaternion.identity, _gameObjects.transform);
            int coins = UnityEngine.Random.Range(0, 8);

            int layers = UnityEngine.Random.Range(2, 5);

            int r = UnityEngine.Random.Range(0, 4);

            GameObject coin = r == 0 && layers <= 3 ? _coin_2 : _coin_1;

            for (int i = 0; i < layers; i++)
            {
                Vector2[] points = CirclePoints(position, 0.5f * (1f + i), coins * (i + 1));

                for (int j = 0; j < points.Length; j++)
                {
                    Instantiate(coin, points[j], Quaternion.identity, arr[_coinPlanets.Length].transform);
                }
            }

            _coinPlanets = arr;
        }
    }

    private void OnApplicationQuit()
    {
        SaveLoadManager.Save();
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (pause)
    //    {
    //        SaveLoadManager.Save();
    //        SceneManager.LoadScene(0);
    //    }
    //}

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //    {
    //        SaveLoadManager.Save();
    //        SceneManager.LoadScene(0);
    //    }
    //}

    private void SetRocket()
    {
        _gameUI.SetRocketID(_rocketID);

        bool bought = SaveLoadManager.CurrentData.rockets[_rocketID];
         _gameUI.SetRocket(bought, PlayerData.RocketsPrice()[_rocketID], 
             _rocketID == _selectedRocketID, SaveLoadManager.CurrentData.coins > PlayerData.RocketsPrice()[_rocketID]);
    }

    private void ChangeSfxVolume(float volume)
    {
        SaveLoadManager.CurrentData.sfxVolume = volume;
        _soundController.SetSfxVolume(volume);
    }

    private void ChangeMusicVolume(float volume)
    {
        SaveLoadManager.CurrentData.musicVolume = volume;
        _soundController.SetMusicVolume(volume);
    }

    private void SetSfxVolume(float volume)
    {
        _soundController.SetSfxVolume(volume);
        _gameUI.SetSfxVolume(volume);
    }

    private void SetMusicVolume(float volume)
    {
        _soundController.SetMusicVolume(volume);
        _gameUI.SetMusicVolume(volume);
    }

    private void ChangeCoins(int delta)
    {
        _gameUI.SetCoins(SaveLoadManager.CurrentData.coins += delta);
        SetRocket();
        _collectedGold += delta;
    }

    private void SetBestScore(int score)
    {
        SaveLoadManager.CurrentData.bestScore = score;
    }

    private void SetScore(int score)
    {
        _score = score;
        _gameUI.SetScore(_score);
    }

    private Vector2[] CirclePoints(Vector2 center, float radius, int segments)
    {
        Vector2[] arr = new Vector2[segments];

        float angle = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle * i) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle * i) * radius;

            arr[i] = new Vector2(x + center.x, y + center.y);
        }


        return arr;
    }


    private void StartGame(int score = 0, int collected_gold = 0)
    {
        _isStarted = true;

        _minYPos = 1f;
        _rocketMinYPos = -10f;
 
        SetScore(score);
        _collectedGold = collected_gold;
        _gameUI.OpenGame();

       _player = Instantiate(_rocketsPrefab[_selectedRocketID], new Vector3(0f, -1f, 0f),
            Quaternion.identity, _gameObjects.transform);

        _soundController.SetMusic(SoundController.EMusicType.game);
        _player.OnCalculateForce += () => CalculatePlanetsForce();


        _gameUI.OnDrive += _player.Drive;


        _player.OnGameOver += () =>
        {
            _soundController.PlaySfx(SoundController.ESfx.destroyRocket);
            StartCoroutine(Over());
        };

        _player.OnCoinCollect += () =>
        {
            ChangeCoins(1);
            _soundController.PlaySfx(SoundController.ESfx.collectCoin);
        };

        _player.OnExtraCoinCollect += () =>
        {
            ChangeCoins(100);
            _soundController.PlaySfx(SoundController.ESfx.collectCoin);

            CreateSpaceShip();
        };

        AddPlanets(10);
        _coinPlanets = new GameObject[0];
    }

    private void CreateSpaceShip()
    {
        _timeRemainsToFindShip = 6f;

        if(_spaceShip == null)
        {
          _spaceShip = Instantiate(_spaceShipPrefab, new Vector3(0f,
                _player.transform.position.y - SpaceShip.MAX_PLAYER_DISTANCE * 2f, 0f), Quaternion.identity);

            _spaceShip.OnCoinCollect += () =>
            {
                ChangeCoins(1);
                _soundController.PlaySfx(SoundController.ESfx.collectCoin);
            };

            _spaceShip.OnExtraCoinCollect += () =>
            {
                ChangeCoins(100);
                _soundController.PlaySfx(SoundController.ESfx.collectCoin);
            };

            _spaceShip.OnKill += () =>
            {
                _soundController.PlaySfx(SoundController.ESfx.destroyRocket);
            };
        }
    }

    private void DeleteSpaceShip()
    {
        _timeRemainsToFindShip = 0f;

        if(_spaceShip != null && !_spaceShip.IsPlayerFound)
        {
            Destroy(_spaceShip.gameObject);
        }
    }

    private void GameOver()
    {
        int gold = 0;

        if (_score > SaveLoadManager.CurrentData.bestScore)
        {
            int oldScore = SaveLoadManager.CurrentData.bestScore;

            SetBestScore(_score);

            gold += (_score - oldScore) * 101;
        }

        gold += _score * 10;

        _gameUI.OpenOver(false);


        _gameUI.SetOver(_score, SaveLoadManager.CurrentData.bestScore, gold, _collectedGold);

        foreach (Transform t in _gameObjects.transform)
        {
            Destroy(t.gameObject);
        }

        _planets = new Planet[0];

        _gameUI.OnDrive -= _player.Drive;

        _isStarted = false;

        ChangeCoins(gold);
        _gameUI.SetScore(SaveLoadManager.CurrentData.bestScore);

        _soundController.SetMusic(SoundController.EMusicType.main);


        SaveLoadManager.Save();
    }

    private IEnumerator Over()
    {
        yield return new WaitForSeconds(2f);
        GameOver();
    }
}
