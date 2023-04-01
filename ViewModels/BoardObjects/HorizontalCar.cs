using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Traffic.Models;

namespace Traffic.ViewModels.BoardObjects
{
    internal class HorizontalCar : CarRectangle
    {
        private double? minX = null;
        private double? maxX = null;

        public HorizontalCar(Car car, BoardCanvas board) : base(car, board)
        {
            PointerMoved += MoveHandler;
        }

        protected override void MoveHandler(object? sender, PointerEventArgs e)
        {
            if (!isClicked)
                return;

            Point pointOnBoard = e.GetCurrentPoint(Parent).Position;

            double leftX = pointOnBoard.X - pointClicked.X;
            if (minX <= leftX && leftX + Width <= maxX)
                Canvas.SetLeft(this, leftX);
        }

        protected override (int row, int column) GetPos()
        {
            Point? currentPoint = this.TranslatePoint(new Point(0, 0), this.GetVisualParent());
            if (currentPoint == null)
                throw new NullReferenceException(nameof(currentPoint));

            int row = (int)(currentPoint.Value.Y / (board.TileSize + board.BorderThickness));
            int column = (int)(currentPoint.Value.X / (board.TileSize + board.BorderThickness));

            //check if covers at least half of the left tile
            if ((int)((currentPoint.Value.X + board.TileSize / 2) / (board.TileSize + board.BorderThickness)) != column)
                column++;

            return (row, column);
        }

        protected override void SetAvailableSpace(PointerPressedEventArgs e)
        {
            double currentLeft = e.GetCurrentPoint(this.GetVisualParent()).Position.X - pointClicked.X;
            minX = currentLeft - car.CountLeftAvailableCells() * (board.BorderThickness + board.TileSize);
            maxX = currentLeft + Width + car.CountRightAvailableCells() * (board.BorderThickness + board.TileSize);
            if (car.IsSpecial && maxX >= board.Width - 2 * board.BorderThickness)
                maxX += board.BorderThickness + Width;
        }

        protected override void ClearAvailableSpace()
        {
            minX = null;
            maxX = null;
        }
    }
}
