using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject[] frontWheels;
    public GameObject[] Wheels;
    private float maxWheelRotation = 30.0f;

    private Rigidbody playerRB;

    private float horizontalInput;
    private float forwardInput;

    public float turnSpeed;
    public float acceleration;
    public float dragCoeff;

    public float balanceFactor;


    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
        
        float wheelRot = maxWheelRotation * horizontalInput; 
        for(int i = 0; i < frontWheels.Length; i++)
        {
            frontWheels[i].transform.localRotation = Quaternion.Euler(0, wheelRot, 0);
        }
    }

    private void FixedUpdate()
    {
        playerRB.drag = playerRB.velocity.magnitude * dragCoeff;

        if (IsGrounded()) {
            playerRB.velocity += transform.forward * forwardInput * acceleration * Time.fixedDeltaTime;
            float deltaAngle = turnSpeed * horizontalInput * Mathf.Abs(forwardInput) * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0, deltaAngle, 0);
            playerRB.MoveRotation(playerRB.rotation * deltaRotation);
        }

        float tiltAngle = Vector3.Angle(Vector3.up, transform.up);
        Vector3 tiltAxis = Vector3.Cross(transform.up, Vector3.up);
        tiltAxis.Normalize();

        tiltAxis.Normalize();
        playerRB.AddTorque(tiltAxis * tiltAngle * balanceFactor);
    }

    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position + transform.up * 0.01f, -transform.up, 0.1f))
            return true;

        int nWheels = Wheels.Length;
        int count = 0;

        for(int i=0; i<nWheels; i++)
        {
            Vector3 pos = Wheels[i].transform.localPosition;
            Vector3 offset = new Vector3(pos.x, 0.01f, pos.z);
            bool contact = Physics.Raycast(transform.position + transform.rotation * offset, -transform.up, 0.2f);
            if (contact) count++;
        }

        if (count >= 2) return true;
        else return false;
    }
}
