using ScottPlot.Drawing;
using ScottPlot.Plottable;
using ScottPlot;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot.SnapLogic;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Font = ScottPlot.Drawing.Font;

namespace WorkpieceTray.Controls
{
    public class Cell : IPlottable, IHasColor, IHasArea, ICell
    {


        // data


        // customization
        public bool IsVisible { get; set; } = true;
        public int XAxisIndex { get; set; } = 0;
        public int YAxisIndex { get; set; } = 0;
        public bool BackgroundFill = false;
        public Color BackgroundColor;
        public Font Font = new Font();
        public Color Color { get ; set; }
        public string FontName { get => Font.Name; set => Font.Name = value; }
        public float FontSize { get => Font.Size; set => Font.Size = value; }
        public bool FontBold { get => Font.Bold; set => Font.Bold = value; }
        public Alignment Alignment { get => Font.Alignment; set => Font.Alignment = value; }
        public float Rotation { get => Font.Rotation; set => Font.Rotation = value; }
        public float BorderSize { get; set; } = 0;
        public Color BorderColor { get; set; } = System.Drawing.Color.Black;
        public float PixelOffsetX { get; set; } = 0;
        public float PixelOffsetY { get; set; } = 0;
        public RectangleF LastRenderRectangleCoordinates { get; set; }
        private double DeltaCX { get; set; } = 0;
        private double DeltaCY { get; set; } = 0;
        public ISnap2D DragSnap { get; set; } = new NoSnap2D();

        public override string ToString() => $"PlottableText \"{Label}\" at ({X}, {Y})";


        /// <summary>
        /// Horizontal center of the circle (axis units)
        /// </summary>
        double X { get; }

        /// <summary>
        /// Vertical center of the circle (axis units)
        /// </summary>
        double Y { get; }

        /// <summary>
        /// Horizontal radius (axis units)
        /// </summary>
        public double RadiusX { get; set; }

        /// <summary>
        /// Vertical radius (axis units)
        /// </summary>
        public double RadiusY { get; set; }


        /// <summary>
        /// Outline thickness (pixel units)
        /// </summary>
        public float BorderLineWidth { get; set; } = 2;

        /// <summary>
        /// Outline line style
        /// </summary>
        public LineStyle BorderLineStyle { get; set; } = LineStyle.Solid;



        /// <summary>
        /// Fill pattern
        /// </summary>
        public HatchStyle HatchStyle { get; set; } = HatchStyle.None;

        /// <summary>
        /// Alternate color for fill pattern
        /// </summary>
        public Color HatchColor { get; set; } = Color.Black;

        /// <summary>
        /// Text to appear in the legend
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Create an ellipse centered at (x, y) with the given horizontal and vertical radius
        /// </summary>
        public Cell(double x, double y, double xRadius, double yRadius)
        {
            X = x;
            Y = y;
            RadiusX = xRadius;
            RadiusY = yRadius;
        }

        // These default values are fine for most cases

        public Cursor CellCursor { get; set; } = Cursor.All;
        public bool CellTestEnabled { get; set; } = true;

        public void ValidateData(bool deep = false)
        {
            if (double.IsNaN(X) || double.IsNaN(Y))
                throw new InvalidOperationException("X and Y cannot be NaN");

            if (double.IsInfinity(X) || double.IsInfinity(Y))
                throw new InvalidOperationException("X and Y cannot be Infinity");

            if (string.IsNullOrWhiteSpace(Label))
                throw new InvalidOperationException("text cannot be null or whitespace");
        }

        // Return an empty array for plottables that do not appear in the legend
        public LegendItem[] GetLegendItems()
        {
            if (string.IsNullOrWhiteSpace(Label))
                return LegendItem.None;

            LegendItem item = new(this)
            {
                label = Label,
                color = Color,
                borderColor = BorderColor,
                borderLineStyle = BorderLineStyle,
                borderWith = BorderLineWidth,
            };

            return LegendItem.Single(item);
        }

        // This method returns the bounds of the data
        public AxisLimits GetAxisLimits()
        {
            return new AxisLimits(
                xMin: X - RadiusX,
                xMax: X + RadiusX,
                yMin: Y - RadiusY,
                yMax: Y + RadiusY);
        }

        // This method describes how to plot the data on the cart.
        public void Render(PlotDimensions dims, System.Drawing.Bitmap bmp, bool lowQuality = false)
        {
            // Use ScottPlot's GDI helper functions to create System.Drawing objects
            using var gfxx = ScottPlot.Drawing.GDI.Graphics(bmp, dims, lowQuality);
            using var pen = ScottPlot.Drawing.GDI.Pen(BorderColor, BorderLineWidth, BorderLineStyle);
            using var brush = ScottPlot.Drawing.GDI.Brush(Color, HatchColor, HatchStyle);

            // Use 'dims' methods to convert between axis coordinates and pixel positions
            float xPixel = dims.GetPixelX(X);
            float yPixel = dims.GetPixelY(Y);

            // Use 'dims' to determine how large the radius is in pixel units
            float xRadiusPixels = dims.GetPixelX(X + RadiusX) - xPixel;
            float yRadiusPixels = dims.GetPixelY(Y + RadiusY) - yPixel;

            // Center, rotate, and scale the canvas so the ellipse fits in a radius 1 rectangle at the origin
            gfxx.TranslateTransform(xPixel, yPixel);
            gfxx.RotateTransform(Rotation);
            gfxx.ScaleTransform(xRadiusPixels, yRadiusPixels);
            RectangleF rect = new(-1, -1, 2, 2);

            // Render data by drawing on the Graphics object
            if (Color != Color.Transparent)
                gfxx.FillEllipse(brush, rect);

            // Otherwise the pen width will be scaled as well
            using System.Drawing.Drawing2D.Matrix invertScaleMatrix = new();
            invertScaleMatrix.Scale(1 / xRadiusPixels, 1 / yRadiusPixels);
            pen.Transform = invertScaleMatrix;

            gfxx.DrawEllipse(pen, rect);


            using (Graphics gfx = GDI.Graphics(bmp, dims, lowQuality))
            using (var font = GDI.Font(Font))
            using (var fontBrush = new SolidBrush(Font.Color))
            using (var frameBrush = new SolidBrush(BackgroundColor))
            using (var outlinePen = new System.Drawing.Pen(BorderColor, BorderSize))
            using (var redPen = new System.Drawing.Pen(Color.Red, BorderSize))
            {
                float pixelX = dims.GetPixelX(X) + PixelOffsetX;
                float pixelY = dims.GetPixelY(Y) - PixelOffsetY;
                SizeF stringSize = GDI.MeasureString(gfx, Label, font);

                gfx.TranslateTransform(pixelX, pixelY);
                gfx.RotateTransform(Font.Rotation);

                (float dX, float dY) = GDI.TranslateString(gfx, Label, Font);
                gfx.TranslateTransform(-dX, -dY);

                if (BackgroundFill)
                {
                    RectangleF stringRect = new(0, 0, stringSize.Width, stringSize.Height);
                    gfx.FillRectangle(frameBrush, stringRect);
                    if (BorderSize > 0)
                        gfx.DrawRectangle(outlinePen, stringRect.X, stringRect.Y, stringRect.Width, stringRect.Height);
                }

                gfx.DrawString(Label, font, fontBrush, new PointF(0, 0));

                GDI.ResetTransformPreservingScale(gfx, dims);

                double degangle = Font.Rotation * Math.PI / 180;
                float xA = pixelX - dX;
                float yA = pixelY - dY;
                float xC = xA + stringSize.Width * (float)Math.Cos(degangle) - stringSize.Height * (float)Math.Sin(degangle);
                float yC = yA + stringSize.Height * (float)Math.Cos(degangle) + stringSize.Width * (float)Math.Sin(degangle);

                PointF pointA = new(xA, yA);
                PointF pointC = new(xC, yC);

                LastRenderRectangleCoordinates = RectangleF.FromLTRB(
                    left: (float)dims.GetCoordinateX(pointA.X),
                    top: (float)dims.GetCoordinateY(pointC.Y),
                    right: (float)dims.GetCoordinateX(pointC.X),
                    bottom: (float)dims.GetCoordinateY(pointA.Y));
            }


        }

        public bool CellTest(Coordinate coord)
            => CellTestEnabled && Math.Abs(coord.X - X) <= RadiusX && Math.Abs(coord.Y - Y) <= RadiusY;
    }
}
