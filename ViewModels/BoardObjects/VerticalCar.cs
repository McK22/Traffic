using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traffic.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Traffic.ViewModels.BoardObjects
{
    internal class VerticalCar : CarRectangle
    {
        private double? minY = null;
        private double? maxY = null;

        public VerticalCar(Car car, BoardCanvas board) : base(car, board)
        {
            PointerMoved += MoveHandler;
        }

        protected override void MoveHandler(object? sender, PointerEventArgs e)
        {
            if (!isClicked)
                return;

            Point pointOnBoard = e.GetCurrentPoint(Parent).Position;

            double topY = pointOnBoard.Y - pointClicked.Y;
            if (minY <= topY && topY + Height <= maxY)
                Canvas.SetTop(this, topY);
        }

        protected override (int row, int column) GetPos()
        {
            Point? currentPoint = this.TranslatePoint(new Point(0, 0), this.GetVisualParent());
            if (currentPoint == null)
                throw new NullReferenceException(nameof(currentPoint));

            int row = (int)(currentPoint.Value.Y / (board.TileSize + board.BorderThickness));
            int column = (int)(currentPoint.Value.X / (board.TileSize + board.BorderThickness));

            //check if covers at least half of the top tile
            if ((int)((currentPoint.Value.Y + board.TileSize / 2) / (board.TileSize + board.BorderThickness)) != row)
                row++;

            return (row, column);
        }

        protected override void SetAvailableSpace(PointerPressedEventArgs e)
        {
            double currentTop = e.GetCurrentPoint(Parent).Position.Y - pointClicked.Y;
            minY = currentTop - car.CountUpAvailableCells() * (board.BorderThickness + board.TileSize);
            maxY = currentTop + Height + car.CountDownAvailableCells() * (board.BorderThickness + board.TileSize);
        }

        protected override void ClearAvailableSpace()
        {
            minY = null;
            maxY = null;
        }
    }
}
