using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS_OpenCV
{
    internal class Puzzle
    {
        public static int[,] getLabels(Image<Bgr, byte> img)
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

                int[,] labels = new int[img.Width, img.Height];
                bool changed = true;
                int curr_label = 1;

                int back_b = dataPtrOriginal[0], back_g = dataPtrOriginal[1], back_r = dataPtrOriginal[2];

                changed = false;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (back_b != dataPtrOriginal[0] || back_g != dataPtrOriginal[1] || back_r != dataPtrOriginal[2])
                        {
                            //new piece
                            labels[x, y] = curr_label++;
                            changed = true;
                        }
                        else
                        {
                           /* dataPtrOriginal[0] = 0;
                            dataPtrOriginal[1] = 0;
                            dataPtrOriginal[2] = 0;
                            */
                            labels[x, y] = 0;
                        }

                        dataPtrOriginal += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtrOriginal += padding;
                }

                dataPtrOriginal = originalPtr;
                while (changed)
                {
                    changed = false;
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            int tmp = labels[x, y];
                            if (labels[x, y] < labels[x - 1, y - 1])
                            {
                                labels[x, y] = labels[x - 1, y - 1];
                            }
                            if (labels[x, y] < labels[x, y - 1])
                            {
                                labels[x, y] = labels[x, y - 1];
                            }
                            if (labels[x, y] < labels[x - 1, y])
                            {
                                labels[x, y] = labels[x - 1, y];
                            }
                            if (labels[x, y] < labels[x + 1, y - 1])
                            {
                                labels[x, y] = labels[x + 1, y - 1];
                            }
                            if (labels[x, y] < labels[x - 1, y + 1])
                            {
                                labels[x, y] = labels[x - 1, y + 1];
                            }
                            if (labels[x, y] < labels[x + 1, y])
                            {
                                labels[x, y] = labels[x + 1, y];
                            }
                            if (labels[x, y] < labels[x, y + 1])
                            {
                                labels[x, y] = labels[x, y + 1];
                            }
                            if (labels[x, y] < labels[x + 1, y + 1])
                            {
                                labels[x, y] = labels[x + 1, y + 1];
                            }
                            if (tmp != labels[x, y])
                                changed = true;
                            //labels[x, y] = (labels[x, y] < labels[x - 1, y - 1]) ? labels[x - 1, y - 1] : labels[x, y];

                            dataPtrOriginal += nChan;
                        }
                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal += padding;
                    }

                    for (int y = height - 2; y > 0; y--)
                    {
                        for (int x = width - 2; x > 0; x--)
                        {
                            int tmp = labels[x, y];
                            if (labels[x, y] < labels[x - 1, y - 1])
                            {
                                labels[x, y] = labels[x - 1, y - 1];
                            }
                            if (labels[x, y] < labels[x, y - 1])
                            {
                                labels[x, y] = labels[x, y - 1];
                            }
                            if (labels[x, y] < labels[x - 1, y])
                            {
                                labels[x, y] = labels[x - 1, y];
                            }
                            if (labels[x, y] < labels[x + 1, y - 1])
                            {
                                labels[x, y] = labels[x + 1, y - 1];
                            }
                            if (labels[x, y] < labels[x - 1, y + 1])
                            {
                                labels[x, y] = labels[x - 1, y + 1];
                            }
                            if (labels[x, y] < labels[x + 1, y])
                            {
                                labels[x, y] = labels[x + 1, y];
                            }
                            if (labels[x, y] < labels[x, y + 1])
                            {
                                labels[x, y] = labels[x, y + 1];
                            }
                            if (labels[x, y] < labels[x + 1, y + 1])
                            {
                                labels[x, y] = labels[x + 1, y + 1];
                            }
                            if (tmp != labels[x, y])
                                changed = true;
                            dataPtrOriginal -= nChan;
                        }
                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal -= padding;
                    }


                }

                return labels;

            }

        }
    }
}