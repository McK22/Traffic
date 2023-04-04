using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic.Models
{
    public class Car
    {
        public int Length { get; } 
        public Orientation Orientation { get; }
        public int Id { get; }
        public bool IsSpecial { get => isSpecial; }

        public Position Position { get => position; }

        protected Position position;  //represents the position of top/left end of a car
        protected Board board;
        protected Random random;
        protected bool isSpecial = false;

        public Car(int length, Orientation orientation, int id, Board board)
        {
            Length = length;
            Orientation = orientation;
            Id = id;
            this.board = board;
            random = new Random();
            Place();
        }

        protected virtual void Place()
        {
            Position[] availablePos = board.GetAvailablePos(Length, Orientation);
            if (availablePos.Length == 0)
                throw new WrongPlacementException();

            int pos = random.Next(availablePos.Length);
            
            position = availablePos[pos];
            TakeBoardCells();
        }

        public void RandomMove()
        {
            Position[] availablePos = GetAvailablePosToMove();
            if (availablePos.Length == 0)
                return;

            int pos = random.Next(availablePos.Length);
            FreeBoardCells();
            position = availablePos[pos];
            TakeBoardCells();
        }

        public void Move(Position destination)
        {
            FreeBoardCells();
            position = destination;
            TakeBoardCells();
        }

        public int CountLeftAvailableCells()
        {
            int column = position.Column;
            int result = 0;
            while (board.IsFree(position.Row, --column))
                result++;
            return result;
        }

        public int CountRightAvailableCells()
        {
            int column = position.Column + Length;
            int result = 0;
            while (board.IsFree(position.Row, column++))
                result++;
            return result;
        }

        public int CountUpAvailableCells()
        {
            int row = position.Row;
            int result = 0;
            while (board.IsFree(--row, position.Column))
                result++;
            return result;
        }

        public int CountDownAvailableCells()
        {
            int row = position.Row + Length;
            int result = 0;
            while (board.IsFree(row++, position.Column))
                result++;
            return result;
        }

        public Position[] GetAvailablePosToMove()
        {
            List<Position> availablePos = new List<Position>();

            if(Orientation == Orientation.Horizontal)
            {
                int column = position.Column - 1;
                while (board.IsFree(position.Row, column))
                {
                    availablePos.Add(new Position(position.Row, column));
                    column--;
                }
                column = position.Column + 1;
                while(board.IsFree(position.Row, column + Length - 1))
                {
                    availablePos.Add(new Position(position.Row, column));
                    column++;
                }
            }
            else
            {
                int row = position.Row - 1;
                while(board.IsFree(row, position.Column))
                {
                    availablePos.Add(new Position(row, position.Column));
                    row--;
                }
                row = position.Row + 1;
                while(board.IsFree(row + Length - 1, position.Column))
                {
                    availablePos.Add(new Position(row, position.Column));
                    row++;
                }
            }
            return availablePos.ToArray();
        }

        protected void TakeBoardCells()
        {
            for (int i = 0; i < Length; i++)
            {
                if (Orientation == Orientation.Horizontal)
                    board.Take(position.Row, position.Column + i, this);
                else
                    board.Take(position.Row + i, position.Column, this);
            }
        }

        protected void FreeBoardCells()
        {
            for (int i = 0; i < Length; i++)
            {
                if (Orientation == Orientation.Horizontal)
                    board.Free(position.Row, position.Column + i);
                else
                    board.Free(position.Row + i, position.Column);
            }
        }
    }

    public class SpecialCar : Car
    {
        private const int length = 2;
        private const Orientation orientation = Orientation.Horizontal;
        private readonly Position startingPosition = Board.finishPoint;

        public SpecialCar(Board board): base(length, orientation, 0, board)
        {
            isSpecial = true;
        }

        protected override void Place()
        {
            position = startingPosition;
            TakeBoardCells();
        }
    }

    public enum Orientation
    {
        Horizontal, Vertical
    }
}
