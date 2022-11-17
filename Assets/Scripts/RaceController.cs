using System;
using TMPro;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    [Serializable]
    public struct GameSettings {
        [Header("Scene Settings")]
        public Vector3[] carPositions; /* = {
            new Vector3(-7.28999996f,0.720000029f,0),
            new Vector3(-6.78000021f,0.409999996f,0),
            new Vector3(-6.21999979f,0.150000006f,0),
            new Vector3(-7.32000017f,-0.119999997f,0),
            new Vector3(-6.78000021f,-0.430000007f,0),
            new Vector3(-6.21999979f,-0.74000001f,0),
            new Vector3(-7.32000017f,-0.949999988f,0),
            new Vector3(-6.78000021f,-1.24000001f,0),
        };*/
        
        [Header("User Car Settings")]
        [Tooltip("0 means no movement")]
        public float accelerationFactor; // = 100.0f;

        [Tooltip("Range: [0, 1]. 0 = no drift. 1 = no traction")]
        public float driftFactor; // = 0.1f;

        [Tooltip("How fast to turn. 0 = no turning")]
        public float turnFactor; // = 3.0f;

        [Tooltip("Coefficient in the drag calculation. 0 = no drag")]
        public float dragFactor; // = 0.15f;

        [Tooltip("Base rolling resistance. 0 = slip and slide")]
        public float dragAdjustment; // = 5.0f;

        [Tooltip("Maximum Speed. 0 = no movement. May not apply as speed is also bound by drag")]
        public float maxSpeed; // = 5.0f;
        
        [Header("Game Settings")]
        [Tooltip("Time in seconds between loss of a life.")]
        public float crashCooldown; // = 2;
        
        public GameSettings(
            Vector3[] carPositions,
            float accelerationFactor,
            float driftFactor,
            float turnFactor,
            float dragFactor,
            float dragAdjustment,
            float maxSpeed,
            float crashCooldown
        ) {
            this.carPositions = carPositions;
            this.accelerationFactor = accelerationFactor;
            this.driftFactor = driftFactor;
            this.turnFactor = turnFactor;
            this.dragFactor = dragFactor;
            this.dragAdjustment = dragAdjustment;
            this.maxSpeed = maxSpeed;
            this.crashCooldown = crashCooldown;
        }
    }
    private float[] speedAdjustments = {
        0.975f,
        0.95f,
        0.90f,
        0.80f,
        0.75f,
        0.70f,
        0.60f,
    };

    // Controller Info
    public GameSettings gameSettings;
    public GameObject[] waypoints;

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
            GameObject car = (GameObject) Instantiate(aiCarPrefab, gameSettings.carPositions[i], Quaternion.identity);
            car.GetComponent<AICarController>().gs = gameSettings;
            car.GetComponent<CarController>().gameSettings = gameSettings;
            car.GetComponent<AICarController>().wps = waypoints;
            car.GetComponent<AICarController>().nextWP = 1;
            car.GetComponent<CarController>().speedAdjustment = speedAdjustments[i-1];
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
