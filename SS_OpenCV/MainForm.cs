using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SS_OpenCV
{
    public partial class MainForm : Form
    {
        Image<Bgr, Byte> img = null; // working image
        Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        string title_bak = "";

        // Global Mouse variables
        int mouseX, mouseY;
        bool mouseFlag = false;

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
        }

        /// <summary>
        /// Specify program behaviour in case of an image mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageViewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (mouseFlag)
            {
                mouseX = e.X;
                mouseY = e.Y;

                mouseFlag = false;
            }
        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                imgUndo = img.Copy();
                ImageViewer.Image = img.Bitmap;
                ImageViewer.Refresh();
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null) // verify if the image is already opened
                return; 
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call automated image processing check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();
        }

        /// <summary>
        /// Call image convertion to gray scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToGray(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            Histogram hist = new Histogram(ImageClass.Histogram_All(img));
            hist.ShowDialog();

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call image convertion to red channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.RedChannel(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Change image according to the specified contrast and brightness
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void brightContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();
            
            DoubleInputBox form = new DoubleInputBox("Brightness & Contrast", "Brightness:", "Contrast:");
            form.ValueTextBox1.Text = "int from -255 to 255";
            form.ValueTextBox2.Text = "float from 0 to 3";
            form.ShowDialog();

            bool numericBrightness = int.TryParse(form.ValueTextBox1.Text, out int brightness);
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericContrast = float.TryParse(form.ValueTextBox2.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float contrast);

            // Brightness and Contrast input values are restricted accordingly: -255 <= brightness <= 255 ; 0 <= contrast <= 3
            if (!(numericBrightness && numericContrast && brightness >= -255 && brightness <= 255 && contrast >= 0 && contrast <= 3))
                return;

            ImageClass.BrightContrast(img, brightness , contrast);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Move image according to the specified x and y values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void translationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            DoubleInputBox form = new DoubleInputBox("Translation", "X:", "Y:");
            form.ValueTextBox1.Text = "pos/neg int";
            form.ValueTextBox2.Text = "pos/neg int";
            form.ShowDialog();
            bool numericX = int.TryParse(form.ValueTextBox1.Text, out int x);
            bool numericY = int.TryParse(form.ValueTextBox2.Text, out int y);

            if (!(numericX && numericY))
                return;

            ImageClass.Translation(img, img.Copy(), x, y);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Rotate image according to the specified angle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleRotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Angle (Deg)");
            form.ValueTextBox.Text = "pos/neg float";
            form.ShowDialog();
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericAngle = float.TryParse(form.ValueTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float angle);

            if (!numericAngle)
                return;

            angle = (float) ((Math.PI / 180.0) * angle);

            ImageClass.Rotation(img, img.Copy(), angle);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Rotate image according to the specified angle, applying bilinear interpolation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bilinearRotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Angle (Deg)");
            form.ValueTextBox.Text = "pos/neg float";
            form.ShowDialog();
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericAngle = float.TryParse(form.ValueTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float angle);

            if (!numericAngle)
                return;

            angle = (float)((Math.PI / 180.0) * angle);

            ImageClass.Rotation_Bilinear(img, img.Copy(), angle);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Resize image according to the specified scale factor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Scale Factor");
            form.ValueTextBox.Text = "positive float";
            form.ShowDialog();
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericScaleFactor = float.TryParse(form.ValueTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float scaleFactor);

            if (!numericScaleFactor)
                return;

            ImageClass.Scale(img, img.Copy(), scaleFactor);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Resize image according to the specified scale factor, applying bilinear interpolation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bilinearScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Scale Factor");
            form.ValueTextBox.Text = "positive float";
            form.ShowDialog();
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericScaleFactor = float.TryParse(form.ValueTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float scaleFactor);

            if (!numericScaleFactor)
                return;

            ImageClass.Scale_Bilinear(img, img.Copy(), scaleFactor);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Resize image according to the specified scale factor and point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleScalePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Scale Factor");
            form.ValueTextBox.Text = "positive float";
            form.ShowDialog();
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericScaleFactor = float.TryParse(form.ValueTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float scaleFactor);

            if (!numericScaleFactor)
                return;

            mouseFlag = true;
            while (mouseFlag)
                Application.DoEvents();

            ImageClass.Scale_point_xy(img, img.Copy(), scaleFactor, mouseX, mouseY);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Median(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }


        private void histogramBWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            HistogramGray hist = new HistogramGray(ImageClass.Histogram_Gray(img));
            hist.ShowDialog();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void blakcAndWhiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Threshold");
            form.ValueTextBox.Text = "positive integer";
            form.ShowDialog();
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericScaleFactor = int.TryParse(form.ValueTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out int threshold);

            if (!numericScaleFactor)
                return;

            ImageClass.ConvertToBW(img, threshold);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void blackAndWhiteOtsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToBW_Otsu(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void histogramRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            HistogramRGB hist = new HistogramRGB(ImageClass.Histogram_RGB(img));
            hist.ShowDialog();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void puzzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();
            Image<Bgr, byte> FinalImage = ImageClass.puzzle(img, img.Copy(), out List<int[]> Pieces_positions, out List<int> Pieces_angle, -1);

            ImageViewer.Image = FinalImage.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void robertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Roberts(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Resize image according to the specified scale factor and point, applying bilinear interpolation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bilinearScalePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Scale Factor");
            form.ValueTextBox.Text = "positive float";
            form.ShowDialog();
            // Have to specify any number format and invariant culture to accept numbers separated by dots or commas
            bool numericScaleFactor = float.TryParse(form.ValueTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float scaleFactor);

            if (!numericScaleFactor)
                return;

            mouseFlag = true;
            while (mouseFlag)
                Application.DoEvents();

            ImageClass.Scale_point_xy_Bilinear(img, img.Copy(), scaleFactor, mouseX, mouseY);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }
    }
}