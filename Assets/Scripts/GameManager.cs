using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.VideoioModule;
using OpenCVForUnity.ImgprocModule;
using System.IO;

public class GameManager : MonoBehaviour {
    private int camWidth  = 1024;
    private int camHeight = 1024;
    private int newWidth;
    private int newHeight;
    private Texture2D cam1Tex;
    private Texture2D cam2Tex;
    private Texture2D out1Tex;
	private int frameIndex = 0;
    private int tempVar   = 1;
    public bool saveStereo  = false;

    // Stereo parameters
    public int preFilterSize     = 0; // FIXME:
    public int preFilterCap      = 63;
    public int blockSize         = 5;
    public int minDisparity      = 5;
    public int numDisparities    = 160;
    public int textureThreshold; // FIXME:
    public int uniquenessRatio   = 15;
    public int speckleWindowSize = 0;
    public int speckleRange      = 2;
    public int disp12MaxDiff     = 1;
    public int P1                = 0; // 24*vindosize^2
    public int P2                = 0; // 96 * windowsize ^2

    VideoWriter writerLeft;
    VideoWriter writerRight;
    VideoWriter writerDispMap;

    private string savePathLeft    = "Stream/leftCam.mp4";
    private string savePathRight   = "Stream/rightCam.mp4";
    private string savePathDispMap = "Stream/dispCam.mp4";
    public int fps = 5;

    void Awake() {
		Application.targetFrameRate = 1;
		GameObject.Find ("/Player/Frame/CameraLeft").GetComponent<Camera> ().aspect = 1.0f;
		GameObject.Find ("/Player/Frame/CameraRight").GetComponent<Camera> ().aspect = 1.0f;
    }

    void FixedUpdate ()
    {
        readCameras();
        frameIndex++;
    }

    void Start()
    {
        streamSetup();
        Debug.Log("Started!");
	}

    private void readCameras()
    {
        cam1Tex = new Texture2D(camWidth, camHeight, TextureFormat.RGB24, false);
        RenderTexture cam1RT = GameObject.Find("/Player/Frame/CameraLeft").GetComponent<Camera>().targetTexture;
        RenderTexture.active = cam1RT;
        cam1Tex.ReadPixels(new UnityEngine.Rect(0, 0, cam1RT.width, cam1RT.height), 0, 0);
        cam1Tex.Apply();
        RenderTexture.active = null;

        cam2Tex = new Texture2D(camWidth, camHeight, TextureFormat.RGB24, false);
        RenderTexture cam2RT = GameObject.Find("/Player/Frame/CameraRight").GetComponent<Camera>().targetTexture;
        RenderTexture.active = cam2RT;
        cam2Tex.ReadPixels(new UnityEngine.Rect(0, 0, cam2RT.width, cam2RT.height), 0, 0);
        cam2Tex.Apply();
        RenderTexture.active = null;

        //Read the left and right images
        Mat imgLeftRGB   = new Mat(cam1Tex.width, cam1Tex.height, CvType.CV_8UC3);
        Mat imgRightRGB  = new Mat(cam2Tex.width, cam2Tex.height, CvType.CV_8UC3);
        Mat imgLeftTemp  = new Mat(cam1Tex.width, cam1Tex.height, CvType.CV_8UC1);
        Mat imgRightTemp = new Mat(cam2Tex.width, cam2Tex.height, CvType.CV_8UC1);
        Utils.texture2DToMat(cam1Tex, imgLeftRGB);
        Utils.texture2DToMat(cam2Tex, imgRightRGB);
        Utils.texture2DToMat(cam1Tex, imgLeftTemp);
        Utils.texture2DToMat(cam2Tex, imgRightTemp);

        // Resize
        float aspcetRatio = cam1Tex.width / cam1Tex.height;
        //float scalingFactor = 0.5f; // [%]
        int scaler = 1;
        int newWidth  = cam1Tex.width / scaler;
        int newHeight = cam1Tex.height / scaler;
        Mat imgLeft   = new Mat(newWidth, newHeight, CvType.CV_8UC1);
        Mat imgRight  = new Mat(newWidth, newHeight, CvType.CV_8UC1);

        Imgproc.resize(imgLeftTemp, imgLeft, new Size(newWidth, newHeight));
        Imgproc.resize(imgRightTemp, imgRight, new Size(newWidth, newHeight));

        Mat imgDisparity16S = new Mat(imgLeft.rows(), imgLeft.cols(), CvType.CV_16S);
        Mat imgDisparity8U  = new Mat(imgLeft.rows(), imgLeft.cols(), CvType.CV_8UC1);

        StereoSGBM SGBM = StereoSGBM.create(
            minDisparity,
            numDisparities,
            blockSize,
            P1,
            P2,
            disp12MaxDiff,
            preFilterCap,
            uniquenessRatio,
            speckleWindowSize,
            speckleRange
        );

        SGBM.compute(imgLeft, imgRight, imgDisparity16S);
        //normalize to CvType.CV_8U
        Core.normalize(imgDisparity16S, imgDisparity8U, 0, 255, Core.NORM_MINMAX, CvType.CV_8U);

        Texture2D texture = new Texture2D(imgDisparity8U.cols(), imgDisparity8U.rows(), TextureFormat.RGBA32, false);

        Utils.matToTexture2D(imgDisparity8U, texture);

        // Rendering on debug screen
        GameObject.Find("/CameraTextures/CameraLeftRawImage").GetComponent<RawImage>().texture = cam1Tex;
        GameObject.Find("/CameraTextures/CameraRightRawImage").GetComponent<RawImage>().texture = cam2Tex;
        GameObject.Find("/CameraTextures/DisparityMap").GetComponent<RawImage>().texture = texture;
        

        /* Write to disk */
        if (saveStereo)
        {
            streamVideo(imgLeftRGB, imgRightRGB, imgDisparity8U);
            /*
            byte[] bytes = cam2Tex.EncodeToPNG();
            simVar += 1;
            string filename = "test" + simVar.ToString() + " .png"; //ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            */
        }

    }

    private void streamSetup()
    {
        string path = "Telemetry/Stream";
        // Clean output directory
        if (Directory.Exists(path)) { Directory.Delete(path, true); }
        Directory.CreateDirectory(path);

        // Create video sources
        writerLeft    = new VideoWriter();
        writerRight   = new VideoWriter();
        writerDispMap = new VideoWriter();
        // Fps are 1/fixedTimeStamp = 1/0.05 = 20 [fps]
        Size outSize = new Size(camWidth, camHeight); // Can be scaled
        writerLeft.open("Telemetry/Stream/leftCam.mp4", VideoWriter.fourcc('M', 'J', 'P', 'G'), 20, outSize, true);
        writerRight.open("Telemetry/Stream/rightCam.mp4", VideoWriter.fourcc('M', 'J', 'P', 'G'), 20, outSize, true);
        writerDispMap.open("Telemetry/Stream/dispCam.mp4", VideoWriter.fourcc('M', 'J', 'P', 'G'), 20, outSize);

        if (!writerLeft.isOpened())
        {
            Debug.LogError("Clean Strem folder!");
        }
    }

    private void streamVideo(Mat camLeft, Mat camRight, Mat dispMap)
    {
        Imgproc.cvtColor(camLeft, camLeft, Imgproc.COLOR_RGB2BGR);
        writerLeft.write(camLeft);

        Imgproc.cvtColor(camRight, camRight, Imgproc.COLOR_RGB2BGR);
        writerRight.write(camRight);

        Size dispSize =   new Size(newWidth, newHeight);
        Mat flippedDisp = dispMap.clone();// new Mat(newWidth, newHeight, CvType.CV_8U);

        Point centerP   = new Point(newWidth / 2, newHeight / 2);
        // Rotate by 180
        Mat M1 = Imgproc.getRotationMatrix2D(centerP, 180, 1);
        // Translate by numDisparities
        Mat M2 = new Mat(2, 2, CvType.CV_32FC2);
        M2.put(0, 0, 1, 0, numDisparities, 0, 1, 0);
        Mat M = M1 + M2;
        // Imgproc.warpAffine(dispMap, flippedDisp, M2, dispSize);
        writerDispMap.write(dispMap);

    }

    void OnApplicationQuit()
    {
        writerLeft.release();
        writerRight.release();
        writerDispMap.release();
        Debug.Log("Saved stream!");
    }
}
