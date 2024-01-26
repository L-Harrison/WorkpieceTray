using ScottPlot;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WorkpieceTray.Controls;
using WorkpieceTray.Models;

namespace WorkpieceTray.Extensions
{
    public static class PanelExtension
    {
        public static PanelModel AddPanel(this Plot plot,int PanelIndex, double panelX, double panelY, double panelWidth, int cellsRow, int cellsCol, double cellSize, double radius, Color? panelColor = null, Color? cellColor = null, Color? cellBorderColor = null)
        {
            var model = new PanelModel()
            {
                PanelX = panelX,
                PanelY = panelY,
                PanelWidth = panelWidth,
                CellsRow = cellsRow,
                cellsCol = cellsCol,
                CellBorderColor = cellBorderColor,
                CellSize = cellSize,
                Radius = radius,
                CellColor = cellColor,
                PanelColor = panelColor,
                PanelIndex= PanelIndex
            };

            var maxX = (cellsCol + 1) * cellSize;
            var maxY = (cellsRow + 1) * cellSize;

            CoordinateRect rect = new(panelX, panelX + maxX, panelY, panelY + maxY);
            CPanel plottable = new(rect)
            {
                BorderColor = panelColor ?? System.Drawing.Color.LightGray,
                DragEnabled = true,
                Color = System.Drawing.Color.Transparent,
            };
            plot.Add(plottable);
            plottable.Dragged += (object? sender, EventArgs e) =>
            {
                var ep = e as DraggedEventArgs;
                for (int i = 0; i < model.Cells.Count; i++)
                {
                    model.Cells[i].DragTo(ep.CoordinateX, ep.CoordinateY, false);
                }
            };

            model.Panel = plottable;

            foreach (var item in (cellsRow, cellsCol).BuilderCells())
            {
                var cX = (item.currentCol + 1) * cellSize + panelX;
                var cY = item.currentRow * cellSize;

                var cell = plot.AddCell(
                    label: item.cellName,
                    x: cX,
                    y: cY,
                    xRadius: radius,
                    yRadius: radius,
                    size: 11,
                    fontColor: System.Drawing.Color.Black,
                    color: cellColor ?? ColorTranslator.FromHtml("#17BECF"),
                    borderColor: cellBorderColor,
                    lineWidth: 1,
                    default);

                cell.Alignment = Alignment.MiddleCenter;
                cell.DragEnabled = false;
                cell.XAxisIndex = 0;
                cell.YAxisIndex = 0;
                model.Cells.Add(cell);
            }
            return model;
        }

    }
}
