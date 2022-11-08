using System;
using TMPro;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    // Controller Info
    public GameSettings gameSettings;

    [Header("UI Elements")]
    [SerializeField] GameObject introUI;
    [SerializeField] GameObject raceUI;
    [SerializeField] GameObject timerObject;
    [SerializeField] GameObject livesObject;
    [SerializeField] GameObject lapsObject;
    
    [Header("Prefabs")]
    [SerializeField] GameObject userCarPrefab;
    [SerializeField] GameObject aiCarPrefab;
    
    // Game State
    enum GameState {
        PreGame,
        Playing,
        EndGame,
    }
    
    GameState state = GameState.PreGame;
    #nullable enable
    GameObject? userCar = null;
    #nullable disable

    // Race State
    // Stores time elapsed during race in seconds
    float time = 0f;
    // Lives left = 5 - {number of collisions}
    ushort lives = 5;
    float crashTime = float.NegativeInfinity;

    // Start is called before the first frame update
    void Start()
    {
        print(gameSettings);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Playing)
        {
            // Keep track of time elapsed while playing
            time += Time.deltaTime;
            // Format time
            // not going to do anything special for > 59:59
            int itime = (int) time;
            string ts = string.Format("{0:d2}:{1:d2}", itime / 60, itime % 60);
            // Set UI Elements text
            timerObject.GetComponent<TextMeshProUGUI>().text = ts;
            
            // Display lives
            string ls = string.Format("{0:d}", lives);
            livesObject.GetComponent<TextMeshProUGUI>().text = ls;
        }
    }
    
    void OnCollisionEnter2D()
    {
        if (crashTime + gameSettings.crashCooldown > time)
            return;
        --lives;
        crashTime = time;
    }
    
    void InitCars()
    {
        // Destroy existing cars - User Car and AI Car
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("UserCar"))
        {
            Destroy(car);
        }
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("AICar"))
        {
            Destroy(car);
        }

        // Instantiate Prefabs for user car and ai car
        // based on the car positions
        userCar = (GameObject) Instantiate(userCarPrefab, gameSettings.carPositions[0], Quaternion.identity);
        for (int i = 1; i < gameSettings.carPositions.Length; i++)
        {
            Instantiate(aiCarPrefab, gameSettings.carPositions[i], Quaternion.identity);
        }
        // Add collision bridge for User Car
        userCar.GetComponent<CarController>().oncol = OnCollisionEnter2D;
        userCar.GetComponent<CarController>().gameSettings = gameSettings;
    }
    
    public void BeginGame() {
        // Setup Scene
        InitCars();
        // Initialize Race State
        lives = 5;
        time = 0f;
        // Change UI
        introUI.SetActive(false);
        raceUI.SetActive(true);
        // Change Game State
        state = GameState.Playing;
    }
    
}
