using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS_OpenCV
{
    class ImageClass
    {
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                                                                //byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // numero de canais 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhamento (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //obtém as 3 componentes
                            /*
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];
                            */

                            // store in the image
                            dataPtr[0] = (byte)(255 - dataPtr[0]);
                            dataPtr[1] = (byte)(255 - dataPtr[1]);
                            dataPtr[2] = (byte)(255 - dataPtr[2]);

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }

                /*
                Bgr aux;
                for (y = 0; y < img.Height; y++)
                {
                    for (x = 0; x < img.Width; x++)
                    {
                        // acesso directo : mais lento 
                        aux = img[y, x];
                        img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);
                    }
                }
                */
            }
        }

        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //obtém as 3 componentes
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)((blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                double blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            blue = dataPtr[0] * contrast + bright;
                            green = dataPtr[1] * contrast + bright;
                            red = dataPtr[2] * contrast + bright;


                            if (blue < 0)
                                blue = 0;
                            else if (blue > 255)
                                blue = 255;
                            dataPtr[0] = (byte)(Math.Round(blue));

                            if (green < 0)
                                green = 0;
                            else if (green > 255)
                                green = 255;
                            dataPtr[1] = (byte)(Math.Round(green));

                            if (red < 0)
                                red = 0;
                            else if (red > 255)
                                red = 255;
                            dataPtr[2] = (byte)(Math.Round(red));

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void RedChannel(Image<Bgr, byte> img)
        {

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direction top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            dataPtr[0] = dataPtr[2];
                            dataPtr[1] = dataPtr[2];

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                // get the image pointer
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                                                                   //int x, y;
                byte* pixel;

                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                int dy_diff;
                int dx_diff;
                byte* copy_pointer;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel address
                        dy_diff = y - dy;
                        dx_diff = x - dx;

                        copy_pointer = dataPtr + y * m.widthStep + x * nChan;

                        if (dy_diff >= 0 && dy_diff < height && dx_diff >= 0 && dx_diff < width)
                        {
                            pixel = dataPtr_copy + dy_diff * m.widthStep + dx_diff * nChan;

                            copy_pointer[0] = pixel[0];
                            copy_pointer[1] = pixel[1];
                            copy_pointer[2] = pixel[2];
                        }
                        else
                        {
                            copy_pointer[0] = 0;
                            copy_pointer[1] = 0;
                            copy_pointer[2] = 0;
                        }

                    }
                }


            }
        }

        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                // get the image pointer
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int step = m.widthStep;
                byte* pixel;

                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                int dy_diff;
                int dx_diff;
                byte* copy_pointer;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel address
                        dx_diff = (int)Math.Round((x - width / 2.0) * Math.Cos(angle) - (height / 2.0 - y) * Math.Sin(angle) + (width / 2.0));
                        dy_diff = (int)Math.Round(height / 2.0 - (x - width / 2.0) * Math.Sin(angle) - (height / 2.0 - y) * Math.Cos(angle));

                        copy_pointer = dataPtr + y * step + x * nChan;

                        if (dy_diff >= 0 && dy_diff < height && dx_diff >= 0 && dx_diff < width)
                        {
                            pixel = dataPtr_copy + dy_diff * step + dx_diff * nChan;

                            copy_pointer[0] = pixel[0];
                            copy_pointer[1] = pixel[1];
                            copy_pointer[2] = pixel[2];
                        }
                        else
                        {
                            copy_pointer[0] = 0;
                            copy_pointer[1] = 0;
                            copy_pointer[2] = 0;
                        }

                    }
                }
            }
        }

        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                // get the image pointer
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                                                                   //int x, y;
                byte* pixel;

                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                int dy_diff;
                int dx_diff;
                byte* copy_pointer;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel address
                        dx_diff = (int)Math.Round(x / scaleFactor);
                        dy_diff = (int)Math.Round(y / scaleFactor);

                        copy_pointer = dataPtr + y * m.widthStep + x * nChan;

                        if (dy_diff >= 0 && dy_diff < height && dx_diff >= 0 && dx_diff < width)
                        {
                            pixel = dataPtr_copy + dy_diff * m.widthStep + dx_diff * nChan;

                            copy_pointer[0] = pixel[0];
                            copy_pointer[1] = pixel[1];
                            copy_pointer[2] = pixel[2];
                        }
                        else
                        {
                            copy_pointer[0] = 0;
                            copy_pointer[1] = 0;
                            copy_pointer[2] = 0;
                        }

                    }
                }
            }
        }

        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                // get the image pointer
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                                                                   //int x, y;
                byte* pixel;

                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                int dy_diff;
                int dx_diff;
                byte* copy_pointer;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel address
                        dx_diff = (int)Math.Round((x - width / 2) / scaleFactor + centerX);
                        dy_diff = (int)Math.Round((y - height / 2) / scaleFactor + centerY);

                        copy_pointer = dataPtr + y * m.widthStep + x * nChan;

                        if (dy_diff >= 0 && dy_diff < height && dx_diff >= 0 && dx_diff < width)
                        {
                            pixel = dataPtr_copy + dy_diff * m.widthStep + dx_diff * nChan;

                            copy_pointer[0] = pixel[0];
                            copy_pointer[1] = pixel[1];
                            copy_pointer[2] = pixel[2];
                        }
                        else
                        {
                            copy_pointer[0] = 0;
                            copy_pointer[1] = 0;
                            copy_pointer[2] = 0;
                        }

                    }
                }
            }
        }

        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                //Original Image
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();
                byte* tmp_original = dataPtrOriginal;

                //Copy Image
                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)m_copy.imageData.ToPointer();
                byte* tmp_copy = dataPtrCopy;

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                //+step = move down
                //+nChan = move right

                /* --- START BORDERS --- */

                //Pointing at (0,0)

                //Pixel 1 (TOP-LEFT)
                dataPtrOriginal[0] = (byte)(Math.Round(
                    ((dataPtrCopy)[0] * 4 + (dataPtrCopy + nChan)[0] * 2 + (dataPtrCopy + step)[0] * 2 + (dataPtrCopy + step + nChan)[0])
                    / 9.0));
                dataPtrOriginal[1] = (byte)(Math.Round(
                    ((dataPtrCopy)[1] * 4 + (dataPtrCopy + nChan)[1] * 2 + (dataPtrCopy + step)[1] * 2 + (dataPtrCopy + step + nChan)[1])
                    / 9.0));
                dataPtrOriginal[2] = (byte)(Math.Round(
                    ((dataPtrCopy)[2] * 4 + (dataPtrCopy + nChan)[2] * 2 + (dataPtrCopy + step)[2] * 2 + (dataPtrCopy + step + nChan)[2])
                    / 9.0));

                //Line 1 (TOP)
                //Point to pixel (1,0)
                dataPtrOriginal += nChan;
                dataPtrCopy += nChan;

                for (int i = 1; i < width - 1; i++)
                {
                    dataPtrOriginal[0] = (byte)(Math.Round(
                        ((dataPtrCopy - nChan)[0] * 2 + (dataPtrCopy)[0] * 2 + (dataPtrCopy + nChan)[0] * 2
                        + (dataPtrCopy + step - nChan)[0] + (dataPtrCopy + step)[0] + (dataPtrCopy + step + nChan)[0])
                        / 9.0));
                    dataPtrOriginal[1] = (byte)(Math.Round(
                        ((dataPtrCopy - nChan)[1] * 2 + (dataPtrCopy)[1] * 2 + (dataPtrCopy + nChan)[1] * 2
                        + (dataPtrCopy + step - nChan)[1] + (dataPtrCopy + step)[1] + (dataPtrCopy + step + nChan)[1])
                        / 9.0));
                    dataPtrOriginal[2] = (byte)(Math.Round(
                        ((dataPtrCopy - nChan)[2] * 2 + (dataPtrCopy)[2] * 2 + (dataPtrCopy + nChan)[2] * 2
                        + (dataPtrCopy + step - nChan)[2] + (dataPtrCopy + step)[2] + (dataPtrCopy + step + nChan)[2])
                        / 9.0));

                    dataPtrOriginal += nChan;
                    dataPtrCopy += nChan;
                }
                //Pointing at pixel (width-1, 0)

                //Pixel 2 (TOP-RIGHT)
                dataPtrOriginal[0] = (byte)(Math.Round(
                    ((dataPtrCopy)[0] * 4 + (dataPtrCopy - nChan)[0] * 2 + +(dataPtrCopy + step)[0] * 2 + (dataPtrCopy + step - nChan)[0])
                    / 9.0));
                dataPtrOriginal[1] = (byte)(Math.Round(
                    ((dataPtrCopy)[1] * 4 + (dataPtrCopy - nChan)[1] * 2 + +(dataPtrCopy + step)[1] * 2 + (dataPtrCopy + step - nChan)[1])
                    / 9.0));
                dataPtrOriginal[2] = (byte)(Math.Round(
                    ((dataPtrCopy)[2] * 4 + (dataPtrCopy - nChan)[2] * 2 + +(dataPtrCopy + step)[2] * 2 + (dataPtrCopy + step - nChan)[2])
                    / 9.0));

                //Line 2 (RIGHT)
                //Point to pixel (width-1, 1)
                dataPtrOriginal += step;
                dataPtrCopy += step;

                for (int i = 1; i < height - 1; i++)
                {
                    dataPtrOriginal[0] = (byte)(Math.Round(
                                            ((dataPtrCopy - step)[0] * 2 + (dataPtrCopy)[0] * 2 + +(dataPtrCopy + step)[0] * 2
                                            + (dataPtrCopy - step - nChan)[0] + (dataPtrCopy - nChan)[0] + (dataPtrCopy + step - nChan)[0])
                                            / 9.0));
                    dataPtrOriginal[1] = (byte)(Math.Round(
                                         ((dataPtrCopy - step)[1] * 2 + (dataPtrCopy)[1] * 2 + +(dataPtrCopy + step)[1] * 2
                                            + (dataPtrCopy - step - nChan)[1] + (dataPtrCopy - nChan)[1] + (dataPtrCopy + step - nChan)[1])
                                            / 9.0));
                    dataPtrOriginal[2] = (byte)(Math.Round(
                                         ((dataPtrCopy - step)[2] * 2 + (dataPtrCopy)[2] * 2 + +(dataPtrCopy + step)[2] * 2
                                            + (dataPtrCopy - step - nChan)[2] + (dataPtrCopy - nChan)[2] + (dataPtrCopy + step - nChan)[2])
                                            / 9.0));

                    dataPtrOriginal += step;
                    dataPtrCopy += step;
                }
                //Pointing at pixel (width-1, height-1)

                //Pixel 3 (BOTTOM-RIGHT)
                dataPtrOriginal[0] = (byte)(Math.Round(
                    ((dataPtrCopy)[0] * 4 + (dataPtrCopy - nChan)[0] * 2 + (dataPtrCopy - step)[0] * 2 + (dataPtrCopy - step - nChan)[0])
                    / 9.0));
                dataPtrOriginal[1] = (byte)(Math.Round(
                    ((dataPtrCopy)[1] * 4 + (dataPtrCopy - nChan)[1] * 2 + (dataPtrCopy - step)[1] * 2 + (dataPtrCopy - step - nChan)[1])
                    / 9.0));
                dataPtrOriginal[2] = (byte)(Math.Round(
                    ((dataPtrCopy)[2] * 4 + (dataPtrCopy - nChan)[2] * 2 + (dataPtrCopy - step)[2] * 2 + (dataPtrCopy - step - nChan)[2])
                    / 9.0));

                //Line 3 (BOTTOM)
                //Point to pixel (width-2, height-1)
                dataPtrOriginal -= nChan;
                dataPtrCopy -= nChan;

                for (int i = 1; i < width - 1; i++)
                {
                    dataPtrOriginal[0] = (byte)(Math.Round(
                                            ((dataPtrCopy - nChan)[0] * 2 + (dataPtrCopy)[0] * 2 + (dataPtrCopy + nChan)[0] * 2
                                            + (dataPtrCopy - step - nChan)[0] + (dataPtrCopy - step)[0] + (dataPtrCopy - step + nChan)[0])
                                            / 9.0));
                    dataPtrOriginal[1] = (byte)(Math.Round(
                                         ((dataPtrCopy - nChan)[1] * 2 + (dataPtrCopy)[1] * 2 + (dataPtrCopy + nChan)[1] * 2
                                            + (dataPtrCopy - step - nChan)[1] + (dataPtrCopy - step)[1] + (dataPtrCopy - step + nChan)[1])
                                            / 9.0));
                    dataPtrOriginal[2] = (byte)(Math.Round(
                                         ((dataPtrCopy - nChan)[2] * 2 + (dataPtrCopy)[2] * 2 + (dataPtrCopy + nChan)[2] * 2
                                            + (dataPtrCopy - step - nChan)[2] + (dataPtrCopy - step)[2] + (dataPtrCopy - step + nChan)[2])
                                            / 9.0));

                    dataPtrOriginal -= nChan;
                    dataPtrCopy -= nChan;
                }
                //Pointing at pixel (0, height-1)

                //Pixel 4 (BOTTOM-LEFT)
                dataPtrOriginal[0] = (byte)(Math.Round(
                    ((dataPtrCopy)[0] * 4 + (dataPtrCopy + nChan)[0] * 2 + (dataPtrCopy - step)[0] * 2 + (dataPtrCopy - step + nChan)[0])
                    / 9.0));
                dataPtrOriginal[1] = (byte)(Math.Round(
                    ((dataPtrCopy)[1] * 4 + (dataPtrCopy + nChan)[1] * 2 + (dataPtrCopy - step)[1] * 2 + (dataPtrCopy - step + nChan)[1])
                    / 9.0));
                dataPtrOriginal[2] = (byte)(Math.Round(
                    ((dataPtrCopy)[2] * 4 + (dataPtrCopy + nChan)[2] * 2 + (dataPtrCopy - step)[2] * 2 + (dataPtrCopy - step + nChan)[2])
                    / 9.0));

                //Line 4 (LEFT)
                //Point to pixel (0, height-2)
                dataPtrOriginal -= step;
                dataPtrCopy -= step;

                for (int i = 1; i < height - 1; i++)
                {
                    dataPtrOriginal[0] = (byte)(Math.Round(
                                            ((dataPtrCopy - step)[0] * 2 + (dataPtrCopy)[0] * 2 + +(dataPtrCopy + step)[0] * 2
                                            + (dataPtrCopy - step + nChan)[0] + (dataPtrCopy + nChan)[0] + (dataPtrCopy + step + nChan)[0])
                                            / 9.0));
                    dataPtrOriginal[1] = (byte)(Math.Round(
                                        ((dataPtrCopy - step)[1] * 2 + (dataPtrCopy)[1] * 2 + +(dataPtrCopy + step)[1] * 2
                                            + (dataPtrCopy - step + nChan)[1] + (dataPtrCopy + nChan)[1] + (dataPtrCopy + step + nChan)[1])
                                            / 9.0));
                    dataPtrOriginal[2] = (byte)(Math.Round(
                                        ((dataPtrCopy - step)[2] * 2 + (dataPtrCopy)[2] * 2 + +(dataPtrCopy + step)[2] * 2
                                            + (dataPtrCopy - step + nChan)[2] + (dataPtrCopy + nChan)[2] + (dataPtrCopy + step + nChan)[2])
                                            / 9.0));

                    dataPtrOriginal -= step;
                    dataPtrCopy -= step;
                }

                //Pointing at pixel (0,0)

                /* --- END BORDERS --- */


                /* --- START CORE --- */

                //Pointing at pixel (1,1)
                dataPtrOriginal += nChan + step;
                dataPtrCopy += nChan + step;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {

                        dataPtrOriginal[0] = (byte)(Math.Round(
                            ((dataPtrCopy - nChan - step)[0] + (dataPtrCopy - step)[0] + (dataPtrCopy + nChan - step)[0]
                            + (dataPtrCopy - nChan)[0] + (dataPtrCopy)[0] + (dataPtrCopy + nChan)[0]
                            + (dataPtrCopy + step - nChan)[0] + (dataPtrCopy + step)[0] + (dataPtrCopy + step + nChan)[0])
                            / 9.0));

                        dataPtrOriginal[1] = (byte)(Math.Round(
                            ((dataPtrCopy - nChan - step)[1] + (dataPtrCopy - step)[1] + (dataPtrCopy + nChan - step)[1]
                            + (dataPtrCopy - nChan)[1] + (dataPtrCopy)[1] + (dataPtrCopy + nChan)[1]
                            + (dataPtrCopy + step - nChan)[1] + (dataPtrCopy + step)[1] + (dataPtrCopy + step + nChan)[1])
                            / 9.0));

                        dataPtrOriginal[2] = (byte)(Math.Round(
                            ((dataPtrCopy - nChan - step)[2] + (dataPtrCopy - step)[2] + (dataPtrCopy + nChan - step)[2]
                            + (dataPtrCopy - nChan)[2] + (dataPtrCopy)[2] + (dataPtrCopy + nChan)[2]
                            + (dataPtrCopy + step - nChan)[2] + (dataPtrCopy + step)[2] + (dataPtrCopy + step + nChan)[2])
                            / 9.0));


                        //Moving one pixel
                        dataPtrOriginal += nChan;
                        dataPtrCopy += nChan;

                    }

                    //Moving one line
                    dataPtrOriginal += nChan * 2 + padding;
                    dataPtrCopy += nChan * 2 + padding;
                }

                /* --- END CORE --- */
            }
        }

        public static void Mean_solutionB(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                //Test.Mean_solutionB(img, imgCopy);
            }
        }

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {
            unsafe
            {
                //Original Image
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();
                byte* tmp_original = dataPtrOriginal;

                //Copy Image
                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)m_copy.imageData.ToPointer();
                byte* tmp_copy = dataPtrCopy;

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                //Helper variables
                double blue, green, red;

                //+step = move down
                //+nChan = move right

                /* --- START BORDERS --- */

                //Pointing at (0,0)

                //Pixel 1 (TOP-LEFT)
                blue = ((dataPtrCopy)[0] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1]) + (dataPtrCopy + nChan)[0] * (matrix[0, 2] + matrix[1, 2]) + (dataPtrCopy + step)[0] * (matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy + step + nChan)[0] * matrix[2, 2]) / matrixWeight;

                if (blue > 255) blue = 255;
                else if (blue < 0) blue = 0;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = ((dataPtrCopy)[1] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1]) + (dataPtrCopy + nChan)[1] * (matrix[0, 2] + matrix[1, 2]) + (dataPtrCopy + step)[1] * (matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy + step + nChan)[1] * matrix[2, 2]) / matrixWeight;

                if (green > 255) green = 255;
                else if (green < 0) green = 0;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = ((dataPtrCopy)[2] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1]) + (dataPtrCopy + nChan)[2] * (matrix[0, 2] + matrix[1, 2]) + (dataPtrCopy + step)[2] * (matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy + step + nChan)[2] * matrix[2, 2]) / matrixWeight;

                if (red > 255) red = 255;
                else if (red < 0) red = 0;
                dataPtrOriginal[2] = (byte)(Math.Round(red));

                //Line 1 (TOP)
                //Point to pixel (1,0)
                dataPtrOriginal += nChan;
                dataPtrCopy += nChan;

                for (int i = 1; i < width - 1; i++)
                {
                    blue = (
                        (dataPtrCopy - nChan)[0] * (matrix[1, 0] + matrix[0, 0]) + (dataPtrCopy)[0] * (matrix[1, 1] + matrix[0, 1]) + (dataPtrCopy + nChan)[0] * (matrix[0, 2] + matrix[1, 2])
                        + (dataPtrCopy + step - nChan)[0] * (matrix[2, 0]) + (dataPtrCopy + step)[0] * (matrix[2, 1]) + (dataPtrCopy + step + nChan)[0] * (matrix[2, 2])) / matrixWeight;

                    if (blue > 255) blue = 255;
                    else if (blue < 0) blue = 0;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = (
                        (dataPtrCopy - nChan)[1] * (matrix[1, 0] + matrix[0, 0]) + (dataPtrCopy)[1] * (matrix[1, 1] + matrix[0, 1]) + (dataPtrCopy + nChan)[1] * (matrix[0, 2] + matrix[1, 2])
                        + (dataPtrCopy + step - nChan)[1] * (matrix[2, 0]) + (dataPtrCopy + step)[1] * (matrix[2, 1]) + (dataPtrCopy + step + nChan)[1] * (matrix[2, 2])) / matrixWeight;

                    if (green > 255) green = 255;
                    else if (green < 0) green = 0;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = (
                        (dataPtrCopy - nChan)[2] * (matrix[1, 0] + matrix[0, 0]) + (dataPtrCopy)[2] * (matrix[1, 1] + matrix[0, 1]) + (dataPtrCopy + nChan)[2] * (matrix[0, 2] + matrix[1, 2])
                        + (dataPtrCopy + step - nChan)[2] * (matrix[2, 0]) + (dataPtrCopy + step)[2] * (matrix[2, 1]) + (dataPtrCopy + step + nChan)[2] * (matrix[2, 2])) / matrixWeight;

                    if (red > 255) red = 255;
                    else if (red < 0) red = 0;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal += nChan;
                    dataPtrCopy += nChan;
                }
                //Pointing at pixel (width-1, 0)

                //Pixel 2 (TOP-RIGHT)
                blue = ((dataPtrCopy)[0] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy - nChan)[0] * (matrix[1, 0] + matrix[0, 0]) + (dataPtrCopy + step)[0] * (matrix[2, 1] + matrix[2, 2]) + (dataPtrCopy + step - nChan)[0] * matrix[2, 0]) / matrixWeight;

                if (blue > 255) blue = 255;
                else if (blue < 0) blue = 0;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = ((dataPtrCopy)[1] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy - nChan)[1] * (matrix[1, 0] + matrix[0, 0]) + (dataPtrCopy + step)[1] * (matrix[2, 1] + matrix[2, 2]) + (dataPtrCopy + step - nChan)[1] * matrix[2, 0]) / matrixWeight;

                if (green > 255) green = 255;
                else if (green < 0) green = 0;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = ((dataPtrCopy)[2] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy - nChan)[2] * (matrix[1, 0] + matrix[0, 0]) + (dataPtrCopy + step)[2] * (matrix[2, 1] + matrix[2, 2]) + (dataPtrCopy + step - nChan)[2] * matrix[2, 0]) / matrixWeight;

                if (red > 255) red = 255;
                else if (red < 0) red = 0;
                dataPtrOriginal[2] = (byte)(Math.Round(red));

                //Line 2 (RIGHT)
                //Point to pixel (width-1, 1)
                dataPtrOriginal += step;
                dataPtrCopy += step;

                for (int i = 1; i < height - 1; i++)
                {
                    blue = ((dataPtrCopy - step)[0] * (matrix[0, 1] + matrix[0, 2]) + (dataPtrCopy)[0] * (matrix[1, 1] + matrix[1, 2]) + (dataPtrCopy + step)[0] * (matrix[2, 1] + matrix[2, 2])
                            + (dataPtrCopy - step - nChan)[0] * (matrix[0, 0]) + (dataPtrCopy - nChan)[0] * (matrix[1, 0]) + (dataPtrCopy + step - nChan)[0] * (matrix[2, 0])) / matrixWeight;

                    if (blue > 255) blue = 255;
                    else if (blue < 0) blue = 0;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = ((dataPtrCopy - step)[1] * (matrix[0, 1] + matrix[0, 2]) + (dataPtrCopy)[1] * (matrix[1, 1] + matrix[1, 2]) + (dataPtrCopy + step)[1] * (matrix[2, 1] + matrix[2, 2])
                            + (dataPtrCopy - step - nChan)[1] * (matrix[0, 0]) + (dataPtrCopy - nChan)[1] * (matrix[1, 0]) + (dataPtrCopy + step - nChan)[1] * (matrix[2, 0])) / matrixWeight;

                    if (green > 255) green = 255;
                    else if (green < 0) green = 0;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = ((dataPtrCopy - step)[2] * (matrix[0, 1] + matrix[0, 2]) + (dataPtrCopy)[2] * (matrix[1, 1] + matrix[1, 2]) + (dataPtrCopy + step)[2] * (matrix[2, 1] + matrix[2, 2])
                            + (dataPtrCopy - step - nChan)[2] * (matrix[0, 0]) + (dataPtrCopy - nChan)[2] * (matrix[1, 0]) + (dataPtrCopy + step - nChan)[2] * (matrix[2, 0])) / matrixWeight;

                    if (red > 255) red = 255;
                    else if (red < 0) red = 0;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal += step;
                    dataPtrCopy += step;
                }
                //Pointing at pixel (width-1, height-1)

                //Pixel 3 (BOTTOM-RIGHT)
                blue = ((dataPtrCopy)[0] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2]) + (dataPtrCopy - nChan)[0] * (matrix[1, 0] + matrix[2, 0]) + (dataPtrCopy - step)[0] * (matrix[0, 1] + matrix[0, 2]) + (dataPtrCopy - step - nChan)[0] * matrix[0, 0]) / matrixWeight;

                if (blue > 255) blue = 255;
                else if (blue < 0) blue = 0;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = ((dataPtrCopy)[1] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2]) + (dataPtrCopy - nChan)[1] * (matrix[1, 0] + matrix[2, 0]) + (dataPtrCopy - step)[1] * (matrix[0, 1] + matrix[0, 2]) + (dataPtrCopy - step - nChan)[1] * matrix[0, 0]) / matrixWeight;

                if (green > 255) green = 255;
                else if (green < 0) green = 0;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = ((dataPtrCopy)[2] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2]) + (dataPtrCopy - nChan)[2] * (matrix[1, 0] + matrix[2, 0]) + (dataPtrCopy - step)[2] * (matrix[0, 1] + matrix[0, 2]) + (dataPtrCopy - step - nChan)[2] * matrix[0, 0]) / matrixWeight;

                if (red > 255) red = 255;
                else if (red < 0) red = 0;
                dataPtrOriginal[2] = (byte)(Math.Round(red));



                //Line 3 (BOTTOM)
                //Point to pixel (width-2, height-1)
                dataPtrOriginal -= nChan;
                dataPtrCopy -= nChan;

                for (int i = 1; i < width - 1; i++)
                {
                    blue = ((dataPtrCopy)[0] * (matrix[1, 1] + matrix[2, 1]) + (dataPtrCopy - nChan)[0] * (matrix[1, 0] + matrix[2, 0]) + (dataPtrCopy + nChan)[0] * (matrix[1, 2] + matrix[2, 2])
                        + (dataPtrCopy - step - nChan)[0] * (matrix[0, 0]) + (dataPtrCopy - step)[0] * (matrix[0, 1]) + (dataPtrCopy - step + nChan)[0] * matrix[0, 2]) / matrixWeight;

                    if (blue > 255) blue = 255;
                    else if (blue < 0) blue = 0;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = ((dataPtrCopy)[1] * (matrix[1, 1] + matrix[2, 1]) + (dataPtrCopy - nChan)[1] * (matrix[1, 0] + matrix[2, 0]) + (dataPtrCopy + nChan)[1] * (matrix[1, 2] + matrix[2, 2])
                        + (dataPtrCopy - step - nChan)[1] * (matrix[0, 0]) + (dataPtrCopy - step)[1] * (matrix[0, 1]) + (dataPtrCopy - step + nChan)[1] * matrix[0, 2]) / matrixWeight;

                    if (green > 255) green = 255;
                    else if (green < 0) green = 0;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = ((dataPtrCopy)[2] * (matrix[1, 1] + matrix[2, 1]) + (dataPtrCopy - nChan)[2] * (matrix[1, 0] + matrix[2, 0]) + (dataPtrCopy + nChan)[2] * (matrix[1, 2] + matrix[2, 2])
                        + (dataPtrCopy - step - nChan)[2] * (matrix[0, 0]) + (dataPtrCopy - step)[2] * (matrix[0, 1]) + (dataPtrCopy - step + nChan)[2] * matrix[0, 2]) / matrixWeight;

                    if (red > 255) red = 255;
                    else if (red < 0) red = 0;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal -= nChan;
                    dataPtrCopy -= nChan;
                }
                //Pointing at pixel (0, height-1)

                //Pixel 4 (BOTTOM-LEFT)

                blue = ((dataPtrCopy)[0] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy + nChan)[0] * (matrix[1, 2] + matrix[2, 2]) + (dataPtrCopy - step)[0] * (matrix[0, 1] + matrix[0, 0]) + (dataPtrCopy - step + nChan)[0] * matrix[0, 2]) / matrixWeight;

                if (blue > 255) blue = 255;
                else if (blue < 0) blue = 0;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = ((dataPtrCopy)[1] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy + nChan)[1] * (matrix[1, 2] + matrix[2, 2]) + (dataPtrCopy - step)[1] * (matrix[0, 1] + matrix[0, 0]) + (dataPtrCopy - step + nChan)[1] * matrix[0, 2]) / matrixWeight;

                if (green > 255) green = 255;
                else if (green < 0) green = 0;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = ((dataPtrCopy)[2] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) + (dataPtrCopy + nChan)[2] * (matrix[1, 2] + matrix[2, 2]) + (dataPtrCopy - step)[2] * (matrix[0, 1] + matrix[0, 0]) + (dataPtrCopy - step + nChan)[2] * matrix[0, 2]) / matrixWeight;

                if (red > 255) red = 255;
                else if (red < 0) red = 0;
                dataPtrOriginal[2] = (byte)(Math.Round(red));

                //Line 4 (LEFT)
                //Point to pixel (0, height-2)
                dataPtrOriginal -= step;
                dataPtrCopy -= step;

                for (int i = 1; i < height - 1; i++)
                {

                    blue = ((dataPtrCopy)[0] * (matrix[1, 1] + matrix[1, 0]) + (dataPtrCopy - step)[0] * (matrix[0, 1] + matrix[0, 0]) + (dataPtrCopy + step)[0] * (matrix[2, 1] + matrix[2, 0])
                        + (dataPtrCopy - step + nChan)[0] * (matrix[0, 2]) + (dataPtrCopy + step + nChan)[0] * (matrix[2, 2]) + (dataPtrCopy + nChan)[0] * matrix[1, 2]) / matrixWeight;

                    if (blue > 255) blue = 255;
                    else if (blue < 0) blue = 0;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = ((dataPtrCopy)[1] * (matrix[1, 1] + matrix[1, 0]) + (dataPtrCopy - step)[1] * (matrix[0, 1] + matrix[0, 0]) + (dataPtrCopy + step)[1] * (matrix[2, 1] + matrix[2, 0])
                        + (dataPtrCopy - step + nChan)[1] * (matrix[0, 2]) + (dataPtrCopy + step + nChan)[1] * (matrix[2, 2]) + (dataPtrCopy + nChan)[1] * matrix[1, 2]) / matrixWeight;

                    if (green > 255) green = 255;
                    else if (green < 0) green = 0;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = ((dataPtrCopy)[2] * (matrix[1, 1] + matrix[1, 0]) + (dataPtrCopy - step)[2] * (matrix[0, 1] + matrix[0, 0]) + (dataPtrCopy + step)[2] * (matrix[2, 1] + matrix[2, 0])
                        + (dataPtrCopy - step + nChan)[2] * (matrix[0, 2]) + (dataPtrCopy + step + nChan)[2] * (matrix[2, 2]) + (dataPtrCopy + nChan)[2] * matrix[1, 2]) / matrixWeight;

                    if (red > 255) red = 255;
                    else if (red < 0) red = 0;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal -= step;
                    dataPtrCopy -= step;
                }

                //Pointing at pixel (0,0)

                /* --- END BORDERS --- */



                /* --- START CORE --- */

                //Point to pixel (1,1)
                dataPtrOriginal += nChan + step;
                dataPtrCopy += nChan + step;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        blue = (
                            (dataPtrCopy - nChan - step)[0] * matrix[0, 0] + (dataPtrCopy - step)[0] * matrix[0, 1] + (dataPtrCopy + nChan - step)[0] * matrix[0, 2]
                            + (dataPtrCopy - nChan)[0] * matrix[1, 0] + (dataPtrCopy)[0] * matrix[1, 1] + (dataPtrCopy + nChan)[0] * matrix[1, 2]
                            + (dataPtrCopy + step - nChan)[0] * matrix[2, 0] + (dataPtrCopy + step)[0] * matrix[2, 1] + (dataPtrCopy + step + nChan)[0] * matrix[2, 2]
                            ) / matrixWeight;

                        if (blue > 255) blue = 255;
                        else if (blue < 0) blue = 0;
                        dataPtrOriginal[0] = (byte)(Math.Round(blue));

                        green = (
                            (dataPtrCopy - nChan - step)[1] * matrix[0, 0] + (dataPtrCopy - step)[1] * matrix[0, 1] + (dataPtrCopy + nChan - step)[1] * matrix[0, 2]
                            + (dataPtrCopy - nChan)[1] * matrix[1, 0] + (dataPtrCopy)[1] * matrix[1, 1] + (dataPtrCopy + nChan)[1] * matrix[1, 2]
                            + (dataPtrCopy + step - nChan)[1] * matrix[2, 0] + (dataPtrCopy + step)[1] * matrix[2, 1] + (dataPtrCopy + step + nChan)[1] * matrix[2, 2]
                            ) / matrixWeight;

                        if (green > 255) green = 255;
                        else if (green < 0) green = 0;
                        dataPtrOriginal[1] = (byte)(Math.Round(green));

                        red = (
                            (dataPtrCopy - nChan - step)[2] * matrix[0, 0] + (dataPtrCopy - step)[2] * matrix[0, 1] + (dataPtrCopy + nChan - step)[2] * matrix[0, 2]
                            + (dataPtrCopy - nChan)[2] * matrix[1, 0] + (dataPtrCopy)[2] * matrix[1, 1] + (dataPtrCopy + nChan)[2] * matrix[1, 2]
                            + (dataPtrCopy + step - nChan)[2] * matrix[2, 0] + (dataPtrCopy + step)[2] * matrix[2, 1] + (dataPtrCopy + step + nChan)[2] * matrix[2, 2]
                            ) / matrixWeight;

                        if (red > 255) red = 255;
                        else if (red < 0) red = 0;
                        dataPtrOriginal[2] = (byte)(Math.Round(red));

                        //Moving one pixel
                        dataPtrOriginal += nChan;
                        dataPtrCopy += nChan;

                    }

                    //Moving one line
                    dataPtrOriginal += nChan * 2 + padding;
                    dataPtrCopy += nChan * 2 + padding;
                }

                /* --- END CORE --- */
            }
        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                //Original Image
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();
                byte* tmp_original = dataPtrOriginal;

                //Copy Image
                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)m_copy.imageData.ToPointer();
                byte* tmp_copy = dataPtrCopy;

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                int blue, green, red;
                byte* a, b, c, d, f, g, h, i;

                //+step = move down
                //+nChan = move right

                /* --- START BORDERS --- */

                //Pointing at (0,0)

                //Pixel 1 (TOP-LEFT)


                a = dataPtrCopy;
                b = dataPtrCopy;
                c = dataPtrCopy + nChan;
                d = dataPtrCopy;
                f = dataPtrCopy + nChan;
                g = dataPtrCopy + step;
                h = dataPtrCopy + step;
                i = dataPtrCopy + nChan + step;

                blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)blue;

                green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)green;

                red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)red;



                //Line 1 (TOP)
                //Point to pixel (1,0)
                dataPtrOriginal += nChan;
                dataPtrCopy += nChan;

                for (int n = 1; n < width - 1; n++)
                {

                    a = dataPtrCopy - nChan;
                    b = dataPtrCopy;
                    c = dataPtrCopy + nChan;
                    d = dataPtrCopy - nChan;
                    f = dataPtrCopy + nChan;
                    g = dataPtrCopy - nChan + step;
                    h = dataPtrCopy + step;
                    i = dataPtrCopy + nChan + step;

                    blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)blue;

                    green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)green;

                    red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)red;


                    dataPtrOriginal += nChan;
                    dataPtrCopy += nChan;
                }
                //Pointing at pixel (width-1, 0)

                //Pixel 2 (TOP-RIGHT)

                a = dataPtrCopy - nChan;
                b = dataPtrCopy;
                c = dataPtrCopy;
                d = dataPtrCopy - nChan;
                f = dataPtrCopy;
                g = dataPtrCopy - nChan + step;
                h = dataPtrCopy + step;
                i = dataPtrCopy + step;

                blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)blue;

                green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)green;

                red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)red;

                //Line 2 (RIGHT)
                //Point to pixel (width-1, 1)
                dataPtrOriginal += step;
                dataPtrCopy += step;

                for (int n = 1; n < height - 1; n++)
                {

                    a = dataPtrCopy - nChan - step;
                    b = dataPtrCopy - step;
                    c = dataPtrCopy - step;
                    d = dataPtrCopy - nChan;
                    f = dataPtrCopy;
                    g = dataPtrCopy - nChan + step;
                    h = dataPtrCopy + step;
                    i = dataPtrCopy + step;

                    blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)blue;

                    green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)green;

                    red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)red;

                    dataPtrOriginal += step;
                    dataPtrCopy += step;
                }
                //Pointing at pixel (width-1, height-1)

                //Pixel 3 (BOTTOM-RIGHT)

                a = dataPtrCopy - nChan - step;
                b = dataPtrCopy - step;
                c = dataPtrCopy - step;
                d = dataPtrCopy - nChan;
                f = dataPtrCopy;
                g = dataPtrCopy - nChan;
                h = dataPtrCopy;
                i = dataPtrCopy;

                blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)blue;

                green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)green;

                red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)red;


                //Line 3 (BOTTOM)
                //Point to pixel (width-2, height-1)
                dataPtrOriginal -= nChan;
                dataPtrCopy -= nChan;

                for (int n = 1; n < width - 1; n++)
                {
                    a = dataPtrCopy - nChan - step;
                    b = dataPtrCopy - step;
                    c = dataPtrCopy + nChan - step;
                    d = dataPtrCopy - nChan;
                    f = dataPtrCopy + nChan;
                    g = dataPtrCopy - nChan;
                    h = dataPtrCopy;
                    i = dataPtrCopy + nChan;

                    blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)blue;

                    green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)green;

                    red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)red;

                    dataPtrOriginal -= nChan;
                    dataPtrCopy -= nChan;
                }
                //Pointing at pixel (0, height-1)

                //Pixel 4 (BOTTOM-LEFT)
                a = dataPtrCopy - step;
                b = dataPtrCopy - step;
                c = dataPtrCopy + nChan - step;
                d = dataPtrCopy;
                f = dataPtrCopy + nChan;
                g = dataPtrCopy;
                h = dataPtrCopy;
                i = dataPtrCopy + nChan;

                blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)blue;

                green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)green;

                red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)red;

                //Line 4 (LEFT)
                //Point to pixel (0, height-2)
                dataPtrOriginal -= step;
                dataPtrCopy -= step;

                for (int n = 1; n < height - 1; n++)
                {
                    a = dataPtrCopy - step;
                    b = dataPtrCopy - step;
                    c = dataPtrCopy + nChan - step;
                    d = dataPtrCopy;
                    f = dataPtrCopy + nChan;
                    g = dataPtrCopy + step;
                    h = dataPtrCopy + step;
                    i = dataPtrCopy + nChan + step;

                    blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)blue;

                    green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)green;

                    red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)red;

                    dataPtrOriginal -= step;
                    dataPtrCopy -= step;
                }

                //Pointing at pixel (0,0)

                /* --- END BORDERS --- */


                /* --- START CORE --- */

                //Pointing at pixel (1,1)
                dataPtrOriginal += nChan + step;
                dataPtrCopy += nChan + step;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {

                        a = (dataPtrCopy - nChan - step);
                        b = (dataPtrCopy - step);
                        c = (dataPtrCopy + nChan - step);
                        d = (dataPtrCopy - nChan);
                        f = (dataPtrCopy + nChan);
                        g = (dataPtrCopy - nChan + step);
                        h = (dataPtrCopy + step);
                        i = (dataPtrCopy + nChan + step);

                        blue = Math.Abs(a[0] + 2 * d[0] + g[0] - (c[0] + 2 * f[0] + i[0])) + Math.Abs(g[0] + 2 * h[0] + i[0] - (a[0] + 2 * b[0] + c[0]));

                        if (blue > 255) blue = 255;
                        dataPtrOriginal[0] = (byte)blue;

                        green = Math.Abs(a[1] + 2 * d[1] + g[1] - (c[1] + 2 * f[1] + i[1])) + Math.Abs(g[1] + 2 * h[1] + i[1] - (a[1] + 2 * b[1] + c[1]));

                        if (green > 255) green = 255;
                        dataPtrOriginal[1] = (byte)green;

                        red = Math.Abs(a[2] + 2 * d[2] + g[2] - (c[2] + 2 * f[2] + i[2])) + Math.Abs(g[2] + 2 * h[2] + i[2] - (a[2] + 2 * b[2] + c[2]));

                        if (red > 255) red = 255;
                        dataPtrOriginal[2] = (byte)red;

                        //Moving one pixel
                        dataPtrOriginal += nChan;
                        dataPtrCopy += nChan;

                    }

                    //Moving one line
                    dataPtrOriginal += nChan * 2 + padding;
                    dataPtrCopy += nChan * 2 + padding;
                }

                /* --- END CORE --- */
            }
        }

        public static void Roberts(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                //Original Image
                var m = img.MIplImage;
                var dataPtrOriginal = (byte*)m.imageData.ToPointer();
                var tmpOriginal = dataPtrOriginal;

                //Copy Image
                var mCopy = imgCopy.MIplImage;
                var dataPtrCopy = (byte*)mCopy.imageData.ToPointer();
                var tmpCopy = dataPtrCopy;

                //Image values
                var width = img.Width;
                var height = img.Height;
                var nChan = m.nChannels;
                var padding = m.widthStep - m.nChannels * m.width;
                var step = m.widthStep;

                // a | b
                // c | d
                int[] channels = { 0, 1, 2 };
                byte* a, b, c, d;
                int color;

                //+step = move down
                //+nChan = move right

                // Right Border
                dataPtrOriginal += nChan * (width - 1);
                dataPtrCopy += nChan * (width - 1);

                for (var i = 0; i < height - 1; i++)
                {
                    a = b = dataPtrCopy;
                    c = d = dataPtrCopy + step;

                    foreach (var ch in channels)
                    {
                        color = (byte)(Math.Abs(a[ch] - d[ch]) + Math.Abs(b[ch] - c[ch]));

                        if (color > 255) color = 255;
                        else if (color < 0) color = 0;
                        dataPtrOriginal[ch] = (byte)color;

                    }

                    dataPtrOriginal += step;
                    dataPtrCopy += step;
                }

                // Right-Bottom Pixel
                a = b = c = d = dataPtrCopy;

                foreach (var ch in channels)
                {
                    color = (byte)(Math.Abs(a[ch] - d[ch]) + Math.Abs(b[ch] - c[ch]));

                    if (color > 255) color = 255;
                    else if (color < 0) color = 0;
                    dataPtrOriginal[ch] = (byte)color;
                }

                // Bottom Border
                dataPtrOriginal -= nChan;
                dataPtrCopy -= nChan;

                for (var i = width - 1; i > 0; i--)
                {
                    a = c = dataPtrCopy;
                    b = d = dataPtrCopy + nChan;

                    foreach (var ch in channels)
                    {
                        color = (byte)(Math.Abs(a[ch] - d[ch]) + Math.Abs(b[ch] - c[ch]));

                        if (color > 255) color = 255;
                        else if (color < 0) color = 0;
                        dataPtrOriginal[ch] = (byte)color;
                    }

                    dataPtrOriginal -= nChan;
                    dataPtrCopy -= nChan;
                }

                dataPtrOriginal = tmpOriginal;
                dataPtrCopy = tmpCopy;

                for (var y = 0; y < height - 1; y++)
                {
                    for (var x = 0; x < width - 1; x++)
                    {
                        a = dataPtrCopy;
                        b = dataPtrCopy + nChan;
                        c = dataPtrCopy + step;
                        d = dataPtrCopy + nChan + step;

                        foreach (var ch in channels)
                        {
                            color = (byte)(Math.Abs(a[ch] - d[ch]) + Math.Abs(b[ch] - c[ch]));

                            if (color > 255) color = 255;
                            else if (color < 0) color = 0;
                            dataPtrOriginal[ch] = (byte)color;
                        }

                        //Moving one pixel
                        dataPtrOriginal += nChan;
                        dataPtrCopy += nChan;
                    }

                    //Moving one line
                    dataPtrOriginal += nChan + padding;
                    dataPtrCopy += nChan + padding;
                }
            }
        }

        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                //Original Image
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();
                byte* tmp_original = dataPtrOriginal;

                //Copy Image
                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)m_copy.imageData.ToPointer();
                byte* tmp_copy = dataPtrCopy;

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                double blue, green, red;
                byte* a, b, c;

                //+step = move down
                //+nChan = move right

                /* --- START BORDERS --- */

                //Pointing at (0,0)

                //Pixel 1 (TOP-LEFT)
                a = dataPtrCopy;
                b = dataPtrCopy + nChan;
                c = dataPtrCopy + step;

                blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)(Math.Round(red));

                //Line 1 (TOP)
                //Point to pixel (1,0)
                dataPtrOriginal += nChan;
                dataPtrCopy += nChan;

                for (int i = 1; i < width - 1; i++)
                {
                    a = dataPtrCopy;
                    b = dataPtrCopy + nChan;
                    c = dataPtrCopy + step;

                    blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal += nChan;
                    dataPtrCopy += nChan;
                }
                //Pointing at pixel (width-1, 0)

                //Pixel 2 (TOP-RIGHT)
                a = dataPtrCopy;
                b = dataPtrCopy;
                c = dataPtrCopy + step;

                blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)(Math.Round(red));

                //Line 2 (RIGHT)
                //Point to pixel (width-1, 1)
                dataPtrOriginal += step;
                dataPtrCopy += step;

                for (int i = 1; i < height - 1; i++)
                {
                    a = dataPtrCopy;
                    b = dataPtrCopy;
                    c = dataPtrCopy + step;

                    blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal += step;
                    dataPtrCopy += step;
                }
                //Pointing at pixel (width-1, height-1)

                //Pixel 3 (BOTTOM-RIGHT)
                a = dataPtrCopy;
                b = dataPtrCopy;
                c = dataPtrCopy;

                blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)(Math.Round(red));

                //Line 3 (BOTTOM)
                //Point to pixel (width-2, height-1)
                dataPtrOriginal -= nChan;
                dataPtrCopy -= nChan;

                for (int i = 1; i < width - 1; i++)
                {
                    a = dataPtrCopy;
                    b = dataPtrCopy + nChan;
                    c = dataPtrCopy;

                    blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal -= nChan;
                    dataPtrCopy -= nChan;
                }
                //Pointing at pixel (0, height-1)

                //Pixel 4 (BOTTOM-LEFT)
                a = dataPtrCopy;
                b = dataPtrCopy + nChan;
                c = dataPtrCopy;

                blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                if (blue > 255) blue = 255;
                dataPtrOriginal[0] = (byte)(Math.Round(blue));

                green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                if (green > 255) green = 255;
                dataPtrOriginal[1] = (byte)(Math.Round(green));

                red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                if (red > 255) red = 255;
                dataPtrOriginal[2] = (byte)(Math.Round(red));

                //Line 4 (LEFT)
                //Point to pixel (0, height-2)
                dataPtrOriginal -= step;
                dataPtrCopy -= step;

                for (int i = 1; i < height - 1; i++)
                {
                    a = dataPtrCopy;
                    b = dataPtrCopy + nChan;
                    c = dataPtrCopy + step;

                    blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                    if (blue > 255) blue = 255;
                    dataPtrOriginal[0] = (byte)(Math.Round(blue));

                    green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                    if (green > 255) green = 255;
                    dataPtrOriginal[1] = (byte)(Math.Round(green));

                    red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                    if (red > 255) red = 255;
                    dataPtrOriginal[2] = (byte)(Math.Round(red));

                    dataPtrOriginal -= step;
                    dataPtrCopy -= step;
                }

                //Pointing at pixel (0,0)

                /* --- END BORDERS --- */


                /* --- START CORE --- */

                //Pointing at pixel (1,1)
                dataPtrOriginal += nChan + step;
                dataPtrCopy += nChan + step;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        a = dataPtrCopy;
                        b = dataPtrCopy + nChan;
                        c = dataPtrCopy + step;

                        blue = Math.Abs(a[0] - b[0]) + Math.Abs(a[0] - c[0]);

                        if (blue > 255) blue = 255;
                        dataPtrOriginal[0] = (byte)(Math.Round(blue));

                        green = Math.Abs(a[1] - b[1]) + Math.Abs(a[1] - c[1]);

                        if (green > 255) green = 255;
                        dataPtrOriginal[1] = (byte)(Math.Round(green));

                        red = Math.Abs(a[2] - b[2]) + Math.Abs(a[2] - c[2]);

                        if (red > 255) red = 255;
                        dataPtrOriginal[2] = (byte)(Math.Round(red));

                        //Moving one pixel
                        dataPtrOriginal += nChan;
                        dataPtrCopy += nChan;

                    }

                    //Moving one line
                    dataPtrOriginal += nChan * 2 + padding;
                    dataPtrCopy += nChan * 2 + padding;
                }

                /* --- END CORE --- */
            }
        }

        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                //Original Image
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();

                MIplImage m_m = imgCopy.SmoothMedian(3).MIplImage;
                byte* dataPtrM = (byte*)m_m.imageData.ToPointer();

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;



                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // store in the image
                        dataPtrOriginal[0] = dataPtrM[0];
                        dataPtrOriginal[1] = dataPtrM[1];
                        dataPtrOriginal[2] = dataPtrM[2];

                        dataPtrOriginal += nChan;
                        dataPtrM += nChan;
                    }

                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtrOriginal += padding;
                    dataPtrM += padding;
                }

            }
        }

        public static long[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                long[] hist = new long[256];

                for (int i = 0; i < 256; i++)
                {
                    hist[i] = 0;
                }
                int gray;
                if (nChan == 3) // image in RGB
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            //gray
                            gray = (int)Math.Round((dataPtrOriginal[0] + dataPtrOriginal[1] + dataPtrOriginal[2]) / 3.0);
                            hist[gray]++;

                            // advance the pointer to the next pixel
                            dataPtrOriginal += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal += padding;
                    }
                }

                return hist;
            }
        }

        public static long[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                long[,] hist = new long[3, 256];
                for (int j = 0; j < 3; j++)
                    for (int i = 0; i < 256; i++)
                    {
                        hist[j, i] = 0;
                    }
                if (nChan == 3) // image in RGB
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            //blue
                            hist[0, dataPtrOriginal[0]]++;

                            //green
                            hist[1, dataPtrOriginal[1]]++;

                            //red
                            hist[2, dataPtrOriginal[2]]++;

                            // advance the pointer to the next pixel
                            dataPtrOriginal += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal += padding;
                    }
                }

                return hist;
            }
        }

        public static long[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                long[,] hist = new long[4, 256];
                for (int j = 0; j < 4; j++)
                    for (int i = 0; i < 256; i++)
                    {
                        hist[j, i] = 0;
                    }
                int gray;
                if (nChan == 3) // image in RGB
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            //gray
                            gray = (int)Math.Round((dataPtrOriginal[0] + dataPtrOriginal[1] + dataPtrOriginal[2]) / 3.0);
                            hist[0, gray]++;

                            //blue
                            hist[1, dataPtrOriginal[0]]++;

                            //green
                            hist[2, dataPtrOriginal[1]]++;

                            //red
                            hist[3, dataPtrOriginal[2]]++;

                            // advance the pointer to the next pixel
                            dataPtrOriginal += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal += padding;
                    }
                }

                return hist;
            }
        }

        public static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();

                //Image values
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                int gray;
                if (nChan == 3) // image in RGB
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            //gray
                            gray = (int)Math.Round((dataPtrOriginal[0] + dataPtrOriginal[1] + dataPtrOriginal[2]) / 3.0);

                            //> or >=
                            if (gray > threshold)
                            {
                                dataPtrOriginal[0] = 255;
                                dataPtrOriginal[1] = 255;
                                dataPtrOriginal[2] = 255;
                            }

                            else
                            {
                                dataPtrOriginal[0] = 0;
                                dataPtrOriginal[1] = 0;
                                dataPtrOriginal[2] = 0;
                            }


                            // advance the pointer to the next pixel
                            dataPtrOriginal += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtrOriginal += padding;
                    }
                }
            }
        }

        public static void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
        {
            double pixel_numb = img.Width * img.Height;

            long[] hist = Histogram_Gray(img);

            double[] var = new double[hist.Length];

            double[] p = new double[hist.Length];
            double[] pi = new double[hist.Length];

            double q1 = 0, /*q2 = 1,*/ u1 = 0, u2 = 0;

            for (int i = 0; i < hist.Length; i++)
            {
                p[i] = hist[i] / pixel_numb;
                pi[i] = i * p[i];
                u2 += pi[i];

            }

            for (int i = 0; i < hist.Length; i++)
            {

                q1 += p[i];
                //q2 -= p[i];

                u1 += pi[i];
                u2 -= pi[i];

                if (q1 == 0 || q1 == 1)
                    var[i] = q1 * (1.0 - q1) * Math.Pow((u1 - u2), 2);
                else
                    var[i] = q1 * (1.0 - q1) * Math.Pow((u1 / q1 - u2 / (1.0 - q1)), 2);
            }

            ConvertToBW(img, Array.IndexOf(var, var.Max()));
        }



        public static Image<Bgr, byte> puzzle(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)
        {
            var puzzle = new Puzzle(img);
            puzzle.GetPiecesPosition(out Pieces_positions, out Pieces_angle);
            var finalImage = puzzle.GetFinalImage();

            /*
            Console.WriteLine("--------Image START--------");
            Pieces_positions.ForEach((p) => {
                Console.WriteLine("----PIECE_START----");
                Console.WriteLine("X TOP: " + p[0]);
                Console.WriteLine("Y TOP: " + p[1]);
                Console.WriteLine("X BOTTOM: " + p[2]);
                Console.WriteLine("Y BOTTOM: " + p[3]);
                Console.WriteLine("------PIECE_END----");
            });
            Console.WriteLine("--------Image END---------");
            */

            return finalImage;
        }

        public static void Rotation_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                // get the image pointer
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int widthStep = m.widthStep;
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)

                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                // aux variables
                double halfHeight = height / 2.0;
                double halfWidth = width / 2.0;

                double cos = Math.Cos(angle);
                double sin = Math.Sin(angle);

                double xOrig;
                double yOrig;

                byte* copy_from;

                double aux_x;
                double aux_y;

                // Bilinear Interpolation aux variables
                double yOffset, yComplement;
                double xOffset, xComplement;

                int iMin, iMax, jMin, jMax;

                byte[,] colorsPerNeighbor = new byte[4, 3];

                double[,] tmpValues = new double[3, 2];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        aux_x = x - halfWidth;
                        aux_y = halfHeight - y;

                        xOrig = aux_x * cos - aux_y * sin + halfWidth;
                        yOrig = halfHeight - aux_x * sin - aux_y * cos;

                        // Pixels that fall outside of the original image are painted black
                        if (yOrig >= 0 && yOrig < height - 1 && xOrig >= 0 && xOrig < width - 1)
                        {
                            // Get the rows and columns of the neighbors to the original position
                            iMin = (int)yOrig;
                            iMax = (int)Math.Ceiling(yOrig);
                            jMin = (int)xOrig;
                            jMax = (int)Math.Ceiling(xOrig);

                            yOffset = yOrig - iMin;
                            xOffset = xOrig - jMin;

                            yComplement = 1 - yOffset;
                            xComplement = 1 - xOffset;

                            // Upper Left Neighbor
                            copy_from = dataPtr_copy + iMin * widthStep + jMin * nChan;

                            colorsPerNeighbor[0, 0] = copy_from[0];
                            colorsPerNeighbor[0, 1] = copy_from[1];
                            colorsPerNeighbor[0, 2] = copy_from[2];

                            // Upper Right Neighbor
                            copy_from += nChan;

                            colorsPerNeighbor[1, 0] = copy_from[0];
                            colorsPerNeighbor[1, 1] = copy_from[1];
                            colorsPerNeighbor[1, 2] = copy_from[2];

                            // Lower Right Neighbor
                            copy_from += widthStep;

                            colorsPerNeighbor[2, 0] = copy_from[0];
                            colorsPerNeighbor[2, 1] = copy_from[1];
                            colorsPerNeighbor[2, 2] = copy_from[2];

                            // Lower Left Neighbor
                            copy_from -= nChan;

                            colorsPerNeighbor[3, 0] = copy_from[0];
                            colorsPerNeighbor[3, 1] = copy_from[1];
                            colorsPerNeighbor[3, 2] = copy_from[2];

                            // Get temporary values for upper x interpolation for each color component
                            tmpValues[0, 0] = xComplement * colorsPerNeighbor[0, 0] + xOffset * colorsPerNeighbor[1, 0];
                            tmpValues[1, 0] = xComplement * colorsPerNeighbor[0, 1] + xOffset * colorsPerNeighbor[1, 1];
                            tmpValues[2, 0] = xComplement * colorsPerNeighbor[0, 2] + xOffset * colorsPerNeighbor[1, 2];

                            // Get temporary vxComplementer x interpolation for each color component
                            tmpValues[0, 1] = xComplement * colorsPerNeighbor[3, 0] + xOffset * colorsPerNeighbor[2, 0];
                            tmpValues[1, 1] = xComplement * colorsPerNeighbor[3, 1] + xOffset * colorsPerNeighbor[2, 1];
                            tmpValues[2, 1] = xComplement * colorsPerNeighbor[3, 2] + xOffset * colorsPerNeighbor[2, 2];

                            // Make final y interpolation
                            dataPtr[0] = (byte)Math.Round(yComplement * tmpValues[0, 0] + yOffset * tmpValues[0, 1]);
                            dataPtr[1] = (byte)Math.Round(yComplement * tmpValues[1, 0] + yOffset * tmpValues[1, 1]);
                            dataPtr[2] = (byte)Math.Round(yComplement * tmpValues[2, 0] + yOffset * tmpValues[2, 1]);
                        }
                        else
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        dataPtr += nChan;
                    }
                    dataPtr += padding;
                }
            }
        }

        public static void Scale_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                // get the image pointer
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int widthStep = m.widthStep;

                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                byte* copy_from;
                byte* copy_to;

                double dy_diff;
                double dx_diff;

                // Bilinear Interpolation aux variables
                double yOffset, yComplement;
                double xOffset, xComplement;

                int iMin, iMax, jMin, jMax;

                byte[,] colorsPerNeighbor = new byte[4, 3];

                double[,] tmpValues = new double[3, 2];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel address
                        dx_diff = x / scaleFactor;
                        dy_diff = y / scaleFactor;

                        copy_to = dataPtr + y * widthStep + x * nChan;

                        if (dy_diff >= 0 && dy_diff < height && dx_diff >= 0 && dx_diff < width)
                        {
                            // Get the rows and columns of the neighbors to the original position
                            iMin = (int)dy_diff;
                            iMax = (int)Math.Ceiling(dy_diff);
                            jMin = (int)dx_diff;
                            jMax = (int)Math.Ceiling(dx_diff);

                            yOffset = dy_diff - iMin;
                            xOffset = dx_diff - jMin;

                            yComplement = 1 - yOffset;
                            xComplement = 1 - xOffset;

                            // Upper Left Neighbor
                            copy_from = dataPtr_copy + iMin * widthStep + jMin * nChan;

                            colorsPerNeighbor[0, 0] = copy_from[0];
                            colorsPerNeighbor[0, 1] = copy_from[1];
                            colorsPerNeighbor[0, 2] = copy_from[2];

                            // Upper Right Neighbor
                            copy_from += nChan;

                            colorsPerNeighbor[1, 0] = copy_from[0];
                            colorsPerNeighbor[1, 1] = copy_from[1];
                            colorsPerNeighbor[1, 2] = copy_from[2];

                            // Lower Right Neighbor
                            copy_from += widthStep;

                            colorsPerNeighbor[2, 0] = copy_from[0];
                            colorsPerNeighbor[2, 1] = copy_from[1];
                            colorsPerNeighbor[2, 2] = copy_from[2];

                            // Lower Left Neighbor
                            copy_from -= nChan;

                            colorsPerNeighbor[3, 0] = copy_from[0];
                            colorsPerNeighbor[3, 1] = copy_from[1];
                            colorsPerNeighbor[3, 2] = copy_from[2];

                            // Get temporary values for upper x interpolation for each color component
                            tmpValues[0, 0] = xComplement * colorsPerNeighbor[0, 0] + xOffset * colorsPerNeighbor[1, 0];
                            tmpValues[1, 0] = xComplement * colorsPerNeighbor[0, 1] + xOffset * colorsPerNeighbor[1, 1];
                            tmpValues[2, 0] = xComplement * colorsPerNeighbor[0, 2] + xOffset * colorsPerNeighbor[1, 2];

                            // Get temporary vxComplementer x interpolation for each color component
                            tmpValues[0, 1] = xComplement * colorsPerNeighbor[3, 0] + xOffset * colorsPerNeighbor[2, 0];
                            tmpValues[1, 1] = xComplement * colorsPerNeighbor[3, 1] + xOffset * colorsPerNeighbor[2, 1];
                            tmpValues[2, 1] = xComplement * colorsPerNeighbor[3, 2] + xOffset * colorsPerNeighbor[2, 2];

                            // Make final y interpolation
                            copy_to[0] = (byte)Math.Round(yComplement * tmpValues[0, 0] + yOffset * tmpValues[0, 1]);
                            copy_to[1] = (byte)Math.Round(yComplement * tmpValues[1, 0] + yOffset * tmpValues[1, 1]);
                            copy_to[2] = (byte)Math.Round(yComplement * tmpValues[2, 0] + yOffset * tmpValues[2, 1]);
                        }
                        else
                        {
                            copy_to[0] = 0;
                            copy_to[1] = 0;
                            copy_to[2] = 0;
                        }

                    }
                }
            }
        }

        public static void Scale_point_xy_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                // get the image pointer
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int widthStep = m.widthStep;

                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                int halfHeight = height / 2;    // needs to be int division to match openCV results
                int halfWidth = width / 2;    // needs to be int division to match openCV results

                byte* copy_from;
                byte* copy_to;

                double dy_diff;
                double dx_diff;

                // Bilinear Interpolation aux variables
                double yOffset, yComplement;
                double xOffset, xComplement;

                int iMin, iMax, jMin, jMax;

                byte[,] colorsPerNeighbor = new byte[4, 3];

                double[,] tmpValues = new double[3, 2];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // get pixel address
                        dx_diff = (x - halfWidth) / scaleFactor + centerX;
                        dy_diff = (y - halfHeight) / scaleFactor + centerY;

                        copy_to = dataPtr + y * widthStep + x * nChan;

                        if (dy_diff >= 0 && dy_diff < height && dx_diff >= 0 && dx_diff < width)
                        {
                            // Get the rows and columns of the neighbors to the original position
                            iMin = (int)dy_diff;
                            iMax = (int)Math.Ceiling(dy_diff);
                            jMin = (int)dx_diff;
                            jMax = (int)Math.Ceiling(dx_diff);

                            yOffset = dy_diff - iMin;
                            xOffset = dx_diff - jMin;

                            xComplement = 1 - xOffset;
                            yComplement = 1 - yOffset;

                            // Upper Left Neighbor
                            copy_from = dataPtr_copy + iMin * widthStep + jMin * nChan;

                            colorsPerNeighbor[0, 0] = copy_from[0];
                            colorsPerNeighbor[0, 1] = copy_from[1];
                            colorsPerNeighbor[0, 2] = copy_from[2];

                            // Upper Right Neighbor
                            copy_from += nChan;

                            colorsPerNeighbor[1, 0] = copy_from[0];
                            colorsPerNeighbor[1, 1] = copy_from[1];
                            colorsPerNeighbor[1, 2] = copy_from[2];

                            // Lower Right Neighbor
                            copy_from += widthStep;

                            colorsPerNeighbor[2, 0] = copy_from[0];
                            colorsPerNeighbor[2, 1] = copy_from[1];
                            colorsPerNeighbor[2, 2] = copy_from[2];

                            // Lower Left Neighbor
                            copy_from -= nChan;

                            colorsPerNeighbor[3, 0] = copy_from[0];
                            colorsPerNeighbor[3, 1] = copy_from[1];
                            colorsPerNeighbor[3, 2] = copy_from[2];

                            // Get temporary values for upper x interpolation for each color component
                            tmpValues[0, 0] = xComplement * colorsPerNeighbor[0, 0] + xOffset * colorsPerNeighbor[1, 0];
                            tmpValues[1, 0] = xComplement * colorsPerNeighbor[0, 1] + xOffset * colorsPerNeighbor[1, 1];
                            tmpValues[2, 0] = xComplement * colorsPerNeighbor[0, 2] + xOffset * colorsPerNeighbor[1, 2];

                            // Get temporary values for lower x interpolation for each color component
                            tmpValues[0, 1] = xComplement * colorsPerNeighbor[3, 0] + xOffset * colorsPerNeighbor[2, 0];
                            tmpValues[1, 1] = xComplement * colorsPerNeighbor[3, 1] + xOffset * colorsPerNeighbor[2, 1];
                            tmpValues[2, 1] = xComplement * colorsPerNeighbor[3, 2] + xOffset * colorsPerNeighbor[2, 2];

                            // Make final y interpolation
                            copy_to[0] = (byte)Math.Round(yComplement * tmpValues[0, 0] + yOffset * tmpValues[0, 1]);
                            copy_to[1] = (byte)Math.Round(yComplement * tmpValues[1, 0] + yOffset * tmpValues[1, 1]);
                            copy_to[2] = (byte)Math.Round(yComplement * tmpValues[2, 0] + yOffset * tmpValues[2, 1]);
                        }
                        else
                        {
                            copy_to[0] = 0;
                            copy_to[1] = 0;
                            copy_to[2] = 0;
                        }

                    }
                }
            }
        }


        public static void Mean_solutionC(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int size)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                byte* dataCopyPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtr = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the destination image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int step = m.widthStep;
                int x, y;

                //ulong[,] integral_img = new ulong[height, width];

                //Image<Bgr, ulong> integral_img = new Image<Bgr, ulong>(width, height);
                //ulong* integralPtr = (ulong*)integral_img.MIplImage.imageData.ToPointer();
                //int integral_step = integral_img.MIplImage.widthStep;
                //int integral_padding = integral_step - nChan * width;

                ulong[,,] integral_img = new ulong[height, width, 3];
                int k = ((size - 1) / 2);

                int i, j;

                // Compute Integral Image
                //Top Line
                integral_img[0, 0, 0] = dataCopyPtr[0];
                integral_img[0, 0, 1] = dataCopyPtr[1];
                integral_img[0, 0, 2] = dataCopyPtr[2];
                dataCopyPtr += nChan;

                for (i = 1; i < width; i++)
                {
                    integral_img[0, i, 0] = dataCopyPtr[0] + integral_img[0, i - 1, 0];
                    integral_img[0, i, 1] = dataCopyPtr[1] + integral_img[0, i - 1, 1];
                    integral_img[0, i, 2] = dataCopyPtr[2] + integral_img[0, i - 1, 2];
                    dataCopyPtr += nChan;
                }

                dataCopyPtr += padding;
                for (j = 1; j < height; j++)
                {
                    integral_img[j, 0, 0] = dataCopyPtr[0] + integral_img[j - 1, 0, 0];
                    integral_img[j, 0, 1] = dataCopyPtr[1] + integral_img[j - 1, 0, 1];
                    integral_img[j, 0, 2] = dataCopyPtr[2] + integral_img[j - 1, 0, 2];
                    dataCopyPtr += step;
                }

                dataCopyPtr = (byte*)m.imageData.ToPointer() + nChan + step;
                for (j = 1; j < height; j++)
                {
                    for (i = 1; i < width; i++)
                    {
                        integral_img[j, i, 0] = dataCopyPtr[0] + integral_img[j - 1, i, 0] - integral_img[j - 1, i - 1, 0] + integral_img[j, i - 1, 0];
                        integral_img[j, i, 1] = dataCopyPtr[1] + integral_img[j - 1, i, 1] - integral_img[j - 1, i - 1, 1] + integral_img[j, i - 1, 1];
                        integral_img[j, i, 2] = dataCopyPtr[2] + integral_img[j - 1, i, 2] - integral_img[j - 1, i - 1, 2] + integral_img[j, i - 1, 2];

                        dataCopyPtr += nChan;
                    }
                    dataCopyPtr += nChan + padding;
                }

                // Compute Core Mean
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (k + 1) * (nChan + step);
                for (y = k + 1; y < height - (k + 1); y++)
                {
                    for (x = k + 1; x < width - (k + 1); x++)
                    {
                        dataPtr[0] = (byte)Math.Round((integral_img[y + k, x + k, 0] - integral_img[y - k - 1, x + k, 0] + integral_img[y - k - 1, x - k - 1, 0] - integral_img[y + k, x - k - 1, 0]) / (double)(size * size));
                        dataPtr[1] = (byte)Math.Round((integral_img[y + k, x + k, 1] - integral_img[y - k - 1, x + k, 1] + integral_img[y - k - 1, x - k - 1, 1] - integral_img[y + k, x - k - 1, 1]) / (double)(size * size));
                        dataPtr[2] = (byte)Math.Round((integral_img[y + k, x + k, 2] - integral_img[y - k - 1, x + k, 2] + integral_img[y - k - 1, x - k - 1, 2] - integral_img[y + k, x - k - 1, 2]) / (double)(size * size));

                        dataPtr += nChan;
                    }
                    dataPtr += padding + 2 * (k + 1) * nChan;
                }

                ulong area = 0;

                // Compute the mean for the Top Left Corner
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer();
                for (j = 0; j < k + 1; j++)
                {
                    for (i = 0; i < k + 1; i++)
                    {
                        area = integral_img[j + k, i + k, 0] + integral_img[0, i + k, 0] * ((ulong)(k - j)) + integral_img[0, 0, 0] * ((ulong)((k - i) * (k - j))) + integral_img[j + k, 0, 0] * ((ulong)(k - i));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, i + k, 1] + integral_img[0, i + k, 1] * ((ulong)(k - j)) + integral_img[0, 0, 1] * ((ulong)((k - i) * (k - j))) + integral_img[j + k, 0, 1] * ((ulong)(k - i));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, i + k, 2] + integral_img[0, i + k, 2] * ((ulong)(k - j)) + integral_img[0, 0, 2] * ((ulong)((k - i) * (k - j))) + integral_img[j + k, 0, 2] * ((ulong)(k - i));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));

                        dataPtr += nChan;
                    }
                    dataPtr += step - (k + 1) * nChan;
                }

                // Compute the mean for the Top
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (k + 1) * nChan;
                for (j = 0; j < k + 1; j++)
                {
                    for (i = k + 1; i < width - (k + 1); i++)
                    {
                        area = integral_img[j + k, i + k, 0] - integral_img[j + k, i - k - 1, 0] + (integral_img[0, i + k, 0] - integral_img[0, i - k - 1, 0]) * ((ulong)(k - j));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, i + k, 1] - integral_img[j + k, i - k - 1, 1] + (integral_img[0, i + k, 1] - integral_img[0, i - k - 1, 1]) * ((ulong)(k - j));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, i + k, 2] - integral_img[j + k, i - k - 1, 2] + (integral_img[0, i + k, 2] - integral_img[0, i - k - 1, 2]) * ((ulong)(k - j));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));

                        dataPtr += nChan;
                    }
                    dataPtr += (2 * k + 2) * nChan + padding;
                }

                // Compute the mean for the Top Right Corner
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (width - (k + 1)) * nChan;
                dataCopyPtr = (byte*)m.imageData.ToPointer() + (width - 1) * nChan; // Pointer to the original image
                for (j = 0; j < k + 1; j++)
                {
                    for (i = width - (k + 1); i < width; i++)
                    {
                        area = integral_img[j + k, width - 1, 0] - integral_img[j + k, i - k - 1, 0] 
                            + (integral_img[j + k, width - 1, 0] - integral_img[j + k, width - 2, 0]) * ((ulong)(i + k - (width - 1))) 
                            + (integral_img[0, width - 1, 0] - integral_img[0, i - k - 1, 0]) * ((ulong)(k - j)) 
                            + dataCopyPtr[0] * ((ulong)((i + k - (width - 1)) * (k - j)));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, width - 1, 1] - integral_img[j + k, i - k - 1, 1]
                            + (integral_img[j + k, width - 1, 1] - integral_img[j + k, width - 2, 1]) * ((ulong)(i + k - (width - 1)))
                            + (integral_img[0, width - 1, 1] - integral_img[0, i - k - 1, 1]) * ((ulong)(k - j))
                            + dataCopyPtr[1] * ((ulong)((i + k - (width - 1)) * (k - j)));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, width - 1, 2] - integral_img[j + k, i - k - 1, 2]
                            + (integral_img[j + k, width - 1, 2] - integral_img[j + k, width - 2, 2]) * ((ulong)(i + k - (width - 1)))
                            + (integral_img[0, width - 1, 2] - integral_img[0, i - k - 1, 2]) * ((ulong)(k - j))
                            + dataCopyPtr[2] * ((ulong)((i + k - (width - 1)) * (k - j)));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));

                        dataPtr += nChan;
                    }
                    dataPtr += step - (k + 1) * nChan;
                }

                // Compute the mean for the Left margin
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (k + 1) * step;
               
                for (j = k + 1; j < height - (k + 1); j++)
                {
                    for (i = 0; i < k + 1; i++)
                    {
                        area = integral_img[j + k, i + k, 0] - integral_img[j - k - 1, i + k, 0] + (integral_img[j + k, 0, 0] - integral_img[j - k - 1, 0, 0]) * ((ulong)(k - i));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, i + k, 1] - integral_img[j - k - 1, i + k, 1] + (integral_img[j + k, 0, 1] - integral_img[j - k - 1, 0, 1]) * ((ulong)(k - i));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, i + k, 2] - integral_img[j - k - 1, i + k, 2] + (integral_img[j + k, 0, 2] - integral_img[j - k - 1, 0, 2]) * ((ulong)(k - i));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));


                        dataPtr += nChan;
                    }
                    dataPtr += step - (k + 1) * nChan;
                }

                // Compute the mean for the Bottom Left Corner
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (height - (k + 1)) *step;
                dataCopyPtr = (byte*)m.imageData.ToPointer() + (height - 1) * step; // Pointer to the original image
                for (j = height - (k+1); j < height; j++)
                {
                    for (i = 0; i < k + 1; i++)
                    {
                        area = integral_img[height - 1, i + k, 0] - integral_img[j - k - 1, i + k, 0] 
                            + (integral_img[height - 1, 0, 0] - integral_img[j - k - 1, 0, 0]) * ((ulong)(k - i)) 
                            + dataCopyPtr[0] * ((ulong)((k - i)*(j + k - (height - 1))));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[height - 1, i + k, 1] - integral_img[j - k - 1, i + k, 1] 
                            + (integral_img[height - 1, 0, 1] - integral_img[j - k - 1, 0, 1]) * ((ulong)(k - i)) 
                            + dataCopyPtr[1] * ((ulong)((k - i) * (j + k - (height - 1))));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[height - 1, i + k, 2] - integral_img[j - k - 1, i + k, 2] 
                            + (integral_img[height - 1, 0, 2] - integral_img[j - k - 1, 0, 2]) * ((ulong)(k - i)) 
                            + dataCopyPtr[2] * ((ulong)((k - i) * (j + k - (height - 1))));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));

                        dataPtr += nChan;
                    }
                    dataPtr += step - (k + 1) * nChan;
                }

                // Compute the mean for the Bottom
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (k + 1) * nChan + (height - (k + 1)) * step;
                for (j = height - (k + 1); j < height; j++)
                {
                    for (i = k + 1; i < width - (k + 1); i++)
                    {
                        area = integral_img[height-1, i + k, 0] - integral_img[j - k - 1, i + k, 0] + integral_img[j - k - 1, i - k - 1, 0] - integral_img[height - 1, i - k - 1, 0] + (integral_img[height - 1, i + k, 0] - integral_img[height - 1, i - k - 1, 0]) * ((ulong)(j + k - (height - 1)));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[height - 1, i + k, 1] - integral_img[j - k - 1, i + k, 1] + integral_img[j - k - 1, i - k - 1, 1] - integral_img[height - 1, i - k - 1, 1] + (integral_img[height - 1, i + k, 1] - integral_img[height - 1, i - k - 1, 1]) * ((ulong)(j + k - (height - 1)));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[height - 1, i + k, 2] - integral_img[j - k - 1, i + k, 2] + integral_img[j - k - 1, i - k - 1, 2] - integral_img[height - 1, i - k - 1, 2] + (integral_img[height - 1, i + k, 2] - integral_img[height - 1, i - k - 1, 2]) * ((ulong)(j + k - (height - 1)));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));

                        dataPtr += nChan;
                    }
                    dataPtr += (2 * k + 2) * nChan + padding;
                }

                // Compute the mean for the Bottom Right Corner
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (width - (k + 1)) * nChan + (height - (k + 1)) * step;
                dataCopyPtr = (byte*)m.imageData.ToPointer() + (width - (k + 1)) * nChan + (height - (k + 1)) * step; // Pointer to the original image
                for (j = height - (k + 1); j < height; j++)
                {
                    for (i = width - (k + 1); i < width; i++)
                    {
                        area = integral_img[height - 1, width - 1, 0] - integral_img[j - k - 1, width - 1, 0] + integral_img[j - k - 1, i - k - 1, 0] - integral_img[height - 1, i - k - 1, 0]
                            + (integral_img[height - 1, width - 1, 0] - integral_img[height - 1, i - k - 1, 0]) * ((ulong)(j + k - (height - 1))) 
                            + (integral_img[height - 1, width - 1, 0] - integral_img[height - 1, width - 2, 0] - integral_img[j - k - 1, width - 1, 0]) * ((ulong)(i + k - (width - 1)))
                            + dataCopyPtr[0] * ((ulong)((i + k - (width - 1)) * (j + k - (height - 1))));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[height - 1, width - 1, 1] - integral_img[j - k - 1, width - 1, 1] + integral_img[j - k - 1, i - k - 1, 1] - integral_img[height - 1, i - k - 1, 1]
                            + (integral_img[height - 1, width - 1, 1] - integral_img[height - 1, i - k - 1, 1]) * ((ulong)(j + k - (height - 1)))
                            + (integral_img[height - 1, width - 1, 1] - integral_img[height - 1, width - 2, 1] - integral_img[j - k - 1, width - 1, 1]) * ((ulong)(i + k - (width - 1)))
                            + dataCopyPtr[1] * ((ulong)((i + k - (width - 1)) * (j + k - (height - 1))));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[height - 1, width - 1, 2] - integral_img[j - k - 1, width - 1, 2] + integral_img[j - k - 1, i - k - 1, 2] - integral_img[height - 1, i - k - 1, 2]
                            + (integral_img[height - 1, width - 1, 2] - integral_img[height - 1, i - k - 1, 2]) * ((ulong)(j + k - (height - 1)))
                            + (integral_img[height - 1, width - 1, 2] - integral_img[height - 1, width - 2, 2] - integral_img[j - k - 1, width - 1, 2]) * ((ulong)(i + k - (width - 1)))
                            + dataCopyPtr[2] * ((ulong)((i + k - (width - 1)) * (j + k - (height - 1))));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));

                        dataPtr += nChan;
                    }
                    dataPtr += step - (k + 1) * nChan;
                }

                // Compute the mean for the Right margin
                dataPtr = (byte*)img.MIplImage.imageData.ToPointer() + (k + 1) * step + (width - (k + 1)) * nChan;
                for (j = k + 1; j < height - (k + 1); j++)
                {
                    for (i = width - (k + 1); i < width; i++)
                    {
                        area = integral_img[j + k, width - 1, 0] - integral_img[j - k - 1, width - 1, 0] + integral_img[j - k - 1, i - k - 1, 0] - integral_img[j + k, i - k - 1, 0]
                             + (integral_img[j + k, width - 1, 0] - integral_img[j + k, width - 2, 0] - integral_img[j - k - 1, width - 1, 0] + integral_img[j - k - 1, width - 2, 0]) * ((ulong)(i + k - (width - 1)));
                        dataPtr[0] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, width - 1, 1] - integral_img[j - k - 1, width - 1, 1] + integral_img[j - k - 1, i - k - 1, 1] - integral_img[j + k, i - k - 1, 1]
                             + (integral_img[j + k, width - 1, 1] - integral_img[j + k, width - 2, 1] - integral_img[j - k - 1, width - 1, 1] + integral_img[j - k - 1, width - 2, 1]) * ((ulong)(i + k - (width - 1)));
                        dataPtr[1] = (byte)Math.Round(area / (double)(size * size));

                        area = integral_img[j + k, width - 1, 2] - integral_img[j - k - 1, width - 1, 2] + integral_img[j - k - 1, i - k - 1, 2] - integral_img[j + k, i - k - 1, 2]
                             + (integral_img[j + k, width - 1, 2] - integral_img[j + k, width - 2, 2] - integral_img[j - k - 1, width - 1, 2] + integral_img[j - k - 1, width - 2, 2]) * ((ulong)(i + k - (width - 1)));
                        dataPtr[2] = (byte)Math.Round(area / (double)(size * size));


                        dataPtr += nChan;
                    }
                    dataPtr += step - (k + 1) * nChan;
                }
            }
        }

    }
}
