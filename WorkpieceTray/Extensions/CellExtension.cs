using ScottPlot;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

using WorkpieceTray.Controls;
using ScottPlot.Plottable;

namespace WorkpieceTray.Extensions
{
    public static class CellExtension
    {
        public static (int index, string cellName, int currentRow, int currentCol) ToIndexAndName(this int coloums, int currentRow, int currentCol)
        {
            var rowFlag = ASCIIEncoding.UTF8.GetString(new byte[] { (byte)(currentRow + 64) });
            var index = coloums * (currentRow - 1) + currentCol;
            var name = $"{rowFlag}{currentCol}";
            return (index, name, currentRow, currentCol);
        }
        /// <summary>
        /// Add an ellipse to the plot
        /// </summary>
        public static Cell AddCell(this Plot plot, string label, double x, double y, double xRadius, double yRadius, float size=12, System.Drawing.Color? lableColor = null, System.Drawing.Color? color = null, float lineWidth = 2, LineStyle lineStyle = LineStyle.Solid)
        {
            var font = new ScottPlot.Drawing.Font() { Size = size, Color = lableColor ?? plot.GetNextColor() };
            var c = color ?? plot.GetNextColor();
            Cell plottable = new(x, y, xRadius, yRadius)
            {
                BorderColor = c,
                BorderLineWidth = lineWidth,
                BorderLineStyle = lineStyle,
                Label = label,
                Font=font
            };
            plot.Add(plottable);
            return plottable;
        }

        /// <summary>
        /// Return the highest hittable plottable at the given point (or null if no hit)
        /// </summary>
        public static IPlottable GetCelltable(this Plot plot, double xPixel, double yPixel)
        {
            foreach (var plottable in plot.GetPlottables().Where(x => x is ICell).Reverse())
            {
                int xAxisIndex = plottable.XAxisIndex;
                int yAxisIndex = plottable.YAxisIndex;

                double xCoords = plot.GetCoordinateX((float)xPixel, xAxisIndex);
                double yCoords = plot.GetCoordinateY((float)yPixel, yAxisIndex);
                Coordinate c = new(xCoords, yCoords);

                ICell hittable = (ICell)plottable;
                if (hittable.CellTest(c))
                    return plottable;
            }

            return null;
        }
        /// <summary>
        /// Return the highest hittable plottable at the given point (or null if no hit)
        /// </summary>
        public static IPlottable[] GetCelltable(this Plot plot)
            => plot.GetPlottables().Where(x => x is ICell).ToArray();
    }
}
