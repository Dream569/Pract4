using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Pract4
{
    public partial class MainWindow : Window
    {
        private int currentFractal;
        private Color fractalColor = Colors.Black;

        public MainWindow()
        {
            InitializeComponent();
            FractalComboBox.SelectedIndex = 0;
            DrawFractal();
            
        }
        private void FractalComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DrawFractal();
        }

        private void StepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawFractal();
            
        }

        private void ColorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ColorComboBox.SelectedItem;
            if (selectedItem != null)
            {
                string colorName = selectedItem.Tag.ToString();
                fractalColor = (Color)ColorConverter.ConvertFromString(colorName);
                DrawFractal();
            }
        }

        private void DrawFractal()
        {
            if (FractalCanvas == null)
            {
                return;
            }

            FractalCanvas.Children.Clear();

            int steps = (int)StepSlider.Value;
            
            
            if (steps < 1 || steps > 10)
            {
                MessageBox.Show("Недопустимое количество шагов. Введите значение от 1 до 10.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            currentFractal = FractalComboBox.SelectedIndex;
            switch (currentFractal)
            {
                case 0:
                    DrawFractalTree(FractalCanvas.ActualWidth / 2, FractalCanvas.ActualHeight, -90, steps);
                    break;
                case 1:
                    DrawKochCurve(FractalCanvas, steps, fractalColor);
                    break;
                case 2:
                    DrawSierpinskiCarpet(FractalCanvas, steps);
                    break;
                case 3:
                    DrawSierpinskiTriangle(FractalCanvas, steps, fractalColor);
                    break;
                case 4:
                    DrawCantorSet(FractalCanvas, steps);
                    break;
                default:
                    break;
            }
        }

        
        private void DrawFractalTree(double x, double y, double angle, int depth)
        {
            if (depth == 0) return;

            double xEnd = x + Math.Cos(angle * Math.PI / 180) * depth * 10;
            double yEnd = y + Math.Sin(angle * Math.PI / 180) * depth * 10;

            Line line = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = xEnd,
                Y2 = yEnd,
                Stroke = new SolidColorBrush(fractalColor),
                StrokeThickness = depth
            };

            FractalCanvas.Children.Add(line);
            DrawFractalTree(xEnd, yEnd, angle - 20, depth - 1);
            DrawFractalTree(xEnd, yEnd, angle + 20, depth - 1);
        }

       
        private void DrawKochCurve(Canvas canvas, int depth, Color color)
        {
            double length = 300;

            DrawKochSegment(canvas, depth, color, 50, 150, 50 + length, 150);
        }

        private void DrawKochSegment(Canvas canvas, int depth, Color color, double x1, double y1, double x2, double y2)
        {
            if (depth == 0)
            {
                Line line = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
                return;
            }

            double deltaX = (x2 - x1) / 3;
            double deltaY = (y2 - y1) / 3;

            double x3 = x1 + deltaX;
            double y3 = y1 + deltaY;

            double x4 = x1 + 2 * deltaX;
            double y4 = y1 + 2 * deltaY;

            double xMid = (x1 + x2) / 2 - Math.Sqrt(3) * (deltaY / 2);
            double yMid = (y1 + y2) / 2 + Math.Sqrt(3) * (deltaX / 2);

            DrawKochSegment(canvas, depth - 1, color, x1, y1, x3, y3);
            DrawKochSegment(canvas, depth - 1, color, x3, y3, xMid, yMid);
            DrawKochSegment(canvas, depth - 1, color, xMid, yMid, x4, y4);
            DrawKochSegment(canvas, depth - 1, color, x4, y4, x2, y2);
        }

        
        private void DrawSierpinskiCarpet(Canvas canvas, int depth)
        {
            DrawSierpinskiCarpet(canvas, depth, 50, 50, 300);
        }

        private void DrawSierpinskiCarpet(Canvas canvas, int depth, double x, double y, double size)
        {
            if (depth == 0)
            {
                Rectangle rect = new Rectangle
                {
                    Width = size,
                    Height = size,
                    Fill = new SolidColorBrush(fractalColor)
                };
                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                canvas.Children.Add(rect);
                return;
            }

            double newSize = size / 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1) continue;
                    DrawSierpinskiCarpet(canvas, depth - 1, x + i * newSize, y + j * newSize, newSize);
                }
            }
        }

        
        private void DrawSierpinskiTriangle(Canvas canvas, int depth, Color color)
        {
            Point p1 = new Point(400, 50);
            Point p2 = new Point(50, 400);
            Point p3 = new Point(750, 400);
            DrawSierpinskiTriangle(canvas, depth, color, p1, p2, p3);
        }

        private void DrawSierpinskiTriangle(Canvas canvas, int depth, Color color, Point p1, Point p2, Point p3)
        {
            if (depth == 0)
            {
                Polygon triangle = new Polygon
                {
                    Points = new PointCollection { p1, p2, p3 },
                    Fill = new SolidColorBrush(color)
                };
                canvas.Children.Add(triangle);
                return;
            }

            Point mid1 = Midpoint(p1, p2);
            Point mid2 = Midpoint(p2, p3);
            Point mid3 = Midpoint(p1, p3);

            DrawSierpinskiTriangle(canvas, depth - 1, color, p1, mid1, mid3);
            DrawSierpinskiTriangle(canvas, depth - 1, color, p2, mid1, mid2);
            DrawSierpinskiTriangle(canvas, depth - 1, color, p3, mid2, mid3);
        }

        private Point Midpoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

       
        private void DrawCantorSet(Canvas canvas, int depth)
        {
            double x1 = 50;
            double y1 = 500;
            double length = 600;
            DrawCantorSegment(canvas, depth, x1, y1, length);
        }

        private void DrawCantorSegment(Canvas canvas, int depth, double x, double y, double length)
        {
            if (depth == 0)
            {
                Line line = new Line
                {
                    X1 = x,
                    Y1 = y,
                    X2 = x + length,
                    Y2 = y,
                    Stroke = new SolidColorBrush(fractalColor),
                    StrokeThickness = 4
                };
                canvas.Children.Add(line);
                return;
            }

            DrawCantorSegment(canvas, depth - 1, x, y, length / 3);
            DrawCantorSegment(canvas, depth - 1, x + 2 * length / 3, y, length / 3);
        }
    }
}