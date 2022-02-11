using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip drumIntro;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip transitionMusic;
    [SerializeField] AudioClip gameMusic;
    AudioSource audio;
    bool playerInMenu;

    // Use this for initialization
    void Awake ()
    {
        SetUpSingleton();	
	}

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        StartCoroutine(PlayMenuMusic());
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadMenuMusic()
    {
        if(!playerInMenu)
        {
            StartCoroutine(PlayMenuMusic());
        }
    }

    IEnumerator PlayMenuMusic()
    {
        playerInMenu = true;
        PlayAudioClip(drumIntro);
        yield return new WaitForSeconds(audio.clip.length - 0.1f);
        if(playerInMenu)
        {
            PlayAudioClip(menuMusic);
        }
    }

    public void LoadGameMusic()
    {
        StartCoroutine(PlayGameMusic());
    }

    IEnumerator PlayGameMusic()
    {
        if(playerInMenu)
        {
            playerInMenu = false;
            PlayAudioClip(transitionMusic);
            yield return new WaitForSeconds(audio.clip.length - 0.1f);
        }
        PlayAudioClip(gameMusic);
    }

    private float PlayAudioClip(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
        return audio.clip.length;
    }
}
