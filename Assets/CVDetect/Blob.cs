using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using System.Collections.Generic;

public class Blob {
    public Point[] currentContour;

    public OpenCvSharp.CPlusPlus.Rect currentBoundingRect;

    public Vector2 BaseCenterPoint = Vector2.zero;
    public List<Point> centerPositions = new List<Point>();

    public double dblCurrentDiagonalSize;
    public double dblCurrentAspectRatio;

    public bool blnCurrentMatchFoundOrNewBlob;

    public bool blnStillBeingTracked;

    public int intNumOfConsecutiveFramesWithoutAMatch;

    public Point predictedNextPosition;


    //CV座標   X  --->
    //      Y
    //      |
    //      v
    //長寬限制 = webcam 長寬 , 本次範例 720 * 480
    public Blob(Point[] _contour)
    {
        currentContour = _contour;

        currentBoundingRect = Cv2.BoundingRect(currentContour);
        
        //2019.06.25 新中心點 Vector2
        float c_x = currentBoundingRect.X + (currentBoundingRect.Width / 2.0f);
        float c_y = currentBoundingRect.Y + (currentBoundingRect.Height / 2.0f);
        BaseCenterPoint = new Vector2(c_x, c_y);
        //Debug.Log("currentBoundingRect.x : " + BaseCenterPoint.x + "  currentBoundingRect.y :" + BaseCenterPoint.y);

        // 很怪的中心點算法 ??? 媛安踹共
        //Point currentCenter;
        //currentCenter.X = (currentBoundingRect.X + currentBoundingRect.X + currentBoundingRect.Width) / 2;
        //currentCenter.Y = (currentBoundingRect.Y + currentBoundingRect.Y + currentBoundingRect.Height) / 2;
        //centerPositions.Add(currentCenter);

        //對角線長度
        dblCurrentDiagonalSize = Mathf.Sqrt(Mathf.Pow(currentBoundingRect.Width, 2) + Mathf.Pow(currentBoundingRect.Height, 2));
        //寬為1時，長度為寬的幾倍 = X_Ration 倍率
        dblCurrentAspectRatio = (float)currentBoundingRect.Width / (float)currentBoundingRect.Height;

        //追蹤用
        blnStillBeingTracked = true;
        blnCurrentMatchFoundOrNewBlob = true;

        //拋棄過期點 Counter , 使用於這個點過久沒被更新的話，將這個點拋棄
        intNumOfConsecutiveFramesWithoutAMatch = 0;
    }

   public void predictNextPosition()
    {

        int numPositions = (int)centerPositions.Count;

        if (numPositions == 1)
        {

            predictedNextPosition.X = centerPositions[numPositions-1].X;
            predictedNextPosition.Y = centerPositions[numPositions-1].Y;

        }
        else if (numPositions == 2)
        {

            int deltaX = centerPositions[1].X - centerPositions[0].X;
            int deltaY = centerPositions[1].Y - centerPositions[0].Y;

            predictedNextPosition.X = centerPositions[numPositions-1].X + deltaX;
            predictedNextPosition.Y = centerPositions[numPositions-1].Y + deltaY;

        }
        else if (numPositions == 3)
        {

            int sumOfXChanges = ((centerPositions[2].X - centerPositions[1].X) * 2) +
                ((centerPositions[1].X - centerPositions[0].X) * 1);

            int deltaX = (int)Mathf.Round((float)sumOfXChanges / 3.0f);

            int sumOfYChanges = ((centerPositions[2].Y - centerPositions[1].Y) * 2) +
                ((centerPositions[1].Y - centerPositions[0].Y) * 1);

            int deltaY = (int)Mathf.Round((float)sumOfYChanges / 3.0f);

            predictedNextPosition.X = centerPositions[numPositions-1].X + deltaX;
            predictedNextPosition.Y = centerPositions[numPositions-1].Y + deltaY;

        }
        else if (numPositions == 4)
        {

            int sumOfXChanges = ((centerPositions[3].X - centerPositions[2].X) * 3) +
                ((centerPositions[2].X - centerPositions[1].X) * 2) +
                ((centerPositions[1].X - centerPositions[0].X) * 1);

            int deltaX = (int)Mathf.Round((float)sumOfXChanges / 6.0f);

            int sumOfYChanges = ((centerPositions[3].Y - centerPositions[2].Y) * 3) +
                ((centerPositions[2].Y - centerPositions[1].Y) * 2) +
                ((centerPositions[1].Y - centerPositions[0].Y) * 1);

            int deltaY = (int)Mathf.Round((float)sumOfYChanges / 6.0f);

            predictedNextPosition.X = centerPositions[numPositions-1].X + deltaX;
            predictedNextPosition.Y = centerPositions[numPositions-1].Y + deltaY;

        }
        else if (numPositions >= 5)
        {

            int sumOfXChanges = ((centerPositions[numPositions - 1].X - centerPositions[numPositions - 2].X) * 4) +
                ((centerPositions[numPositions - 2].X - centerPositions[numPositions - 3].X) * 3) +
                ((centerPositions[numPositions - 3].X - centerPositions[numPositions - 4].X) * 2) +
                ((centerPositions[numPositions - 4].X - centerPositions[numPositions - 5].X) * 1);

            int deltaX = (int)Mathf.Round((float)sumOfXChanges / 10.0f);

            int sumOfYChanges = ((centerPositions[numPositions - 1].Y - centerPositions[numPositions - 2].Y) * 4) +
                ((centerPositions[numPositions - 2].Y - centerPositions[numPositions - 3].Y) * 3) +
                ((centerPositions[numPositions - 3].Y - centerPositions[numPositions - 4].Y) * 2) +
                ((centerPositions[numPositions - 4].Y - centerPositions[numPositions - 5].Y) * 1);

            int deltaY = (int)Mathf.Round((float)sumOfYChanges / 10.0f);

            predictedNextPosition.X = centerPositions[numPositions-1].X + deltaX;
            predictedNextPosition.Y = centerPositions[numPositions-1].Y + deltaY;

        }
        else
        {
            // should never get here
        }

    }

}
