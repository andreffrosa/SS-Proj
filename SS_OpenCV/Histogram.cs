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
    public partial class Histogram : Form
    {
        public Histogram(int[,] array)
        {
            InitializeComponent();
           
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Histogram";
            myPane.XAxis.Title.Text = "Intensidade";
            myPane.YAxis.Title.Text = "Numero Pixeis";
            PointPairList blue = new PointPairList();
            PointPairList green = new PointPairList();
            PointPairList red = new PointPairList();
            PointPairList gray = new PointPairList();

            myPane.XAxis.Scale.Max = 256;
            
            for (int i = 0; i < 256; i++)
            {
                gray.Add(i, array[0, i]);
                blue.Add(i, array[1, i]);
                green.Add(i, array[2, i]);
                red.Add(i, array[3, i]);
                
            }

            LineItem blueCurve = myPane.AddCurve("BLUE",  blue, Color.Blue, SymbolType.None);
            LineItem greenCurve = myPane.AddCurve("GREEN", green, Color.Green, SymbolType.None);
            LineItem redCurve = myPane.AddCurve("RED", red, Color.Red, SymbolType.None);
            LineItem grayCurve = myPane.AddCurve("GRAY", gray, Color.Gray, SymbolType.None);

            zedGraphControl1.AxisChange();

        }

    }
}
