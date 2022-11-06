using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    // UI Components
    [SerializeField] GameObject timerObject;
    [SerializeField] GameObject livesObject;
    [SerializeField] GameObject lapsObject;

    // State
    // Stores time elapsed during race in seconds
    float time = 0f;
    ushort lives = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Keep track of time elapsed while playing
        time += Time.deltaTime;

        // Format time
        // not going to do anything special for > 59:59
        int itime = (int) time;
        string ts = string.Format("{0:d2}:{1:d2}", itime / 60, itime % 60);

        // Set UI Elements text
        timerObject.GetComponent<TextMeshProUGUI>().text = ts;
    }
}
