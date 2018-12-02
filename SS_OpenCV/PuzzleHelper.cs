using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace SS_OpenCV
{
    public static class PuzzleHelper
    {
        public static Image<Bgr, byte> CombineImageTopBottom(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
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
                piece2 = piece2.Resize(piece1.Width, piece2.Height, INTER.CV_INTER_LINEAR);

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
                piece1 = piece1.Resize(piece2.Width, piece1.Height, INTER.CV_INTER_LINEAR);

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

        public static Image<Bgr, byte> CombineImageRightLeft(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
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
                piece2 = piece2.Resize(piece2.Width, piece1.Height, INTER.CV_INTER_LINEAR);

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
                piece1 = piece1.Resize(piece1.Width, piece2.Height, INTER.CV_INTER_LINEAR);

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

        public static Image<Bgr, byte> CombineImageBottomTop(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
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
                piece2 = piece2.Resize(piece1.Width, piece2.Height, INTER.CV_INTER_LINEAR);

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
                piece1 = piece1.Resize(piece2.Width, piece1.Height, INTER.CV_INTER_LINEAR);

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

        public static Image<Bgr, byte> CombineImageLeftRight(Image<Bgr, byte> piece1, Image<Bgr, byte> piece2)
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
                piece2 = piece2.Resize(piece2.Width, piece1.Height, INTER.CV_INTER_LINEAR);

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
                piece1 = piece1.Resize(piece1.Width, piece2.Height, INTER.CV_INTER_LINEAR);

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