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
        private readonly double tileSize = 70;
        private readonly double margin = 5;
        private readonly int rows = 6;
        private readonly int columns = 6;
        private readonly IBrush borderFill = Brushes.Gray;
        private readonly int carCount = 10;
        private readonly IBrush[] colors = new IBrush[] { Brushes.Pink, Brushes.LightBlue, Brushes.LightCyan, Brushes.Lime, Brushes.Navy, Brushes.Orange, Brushes.Salmon,
                                                          Brushes.SaddleBrown, Brushes.Olive, Brushes.SeaGreen };

        public double TileSize { get => tileSize; }
        public double BorderThickness { get => margin; }
        public int Columns { get => columns; }

        private Board board;

        public BoardCanvas(double tileSize, Board board)
        {
            this.tileSize = tileSize;
            margin = 0.05 * tileSize;
            this.board = board;
            DrawBoard();
            DrawCars();
        }

        public Board Board { get => board; }

        public Point GetTileUpperLeftCorner(int row, int column)
        {
            return new Point(BorderThickness + (TileSize + BorderThickness) * column, BorderThickness + (TileSize + BorderThickness) * row);
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
                Rectangle rect = new Rectangle();
                rect.Width = margin;
                rect.Height = Height;
                rect.Fill = borderFill;
                SetLeft(rect, (margin + tileSize) * i);
                Children.Add(rect);
            }

            //create last vertical line with a hole
            {
                Rectangle rect = new Rectangle();
                rect.Width = BorderThickness;
                rect.Height = BorderThickness + (tileSize + BorderThickness) * board.FinishRow;
                rect.Fill = borderFill;
                SetLeft(rect, (margin + tileSize) * columns);
                Children.Add(rect);

                rect = new Rectangle();
                rect.Width = BorderThickness;
                rect.Height = BorderThickness + (tileSize + BorderThickness) * (rows - board.FinishRow - 1);
                rect.Fill = borderFill;
                SetLeft(rect, (margin + tileSize) * columns);
                SetTop(rect, (margin + tileSize) * (rows - board.FinishRow - 1));
                Children.Add(rect);
            }
        }

        private void DrawCars()
        {
            for(int i = 0; i < board.Cars.Length; i++)
            {
                Car car = board.Cars[i];

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
            return board.IsAvailableForCar(column, row, car);
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
            LevelMenager.SerializeBoard(board);
        }
    }
}
