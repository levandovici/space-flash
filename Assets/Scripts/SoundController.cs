using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource _sfx;
    public AudioSource _music;

    public AudioClip _mainMusic;
    public AudioClip _gameMusic;

    public AudioClip _click;
    public AudioClip _collectCoin;
    public AudioClip _destroyRocket;
    public AudioClip _addScore;



    public void SetSfxVolume(float volume)
    {
        _sfx.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        _music.volume = volume;
    }

    public void PlaySfx(ESfx sfx)
    {
        AudioClip clip = null;

        switch(sfx)
        {
            case ESfx.click:
                clip = _click;
                break;

            case ESfx.collectCoin:
                clip = _collectCoin;
                break;

            case ESfx.destroyRocket:
                clip = _destroyRocket;
                break;

            case ESfx.addScore:
                clip = _addScore;
                break;
        }

        _sfx.PlayOneShot(clip);
    }

    public enum ESfx
    {
        click, collectCoin, destroyRocket, addScore
    }

    public void Mute(bool mute)
    {
        _sfx.mute = mute;
        _music.mute = mute;
    }

    public void SetMusic(EMusicType type)
    {
        _music.Stop();

        if (type == EMusicType.main)
        {
            _music.clip = _mainMusic;
        }
        else if (type == EMusicType.game)
        {
            _music.clip = _gameMusic;
        }

        _music.Play();
    }

    public enum EMusicType
    {
        main, game
    }
}