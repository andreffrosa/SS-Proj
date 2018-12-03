using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;

namespace SS_OpenCV
{
    public static class PuzzleHelper
    {

        private static unsafe double CompareSide(int lenght1, int lenght2, byte* img1, byte* img2, int inc1, int inc2, int in1, int in2)
        {
            double points = 0;
            double scale = (lenght1 > lenght2) ? lenght1 / (double) lenght2 : lenght2 / (double) lenght1;
            double maxLenght = lenght1 > lenght2 ? lenght1 : lenght2;

            byte* copy1 = img1;
            byte* copy2 = img2;

            const int thresholdDiffMin = 5;
            const int thresholdDiffMax = 250;

            var tmpR = 0;
            var tmpG = 0;
            var tmpB = 0;

            for (var y = 0; y < maxLenght; y++)
            {
                for (var i = 0; i < 1; i++)
                {
                   

                    if(Math.Abs((img1 + in1 * i)[0] - (img2 + in2 * i)[0]) <= thresholdDiffMin)
                    {
                        tmpB++;
                    }
                    if (Math.Abs((img1 + in1 * i)[1] - (img2 + in2 * i)[1]) <= thresholdDiffMin)
                    {
                        tmpG++;
                    }
                    if (Math.Abs((img1 + in1 * i)[2] - (img2 + in2 * i)[2]) <= thresholdDiffMin)
                    {
                        tmpR++;
                    }

                    if (Math.Abs((img1 + in1 * i)[0] - (img2 + in2 * i)[0]) >= thresholdDiffMax)
                    {
                        tmpB--;
                    }
                    if (Math.Abs((img1 + in1 * i)[1] - (img2 + in2 * i)[1]) >= thresholdDiffMax)
                    {
                        tmpG--;
                    }
                    if (Math.Abs((img1 + in1 * i)[2] - (img2 + in2 * i)[2]) >= thresholdDiffMax)
                    {
                        tmpR--;
                    }

                   
                    /*
                    diff += ((
                        Math.Abs((img1 + in1 * i)[0] - (img2 + in2 * i)[0]) 
                        + Math.Abs((img1 + in1 * i)[1] - (img2 + in2 * i)[1]) 
                        + Math.Abs((img1 + in1 * i)[2] - (img2 + in2 * i)[2])) / 3.0 / 255.0 / ((i+1)));*/
                }

                if(lenght1 > lenght2)
                {
                    img1 += inc1;
                    img2 = copy2 + ((int)Math.Floor(y / scale) * inc2);
                }
                else if(lenght1 < lenght2)
                {
                    img1 = copy1 + ((int)Math.Floor(y / scale) * inc1);
                    img2 += inc2;
                }
                else
                {
                    img1 += inc1;
                    img2 += inc2;
                }
                
                //Debug.Assert(y < lenght1, "Failed 1");
                //Debug.Assert(y < lenght2, "Failed 2");
            }

            // 1 -> Best
            //Console.WriteLine(points / maxLenght);

            if(tmpB < 0 || tmpG < 0 || tmpR < 0)
            {
                return 0;
            }

            var res1 = ((double)tmpB + maxLenght) / (double)maxLenght * 2;
            var res2 = ((double)tmpG + maxLenght) / (double)maxLenght * 2;
            var res3 = ((double)tmpR +maxLenght) / (double)maxLenght * 2;

            // Console.WriteLine("WWWWWWWWWWWWWWWWWWWWWWW: " + (res1 + res2 + res3) / 3.0);

            return (res1 + res2 + res3) / 3.0;
        }
        
        public static SideValues CompareSides(Image<Bgr, byte> img1, Image<Bgr, byte> img2)
        {
            unsafe
            {
                // top = 0; right = 1; bottom = 2; left = 3;
                var bestSide = -1;
                var topPoints = 0.0;

                // Image 1 data
                var image1 = img1.MIplImage;
                byte* image1Pointer = (byte*) image1.imageData.ToPointer();
                byte* pointerCopy1 = image1Pointer;
                var nChan1 = image1.nChannels;
                var padding1 = image1.widthStep - image1.nChannels * image1.width;
                var step1 = image1.widthStep;
                var width1 = img1.Width;
                var height1 = img1.Height;

                // Image 2 data
                var image2 = img2.MIplImage;
                byte* image2Pointer = (byte*)image2.imageData.ToPointer();
                byte* pointerCopy2 = image2Pointer;
                var nChan2 = image2.nChannels;
                var padding2 = image2.widthStep - image2.nChannels * image2.width;
                var step2 = image2.widthStep;
                var width2 = img2.Width;
                var height2 = img2.Height;

                double points = 0;

                // [START] 0 TOP->BOTTOM
                image1Pointer = pointerCopy1;
                image2Pointer = pointerCopy2 + step2 * (height2 - 1);
                points = CompareSide(width1, width2, image1Pointer, image2Pointer, nChan1, nChan2, step1, -step2);
                Console.WriteLine("New Diff: " + points + "\tOld Diff: " + topPoints + "\tSide: " + 0);

                if (points > topPoints)
                {
                    topPoints = points;
                    bestSide = 0;
                }
                // [END] 0 TOP->BOTTOM

                // [START] 1 RIGHT->LEFT
                image1Pointer = pointerCopy1 + nChan1 * (width1 - 1);
                image2Pointer = pointerCopy2;
                points = CompareSide(height1, height2, image1Pointer, image2Pointer, step1, step2, -nChan1, nChan2);
                Console.WriteLine("New Diff: " + points + "\tOld Diff: " + topPoints + "\tSide: " + 1);

                if (points > topPoints)
                {
                    topPoints = points;
                    bestSide = 1;
                }
                // [END] 1 RIGHT->LEFT

                // [START] 2 BOTTOM->TOP
                image1Pointer = pointerCopy1 + step1 * (height1 - 1);
                image2Pointer = pointerCopy2;
                points = CompareSide(width1, width2, image1Pointer, image2Pointer, nChan1, nChan2, -step1, step2);
                Console.WriteLine("New Diff: " + points + "\tOld Diff: " + topPoints + "\tSide: " + 2);

                if (points > topPoints)
                {
                    topPoints = points;
                    bestSide = 2;
                }
                // [END] 2 BOTTOM->TOP

                // [START] 3 LEFT->RIGHT
                image1Pointer = pointerCopy1;
                image2Pointer = pointerCopy2 + nChan2 * (width2 - 1);
                points = CompareSide(height1, height2, image1Pointer, image2Pointer, step1, step2, nChan1, -nChan2);
                Console.WriteLine("New Diff: " + points + "\tOld Diff: " + topPoints + "\tSide: " + 3);

                if (points > topPoints)
                {
                    topPoints = points;
                    bestSide = 3;
                }
                // [END] 3 LEFT->RIGHT

                Console.WriteLine("Best Diff: " + points + "\tBest Side: " + bestSide);

                return new SideValues(bestSide, topPoints);
            }
        }
        
        public static Image<Bgr, byte> CombinePiecesTopBottom(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
        {
            if (piece1.Width == piece2.Width)
            {
                var newImage = new Image<Bgr, byte>(piece1.Width, piece1.Height + piece2.Height)
                {
                    ROI = new Rectangle(0, 0, piece2.Width, piece2.Height)
                };
                piece2.CopyTo(newImage);
                newImage.ROI = new Rectangle(0, piece2.Height, piece1.Width, piece1.Height);
                piece1.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;
                return newImage;
            }

            if (piece1.Width > piece2.Width)
            {
                // Scale img2
                double ratio = piece1.Width / (double) piece2.Width;

                piece2 = piece2.Resize(piece1.Width, (int)Math.Round(piece2.Height * ratio), INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width, piece1.Height + piece2.Height)
                {
                    ROI = new Rectangle(0, 0, piece2.Width, piece2.Height)
                };
                piece2.CopyTo(newImage);
                newImage.ROI = new Rectangle(0, piece2.Height, piece1.Width, piece1.Height);
                piece1.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;
                return newImage;
            }
            else
            {
                // Scale img1
                double ratio =  piece2.Width / (double)piece1.Width;

                piece1 = piece1.Resize(piece2.Width, (int)Math.Round(piece1.Height * ratio), INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width, piece1.Height + piece2.Height)
                {
                    ROI = new Rectangle(0, 0, piece2.Width, piece2.Height)
                };
                piece2.CopyTo(newImage);
                newImage.ROI = new Rectangle(0, piece2.Height, piece1.Width, piece1.Height);
                piece1.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;
                return newImage;
            }
        }

        public static Image<Bgr, byte> CombinePiecesRightLeft(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
        {
            if (piece1.Height == piece2.Height)
            {
                var newImage = new Image<Bgr, byte>(piece1.Width + piece2.Width, piece1.Height)
                {
                    ROI = new Rectangle(0, 0, piece1.Width, piece1.Height)
                };
                piece1.CopyTo(newImage);
                newImage.ROI = new Rectangle(piece1.Width, 0, piece2.Width, piece2.Height);
                piece2.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;

                return newImage;
            }

            if (piece1.Height > piece2.Height)
            {
                // Scale img2
                double ratio = piece1.Height / (double)piece2.Height;

                piece2 = piece2.Resize((int)Math.Round(piece2.Width * ratio), piece1.Height, INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width + piece2.Width, piece1.Height)
                {
                    ROI = new Rectangle(0, 0, piece1.Width, piece1.Height)
                };
                piece1.CopyTo(newImage);
                newImage.ROI = new Rectangle(piece1.Width, 0, piece2.Width, piece2.Height);
                piece2.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;

                return newImage;
            }
            else
            {
                // Scale img1
                double ratio = piece2.Height / (double)piece1.Height;
                
                piece1 = piece1.Resize((int)Math.Round(piece1.Width * ratio), piece2.Height, INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width + piece2.Width, piece1.Height)
                {
                    ROI = new Rectangle(0, 0, piece1.Width, piece1.Height)
                };
                piece1.CopyTo(newImage);
                newImage.ROI = new Rectangle(piece1.Width, 0, piece2.Width, piece2.Height);
                piece2.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;

                return newImage;
            }
        }

        public static Image<Bgr, byte> CombinePiecesBottomTop(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
        {
            if (piece1.Width == piece2.Width)
            {
                var newImage = new Image<Bgr, byte>(piece1.Width, piece1.Height + piece2.Height)
                {
                    ROI = new Rectangle(0, 0, piece1.Width, piece1.Height)
                };
                piece1.CopyTo(newImage);
                newImage.ROI = new Rectangle(0, piece1.Height, piece2.Width, piece2.Height);
                piece2.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;

                return newImage;
            }

            if (piece1.Width > piece2.Width)
            {
                // Scale img2
                double ratio = piece1.Width / (double)piece2.Width;

                piece2 = piece2.Resize(piece1.Width, (int)Math.Round(piece2.Height * ratio), INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width, piece1.Height + piece2.Height)
                {
                    ROI = new Rectangle(0, 0, piece1.Width, piece1.Height)
                };
                piece1.CopyTo(newImage);
                newImage.ROI = new Rectangle(0, piece1.Height, piece2.Width, piece2.Height);
                piece2.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;
                return newImage;
            }
            else
            {
                // Scale img1
                double ratio = piece2.Width / (double)piece1.Width;

                piece1 = piece1.Resize(piece2.Width, (int)Math.Round(piece1.Height * ratio), INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width, piece1.Height + piece2.Height)
                {
                    ROI = new Rectangle(0, 0, piece1.Width, piece1.Height)
                };
                piece1.CopyTo(newImage);
                newImage.ROI = new Rectangle(0, piece1.Height, piece2.Width, piece2.Height);
                piece2.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;
                return newImage;
            }
        }

        public static Image<Bgr, byte> CombinePiecesLeftRight(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
        {
            if (piece1.Height == piece2.Height)
            {
                var newImage = new Image<Bgr, byte>(piece1.Width + piece2.Width, piece1.Height)
                {
                    ROI = new Rectangle(0, 0, piece2.Width, piece2.Height)
                };
                piece2.CopyTo(newImage);
                newImage.ROI = new Rectangle(piece2.Width, 0, piece1.Width, piece1.Height);
                piece1.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;

                return newImage;
            }

            if (piece1.Height > piece2.Height)
            {
                // Scale img2
                double ratio = piece1.Height / (double)piece2.Height;

                piece2 = piece2.Resize((int)Math.Round(piece2.Width * ratio), piece1.Height, INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width + piece2.Width, piece1.Height)
                {
                    ROI = new Rectangle(0, 0, piece2.Width, piece2.Height)
                };
                piece2.CopyTo(newImage);
                newImage.ROI = new Rectangle(piece2.Width, 0, piece1.Width, piece1.Height);
                piece1.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;

                return newImage;
            }
            else
            {
                // Scale img1
                double ratio = piece2.Height / (double)piece1.Height;

                piece1 = piece1.Resize((int)Math.Round(piece1.Width * ratio), piece2.Height, INTER.CV_INTER_LINEAR);

                var newImage = new Image<Bgr, byte>(piece1.Width + piece2.Width, piece1.Height)
                {
                    ROI = new Rectangle(0, 0, piece2.Width, piece2.Height)
                };
                piece2.CopyTo(newImage);
                newImage.ROI = new Rectangle(piece2.Width, 0, piece1.Width, piece1.Height);
                piece1.CopyTo(newImage);
                newImage.ROI = Rectangle.Empty;

                return newImage;
            }
        }
    }
}