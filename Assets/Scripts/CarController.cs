using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [Tooltip("0 means no movement")]
    public float accelerationFactor = 20.0f;

    [Tooltip("Range: [0, 1]. 0 = no drift. 1 = no traction")]
    public float driftFactor = 0.1f;

    [Tooltip("How fast to turn. 0 = no turning")]
    public float turnFactor = 3.0f;

    [Tooltip("Coefficient in the drag calculation. 0 = no drag")]
    public float dragFactor = 0.15f;

    [Tooltip("Base rolling resistance. 0 = slip and slide")]
    public float dragAdjustment = 2.0f;

    [Tooltip("Maximum Speed. 0 = no movement. May not apply as speed is also bound by drag")]
    public float maxSpeed = 20.0f;

    // Input
    float accelerationInput = 0;
    float steeringInput = 0;
    
    // State
    float rotationAngle = 0;
    float velocityUp = 0;

    // Components
    private Rigidbody2D carRigid;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Collect Components
        carRigid = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Frame-rate independent update for physics calculations
    void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    // Calculates and applies the relative force that the engine should exert on the car
    void ApplyEngineForce()
    {
        // Calculate amount of velocity that is forward
        velocityUp = Vector2.Dot(transform.up, carRigid.velocity);

        // Produce drag to slow car
        // Actual car drag involves a constant tire drag
        // plus a drag curve relative to the square of velocity
        carRigid.drag = velocityUp * velocityUp * dragFactor * Time.fixedDeltaTime + dragAdjustment;

        // Don't generate force if at max speed already
        if (velocityUp > maxSpeed)
            return;

        // Generate the engine force
        Vector2 engineVector = transform.up * accelerationInput * accelerationFactor;

        // Apply the force to the car
        carRigid.AddForce(engineVector, ForceMode2D.Force);
    }

    // Calculates and the applies the steering force onto the car
    void ApplySteering()
    {
        rotationAngle -= steeringInput * turnFactor;

        carRigid.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigid.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigid.velocity, transform.right);

        carRigid.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
}
