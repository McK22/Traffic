using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Remote.Protocol.Input;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traffic.Models;

namespace Traffic.ViewModels.BoardObjects
{
    internal abstract class CarRectangle : Rectangle
    {
        protected readonly Car car;
        protected readonly BoardCanvas board;

        protected bool isClicked = false;
        protected Point pointClicked;

        public CarRectangle(Car car, BoardCanvas board)
        {
            this.car = car;
            this.board = board;
            PointerPressed += PointerPressedHandler;
            PointerReleased += PointerReleaseHandler;
        }

        private void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
        {
            pointClicked = e.GetCurrentPoint(this).Position;
            SetAvailableSpace(e);
            isClicked = true;
        }

        private void PointerReleaseHandler(object? sender, PointerReleasedEventArgs e)
        {
            ClearAvailableSpace();
            isClicked = false;
            SetPosition();
        }

        private void SetPosition()
        {
            (int row, int column) = GetPos();

            Point newPos = board.GetTileUpperLeftCorner(row, column);
            Canvas.SetLeft(this, newPos.X);
            Canvas.SetTop(this, newPos.Y);
            if(!car.IsSpecial || column + car.Length - 1 < board.Columns)
                car.Move(new Position(row, column));
            if (column == board.Columns)
                board.Win();
        }

        protected abstract (int row, int column) GetPos();

        protected abstract void MoveHandler(object? sender, PointerEventArgs args);

        protected abstract void SetAvailableSpace(PointerPressedEventArgs e);

        protected abstract void ClearAvailableSpace();
    }
}
