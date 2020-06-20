using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    private Rigidbody car;
    private GameObject wheelFR;
    private GameObject wheelFL;
    public float step    = 0.01f;
    public float rotStep = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        car     = GameObject.Find("Player/Frame").GetComponent<Rigidbody>();
        wheelFR = GameObject.Find("Player/Frame/TransformWheel/wheelFR");
        wheelFL = GameObject.Find("Player/Frame/TransformWheel/wheelFL");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            //frame.AddForce(Vector3.left);
            car.transform.Translate(-step, 0, 0, Space.Self);
        if (Input.GetKey(KeyCode.D))
            // frame.AddForce(Vector3.right);
            car.transform.Translate(step, 0, 0, Space.Self);
        if (Input.GetKey(KeyCode.W))
            // frame.AddForce(Vector3.up);
            car.transform.Translate(0, 0, step, Space.Self);
        if (Input.GetKey(KeyCode.S))
            // frame.AddForce(Vector3.down);
            car.transform.Translate(0, 0, -step, Space.Self);

        /*
        Vector3 v3Velocity = car.GetComponent<Rigidbody>().velocity;
        float longSPeed = (float)Math.Sqrt(Math.Pow(v3Velocity.x, 2.0f) + Math.Pow(v3Velocity.y, 2.0f));
        float rotSpeed = longSPeed / 0.33f;
        // Debug.Log(rotSpeed);

        wheelFR.transform.Rotate(rotSpeed, 0, 0, Space.Self);
        wheelFL.transform.Rotate(rotSpeed, 0, 0, Space.Self);
        */
    }
}
