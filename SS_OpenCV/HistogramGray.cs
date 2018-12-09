using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

namespace SS_OpenCV
{
    public partial class HistogramGray : Form
    {
        public HistogramGray(int[] array)
        {
            InitializeComponent();
           
            GraphPane myPane = zedGraphControl2.GraphPane;
            myPane.Title.Text = "Histogram";
            myPane.XAxis.Title.Text = "Intensidade";
            myPane.YAxis.Title.Text = "Numero Pixeis";
            PointPairList gray = new PointPairList();

            myPane.XAxis.Scale.Max = 256;
            
            for (int i = 0; i < 256; i++)
            {
                gray.Add(i, array[i]);
                
            }
            LineItem grayCurve = myPane.AddCurve("GRAY", gray, Color.Gray, SymbolType.None);

            zedGraphControl2.AxisChange();

        }

    }
}
