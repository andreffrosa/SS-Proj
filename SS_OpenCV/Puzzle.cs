using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace SS_OpenCV
{
    internal class Puzzle
    {
        const int MaxNumberPieces = 10;
        const double RadToDegreeFactor = 180.0 / Math.PI;
        private Image<Bgr, byte> img;
        List<Image<Bgr, byte>> images_pieces;
        uint[,] labels;
        uint[] labels_index;
        int curr_label_index;

        public Puzzle(Image<Bgr, byte> img)
        {
            this.img = img;
            labels = new uint[img.Width, img.Height];
            labels_index = new uint[MaxNumberPieces];
            curr_label_index = 0;
            images_pieces = new List<Image<Bgr, byte>>();

            this.getLabels();
        }

        private uint[,] getLabels()
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

                // Cycle
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
                return labels;
            }

        }

        private int getLabelIndex(uint label)
        {
            int index = Array.IndexOf(this.labels_index, label);
            if (index == -1)
            {
                labels_index[this.curr_label_index] = label;
                index = this.curr_label_index++;
            }
            return index;
        }

        private int RadsToDegrees(double rads)
        {
            return (int)Math.Round(rads * RadToDegreeFactor);
        }

        private unsafe Image<Bgr, byte> GetImagesPieces(int[] piece, double angle, int helper_x, int helper_y)
        {
            unsafe
            {
                MIplImage original = this.img.MIplImage;
                byte* dataPtrOriginal = (byte*)original.imageData.ToPointer();
                int nChan = original.nChannels;
                int padding = original.widthStep - original.nChannels * original.width;
                int step = original.widthStep;
                int width = this.img.Width;
                int height = this.img.Height;

                int cols = 0;
                int rows = 0;
                if (angle != 0.0)
                {
                    cols = (int)Math.Round(Math.Sqrt(Math.Pow(helper_x - piece[0], 2) + Math.Pow(piece[1] - helper_y, 2))) + 1;
                    rows = (int)Math.Round(Math.Sqrt(Math.Pow(piece[2] - helper_x, 2) + Math.Pow(helper_y - piece[3], 2))) + 1;
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
                int x2;
                int y2;

                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (angle != 0.0)
                        {
                            x2 = (int)Math.Round(x * Math.Cos(-angle) - y * Math.Sin(-angle) + x_p);
                            y2 = (int)Math.Round(x * Math.Sin(-angle) + y * Math.Cos(-angle) + y_p);
                        }
                        else
                        {
                            x2 = x + x_p;
                            y2 = y + y_p;
                        }

                        newImagePointer[0] = (dataPtrOriginal + (x2 * nChan) + (y2 * step))[0];
                        newImagePointer[1] = (dataPtrOriginal + (x2 * nChan) + (y2 * step))[1];
                        newImagePointer[2] = (dataPtrOriginal + (x2 * nChan) + (y2 * step))[2];

                        newImagePointer += nChan_new;
                    }
                    newImagePointer += padding_new;
                }

                if (angle != 0.0)
                {
                    //TODO fix borders
                }

                    return tmp;
            }
        }

        public void getPiecesPosition(out List<int[]> Pieces_positions, out List<int> Pieces_angle)
        {
            int width = labels.GetLength(0);
            int height = labels.GetLength(1);

            Pieces_positions = new List<int[]>();
            Pieces_angle = new List<int>();

            int[] piece_vector;
            int[] x_top_left = new int[MaxNumberPieces];
            int[] y_top_left = new int[MaxNumberPieces];
            int[] helper_point_x = new int[MaxNumberPieces];
            int[] helper_point_y = new int[MaxNumberPieces];
            int[] x_bottom_right = new int[MaxNumberPieces];
            int[] y_bottom_right = new int[MaxNumberPieces];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (labels[x, y] != UInt32.MaxValue)
                    {
                        //TODO if a value is already set ignore / overwrite depending
                        if (labels[x, y - 1] == UInt32.MaxValue && labels[x - 1, y] == UInt32.MaxValue && labels[x - 1, y + 2] == UInt32.MaxValue)
                        {
                            // Found a top left corner
                            int index = getLabelIndex(labels[x, y]);

                            x_top_left[index] = x;
                            y_top_left[index] = y;

                            Console.WriteLine("Label: " + labels[x, y]);
                            Console.WriteLine("Top Left X: " + x);
                            Console.WriteLine("Top Left Y: " + y);

                        }
                        else if (labels[x, y + 1] == UInt32.MaxValue && labels[x + 1, y] == UInt32.MaxValue && labels[x + 1, y - 2] == UInt32.MaxValue)
                        {
                            // Found a bottom right corner
                            int index = getLabelIndex(labels[x, y]);
                            x_bottom_right[index] = x;
                            y_bottom_right[index] = y;

                            Console.WriteLine("Label: " + labels[x, y]);
                            Console.WriteLine("Bottom Right X: " + x);
                            Console.WriteLine("Bottom Right Y: " + y);
                        }
                        else if (labels[x, y - 1] == UInt32.MaxValue && labels[x + 1, y] == UInt32.MaxValue && labels[x - 2, y - 1] == UInt32.MaxValue)
                        {
                            // Found a top right corner (helper)
                            int index = getLabelIndex(labels[x, y]);

                            helper_point_x[index] = x;
                            helper_point_y[index] = y;

                            Console.WriteLine("Label: " + labels[x, y]);
                            Console.WriteLine("Top Right X: " + x);
                            Console.WriteLine("Top Right Y: " + y);
                        }
                    }
                }
            }

            for (int i = 0; i < this.curr_label_index; i++)
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
                    rads = 0.0;
                }
                else
                {
                    //Calculate Angle
                    double oppositive = y_top_left[i] - helper_point_y[i];
                    double adjancent = helper_point_x[i] - x_top_left[i];
                    rads = Math.Tanh(oppositive / adjancent);
                    Console.WriteLine("Angle: " + this.RadsToDegrees(rads));
                    Pieces_angle.Add(this.RadsToDegrees(rads));
                }

                images_pieces.Add(this.GetImagesPieces(piece_vector, rads, helper_point_x[i], helper_point_y[i]));
            }
        }

        private SideValues compareSides(Image<Bgr, byte> img1, Image<Bgr, byte> img2)
        {
            unsafe
            {
                // top = 0; right = 1; bottom = 2; left = 3;
                int best_side = -1;
                double best_diff = Double.MaxValue;

                // Image 1 data
                MIplImage image1 = img1.MIplImage;
                byte* image1Pointer = (byte*)image1.imageData.ToPointer();
                byte* pointerCopy1 = image1Pointer;
                int nChan1 = image1.nChannels;
                int padding1 = image1.widthStep - image1.nChannels * image1.width;
                int step1 = image1.widthStep;
                int width1 = img1.Width;
                int height1 = img1.Height;

                // Image 2 data
                MIplImage image2 = img2.MIplImage;
                byte* image2Pointer = (byte*)image2.imageData.ToPointer();
                byte* pointerCopy2 = image2Pointer;
                int nChan2 = image2.nChannels;
                int padding2 = image2.widthStep - image2.nChannels * image2.width;
                int step2 = image2.widthStep;
                int width2 = img2.Width;
                int height2 = img2.Height;

                double diff;
                double scale;

                // [START] 0 TOP->BOTTOM
                Console.WriteLine("Comparing TOP img1 to BOTTOM img2");
                image2Pointer += step2 * (height2 - 1);

                diff = 0;
                scale = (width1 > width2) ? width1 / (double)width2 : width2 / (double)width1;
                Console.WriteLine("Scale: " + scale);
                if (width1 == width2)
                {
                    Console.WriteLine("Width img1 == Wigth img2");
                    for (int y = 0; y < width1; y++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ",\t" + image1Pointer[1] + ",\t" + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ",\t" + image2Pointer[1] + ",\t" + image2Pointer[2]);
                        image1Pointer += nChan1;
                        image2Pointer += nChan2;
                    }
                }
                else if (width1 > width2)
                {
                    Console.WriteLine("Width img1 > Wigth img2");
                    int pos = 1;
                    for (int y = 0; y < width1; y++)
                    {
                        diff += ( Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2]) ) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image1Pointer += nChan1;
                        if(pos % scale == 0)
                            image2Pointer +=  nChan2;
                        pos++;
                    }
                }
                else
                {
                    Console.WriteLine("Width img1 < Wigth img2");
                    int pos = 1;
                    for (int y = 0; y < width2; y++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2]) ) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image2Pointer += nChan2;
                        if (pos % scale == 0)
                            image1Pointer += nChan1;
                        pos++;
                    }
                }
                Console.WriteLine("DIFF: " + diff);
                if(diff < best_diff)
                {
                    best_diff = diff;
                    best_side = 0; 
                }
                // [END] 0 TOP->BOTTOM

                // [START] 1 RIGHT->LEFT
                Console.WriteLine("Comparing RIGHT img1 to LEFT img2");
                image1Pointer = pointerCopy1;
                image1Pointer += nChan1 * (width1 - 1);

                image2Pointer = pointerCopy2;

                diff = 0;
                scale = (height1 > height2) ? height1 / (double)height2 : height2 / (double)height1;
                Console.WriteLine("Scale: " + scale);
                if (height1 == height2)
                {
                    Console.WriteLine("Height img1 == Height img2");
                    for (int x = 0; x < height1; x++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ",\t" + image1Pointer[1] + ",\t" + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ",\t" + image2Pointer[1] + ",\t" + image2Pointer[2]);
                        image1Pointer += step1;
                        image2Pointer += step2;
                    }
                }
                else if (height1 > height2)
                {
                    Console.WriteLine("Height img1 > Height img2");
                    int pos = 1;
                    for (int x = 0; x < height1; x++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image1Pointer += step1;
                        if (pos % scale == 0)
                            image2Pointer += step2;
                        pos++;
                    }
                }
                else
                {
                    Console.WriteLine("Height img1 < Height img2");
                    int pos = 1;
                    for (int x = 0; x < height2; x++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image2Pointer += step2;
                        if (pos % scale == 0)
                            image1Pointer += step1;
                        pos++;
                    }
                }
                Console.WriteLine("DIFF: " + diff);
                if (diff < best_diff)
                {
                    best_diff = diff;
                    best_side = 1;
                }
                // [END] 1 RIGHT->LEFT

                // [START] 2 BOTTOM->TOP
                Console.WriteLine("Comparing BOTTOM img1 to LEFT TOP");
                image1Pointer = pointerCopy1;
                image1Pointer += step1 * (height1 - 1);

                image2Pointer = pointerCopy2;

                diff = 0;
                scale = (width1 > width2) ? width1 / (double)width2 : width2 / (double)width1;
                Console.WriteLine("Scale: " + scale);
                if (width1 == width2)
                {
                    Console.WriteLine("Width img1 == Height img2");
                    for (int y = 0; y < width1; y++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ",\t" + image1Pointer[1] + ",\t" + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ",\t" + image2Pointer[1] + ",\t" + image2Pointer[2]);
                        image1Pointer += nChan1;
                        image2Pointer += nChan2;
                    }
                }
                else if (width1 > width2)
                {
                    Console.WriteLine("Width img1 > Height img2");
                    int pos = 1;
                    for (int y = 0; y < width1; y++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image1Pointer += nChan1;
                        if (pos % scale == 0)
                            image2Pointer += nChan2;
                        pos++;
                    }
                }
                else
                {
                    Console.WriteLine("Width img1 < Height img2");
                    int pos = 1;
                    for (int y = 0; y < width2; y++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image2Pointer += nChan2;
                        if (pos % scale == 0)
                            image1Pointer += nChan1;
                        pos++;
                    }
                }
                Console.WriteLine("DIFF: " + diff);
                if (diff < best_diff)
                {
                    best_diff = diff;
                    best_side = 2;
                }
                // [END] 2 BOTTOM->TOP

                // [START] 3 LEFT->RIGHT
                Console.WriteLine("Comparing LEFT img1 to RIGHT img2");
                image1Pointer = pointerCopy1;

                image2Pointer = pointerCopy2;
                image2Pointer += nChan2 * (width2 - 1);

                diff = 0;
                scale = (height1 > height2) ? height1 / (double)height2 : height2 / (double)height1;
                Console.WriteLine("Scale: " + scale);
                if (height1 == height2)
                {
                    Console.WriteLine("Height img1 == Height img2");
                    for (int x = 0; x < height1; x++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ",\t" + image1Pointer[1] + ",\t" + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ",\t" + image2Pointer[1] + ",\t" + image2Pointer[2]);
                        image1Pointer += step1;
                        image2Pointer += step2;
                    }
                }
                else if (height1 > height2)
                {
                    Console.WriteLine("Height img1 > Height img2");
                    int pos = 1;
                    for (int x = 0; x < height1; x++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image1Pointer += step1;
                        if (pos % scale == 0)
                            image2Pointer += step2;
                        pos++;
                    }
                }
                else
                {
                    Console.WriteLine("Height img1 < Height img2");
                    int pos = 1;
                    for (int x = 0; x < height2; x++)
                    {
                        diff += (Math.Abs(image1Pointer[0] - image2Pointer[0]) + Math.Abs(image1Pointer[1] - image2Pointer[1]) + Math.Abs(image1Pointer[2] - image2Pointer[2])) / 3.0 / 255;
                        Console.WriteLine("Image1 = " + image1Pointer[0] + ", " + image1Pointer[1] + ", " + image1Pointer[2]);
                        Console.WriteLine("Image2 = " + image2Pointer[0] + ", " + image2Pointer[1] + ", " + image2Pointer[2]);
                        image2Pointer += step2;
                        if (pos % scale == 0)
                            image1Pointer += step1;
                        pos++;
                    }
                }
                Console.WriteLine("DIFF: " + diff);
                if (diff < best_diff)
                {
                    best_diff = diff;
                    best_side = 3;
                }
                // [END] 3 LEFT->RIGHT

                // TODO
                // IF two sides have same diff ?

                Console.WriteLine("Best side: " + best_side + ", diff: " + best_diff);


                return new SideValues(best_side, best_diff); ;
            }
        }

        private struct SideValues
        {
            public int side;
            public double value;
            public SideValues(int side, double value)
            {
                this.side = side;
                this.value = value;
            }
        }

        public Image<Bgr, byte> getFinalImage()
        {
            SideValues[,] best_diffs = new SideValues[images_pieces.Count, images_pieces.Count];

            while (images_pieces.Count != 1)
            {
                int curr_pos = 0;
                foreach (Image<Bgr, byte> curr_piece in images_pieces)
                {
                    int next_pos = 0;
                    foreach (Image<Bgr, byte> next_piece in images_pieces)
                    {
                        if(curr_pos == next_pos || curr_pos > next_pos)
                        {
                            //Be gone
                        }

                        else
                        {
                            Console.WriteLine("Curr: " + curr_pos + "  \tNext: " + next_pos);
                            best_diffs[curr_pos, next_pos] = compareSides(curr_piece, next_piece);
                        }
                        
                        next_pos++;
                    }

                    curr_pos++;
                }
               
                break;
            }


            return images_pieces[0];
        }

    }
}