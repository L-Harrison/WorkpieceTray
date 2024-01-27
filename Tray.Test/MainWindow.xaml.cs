using ScottPlot;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WorkpieceTray.Controls;

using WorkpieceTray.Models;

namespace Tray.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<WorkpieceTray.Models.Tray> Trays { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            Trays = new();
            Init();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(1000);
                        foreach (var item in Trays)
                        {
                            foreach (var cell in item.Cells)
                            {
                                cell.Volumns = 15;
                                while (true)
                                {
                                    if (cell.CurrentVol + .05 >= cell.Volumns)
                                    {
                                        break;
                                    }
                                    cell.CurrentVol += 1.3;
                                    await Task.Delay(10);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            });

        }
        double x = 0d;
        double y = 0d;

        int row = 16;
        int col = 6;
        double cellSize = 20d;
        double radius = 8.5d;

        int plateIndex = 0;
        private void Init()
        {

            var panelWidth = (col + 1.5) * cellSize;

            var pane = new WorkpieceTray.Models.Tray(index: plateIndex++, xPanel: x, yPanel: y, panelWidth: panelWidth, rows: row, cols: col, radius, radius, cellSize: cellSize, headerMode: TrayHeaderMode.Header);
            pane.EnableDraggable = false;
            //pane.CellFont.Color = ColorTranslator.FromHtml("#161616");
            //pane.CellBorderColor = ColorTranslator.FromHtml("#161616");
            //pane.CellFont.Color = System.Drawing.Color.Black;
            //pane.CellColor =ColorTranslator.FromHtml("#0A8066");
            //pane.CellColor =ColorTranslator.FromHtml("#346166");
            //pane.CellColor =ColorTranslator.FromHtml("#698D68");
            Trays.Add(pane);

            x += panelWidth;

            row = 14;
            col = 5;
            cellSize = 22.7;
            radius = 9;
            panelWidth = (col + 1.5) * cellSize;

            for (int panel = 1; panel < 5; panel++)
            {
                pane = new WorkpieceTray.Models.Tray(index: plateIndex++, xPanel: x, yPanel: y, panelWidth: panelWidth, rows: row, cols: col, radius, radius, cellSize: cellSize, headerMode: TrayHeaderMode.Header);
                pane.EnableDraggable = false;
                //pane.CellFont.Color = ColorTranslator.FromHtml(htmlColor: "#161616");
                //pane.Header2.IsVisible = true;
                Trays.Add(pane);
                x += panelWidth;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            row = 14;
            col = 5;
            cellSize = 22.7;
            radius = 9;

            var panelWidth = (col + 1.5) * cellSize;

            var tray0 = new WorkpieceTray.Models.Tray(index: plateIndex++, xPanel: x, yPanel: y, panelWidth: panelWidth, rows: row, cols: col, radius, radius, cellSize: cellSize, headerMode: TrayHeaderMode.Header);
            tray0.EnableDraggable = false;
            tray0.HeaderMode = currentMode;
            Trays.Add(tray0);

            x += panelWidth;

        }

        private void Button_remove_Click(object sender, RoutedEventArgs e)
        {
            if (Trays.Any())
            {
                var panelWidth = Trays[Trays.Count - 1].PanelWidth;
                Trays.RemoveAt(Trays.Count - 1);
                x -= panelWidth;
                plateIndex--;
            }
        }

        TrayHeaderMode currentMode = TrayHeaderMode.Header;
        private void TrayCore_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == TrayHeaderMode.None)
            {
                currentMode = TrayHeaderMode.Header;
            }
            else if (currentMode == TrayHeaderMode.Header)
            {
                currentMode = TrayHeaderMode.Header2;
            }
            else
            {
                currentMode = TrayHeaderMode.None;
            }
            foreach (var item in Trays)
            {
                item.HeaderMode = currentMode;
            }

        }

        private void TrayCore_IsCellOver(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Cell cell)
            {
                Trace.WriteLine($"{cell.Name} {DateTime.Now}");
            }
            else
            {
                Trace.WriteLine("NAN");
            }
        }
    }
}