using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgcodecsModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class StereoConfiguration : MonoBehaviour
{
    // Json configuration file
    public TextWriter stereoConfiguration;
    public bool realCameras;

    //[Serializable]
    public class stereoConfig
    {
        public float baseline;

        // Camera matrices
        /*
         * M1 = ...
         * M2 = ...
         */
    }

    public class cameraParam
    {
        public float x;
        public float y;
        public float z;
        public float pitch;
        public float roll;
        public float yaw;

        public String ToLine()
        {
            string state;
            state = this.x.ToString("0.000") + "," + this.y.ToString("0.000") + "," + this.z.ToString("0.000") + "," + this.pitch.ToString("0.000") + "," + this.roll.ToString("0.000") + "," + this.yaw.ToString("0.000");
            return state;
        }
    }

    stereoConfig stereoParam  = new stereoConfig();
    cameraParam leftCamPar    = new cameraParam();
    cameraParam rightCamPar   = new cameraParam();

    public Camera mainCamera;
    public Camera cameraLeft;
    public Camera cameraRight;

	Vector3 baselineLeftShift;
	Vector3 baselineRightShift;
	Vector3 mainCameraPosition;
	//Vector3 mainCameraRotation;

	// Frames
    private Texture2D leftFrame;
    private Texture2D rightFrame;

    // Sensor size
    public Vector2 sensorSize;

    public float stereoHeight  = 1.0f; // [m] from ground
    public float vehicleOffset = -0.9f;
    public float baseline      = 0.30f; // [m]
    public float stereoPitch   = 10.0f; // [°] 
    public float camLeftFoV    = 55.0f;
    public float camRightFoV   = 55.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Test
        /*
        if (realCameras == true)
        {
            Mat leftCamMat  = new Mat(3, 3, CvType.CV_32FC2);
            leftCamMat.put(cameraLeft.focalLength, 0, cameraLeft.lensShift, 0, cameraLeft.focalLength, cameraLeft.lensShift, 0, 0, 1);
            Mat rightCamMat = new Mat(3, 3, CvType.CV_32FC2);
            rightCamMat.put(cameraRight.focalLength, 0, cameraRight.lensShift, 0, cameraRight.focalLength, cameraRight.lensShift, 0, 0, 1);
        }
        */
        // Set extrinsic stereo parameters
        /* Positions wrt the main camera ref frame */
        Vector3 leftTranslation  = new Vector3(-baseline / 2.0f, stereoHeight, vehicleOffset); // [m]
        Vector3 rightTranslation = new Vector3(baseline / 2.0f, stereoHeight, vehicleOffset); // [m]
        cameraLeft.transform.Translate(leftTranslation, Space.Self); // [m]
        cameraLeft.transform.Rotate(stereoPitch, 0.0f, 0.0f, Space.Self); // [°]
        cameraRight.transform.Translate(rightTranslation, Space.Self); // [m]
        cameraRight.transform.Rotate(stereoPitch, 0.0f, 0.0f, Space.Self); // [°]

        // Camera intrinsic parameters initialisation
        // Vector2 sensorSize;

        cameraLeft.fieldOfView = camLeftFoV; // [°] Camera FoV
        //cameraLeft.focalLength = 2.0f; // [mm]

        cameraRight.fieldOfView = camRightFoV; // [°] Camera FoV
        //cameraRight.focalLength = 2.0f; // [mm]

        saveStereoConfig(stereoParam, leftCamPar, rightCamPar);
        saveJSON(stereoParam, leftCamPar, rightCamPar);
        //Debug.Log("Left Cam:" + cameraLeft.transform.position + "Right Cam:" + cameraRight.transform.position + "Main Cam:" + Camera.main.transform.position);
    }

    public void saveStereoConfig(stereoConfig stereoPar, cameraParam lCam, cameraParam rCam)
    {
        stereoPar.baseline = baseline;
        lCam.x = cameraLeft.transform.position.x;
        lCam.y = cameraLeft.transform.position.y;
        lCam.z = cameraLeft.transform.position.z;

        lCam.pitch = cameraLeft.transform.rotation.x;
        lCam.roll  = cameraLeft.transform.rotation.y;
        lCam.yaw   = cameraLeft.transform.rotation.z;
        
        rCam.x = cameraRight.transform.position.x;
        rCam.y = cameraRight.transform.position.y;
        rCam.z = cameraRight.transform.position.z;

        rCam.pitch = cameraRight.transform.rotation.x;
        rCam.roll  = cameraRight.transform.rotation.y;
        rCam.yaw   = cameraRight.transform.rotation.z;
    }

    public void saveJSON(stereoConfig stereoParam, cameraParam leftCam, cameraParam rightCam)
    {
        string stereoJson1 = JsonUtility.ToJson(stereoParam, true) + "\n";
        string stereoJson2 = JsonUtility.ToJson(leftCam, true) + "\n";
        string stereoJson3 = JsonUtility.ToJson(rightCam, true);
        string JSONData    = stereoJson1 + stereoJson2 + stereoJson3;
        File.WriteAllText("Telemetry/stereoConfig.json", JSONData);
        //File.Close();
        Debug.Log("Stereo configuration saved!");
    }

    // Update is called once per frame
    void Update()
    {
        //
	}

}


