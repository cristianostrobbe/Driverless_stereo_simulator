using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class stereoFollower : MonoBehaviour {

	private double pitch;
	private double roll; 
	private double yaw; 
	public double throttle; 

	public double targetPitch;
	public double targetRoll;
	public double targetYaw;

	private Quaternion prevRotation = new Quaternion (0, 1, 0, 0);
    float rot = 0;

	void readRotation () {
		Vector3 rot = GameObject.Find ("Player/Frame").GetComponent<Transform> ().rotation.eulerAngles;
		pitch = rot.x;
		yaw   = rot.y;
		roll  = rot.z;
	}

    void FixedUpdate() {

        float step     = 0.01f;
        float rotStep  = 1.0f;
        Rigidbody frame = GameObject.Find("Player/Frame").GetComponent<Rigidbody>();
        if (Input.GetKey(KeyCode.A))
            // frame.AddForce(Vector3.left);
            frame.transform.Translate(-step, 0, 0, Space.Self);
        if (Input.GetKey(KeyCode.D))
            // frame.AddForce(Vector3.right);
            frame.transform.Translate(step, 0, 0, Space.Self);
        if (Input.GetKey(KeyCode.W))
            // frame.AddForce(Vector3.up);
            frame.transform.Translate(0, 0, step, Space.Self);
        if (Input.GetKey(KeyCode.S))
            // frame.AddForce(Vector3.down);
            frame.transform.Translate(0, 0, -step, Space.Self);
        if (Input.GetKey(KeyCode.E))
            frame.transform.Rotate(0, rotStep, 0, Space.Self);
        if (Input.GetKey(KeyCode.Q))
            frame.transform.Rotate(0, -rotStep, 0, Space.Self);
	}
}
