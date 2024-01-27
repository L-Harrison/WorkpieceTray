using ScottPlot;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using WorkpieceTray.Controls;
using WorkpieceTray.Models;

namespace WorkpieceTray.Extensions
{
    public static class TrayExtension
    {
        public static Tray AddPanel(this Plot plot, Tray tray)
        {
            var maxX = (tray.Cols + 1) * tray.CellSize;
            var maxY = -(tray.Rows + 1) * tray.CellSize;

            #region Header
            if (tray.HeaderFont == default)
            {
                ScottPlot.Drawing.Font font = new ScottPlot.Drawing.Font();
                font.Family = new FontFamily(GenericFontFamilies.Serif);
                font.Alignment = Alignment.MiddleCenter;
                font.Color = plot.GetNextColor();
                font.Size = 20;
                tray.HeaderFont = font;

            }
            var xHeader = tray.XPanel + tray.PanelWidth / 2;
            var yHeader = tray.YPanel + (tray.XRadius + tray.YRadius) / 2;
            tray.HeaderTitle = tray.HeaderTitle ?? $"Plate {tray.Index + 1}";
            tray.Header = plot.AddText(tray.HeaderTitle, xHeader, yHeader, tray.HeaderFont);
            tray.Header.IsVisible = tray.HeaderMode == TrayHeaderMode.Header;
            #endregion

            #region Header2
            if (tray.Header2Font == default)
            {
                ScottPlot.Drawing.Font font = new ScottPlot.Drawing.Font();
                font.Alignment = Alignment.MiddleCenter;
                font.Color = /*Color.LightSlateGray;//*/ plot.GetNextColor();
                font.Size = 45;
                font.Family = new FontFamily(GenericFontFamilies.Serif);
                tray.Header2Font = font;

            }
            xHeader = tray.XPanel + tray.PanelWidth / 2;
            yHeader = tray.YPanel + maxY / 2;
            tray.Header2Title = tray.Header2Title ?? $"Plate {tray.Index + 1}";
            tray.Header2 = plot.AddText(tray.Header2Title, xHeader, yHeader, tray.Header2Font);
            tray.Header2.IsVisible = tray.HeaderMode == TrayHeaderMode.Header2;
            #endregion

            CoordinateRect rect = new(tray.XPanel, tray.XPanel + maxX, tray.YPanel, tray.YPanel + maxY);
            CPanel plottable = new(rect)
            {
                BorderColor = tray.PanelBorderColor ?? System.Drawing.Color.LightGray,
                DragEnabled = tray.EnableDraggable,
                Color = tray.PanelColor ?? System.Drawing.Color.Transparent,
            };
            plot.Add(plottable);
            plottable.Dragged += (object? sender, EventArgs e) =>
            {
                if (e is DraggedEventArgs rect)
                {
                    plot.Remove(tray.Header);
                    var x = rect.CoordinateRect.XMin + tray.PanelWidth / 2;
                    var y = rect.CoordinateRect.YMax + (tray.XRadius + tray.YRadius) / 2;
                    tray.Header = plot.AddText(tray.HeaderTitle, x, y, tray.Header.Font);
                    tray.Header.IsVisible = tray.HeaderMode == TrayHeaderMode.Header;


                    plot.Remove(tray.Header2);
                    var x1 = rect.CoordinateRect.XMin + tray.PanelWidth / 2;
                    var y1 = rect.CoordinateRect.YMax + maxY / 2;
                    tray.Header2 = plot.AddText(tray.Header2Title, x1, y1, tray.Header2.Font);
                    tray.Header2.IsVisible = tray.HeaderMode == TrayHeaderMode.Header2;


                    //var ep = e as DraggedEventArgs;
                    for (int i = 0; i < tray.Cells.Count; i++)
                    {
                        plot.Remove(tray.Cells[i]);
                    }
                    foreach (var item in (tray.Rows, tray.Cols).BuilderCells())
                    {
                        var cX = (item.currentCol + 1) * tray.CellSize + rect.CoordinateRect.XMin;
                        var cY = item.currentRow * tray.CellSize + rect.CoordinateRect.YMin;


                        var cell = plot.AddCell(
                            label: item.cellName,
                            x: cX,
                            y: cY,
                            xRadius: tray.XRadius,
                            yRadius: tray.YRadius,
                            size: tray.CanSetFont ? (float)tray.FontSize! : 11,
                              fontColor: tray.CanSetFont ? tray.FontColor : plot.GetLabeColor(plot.GetSettings().DataBackground.Color)?.TickLabelColor,
                            color: tray.CellColor ?? Color.SkyBlue,// /*System.Drawing.Color.LightGray,// */ColorTranslator.FromHtml("#17BECF"),
                            borderColor: tray.CellBorderColor,
                            lineWidth: 1,
                            default);

                        cell.Alignment = Alignment.MiddleCenter;
                        cell.DragEnabled = false;
                        cell.XAxisIndex = 0;
                        cell.YAxisIndex = 0;
                        tray.Cells.Add(cell);
                    }
                }

            };

            tray.Panel = plottable;

            foreach (var item in (tray.Rows, tray.Cols).BuilderCells())
            {
                var cX = (item.currentCol + 1) * tray.CellSize + tray.XPanel;
                var cY = (item.currentRow * tray.CellSize) * -1;

                //var font = new ScottPlot.Drawing.Font() { Size = size, Color = fontColor ?? plot.GetNextColor() };
                //if (plot.GetSettings().DataBackground.Color != ScottPlot.Style.Default.DataBackgroundColor)
                //{
                //    font.Color = ScottPlot.Style.Black.TickLabelColor;
                //}

                var cell = plot.AddCell(
                   label: item.cellName,
                   x: cX,
                   y: cY,
                   xRadius: tray.XRadius,
                   yRadius: tray.YRadius,
                   size: tray.CanSetFont ? (float)tray.FontSize! : 11,
                   fontColor: tray.CanSetFont ? tray.FontColor : plot.GetLabeColor(plot.GetSettings().DataBackground.Color)?.TickLabelColor,
                   color: tray.CellColor ?? /*System.Drawing.Color.LightGray,//*/ ColorTranslator.FromHtml("#17BECF"),
                   borderColor: tray.CellBorderColor,
                   lineWidth: 1,
                   default);
                cell.Index = item.index;
                cell.Name = item.cellName;
                cell.Id = Guid.NewGuid();

                cell.Alignment = Alignment.MiddleCenter;
                cell.DragEnabled = tray.EnableCellDraggable;
                cell.XAxisIndex = 0;
                cell.YAxisIndex = 0;

                tray.Cells.Add(cell);
            }
            return tray;

        }
      
        public static void Remove(this Plot plot, Tray tray)
        {
            plot.Remove(tray.Header);
            plot.Remove(tray.Header2);
            plot.Remove(tray.Panel);
            foreach (var item in tray.Cells)
            {
                plot.Remove(item);
            }
            tray = null!;
        }

    }
}
