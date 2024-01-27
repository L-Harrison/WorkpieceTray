using ScottPlot.Plottable;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot.Drawing;

using WorkpieceTray.Controls;
using Font = ScottPlot.Drawing.Font;
using ScottPlot;

namespace WorkpieceTray.Models
{
    public class Tray
    {
        public Tray(int index, double xPanel, double yPanel, double panelWidth, int rows, int cols, double xRadius, double yRadius, double cellSize, TrayHeaderMode headerMode = default,Color? fontColor=null,double? fontSize=null)
        {
            Index = index;
            XPanel = xPanel;
            YPanel = yPanel;
            PanelWidth = panelWidth;
            Rows = rows;
            Cols = cols;
            XRadius = xRadius;
            YRadius = yRadius;
            CellSize = cellSize;
            HeaderMode = headerMode;
            FontColor= fontColor;
            FontSize = fontSize;
        }

        public List<Cell> Cells { get; set; } = new();
        public CPanel? Panel { get; set; }
        public Text? Header { set; get; }
        public Text? Header2 { set; get; }
        private TrayHeaderMode headerMode = TrayHeaderMode.Header;
        public TrayHeaderMode HeaderMode
        {
            get => headerMode; set
            {

                headerMode = value;
                switch (value)
                {
                    case TrayHeaderMode.None:
                        if (Header != null)
                            Header.IsVisible = false;
                        if (Header2 != null)
                            Header2.IsVisible = false;
                        break;
                    case TrayHeaderMode.Header:
                        if (Header != null)
                            Header.IsVisible = true;
                        if (Header2 != null)
                            Header2.IsVisible = false;
                        break;
                    case TrayHeaderMode.Header2:
                        if (Header != null)
                            Header.IsVisible = false;
                        if (Header2 != null)
                            Header2.IsVisible = true;
                        break;
                    default:
                        break;
                }

            }
        }
        public Font? HeaderFont { get; set; }
        public Font? Header2Font { get; set; }
        public string? HeaderTitle { get; set; }
        public string? Header2Title { get; set; }



        public int Index { get; set; }
        public double XPanel { get; set; }
        public double YPanel { get; set; }
        public double PanelWidth { get; set; }
        public bool EnableDraggable { get; set; } = false;

        public Color? PanelBorderColor { get; set; }
        public Color? PanelColor { get; set; }


        public int Rows { get; set; }
        public int Cols { get; set; }

        public double XRadius { get; set; }
        public double YRadius { get; set; }
        public double CellSize { get; set; }
        public Color? CellBorderColor { get; set; }
        public Color? CellColor { get; set; }

        public Color? FontColor { get; private set; }
        public double? FontSize { get; private set; }
        private bool canSetFont = false;
        public bool CanSetFont
        {
            get => canSetFont; set
            {
                canSetFont = value;
                if (value)
                {
                    if (FontSize == null)
                        FontSize = 11;
                    if(FontColor==null)
                        FontColor = ColorTranslator.FromHtml("#161616");
                }
            }
        }

        public bool EnableCellDraggable { get; set; } = false;
    }
}
