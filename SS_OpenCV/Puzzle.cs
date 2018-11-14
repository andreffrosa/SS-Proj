using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace SS_OpenCV
{
    internal class Puzzle
    {
        public static uint[,] getLabels(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();
                byte* originalPtr = dataPtrOriginal;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                uint[,] labels = new uint[img.Width, img.Height];
                bool changed = true;
                uint curr_label = 1;

                int back_b = dataPtrOriginal[0], back_g = dataPtrOriginal[1], back_r = dataPtrOriginal[2];

                // First run
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (back_b != dataPtrOriginal[0] || back_g != dataPtrOriginal[1] || back_r != dataPtrOriginal[2])
                        {
                            labels[x, y] = curr_label++;
                        }
                        else
                        {
                            labels[x, y] = UInt32.MaxValue;
                        }
                        dataPtrOriginal += nChan;
                    }
                    dataPtrOriginal += padding;
                }

                // Cicle
                while (changed)
                {
                    changed = false;

                    // Up->Down Left->Right
                    dataPtrOriginal = originalPtr;
                    dataPtrOriginal += nChan + step;
                    for (int y = 1; y < height - 1; y++)
                    {

                        for (int x = 1; x < width - 1; x++)
                        {
                            uint tmp = labels[x, y];
                            if (labels[x, y] != UInt32.MaxValue)
                            {
                                if (labels[x, y] > labels[x - 1, y - 1])
                                {
                                    labels[x, y] = labels[x - 1, y - 1];
                                }
                                if (labels[x, y] > labels[x, y - 1])
                                {
                                    labels[x, y] = labels[x, y - 1];
                                }
                                if (labels[x, y] > labels[x - 1, y])
                                {
                                    labels[x, y] = labels[x - 1, y];
                                }
                                if (labels[x, y] > labels[x + 1, y - 1])
                                {
                                    labels[x, y] = labels[x + 1, y - 1];
                                }
                                if (labels[x, y] > labels[x - 1, y + 1])
                                {
                                    labels[x, y] = labels[x - 1, y + 1];
                                }
                                if (labels[x, y] > labels[x + 1, y])
                                {
                                    labels[x, y] = labels[x + 1, y];
                                }
                                if (labels[x, y] > labels[x, y + 1])
                                {
                                    labels[x, y] = labels[x, y + 1];
                                }
                                if (labels[x, y] > labels[x + 1, y + 1])
                                {
                                    labels[x, y] = labels[x + 1, y + 1];
                                }
                                if (tmp != labels[x, y])
                                    changed = true;
                            }
                            dataPtrOriginal += nChan;
                        }
                        dataPtrOriginal += nChan * 2 + padding;
                    }

                    // Down->Up Right->Left
                    dataPtrOriginal = originalPtr + (nChan * width) + (step * height);
                    dataPtrOriginal -= nChan + step;
                    for (int y = height - 2; y > 0; y--)
                    {
                        for (int x = width - 2; x > 0; x--)
                        {

                            uint tmp = labels[x, y];
                            if (labels[x, y] != UInt32.MaxValue)
                            {
                                if (labels[x, y] > labels[x - 1, y - 1])
                                {
                                    labels[x, y] = labels[x - 1, y - 1];
                                }
                                if (labels[x, y] > labels[x, y - 1])
                                {
                                    labels[x, y] = labels[x, y - 1];
                                }
                                if (labels[x, y] > labels[x - 1, y])
                                {
                                    labels[x, y] = labels[x - 1, y];
                                }
                                if (labels[x, y] > labels[x + 1, y - 1])
                                {
                                    labels[x, y] = labels[x + 1, y - 1];
                                }
                                if (labels[x, y] > labels[x - 1, y + 1])
                                {
                                    labels[x, y] = labels[x - 1, y + 1];
                                }
                                if (labels[x, y] > labels[x + 1, y])
                                {
                                    labels[x, y] = labels[x + 1, y];
                                }
                                if (labels[x, y] > labels[x, y + 1])
                                {
                                    labels[x, y] = labels[x, y + 1];
                                }
                                if (labels[x, y] > labels[x + 1, y + 1])
                                {
                                    labels[x, y] = labels[x + 1, y + 1];
                                }
                                if (tmp != labels[x, y])
                                    changed = true;

                            }
                            dataPtrOriginal -= nChan;
                        }
                        dataPtrOriginal -= nChan * 2 + padding;
                    }
                }

                /*
                dataPtrOriginal = originalPtr;
                List<uint> pieces = new List<uint>();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (labels[x, y] != UInt32.MaxValue)
                        {
                            dataPtrOriginal[0] = (byte) (labels[x, y]*10 % 256);
                            dataPtrOriginal[1] = 0;
                            dataPtrOriginal[2] = (byte)(labels[x, y] * 10 % 256);
                            if (!pieces.Contains(labels[x, y]))
                            {
                                pieces.Add(labels[x, y]);
                               // Console.WriteLine(labels[x, y]);
 
                            }
                        }
                        dataPtrOriginal += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtrOriginal += padding;
                }
                Console.WriteLine(pieces.Count);
                */

                return labels;
            }

        }

        public static void getPiecesPosition(Image<Bgr, byte> originalImage, uint[,] labels, out List<int[]> Pieces_positions, out List<int> Pieces_angle, out List<Image<Bgr, byte>> images_pieces)
        {
            int width = labels.GetLength(0);
            int height = labels.GetLength(1);

            Pieces_positions = new List<int[]>();
            Pieces_angle = new List<int>();
            List<double> angles_rads = new List<double>();
            int[] piece_vector;
            images_pieces = new List<Image<Bgr, byte>>();


            // var map = new Dictionary<uint, int[]>(10);
            uint[] values = new uint[10];
            int curr_pos = 0;

            int[] x_top_left = new int[10];
            int[] y_top_left = new int[10];
            int[] helper_point_x = new int[10];
            int[] helper_point_y = new int[10];

            for (int i = 0; i < 10; i++)
            {
                x_top_left[i] = Int32.MaxValue;
                y_top_left[i] = Int32.MaxValue;
                helper_point_x[i] = -1;
                helper_point_y[i] = -1;
            }

            int[] x_bottom_right = new int[10];
            int[] y_bottom_right = new int[10];


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (labels[x, y] != UInt32.MaxValue)
                    {
                        int index = Array.IndexOf(values, labels[x, y]);
                        if (index == -1)
                        {
                            values[curr_pos] = labels[x, y];
                            index = curr_pos++;
                        }
                        if (helper_point_x[index] == -1)
                        {
                            helper_point_x[index] = x;
                            helper_point_y[index] = y;
                        }

                        if (x_top_left[index] > x)
                        {
                            x_top_left[index] = x;
                            y_top_left[index] = y;
                        }
                        if (x_bottom_right[index] <= x)
                        {
                            x_bottom_right[index] = x;
                            y_bottom_right[index] = y;
                        }
                    }
                }
            }

            for (int i = 0; i < curr_pos; i++)
            {
                piece_vector = new int[4];
                piece_vector[0] = x_top_left[i];   // x- Top-Left 
                piece_vector[1] = y_top_left[i];  // y- Top-Left
                piece_vector[2] = x_bottom_right[i]; // x- Bottom-Right
                piece_vector[3] = y_bottom_right[i]; // y- Bottom-Right
                Pieces_positions.Add(piece_vector);

                double rads;
                if (helper_point_x[i] == x_top_left[i] && helper_point_y[i] == y_top_left[i])
                {
                    // No rotation needed
                    Pieces_angle.Add(0);
                    angles_rads.Add(0.0);
                    rads = 0.0;
                }
                else
                {
                    //Calculate Angle
                    double oppositive = y_top_left[i] - helper_point_y[i];
                    double adjancent = helper_point_x[i] - x_top_left[i];
                    rads = Math.Tanh(oppositive / adjancent);
                    angles_rads.Add(rads);

                    int angle = RadsToDegrees(rads);
                    Pieces_angle.Add(angle);
                }

                images_pieces.Add(GetImagesPieces(originalImage, piece_vector, rads, helper_point_x[i], helper_point_y[i]));
            }
        }

        const double RadToDegreeFactor = 180.0 / Math.PI;
        public static int RadsToDegrees(double rads)
        {
            return (int)Math.Round(rads * RadToDegreeFactor);
        }


        public static unsafe Image<Bgr, byte> GetImagesPieces(Image<Bgr, byte> originalImage, int[] piece, double angle, int helper_x, int helper_y)
        {
            unsafe
            {

                MIplImage original = originalImage.MIplImage;
                byte* dataPtrOriginal = (byte*)original.imageData.ToPointer();
                int nChan = original.nChannels;
                int padding = original.widthStep - original.nChannels * original.width;
                int step = original.widthStep;
                int width = originalImage.Width;
                int height = originalImage.Height;

                int cols = 0;
                int rows = 0;
                if (angle != 0.0)
                {
                    cols = (int) Math.Round(Math.Sqrt(Math.Pow(helper_x - piece[0], 2) + Math.Pow(piece[1] - helper_y, 2))) + 1;
                    rows = (int) Math.Round(Math.Sqrt(Math.Pow(piece[2] - helper_x, 2) + Math.Pow(helper_y - piece[3], 2))) + 1;
                }
                else
                {
                    cols = piece[2] - piece[0] + 1;
                    rows = piece[3] - piece[1] + 1;
                }
                

                Image<Bgr, byte> tmp = new Image<Bgr, byte>(cols, rows);
                MIplImage m = tmp.MIplImage;
                byte* newImagePointer = (byte*)m.imageData.ToPointer();
                int nChan_new = m.nChannels;
                int padding_new = m.widthStep - m.nChannels * m.width;
                int step_new = m.widthStep;

                int x_p = piece[0];
                int y_p = piece[1];


                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if(angle != 0.0)
                        {
                            int dx_diff = (int)Math.Round(x * Math.Cos(-angle) - y * Math.Sin(-angle) + x_p);
                            int dy_diff = (int)Math.Round(x * Math.Sin(-angle) + y * Math.Cos(-angle) + y_p);
                            // int dx_diff = (int)Math.Round(((x+x_p) - width / 2.0) * Math.Cos(angle) - (height / 2.0 - (y+y_p)) * Math.Sin(angle) + (width / 2.0));
                            // int dy_diff = (int)Math.Round(height / 2.0 - ((x_p+x) - width / 2.0) * Math.Sin(angle) - (height / 2.0 - (y_p+y)) * Math.Cos(angle));
                            int x2 = angle == 0.0 ? x : dx_diff;
                            int y2 = angle == 0.0 ? y : dy_diff;
                            newImagePointer[0] = (dataPtrOriginal + ((x2 ) * nChan) + ((y2 ) * step))[0];
                            newImagePointer[1] = (dataPtrOriginal + ((x2 ) * nChan) + ((y2 ) * step))[1];
                            newImagePointer[2] = (dataPtrOriginal + ((x2 ) * nChan) + ((y2 ) * step))[2];
                        }
                        else
                        {
                            int x2 = x;
                            int y2 = y;
                            newImagePointer[0] = (dataPtrOriginal + ((x2 + x_p) * nChan) + ((y2 + y_p) * step))[0];
                            newImagePointer[1] = (dataPtrOriginal + ((x2 + x_p) * nChan) + ((y2 + y_p) * step))[1];
                            newImagePointer[2] = (dataPtrOriginal + ((x2 + x_p) * nChan) + ((y2 + y_p) * step))[2];
                        }
                        

                        newImagePointer += nChan_new;
                    }
                    newImagePointer += padding_new;
                }

                return tmp;

            }
        }

    }
}