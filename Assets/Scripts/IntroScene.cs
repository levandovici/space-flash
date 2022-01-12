using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _video_player;

    [SerializeField]
    private bool _is_loading = false;

    [SerializeField]
    private bool _is_video_started = false;



    private void Start()
    { 
        _video_player.Play();
    }



    private void Update()
    {
        if(_video_player.isPlaying)
        {
            _is_video_started = true;
        }

        if(_is_video_started && !_video_player.isPlaying && !_is_loading)
        {
            _is_loading = true;

            SceneManager.LoadScene(1);
        }
    }
}