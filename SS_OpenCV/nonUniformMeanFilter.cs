using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SS_OpenCV
{
    public partial class nonUniformMeanFilter : Form
    {
        private float[,] matrix;

        public nonUniformMeanFilter(out float[,] matrix)
        {
            InitializeComponent();

            this.matrix = new float[3, 3];
            matrix = this.matrix;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            matrix[0, 0] = float.Parse(this.textBox1.Text);
            matrix[0, 1] = float.Parse(this.textBox2.Text);
            matrix[0, 2] = float.Parse(this.textBox3.Text);
            matrix[1, 0] = float.Parse(this.textBox4.Text);
            matrix[1, 1] = float.Parse(this.textBox5.Text);
            matrix[1, 2] = float.Parse(this.textBox6.Text);
            matrix[2, 0] = float.Parse(this.textBox7.Text);
            matrix[2, 1] = float.Parse(this.textBox8.Text);
            matrix[2, 2] = float.Parse(this.textBox9.Text);

            this.Close();
        }

    }
}
