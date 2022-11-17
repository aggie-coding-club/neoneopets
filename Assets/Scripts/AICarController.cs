using System;
using UnityEngine;

public class AICarController : MonoBehaviour
{
    public RaceController.GameSettings gs;
    public GameObject[] wps;
    public int nextWP = 0;

    CarController carController;
    
    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // TODO - Calculate what input to give and then send it to car controller.
        GameObject cwp = wps[nextWP];
        // Move next waypoint if we are within margin of error to current waypoint
        if (Vector3.Distance(cwp.transform.position, transform.position) < 1.5)
        {
            nextWP = (nextWP+1) % wps.Length;
        }
        // Calculate heading to next waypoint
        Vector2 h = (cwp.transform.position - transform.position).normalized;
        float d = Vector2.SignedAngle(transform.up, h);
        Vector2 inp = new Vector2(Math.Clamp(-d, -1, 1), 1);
        carController.SetInputVector(inp);
    }
}
