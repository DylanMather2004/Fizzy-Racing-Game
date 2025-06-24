using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    [Header("Movement Modifiers")]
    public float topSpeed = 10.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    public float turnAngle = 10;
    public float driftAngle = 20;
    float currentTurnAngle = 10;
    public float upThrust = 10;
    public float gravity = 10;
    float currentSpeed = 0.0f;
    private bool isAccelerating = false;
    private bool isDrifting = false; 
    float rotDir;
    [Header("PID Configs")]
    public PIDController PID;
    public float maxDistanceFromGround;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentTurnAngle = turnAngle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(rb.position, -rb.transform.up, out hit))
        {

            Debug.DrawLine(rb.position, hit.point, color: Color.green, 0.1f);

            float Input = PID.PIDUpdate(Time.fixedDeltaTime, rb.position.y, hit.point.y + maxDistanceFromGround);
            rb.AddForce(new Vector3(0, upThrust * Input, 0));
            if (rb.position.y>hit.point.y+maxDistanceFromGround+1)
            {
                rb.AddForce(-Vector3.up * gravity);
            }
            
        }
        else
        {
            rb.AddForce(-Vector3.up * gravity);
        }
     
        CalculateThrust();
        rb.velocity=new Vector3(transform.forward.x*currentSpeed,rb.velocity.y,transform.forward.z*currentSpeed);
        if (rotDir != 0)
        {
            rb.AddTorque(Vector3.up * currentTurnAngle * rotDir);
        }
        Vector3 rotVector = Vector3.Cross(hit.normal, transform.up);
        rb.AddTorque(-rotVector*30);
        //Debug.Log(transform.forward);
    }

    public void ToggleThruster(InputAction.CallbackContext context)
    {
        Debug.Log("Input");
        Debug.Log(context.phase);
        if (context.started)
        {
            Debug.Log("started");
            isAccelerating = true;
        }
        if (context.canceled)
        {
            Debug.Log("cancelled");
            isAccelerating = false;
        }
    }

    void CalculateThrust()
    {
        if (isAccelerating)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, topSpeed, acceleration);
            rb.drag = 0;
        }
        else if (!isAccelerating)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration);
            if (currentSpeed >0.1)
            {
                Debug.Log(currentSpeed);
                rb.drag = 4;
            }
            else
            {
                currentSpeed = 0;
                rb.drag = 4;
            }
        }
    }
    public void RotateShip(InputAction.CallbackContext context)
    {
        rotDir = context.ReadValue<float>();
        Debug.Log(rotDir);
        rb.AddRelativeForce((Vector3.right * rotDir) * 50);
 
 
        
    }

    public void AirBrake(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            currentTurnAngle = driftAngle;
            Debug.Log("started");
        }
        else if (context.performed || context.canceled)
        {
            currentTurnAngle = turnAngle;
            Debug.Log("stopped");
        }
    }
}
