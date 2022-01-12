using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private SoundController _soundController;
    
    [SerializeField]
    private float _loadAfter = 1f;



    private void Awake()
    {
        SaveLoadManager.Load();

        PlayerData data = SaveLoadManager.CurrentData;
        _soundController.SetSfxVolume(data.sfxVolume);
        _soundController.SetMusicVolume(data.musicVolume);

        StartCoroutine(LoadGame());
    }



    IEnumerator LoadGame()
    {
        yield return  new WaitForSeconds(_loadAfter);



        AsyncOperation operation = SceneManager.LoadSceneAsync(2);

        while(operation.progress < 1f)
        {
            yield return null;
        }
    }
}