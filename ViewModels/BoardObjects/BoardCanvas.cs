using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using Avalonia.Controls.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Layout;
using Traffic.Models;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia;
using JetBrains.Annotations;

namespace Traffic.ViewModels.BoardObjects
{
    internal class BoardCanvas : Canvas
    {
        private readonly int rows = 6;
        private readonly int columns = 6;
        private readonly IBrush borderFill = Brushes.Gray;
        private readonly IBrush[] colors = new IBrush[] { Brushes.Pink, Brushes.LightBlue, Brushes.LightCyan, Brushes.Lime, Brushes.Navy, Brushes.Orange, Brushes.Salmon,
                                                          Brushes.SaddleBrown, Brushes.Olive, Brushes.SeaGreen };

        private const double marginScale = 0.05;

        private double tileSize;
        private double margin;

        private Board? board = null;

        public double TileSize { get => tileSize; }
        public double BorderThickness { get => margin; }
        public int Columns { get => columns; }

        public Board? Board 
        {
            get => board;
            set
            {
                board = value;
                tileSize = (Width - 1) / (board!.Columns * (marginScale + 1));
                margin = marginScale * tileSize;
                Draw();
            }
        }

        public BoardCanvas(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public Point GetTileUpperLeftCorner(int row, int column)
        {
            return new Point(BorderThickness + (TileSize + BorderThickness) * column, BorderThickness + (TileSize + BorderThickness) * row);
        }

        public void Draw()
        {
            if(board is null)
            {
                Background = Brushes.Blue;
                Label label = new()
                {
                    Foreground = Brushes.White,
                    Content = "Pending...",
                    FontSize = 10
                };
                SetTop(label, 10);
                Children.Add(label);
            }
            else
            {
                Children.Clear();
                DrawBoard();
                DrawCars();
            }
        }

        private void DrawBoard()
        {
            Background = Brushes.Beige;
            Width = margin + (margin + tileSize) * columns;
            Height = margin + (margin + tileSize) * rows;

            //create horizontal lines
            for(int i = 0; i <= rows; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = Width;
                rect.Height = margin;
                rect.Fill = borderFill;
                SetTop(rect, (margin + tileSize) * i);
                Children.Add(rect);
            }

            //create vertical lines
            for(int i = 0; i < columns; i++)
            {
                Rectangle rect = new()
                {
                    Width = margin,
                    Height = Height,
                    Fill = borderFill
                };
                SetLeft(rect, (margin + tileSize) * i);
                Children.Add(rect);
            }

            //create last vertical line with a hole
            {
                Rectangle rect = new()
                {
                    Width = BorderThickness,
                    Height = BorderThickness + (tileSize + BorderThickness) * Board!.FinishRow,
                    Fill = borderFill
                };
                SetLeft(rect, (margin + tileSize) * columns);
                Children.Add(rect);

                rect = new()
                {
                    Width = BorderThickness,
                    Height = BorderThickness + (tileSize + BorderThickness) * (rows - Board.FinishRow - 1),
                    Fill = borderFill
                };
                SetLeft(rect, (margin + tileSize) * columns);
                SetTop(rect, (margin + tileSize) * (rows - Board.FinishRow - 1));
                Children.Add(rect);
            }
        }

        private void DrawCars()
        {
            for(int i = 0; i < Board!.Cars.Length; i++)
            {
                Car car = Board.Cars[i];

                CarRectangle rect;
                if(car.Orientation == Models.Orientation.Horizontal)
                {
                    rect = new HorizontalCar(car, this);
                    rect.Width = (margin + tileSize) * car.Length - margin;
                    rect.Height = tileSize;
                }
                else
                {
                    rect = new VerticalCar(car, this);
                    rect.Width = tileSize;
                    rect.Height = (margin + tileSize) * car.Length - margin;
                }
                if (car.IsSpecial)
                    rect.Fill = Brushes.Red;
                else
                    rect.Fill = colors[i];
                SetLeft(rect, car.Position.Column * (tileSize + margin) + margin);
                SetTop(rect, car.Position.Row * (tileSize + margin) + margin);
                Children.Add(rect);
            }
        }

        public void Win()
        {
            IsEnabled = false;
        }

        public bool IsPosValid(double x, double y, Car car)
        {
            if (x < margin || x > Width - margin || y < margin || y > Height - margin)
                return false;

            int column = (int)((x - 1) / (margin + tileSize));
            int row = (int)(y / (margin + tileSize));
            return Board!.IsAvailableForCar(column, row, car);
        }

        public bool IsXValid(double x)
        {
            if (x < margin || x > Width - margin)
                return false;

            return true;
        }

        public bool IsYValid(double y)
        {
            if (y < margin || y > Height - margin)
                return false;
            return true;
        }

        public void Save()
        {
            LevelMenager.SerializeBoard(Board!);
        }
    }
}
