using UnityEngine;
using System;

public class CarController : MonoBehaviour
{
    public GameSettings gameSettings;

    // Input
    float accelerationInput = 0;
    float steeringInput = 0;
    
    // State
    float rotationAngle = 0;
    float velocityUp = 0;
    
    public float speedAdjustment = 1f;

    // Components
    private Rigidbody2D carRigid;
    public Action oncol = () => {}; 

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
    
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "AICar")
        {
            oncol();
        }
    }

    // Calculates and applies the relative force that the engine should exert on the car
    void ApplyEngineForce()
    {
        // Calculate amount of velocity that is forward
        velocityUp = Math.Abs(Vector2.Dot(transform.up, carRigid.velocity));

        // Produce drag to slow car
        // Actual car drag involves a constant tire drag
        // plus a drag curve relative to the square of velocity
        carRigid.drag = velocityUp * velocityUp * gameSettings.dragFactor * Time.fixedDeltaTime + gameSettings.dragAdjustment;

        // Don't generate force if at max speed already
        if (velocityUp > gameSettings.maxSpeed * speedAdjustment)
            return;

        // Generate the engine force
        Vector2 engineVector = transform.up * accelerationInput * gameSettings.accelerationFactor;

        // Apply the force to the car
        carRigid.AddForce(engineVector, ForceMode2D.Force);
    }

    // Calculates and the applies the steering force onto the car
    void ApplySteering()
    {
        rotationAngle -= steeringInput * gameSettings.turnFactor;

        carRigid.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigid.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigid.velocity, transform.right);

        carRigid.velocity = forwardVelocity + rightVelocity * gameSettings.driftFactor;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
}
