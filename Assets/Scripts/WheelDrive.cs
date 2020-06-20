using UnityEngine;
using System;

[Serializable]
public enum DriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	AllWheelDrive
}

public class WheelDrive : MonoBehaviour
{
    [Tooltip("Maximum steering angle of the wheels")]
	public float maxAngle = 30f;
	[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 300f;
	[Tooltip("Maximum brake torque applied to the driving wheels")]
	public float brakeTorque = 30000f;
	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
    public GameObject wheelShape;
    private GameObject wheelShapeFR;
    private GameObject wheelShapeFL;
    private GameObject frame;
    float speed = 0.0f;
    float deltaAngle = 0.0f;

    [Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed = 5f;
	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsBelow = 5;
	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

    private WheelCollider[] m_Wheels;

    // Find all the WheelColliders down in the hierarchy.
	void Start()
	{
		m_Wheels = GetComponentsInChildren<WheelCollider>();
        // 
        for (int i = 0; i < m_Wheels.Length; ++i) 
		{
			var wheel = m_Wheels [i];
            // Create wheel shapes only when needed.
			if (wheelShape != null)
			{
                Debug.LogError("Wheel collider not instatiated!");
			}
        }
        wheelShapeFL = GameObject.Find("Frame/TransformWheel/wheelFL");
        wheelShapeFR = GameObject.Find("Frame/TransformWheel/wheelFR");
        frame        = GameObject.Find("Frame");
    }

	void Update()
	{
		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

		float angle  = maxAngle * Input.GetAxis("Horizontal");
		float torque = maxTorque * Input.GetAxis("Vertical");

		float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;

		foreach (WheelCollider wheel in m_Wheels)
		{
			// A simple car where front wheels steer while rear ones drive.
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
			{
				wheel.brakeTorque = handBrake;
			}

			if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
			{
				wheel.motorTorque = torque;
			}

			if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
			{
				wheel.motorTorque = torque;
			}

            float framePoseY   = frame.transform.rotation.eulerAngles.y;
            Vector3 v3Velocity = frame.GetComponent<Rigidbody>().velocity;
            float longSPeed    = (float)Math.Sqrt(Math.Pow(v3Velocity.x, 2.0f) + Math.Pow(v3Velocity.y, 2.0f));
            deltaAngle         += (longSPeed / 0.33f) * Mathf.Rad2Deg * Time.deltaTime;

            Debug.Log(deltaAngle);
            // Steering wheels animation
            if (wheelShapeFR)
            {
                //wheelShapeFR.transform.Rotate(deltaAngle, 0, 0, Space.Self);
                wheelShapeFR.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            }

            if (wheelShapeFL)
            {
                //wheelShapeFL.transform.Rotate(deltaAngle, 0, 0, Space.Self);
                wheelShapeFL.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            }
        }
    }
}
