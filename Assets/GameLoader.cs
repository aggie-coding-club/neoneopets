using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    public Button games;
    // Start is called before the first frame update
    void Start()
    {
        games.onClick.AddListener(LoadGames);
    }

    void LoadGames()
    {
        SceneManager.LoadSceneAsync("GameScreen");
    }
}
