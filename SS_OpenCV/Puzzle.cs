using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

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

                changed = true;
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

                
                while (changed)
                {
                    changed = false;
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
                            
                            //labels[x, y] = (labels[x, y] < labels[x - 1, y - 1]) ? labels[x - 1, y - 1] : labels[x, y];

                            dataPtrOriginal += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal += nChan * 2 + padding;
                    }

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
                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal -= nChan * 2 + padding;
                    }
                    
                }

                Console.WriteLine(numb_pieces);
                
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

        public List<int[]> getPiecesPosition(uint[,] labels)
        {
            int width = labels.GetLength(0);
            int height = labels.GetLength(1);

            List<int[]> Pieces_positions = new List<int[]>();
            int[] piece_vector = new int[4];


            var map = new Dictionary<uint, int[]>(30);


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if(labels[x,y] != UInt32.MaxValue)
                    {
                        if (map.ContainsKey(labels[x, y]))
                        {

                        }
                    }

                }
            }

            

            piece_vector[0] = 65;   // x- Top-Left 
            piece_vector[1] = 385;  // y- Top-Left
            piece_vector[2] = 1089; // x- Bottom-Right
            piece_vector[3] = 1411; // y- Bottom-Right

            Pieces_positions.Add(piece_vector);


            return Pieces_positions;

    }
    }
}