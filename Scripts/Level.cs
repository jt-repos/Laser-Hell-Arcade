using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [Range(0, 100)] [SerializeField] float screenHeightPercentage;

    [SerializeField] float delayInSeconds = 2f;
    int dummyAugmentIndex = 1;
    int currentAugmentIndex = 1;
    string dummyAugmentName;

    private void SetUpSingleton()
    {
        int numberGameSessions = FindObjectsOfType<Level>().Length;
        if (numberGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Awake()
    {
        SetUpSingleton();
        SetScreenSize();
    }

    private void SetScreenSize()
    {
        var screenWidth = Mathf.RoundToInt(Screen.currentResolution.height * 9/16 * screenHeightPercentage / 100);
        var screenHeight = Mathf.RoundToInt(Screen.currentResolution.height * screenHeightPercentage / 100);
        Screen.SetResolution(screenWidth, screenHeight, false);
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
        FindObjectOfType<MusicPlayer>().LoadMenuMusic();
        FindObjectOfType<BackgroundScroller>().SlowDownScorlling();
        var particles = FindObjectsOfType<ParticleSpeed>();
        foreach (ParticleSpeed particle in particles)
        {
            particle.SlowDownSimulation();
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
        FindObjectOfType<MusicPlayer>().LoadGameMusic();
        FindObjectOfType<BackgroundScroller>().SpeedUpScorlling();
        var particles = FindObjectsOfType<ParticleSpeed>();
        foreach(ParticleSpeed particle in particles)
        {
            particle.SpeedUpSimulation();
        }
        if (FindObjectsOfType<GameSession>().Length > 0)
        {
            FindObjectOfType<GameSession>().ResetGame();
        }
    }

    public void LoadAugmentsMenu()
    {
        SceneManager.LoadScene("Augments Menu");
    }

    public void LoadShowAugmentMenu(string name, int index)
    {
        SetDummyAugmentIndex(index); //co ciekawe tu chyba nie trzeba referefowac do siebie samego
        dummyAugmentName = name;
        SceneManager.LoadScene("Show Augment Menu");
    }

    public void LoadCreditsMenu()
    {
        SceneManager.LoadScene("Credits Menu");
    }

    public void LoadGameOver()
    {
        StartCoroutine(WaitAndLoad("Game Over Menu"));
    }

    public void LoadGameFinished()
    {
        StartCoroutine(WaitAndLoad("Game Finished Menu"));
    }

    public void LoadCreoURL()
    {
        Application.OpenURL("https://creo-music.com/");
        Application.OpenURL("https://www.youtube.com/channel/UCsCWA3Y3JppL6feQiMRgm6Q");
    }

    public void LoadKenneyURL()
    {
        Application.OpenURL("https://www.kenney.nl/");
    }

    private IEnumerator WaitAndLoad(string sceneName)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(sceneName);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ConfirmAugmentIndex()
    {
        var level = FindObjectOfType<Level>(); //button probowal odpalic funkcje levela z prefabow ktory oczywiscie ma ustawione wartosci na zero, z prefabowego skryptu szuka samego siebie na scenie
        level.SetAugmentIndex(); //po czym calluje funkcje u tej instancji na scenie a nie u samego siebie bo to nic nie da
        LoadStartMenu();
    }

    public void SetAugmentIndex()
    {
        currentAugmentIndex = dummyAugmentIndex;
    }

    private void SetDummyAugmentIndex(int index)
    {
        dummyAugmentIndex = index;
    }

    public int GetAugmentIndex()
    {
        return currentAugmentIndex;
    }

    public int GetDummyAugmentIndex()
    {
        return dummyAugmentIndex;
    }

    public string GetDummyAugmentName()
    {
        return dummyAugmentName;    
    }
}
