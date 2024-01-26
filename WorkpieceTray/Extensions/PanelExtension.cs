using ScottPlot;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

using WorkpieceTray.Controls;
using WorkpieceTray.Models;

namespace WorkpieceTray.Extensions
{
    public static class PanelExtension
    {
        public static PanelModel AddPanel(this Plot plot, int PanelIndex, double panelX, double panelY, double panelWidth, int cellsRow, int cellsCol, double cellSize, double radius, Color? panelColor = null, Color? cellColor = null, Color? cellBorderColor = null)
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
                PanelIndex = PanelIndex
            };

            var maxX = (cellsCol + 1) * cellSize;
            var maxY = -(cellsRow + 1) * cellSize;

            ScottPlot.Drawing.Font font = new ScottPlot.Drawing.Font();
            font.Family = new FontFamily(GenericFontFamilies.Serif);
            font.Alignment = Alignment.MiddleCenter;
            font.Color =plot.GetNextColor();
            var txtX = panelX + panelWidth / 2;
            var txtY = panelY + radius;
            var txt = plot.AddText($"Plate {PanelIndex + 1}", txtX, txtY, font);
            model.Header = txt;


            //ScottPlot.Drawing.Font fon1t = new ScottPlot.Drawing.Font();
            //fon1t.Alignment = Alignment.MiddleCenter;
            //fon1t.Color = Color.LightSlateGray;// plot.GetNextColor();
            //fon1t.Size =160;
            //fon1t.Family = new FontFamily(GenericFontFamilies.Serif);
            //var xx = panelX + panelWidth / 2;
            //var yy = panelY + maxY/2;
            //model.Header2 = plot.AddText($"{PanelIndex + 1}", xx, yy, fon1t);


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
                if (e is DraggedEventArgs rect)
                {
                    plot.Remove(model.Header);
                    var x = rect.CoordinateRect.XMin + panelWidth / 2;
                    var y = rect.CoordinateRect.YMax + radius;
                    model.Header = plot.AddText($"Plate {PanelIndex + 1}", x, y, model.Header.Font);


                    //plot.Remove(model.Header2);
                    //var x1 = rect.CoordinateRect.XMin + panelWidth / 2;
                    //var y1 = rect.CoordinateRect.YMax + maxY / 2;
                    //model.Header2 = plot.AddText($"{PanelIndex + 1}", x1, y1, model.Header2.Font);


                    //var ep = e as DraggedEventArgs;
                    for (int i = 0; i < model.Cells.Count; i++)
                    {
                        plot.Remove(model.Cells[i]);
                    }
                    foreach (var item in (cellsRow, cellsCol).BuilderCells())
                    {
                        var cX = (item.currentCol + 1) * cellSize + rect.CoordinateRect.XMin;
                        var cY = item.currentRow * cellSize + rect.CoordinateRect.YMin;

                        var cell = plot.AddCell(
                            label: item.cellName,
                            x: cX,
                            y: cY,
                            xRadius: radius,
                            yRadius: radius,
                            size: 11,
                            fontColor: System.Drawing.Color.Black,
                            color: cellColor ?? System.Drawing.Color.LightGray,// ColorTranslator.FromHtml("#17BECF"),
                            borderColor: cellBorderColor ?? System.Drawing.Color.LightGray,
                            lineWidth: 1,
                            default);

                        cell.Alignment = Alignment.MiddleCenter;
                        cell.DragEnabled = false;
                        cell.XAxisIndex = 0;
                        cell.YAxisIndex = 0;
                        model.Cells.Add(cell);
                    }
                }



                //var ep = e as DraggedEventArgs;
                //for (int i = 0; i < model.Cells.Count; i++)
                //{
                //    model.Cells[i].DragTo(ep.CoordinateX, ep.CoordinateY, false);
                //}
            };

            model.Panel = plottable;

            foreach (var item in (cellsRow, cellsCol).BuilderCells())
            {
                var cX = (item.currentCol + 1) * cellSize + panelX;
                var cY = (item.currentRow * cellSize) * -1;

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
