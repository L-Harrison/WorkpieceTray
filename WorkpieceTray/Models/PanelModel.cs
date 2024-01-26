using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WorkpieceTray.Controls;

namespace WorkpieceTray.Models
{
    public class PanelModel
    {
        public List<Cell> Cells { get; set; } = new();
        public CPanel Panel { get; set; }

        public int PanelIndex { get; set; }
        public double PanelX { get; set; } 
        public double PanelY { get; set; } 
        public double PanelWidth { get; set; } 
        public int CellsRow { get; set; } 
        public int cellsCol { get; set; }

        public double CellSize { get; set; }
        public double Radius { get; set; }

        public Color? PanelColor { get; set; }
        public Color? CellColor { get; set; }
        public Color? CellBorderColor { get; set; }


    }
}
