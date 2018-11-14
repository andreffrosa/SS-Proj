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

        public static List<int[]> getPiecesPosition(uint[,] labels, out List<int[]> Pieces_positions, out List<int> Pieces_angle)
        {
            int width = labels.GetLength(0);
            int height = labels.GetLength(1);

            Pieces_positions = new List<int[]>();
            Pieces_angle = new List<int>();
            int[] piece_vector;


            // var map = new Dictionary<uint, int[]>(10);
            uint[] values = new uint[10];
            int curr_pos = 0;

            int[] x_top_left = new int[10];
            int[] y_top_left = new int[10];

            for(int i = 0; i< 10; i++)
            {
                x_top_left[i] = Int32.MaxValue;
                y_top_left[i] = Int32.MaxValue;
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
                Pieces_angle.Add(0);
            }

            return Pieces_positions;
        }



    }
}