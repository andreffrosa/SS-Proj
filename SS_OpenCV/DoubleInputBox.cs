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
    public partial class DoubleInputBox : Form
    {
        public DoubleInputBox()
        {
            InitializeComponent();
        }

        public DoubleInputBox(string _title)
        {
            InitializeComponent();

            this.Text = _title;

        }

        public DoubleInputBox(string _title, string label1, string label2)
        {
            InitializeComponent();

            this.ValueLabel1.Text = label1;
            this.ValueLabel2.Text = label2;

            this.Text = _title;

        }
    }
}
