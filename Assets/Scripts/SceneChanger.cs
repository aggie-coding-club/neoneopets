using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    public string username;
    public string password;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }



    public void LoadWelcomeScreen()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LoginScreen"))
        {
            Instance.username = GameObject.FindWithTag("Username").GetComponent<TMP_InputField>().text;
            Instance.password = GameObject.FindWithTag("Password").GetComponent<TMP_InputField>().text;
        }
        SceneManager.LoadSceneAsync("WelcomeScreen");
    }

    public void LoadLogin()
    {
        SceneManager.LoadSceneAsync("LoginScreen");
    }

    public void LoadCharacterCreation()
    {
        SceneManager.LoadSceneAsync("CharacterCreation");
    }

    public void LoadTetrisTitleScreen()
    {
        SceneManager.LoadSceneAsync("TetrisGame-TitleScreen");
    }

    public void LoadTetrisGame()
    {
        SceneManager.LoadSceneAsync("TetrisGame-Game");
    }
}
