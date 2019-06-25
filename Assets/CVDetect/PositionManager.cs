using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

using System.Collections.Generic;
using System.Linq;
using System;

public class PositionManager : MonoBehaviour
{
    public static PositionManager instance;
    [Header("G Capture 攝影機系統名稱，可於OBS確認是否運作")]
    public string GCaptureName = "Dexetek Polaris Video Capture";

    [Header("輸出貼圖,測試用")]
    public Renderer outputRenderer;

    [Header("攝影機設定 (翻轉)")]
    public bool flipY = false;

    [Header("場景長寬 (Unity 單位), 輸出轉換用")]
    public float sceneWidth = 10;
    public float sceneHeight = 5;

    [Header("偵測靈敏度, 數值越小越靈敏")]
    [Range(1, 255)]
    public float detectThreshold = 30f;
    [Header("判定規則 (基礎大小, 容許誤差, 基礎形狀比, 容許誤差) , 在4公尺高時的基礎大小建議值為1200")]
    [Range(0, 4000)]
    public int acceptRect = 1200;
    [Range(10, 2000)]
    public int acceptRectAllowance = 800;
    [Range(0.01f, 5f)]
    public float acceptRatio = 1.0f;
    [Range(0, 2)]
    public float acceptRatioAllowance = 0.5f;

    [Header("最終輸出點,可直接存取")]
    public List<Vector3> DetectResult = new List<Vector3>();


    /// <summary>
    /// 攝影機存取以及轉換至Texture2D
    /// </summary>
    WebCamTexture webcam;
    Texture2D texComing;

    int frameCount = 0;
    Mat imgPrevious = new Mat();
    Mat imgNext = new Mat();
    Mat imgWork1 = new Mat();
    Mat imgWork2 = new Mat();
    Mat imgDifference;
    Mat imgBinary;

    //所有觀眾座標資料
    List<Blob> AllBlobs = new List<Blob>();
    List<Blob> currentFrameBlobs = new List<Blob>();

    //場地限制相關
    Point[] crossingLine = new Point[2];
    int intHorizontalLinePosition;

    //For Kinect
    [Header("Kinect 相關")]
	public int playerIndex;

    void Awake() {
        instance = this;
    }

    void Start()
    {
        //WebCamDevice[] devices = WebCamTexture.devices;
        webcam = new WebCamTexture(GCaptureName);
        webcam.Play();
        Debug.Log("webcam width:"+ webcam.width);
        Debug.Log("webcam height:"+ webcam.height);

        // Output Renderer Texture
        texComing = new Texture2D(webcam.width, webcam.height);
        outputRenderer.material.mainTexture = texComing;

        if(flipY){
            outputRenderer.transform.localScale = outputRenderer.transform.localScale + new Vector3(-2 * outputRenderer.transform.localScale.x, 0, 0);
        }

        // ??? 媛安踹共
        crossingLine[0].X = 0;
        crossingLine[0].Y = intHorizontalLinePosition;
        crossingLine[1].X = imgPrevious.Cols - 1;
        crossingLine[1].Y = intHorizontalLinePosition;
        intHorizontalLinePosition = (int)Mathf.Round((imgPrevious.Rows) * 0.5f);

        //Initial mat data with same value
        texComing.SetPixels(webcam.GetPixels());
        texComing.Apply();
        imgPrevious = Mat.FromImageData(texComing.EncodeToPNG());
        imgNext = Mat.FromImageData(texComing.EncodeToPNG());

        //rgbaMat = new Mat (webcam.height, webcam.width, MatType.CV_8UC4);

    }

    // Update is called once per frame
    void Update()
    {
        //Update tex data
        texComing.SetPixels(webcam.GetPixels());
        texComing.Apply();

        //Clear Blob Infomation for next frame
        currentFrameBlobs.Clear();

        //兩個img 宣告
        imgWork1 = imgPrevious.Clone();
        imgWork2 = imgNext.Clone();
        //Cv2.ImShow("coming", imgNext);

        
        //必要步驟1. 變成黑白
        Cv2.CvtColor(imgWork1, imgWork1, ColorConversion.BgrToGray);
        Cv2.CvtColor(imgWork2, imgWork2, ColorConversion.BgrToGray);
        
        //必要步驟2. 高斯模糊
        Size size = new Size(5, 5);
        Cv2.GaussianBlur(imgWork1, imgWork1, size, 0);
        Cv2.GaussianBlur(imgWork2, imgWork2, size, 0);
        
        //兩個影像的差別 = imgDifference
        imgDifference = new Mat();
        Cv2.Absdiff(imgWork1, imgWork2, imgDifference);
        //Cv2.ImShow("imgDifference", imgDifference);
        
        //二元法，低於Threshold是0，高於Threshold是 255
        imgBinary = new Mat();
        Cv2.Threshold(imgDifference, imgBinary, detectThreshold, 255.0, ThresholdType.Binary);
        //Cv2.ImShow("imgBinary", imgBinary);

        Size size3 = new Size(3, 3);
        Size size5 = new Size(5, 5);
        Size size7 = new Size(7, 7);
        Size size15 = new Size(15, 15);

        Mat structuringElement3x3 = Cv2.GetStructuringElement(StructuringElementShape.Rect,size3);
        Mat structuringElement5x5 = Cv2.GetStructuringElement(StructuringElementShape.Rect, size5);
        Mat structuringElement7x7 = Cv2.GetStructuringElement(StructuringElementShape.Rect, size7);
        Mat structuringElement15x15 = Cv2.GetStructuringElement(StructuringElementShape.Rect, size15);
        
        //擴張，縮小 二元影像
        for ( int i = 0; i < 2; i++)
        {
            Cv2.Dilate(imgBinary, imgBinary, structuringElement5x5);
            Cv2.Dilate(imgBinary, imgBinary, structuringElement5x5);
            Cv2.Erode(imgBinary, imgBinary, structuringElement5x5);
        }

        Mat imgBinaryCopy = imgBinary.Clone();

        //開始抓輪廓
        // Cv2.ImShow("imgThresh2", imgBinaryCopy);
        Point[][] contourPointGroup;
        HierarchyIndex[] hierarchyIndexes;
        Cv2.FindContours( imgBinaryCopy, out contourPointGroup , out hierarchyIndexes, ContourRetrieval.External, ContourChain.ApproxSimple, null);

        //drawAndShowContours(imgThresh.Size(), contourpp, "imgContours");

        List<List<Point>> contourWork = new List<List<Point>>();

        /*--------------test for c# list structure---------------------
        List<List<Point>> contourss = new List<List<Point>>();
        List<Point> ppp = new List<Point>();
        Point pp = new Point(2, 3);
        ppp.Add(pp);
        
        contourss.Add(ppp);
       --------------end of test for c# list structure--------------------- */
       
        //每個contourpp 群組 分給 contourss
        for (int i = 0; i < contourPointGroup.Length; i++)
        {
                List<Point> ppp = new List<Point>();
                for (int j = 0; j < contourPointGroup[i].Length; j++)
                {
                     ppp.Add(contourPointGroup[i][j]);
                }
                contourWork.Add(ppp);
        }
        Point[][] convexHulls = new Point[contourPointGroup.Length][];
        for ( int i = 0; i < contourPointGroup.Length; i++)
        {
            List<Point> contoursp = contourWork[i];
            IEnumerable<Point> contourse = contoursp ;
            convexHulls[i] = Cv2.ConvexHull(contourse);
        }

        //2019.06.25 畫出區塊
        //drawAndShowContours(imgBinary.Size(), convexHulls, "imgConvexHulls");
        
        foreach (Point[] item in convexHulls)
        {
            //宣告出一個泡泡
            Blob possibleBlob = new Blob(item);

            int area = possibleBlob.currentBoundingRect.Width * possibleBlob.currentBoundingRect.Height;
            Debug.Log("泡泡寬度: " + area + " , X 倍率: " + possibleBlob.dblCurrentAspectRatio);

            //2019.06.25 新竹場地參數
            if( area < acceptRect + acceptRectAllowance &&
                area > acceptRect - acceptRectAllowance &&
                possibleBlob.dblCurrentAspectRatio < acceptRatio + acceptRatioAllowance &&
                possibleBlob.dblCurrentAspectRatio > acceptRatio - acceptRatioAllowance)
            {
                currentFrameBlobs.Add(possibleBlob);
            }
                        
            //面積多少為人? 媛安踹共
            //if (area > 100 &&
            //    possibleBlob.dblCurrentAspectRatio > 0.2 &&
            //    possibleBlob.dblCurrentAspectRatio < 4.0 &&
            //    possibleBlob.currentBoundingRect.Width > 50 &&
            //    possibleBlob.currentBoundingRect.Height > 50 &&
            //    possibleBlob.dblCurrentDiagonalSize > 60.0 &&
            //    (Cv2.ContourArea(possibleBlob.currentContour) / (double)area) > 0.50)
            //{
            //    currentFrameBlobs.Add(possibleBlob);
            //}
        }

        //設置全局擁有的Blob
        AllBlobs.Clear();
        DetectResult.Clear();
        AllBlobs.AddRange(currentFrameBlobs);
        foreach (var item in AllBlobs)
        {
            //CV跟Unity 中心座標不一樣、Y值正方向座標不一樣
            Vector3 rawPoint = item.BaseCenterPoint;
            float _x = (rawPoint.x / webcam.width) * (sceneWidth * 2) - sceneWidth;
            float _y = -((rawPoint.y / webcam.height) * (sceneHeight * 2) - sceneHeight);

            DetectResult.Add(new Vector3(_x, _y, 0));
        }

        //drawAndShowContours(imgThresh.Size(), currentFrameBlobs, "imgCurrentFrameBlobs");

        //追蹤部分, 將新增進來的人 (Blob) 與現有 Blob 做比對
        //if (blnFirstFrame == true)
        //{
        //    foreach (Blob currentFrameBlob in currentFrameBlobs)
        //    {
        //        blobs.Add(currentFrameBlob);
        //    }
        //}
        //else
        //{
        //    matchCurrentFrameBlobsToExistingBlobs(blobs, currentFrameBlobs);
        //}
        // drawAndShowContours(imgThresh.Size(), blobs, "imgBlobs"); 
        
        //2019.06.25 測試用
        //imgWork2 = imgNext.Clone(); // get another copy of frame 2 since we changed the previous frame 2 copy in the processing above
        //DrawBlobInfoOnMat(AllBlobs, imgWork2);
        //Cv2.ImShow("ResultDetect", imgWork2);

        //重設目前Frame 存在的人
        currentFrameBlobs.Clear();
        imgPrevious = imgNext;
        imgNext = Mat.FromImageData(texComing.EncodeToPNG());
        if(flipY)
            Cv2.Flip(imgNext, imgNext, FlipMode.Y);  //攝影機翻轉,依展場需求
        frameCount++;
    }
    public void DrawBlobInfoOnMat(List<Blob> blobs, Mat imgFrame2Copy){
        for (int i = 0; i < blobs.Count; i++)
        {
            Point targetPoint = new Point(blobs[i].BaseCenterPoint.x, blobs[i].BaseCenterPoint.y);
            Vector2 targetVector = new Vector2(targetPoint.X, targetPoint.Y);

            Cv2.Rectangle(imgFrame2Copy, blobs[i].currentBoundingRect, PMUtility.SCALAR_RED, 2);
            Cv2.Circle(imgFrame2Copy,targetPoint.X,targetPoint.Y,3,PMUtility.SCALAR_GREEN, 1);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public void matchCurrentFrameBlobsToExistingBlobs(List<Blob> existingBlobs, List<Blob> currentFrameBlobs)
    {

        foreach (Blob existingBlob in existingBlobs)
        {

            existingBlob.blnCurrentMatchFoundOrNewBlob = false;
            existingBlob.predictNextPosition();
            
        }
        

        foreach (Blob currentFrameBlob in currentFrameBlobs)
        { 

            int intIndexOfLeastDistance = 0;
            double dblLeastDistance = 100000.0;

            for ( int i = 0; i < existingBlobs.Count; i++)
            {

                if (existingBlobs[i].blnStillBeingTracked == true)
                {

                    double dblDistance = distanceBetweenPoints(currentFrameBlob.centerPositions[currentFrameBlob.centerPositions.Count-1], existingBlobs[i].predictedNextPosition);

                    if (dblDistance < dblLeastDistance)
                    {
                        dblLeastDistance = dblDistance;
                        intIndexOfLeastDistance = i;
                    }
                }
            }
            
            if (dblLeastDistance < currentFrameBlob.dblCurrentDiagonalSize*0.5 )
            {
              //  Debug.Log("in");
                addBlobToExistingBlobs(currentFrameBlob, existingBlobs, intIndexOfLeastDistance);
            }
            else
            {
                //Debug.Log("innew");
                addNewBlob(currentFrameBlob, existingBlobs);
            }

        }
        
        
        foreach (Blob existingBlob in existingBlobs)
        {
            
            if (existingBlob.blnCurrentMatchFoundOrNewBlob == false)
            {
                existingBlob.intNumOfConsecutiveFramesWithoutAMatch++;
                //Debug.Log(existingBlob.intNumOfConsecutiveFramesWithoutAMatch);
            }

            if (existingBlob.intNumOfConsecutiveFramesWithoutAMatch >=50)
            {
                existingBlob.intNumOfConsecutiveFramesWithoutAMatch = 0;
                existingBlob.blnStillBeingTracked = false;
            }
        }

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public void addBlobToExistingBlobs(Blob currentFrameBlob, List<Blob> existingBlobs, int intIndex)
    {

        existingBlobs[intIndex].currentContour = currentFrameBlob.currentContour;
        existingBlobs[intIndex].currentBoundingRect = currentFrameBlob.currentBoundingRect;

        existingBlobs[intIndex].centerPositions.Add(currentFrameBlob.centerPositions[currentFrameBlob.centerPositions.Count-1 ]);

        existingBlobs[intIndex].dblCurrentDiagonalSize = currentFrameBlob.dblCurrentDiagonalSize;
        existingBlobs[intIndex].dblCurrentAspectRatio = currentFrameBlob.dblCurrentAspectRatio;

        existingBlobs[intIndex].blnStillBeingTracked = true;
        existingBlobs[intIndex].blnCurrentMatchFoundOrNewBlob = true;
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public void addNewBlob(Blob currentFrameBlob, List<Blob> existingBlobs)
    {

        currentFrameBlob.blnCurrentMatchFoundOrNewBlob = true;

        existingBlobs.Add(currentFrameBlob);
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public double distanceBetweenPoints(Point point1, Point point2)
    {

        int intX = Mathf.Abs(point1.X - point2.X);
        int intY = Mathf.Abs(point1.Y - point2.Y);

        return (Mathf.Sqrt(Mathf.Pow(intX, 2) + Mathf.Pow(intY, 2)));
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public void drawAndShowContours(Size imageSize, IEnumerable<IEnumerable<Point>> contours, string strImageName)
    {
        Mat image=new Mat(imageSize, MatType.CV_8UC3, PMUtility.SCALAR_BLACK);

        Cv2.DrawContours(image, contours, -1, PMUtility.SCALAR_WHITE, -1);

        Cv2.ImShow(strImageName, image);
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public void drawAndShowContours(Size imageSize, List<Blob> blobs, string strImageName)
    {

        Mat image = new Mat(imageSize, 0, PMUtility.SCALAR_BLACK);

        
        List<Point[]> contourslist=new List<Point[]>();
        

        int i = 0;
        foreach (Blob blob in blobs)
        {
            if (blob.blnStillBeingTracked == true)
            {
                contourslist.Add(blob.currentContour);
                //contourslist.Add(blob.currentContour);
                //contours = contours.Concat(new[] { new IEnumerable<Point>("msg2") })
            }
            i++;
        }
        int num = contourslist.Count;
        Point[][] contourslistt=new Point[num][];
        for (int k=0;k<num;k++)
        {
            contourslistt[k] = contourslist[k];
           
        }
       // IEnumerable<IEnumerable<Point>> contours = contourslist as IEnumerable<IEnumerable<Point>>;
        Cv2.DrawContours(image, contourslistt, -1, PMUtility.SCALAR_WHITE, -1);

        Cv2.ImShow(strImageName, image);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public bool checkIfBlobsCrossedTheLine(List<Blob> blobs, int intHorizontalLinePosition, int carCount)
    {
        bool blnAtLeastOneBlobCrossedTheLine = false;

        foreach (Blob blob in blobs)
        {

            if (blob.blnStillBeingTracked == true && blob.centerPositions.Count >= 2)
            {
                int prevFrameIndex = (int)blob.centerPositions.Count - 2;
                int currFrameIndex = (int)blob.centerPositions.Count - 1;

                if (blob.centerPositions[prevFrameIndex].Y > intHorizontalLinePosition && blob.centerPositions[currFrameIndex].Y <= intHorizontalLinePosition)
                {
                    carCount++;
                    blnAtLeastOneBlobCrossedTheLine = true;
                }
            }

        }

        return blnAtLeastOneBlobCrossedTheLine;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    public void drawCarCountOnImage(int carCount, Mat imgFrame2Copy)
    {

        //int intFontFace = 0;
        double dblFontScale = (imgFrame2Copy.Rows * imgFrame2Copy.Cols) / 300000.0;
        // int intFontThickness = (int)Mathf.Round(dblFontScale * 1.5f);

        int resultInt = 0;
        Size textSize = Cv2.GetTextSize(carCount.ToString(), 0, dblFontScale, 0, out resultInt);

        Point ptTextBottomLeftPosition;

        ptTextBottomLeftPosition.X = imgFrame2Copy.Cols - 1 - (int)((double)textSize.Width * 1.25);

        ptTextBottomLeftPosition.Y = (int)((double)textSize.Height * 1.25);

        Cv2.PutText(imgFrame2Copy, carCount.ToString(), ptTextBottomLeftPosition, 0, dblFontScale, PMUtility.SCALAR_GREEN, 1);

    }
}

public class PMUtility
{
    public static Scalar SCALAR_BLACK = new Scalar(0.0, 0.0, 0.0);
    public static Scalar SCALAR_WHITE = new Scalar(255.0, 255.0, 255.0);
    public static Scalar SCALAR_YELLOW = new Scalar(0.0, 255.0, 255.0);
    public static Scalar SCALAR_GREEN = new Scalar(0.0, 200.0, 0.0);
    public static Scalar SCALAR_RED = new Scalar(0.0, 0.0, 255.0);
}