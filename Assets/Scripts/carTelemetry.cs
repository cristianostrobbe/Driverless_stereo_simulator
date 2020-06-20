using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class carTelemetry : MonoBehaviour
{
    //[Serializable]
    public class Car
    {
        public float timeStamp;
        public float x;
        public float y;
        public float z;
        public float delta;
        public float pitch;
        public float roll;
        public float yaw;
        public float mass;
        public float width;
        public float wheelbase;
        public Vector3 centerPoint;

        public String ToLine()
        {
            string state;
            state = this.x.ToString("0.000") + "," + this.y.ToString("0.000") + "," + this.z.ToString("0.000") + "," + this.pitch.ToString("0.000") + "," + this.roll.ToString("0.000") + "," + this.yaw.ToString("0.000") + "," + this.delta.ToString("0.000");
            return state;
        }
    }

    private Rigidbody vehicle;
    private MeshCollider vehicleCollider;
    private GameObject wheelFR;
    private GameObject blueCone;
    private GameObject yellowCone;
    //private GameObject wheelFL;
    public bool saveTelemetry = true;

    //[Serializable]
    Car myCar = new Car();
    // Text writer
    public TextWriter textFile;
    public TextWriter coneFile;

    void Start()
    {
        // Gameobjects initialisations
        vehicleCollider = GameObject.Find("Player/Frame/Vehicle").GetComponent<MeshCollider>();
        vehicle  = GameObject.Find("Player/Frame").GetComponent<Rigidbody>();  
        wheelFR  = GameObject.Find("Player/Frame/TransformWheel/wheelFR");
        blueCone = GameObject.Find("TrafficConeBlueWhiteStart");
        yellowCone = GameObject.Find("TrafficConeYellowWhiteStart");
        textFile = new StreamWriter("Telemetry/car.csv");
        coneFile = new StreamWriter("Telemetry/coneMap.csv");
        //wheelFL = GameObject.Find("Player/Frame/TransformWheel/wheelFL");

        // Get car parameters (initial pose, mass, etc etc)
        getCarParam(myCar);
        // Save simulation paramters into .json
        saveJSON(myCar);
        // Save cone position into coneMap.csv
        getAllConesPosition(blueCone, yellowCone);
        coneFile.Close();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        getCarParam(myCar);
        // Write car parameters into txt files
        textFile.WriteLine(myCar.ToLine());
    }

    public void saveJSON(Car myCar)
    {
        string JSONData = JsonUtility.ToJson(myCar, true);
        PlayerPrefs.SetString("Telemetry/carTelemetry.json", JSONData);
        PlayerPrefs.Save();
    }

    public void getAllConesPosition(GameObject blueCones, GameObject yellowCones)
    {
        string coneAbsPos = "";
        coneFile.WriteLine("Blue cones");
        coneFile.WriteLine("X,Y");
        foreach (Transform child in blueCones.transform)
        {
            // Debug.Log(child.transform.position.x);
            float X = child.transform.position.x;
            float Y = child.transform.position.z;
            coneAbsPos = X.ToString("0.000") + "," + Y.ToString("0.000");
            coneFile.WriteLine(coneAbsPos);
        }

        coneFile.WriteLine("Yellow cones");
        coneFile.WriteLine("X,Y");
        foreach (Transform child in yellowCones.transform)
        {
            float X = child.transform.position.x;
            float Y = child.transform.position.z;
            coneAbsPos = X.ToString("0.000") + "," + Y.ToString("0.000");
            coneFile.WriteLine(coneAbsPos);
        }
    }

    public void getCarParam(Car myCar)
    {
        // Save car time stamp
        myCar.timeStamp = Time.deltaTime;
        // Car position - rotation (IMU)
        myCar.x = vehicle.transform.position.x;
        myCar.y = vehicle.transform.position.y;
        myCar.z = vehicle.transform.position.z;
        myCar.pitch = vehicle.transform.rotation.x;
        myCar.roll = vehicle.transform.rotation.z;
        myCar.yaw = vehicle.transform.rotation.y;
        // Car control
        myCar.delta = wheelFR.transform.rotation.y;
        // Car physics
        myCar.mass = vehicle.mass;
        myCar.width = vehicleCollider.bounds.max.x;
        myCar.wheelbase = vehicleCollider.bounds.max.y;
        myCar.centerPoint = vehicleCollider.bounds.center;
    }

    void OnApplicationQuit()
    {
        textFile.Close();
        Debug.Log("Telemetry saved!");
    }
}
