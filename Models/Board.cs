﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic.Models
{
    public class Board
    {
        private const int maxLongCars = 4;
        protected const int size = 6;
        private const int longCarLength = 3;
        private const int shortCarLength = 2;

        public static readonly Position finishPoint = new Position(2, 4);
        public int FinishRow { get => finishPoint.Row; }
        public int Rows { get => size; }
        public int Columns { get => size; }

        private Car[,] board;
        protected Car[] cars;
        private Random random;
        protected int nextCarId = 0;

        public Board(int carCount)
        {
            board = new Car[size, size];
            cars = new Car[carCount];
            random = new Random();
            InitCars();
        }

        public int? this[int i, int j]
        {
            get
            {
                if (board[i, j] is null)
                    return null;
                return board[i, j].Id;
            }
        }

        public Car[] Cars
        {
            get => cars;
        }

        public bool IsFree(int row, int column)
        {
            if (row < 0 || row >= size || column < 0 || column >= size)
                return false;

            return board[row, column] is null;
        }

        public bool IsAvailableForCar(int row, int column, Car car)
        {
            return IsFree(row, column) || (row >= 0 && row < size && column >= 0 && column < size && board[row, column] == car);
        }

        public void Free(int row, int column)
        {
            if (board[row, column] is null)
                throw new ArgumentException("The cell is not taken");

            board[row, column] = null;
        }

        public void Take(int row, int column, Car car)
        {
            if (board[row, column] is not null)
                throw new ArgumentException("The cell is already taken");

            board[row, column] = car;
        }

        //get available position for a new car
        public Position[] GetAvailablePos(int length, Orientation orientation)
        {
            List<Position> availablePos = new List<Position>();
            for (int row = 0; row < size; row++)
            {
                for(int column = 0; column < size; column++)
                {
                    bool isAvailable = true;
                    for(int i = 0; i < length; i++)
                    {
                        if (orientation == Orientation.Horizontal)
                            isAvailable = isAvailable && IsFree(row, column + i);
                        else
                            isAvailable = isAvailable && IsFree(row + i, column);
                    }
                    if (isAvailable)
                        availablePos.Add(new Position(row, column));
                }
            }

            return availablePos.ToArray();
        }

        public void RandomMove()
        {
            int car = random.Next(cars.Length);
            cars[car].RandomMove();
        }

        private void InitCars()
        {
            int longCars = random.Next(Math.Min(cars.Length - 1, maxLongCars) + 1);
            cars[0] = new SpecialCar(this);
            PlaceCars(longCarLength, longCars, 1);
            PlaceCars(shortCarLength, cars.Length - longCars - 1, longCars + 1);
        }

        private void PlaceCars(int length, int count, int begin)
        {
            for(int i = begin; i < count + begin; i++)
            {
                Orientation orientation = (Orientation)random.Next(2);
                cars[i] = new Car(length, orientation, nextCarId++, this);
            }
        }

        private int[,] GetMatrixRepresentation()
        {
            int[,] matrix = new int[Rows, Columns];
            int[] carNumber = new int[Cars.Length];
            for (int i = 0; i < carNumber.Length; i++)
                carNumber[i] = -1;

            int nextCarNumber = 0;
            for(int i = 0; i < Rows; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    if (board[i, j] is null)
                        matrix[i, j] = -1;
                    else
                    {
                        if (carNumber[board[i, j].Id] == -1)
                            carNumber[board[i, j].Id] = nextCarNumber++;
                        matrix[i, j] = carNumber[board[i, j].Id];
                    }
                }
            }

            return matrix;
        }

        public bool Shuffle(int maxDepth)
        {
            return Shuffle(maxDepth, 0, new Stack<int[,]>());
        }

        private bool Shuffle(int maxDepth, int currentDepth, Stack<int[,]> prevBoardMatrices)
        {
            if (currentDepth >= maxDepth)
                return true;

            foreach (int[,] prevBoard in prevBoardMatrices)
                if (this == prevBoard)
                    return false;

            prevBoardMatrices.Push(GetMatrixRepresentation());
            foreach(Car car in cars)
            {
                Position[] positions = car.GetAvailablePosToMove();
                foreach (Position position in positions)
                {
                    Position prevPos = car.Position;
                    car.Move(position);
                    if (Shuffle(maxDepth, currentDepth + 1, prevBoardMatrices))
                        return true;
                    car.Move(prevPos);
                }
            }

            return false;
        }

        public override bool Equals(object? otherMatrix)
        {
            if (otherMatrix is not int[,] matrix || Rows != matrix.GetLength(0) || Columns != matrix.GetLength(1))
                return false;

            int[,] thisMatrix = GetMatrixRepresentation();
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    if (thisMatrix[i, j] != matrix[i, j])
                        return false;

            return true;
        }

        public static bool operator ==(Board? left, int[,]? right)
        {
            if (left is null || right is null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(Board? left, int[,]? right) => !(left == right);

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }


        /*public void Print()
        {
            for (int i = 0; i < size + 2; i++)
                Console.Write('-');
            Console.WriteLine();

            for(int i = 0; i < size; i++)
            {
                Console.Write('|');
                for(int j = 0; j < size; j++)
                {
                    if (board[j, i] is null)
                        Console.Write('.');
                    else
                    {
                        Console.ForegroundColor = board[j, i].Color;
                        if (board[j, i].Orientation == Orientation.Horizontal)
                            Console.Write('-');
                        else
                            Console.Write('|');
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                if (i != finishPoint.Y)
                    Console.Write('|');
                Console.WriteLine();
            }

            for (int i = 0; i < size + 2; i++)
                Console.Write('-');
            Console.WriteLine();
        }*/

        //public (List<(ConsoleColor, int)>, int) Solve()
        //{
        //    Point[][] initAvailablePos = new Point[cars.Length][];
        //    for (int i = 0; i < cars.Length; i++)
        //        initAvailablePos[i] = new Point[size * size];

        //    bool[] toMove = new bool[cars.Length];
        //    for (int i = 0; i < cars.Length; i++)
        //        toMove[i] = true;

        //    (List<(ConsoleColor, int)> list, int moves) result;
        //    result.list = new List<(ConsoleColor, int)>();
        //    result.moves = int.MaxValue;
        //    for (int i = 0; result.moves == int.MaxValue && i <= Program.MaxDepth; i++)
        //    {
        //        for (int j = 0; j < cars.Length; j++)
        //            toMove[j] = true;
        //        result = Solve(0, i, toMove);
        //    }

        //    //old version
        //    //int movesToSolve = int.MaxValue;
        //    //for (int i = 0; movesToSolve == int.MaxValue && i <= Program.MaxDepth; i++)
        //    //    movesToSolve = Solve(0, i, -1);

        //    return result;
        //}

        //public int OldSolve(int depth, int maxDepth, int lastMoved)
        //{
        //    if (depth > maxDepth)
        //        return int.MaxValue;

        //    if (cars[0].Position.X == 4)
        //        return depth;

        //    Point[] availablePosForSpecialCar = cars[0].GetAvailablePosToMove();
        //    for (int i = 0; i < availablePosForSpecialCar.Length; i++)
        //        if (availablePosForSpecialCar[i].X == 4)
        //            return depth + 1;


        //    Point[][] availablePos;
        //    availablePos = new Point[cars.Length][];
        //    for (int i = 0; i < cars.Length; i++)
        //        availablePos[i] = cars[i].GetAvailablePosToMove();

        //    int result = int.MaxValue;
        //    for (int i = 0; i < cars.Length; i++)
        //    {
        //        if (i == lastMoved)
        //            continue;

        //        for (int j = 0; j < availablePos[i].Length; j++)
        //        {
        //            Point oldPos = cars[i].Position;
        //            cars[i].Move(availablePos[i][j]);
        //            result = Math.Min(result, OldSolve(depth + 1, maxDepth, i));
        //            cars[i].Move(oldPos);
        //        }
        //    }

        //    return result;
        //}

        //public (List<(ConsoleColor, int)>, int) Solve(int depth, int maxDepth, bool[] toMove)
        //{
        //    if (depth > maxDepth)
        //        return (new List<(ConsoleColor, int)>(), int.MaxValue);

        //    if (cars[0].Position.X == 4)
        //        return (new List<(ConsoleColor, int)>(), depth);

        //    Point[] availablePosForSpecialCar = cars[0].GetAvailablePosToMove();
        //    for (int i = 0; i < availablePosForSpecialCar.Length; i++)
        //    {
        //        if (availablePosForSpecialCar[i].X == finishPoint.X)
        //        {
        //            List<(ConsoleColor, int)> solution = new List<(ConsoleColor, int)>();
        //            solution.Add((cars[0].Color, finishPoint.X - cars[0].Position.X));
        //            return (solution, depth + 1);
        //        }
        //    }


        //    Point[][] availablePos = GetAvailablePosForEachCar();

        //    bool[] oldToMove = new bool[toMove.Length];
        //    for (int i = 0; i < toMove.Length; i++)
        //        oldToMove[i] = toMove[i];

        //    (List<(ConsoleColor, int)> list, int moves) result;
        //    result.list = new List<(ConsoleColor, int)>();
        //    result.moves = int.MaxValue;
        //    for(int i = 0; i < cars.Length; i++)
        //    {
        //        if (!toMove[i])
        //            continue;

        //        for(int j = 0; j < availablePos[i].Length; j++)
        //        {
        //            Point oldPos = cars[i].Position;

        //            cars[i].Move(availablePos[i][j]);

        //            Point[][] newAvailablePos = GetAvailablePosForEachCar();
        //            for (int k = 0; k < cars.Length; k++)
        //                if (newAvailablePos[k].Length > availablePos[k].Length)
        //                    toMove[k] = true;
        //            toMove[i] = false;

        //            (List<(ConsoleColor, int)> list, int moves) res = Solve(depth + 1, maxDepth, toMove);
        //            if(res.moves < result.moves)
        //            {
        //                result.moves = res.moves;
        //                result.list = new List<(ConsoleColor, int)>();
        //                if (cars[i].Orientation == Orientation.Horizontal)
        //                    result.list.Add((cars[i].Color, availablePos[i][j].X - cars[i].Position.X));
        //                else
        //                    result.list.Add((cars[i].Color, availablePos[i][j].Y - cars[i].Position.Y));
        //                result.list.Concat(res.list);
        //            }

        //            cars[i].Move(oldPos);

        //            toMove = new bool[oldToMove.Length];
        //            for(int k = 0; k < oldToMove.Length; k++)
        //                toMove[k] = oldToMove[k];
        //        }
        //    }

        //    return result;
        //}

        public int Solve()
        {
            bool[] toMove = new bool[cars.Length];
            for (int i = 0; i < toMove.Length; i++)
                toMove[i] = true;

            int movesToSolve = int.MaxValue;
            int maxDepth = 8;
            for (int i = 0; movesToSolve == int.MaxValue && i <= maxDepth; i++)
            {
                for (int j = 0; j < toMove.Length; j++)
                    toMove[j] = true;

                movesToSolve = Solve(0, i, toMove);
            }

            return movesToSolve;
        }

        public int Solve(int depth, int maxDepth, bool[] toMove)
        {
            if (depth > maxDepth)
                return int.MaxValue;

            if (cars[0].Position.Column == finishPoint.Column)
                return depth;

            Position[] availablePosForSpecialCar = cars[0].GetAvailablePosToMove();
            for (int i = 0; i < availablePosForSpecialCar.Length; i++)
                if (availablePosForSpecialCar[i].Column == finishPoint.Column)
                    return depth + 1;

            bool[] oldToMove = new bool[toMove.Length];
            for (int i = 0; i < toMove.Length; i++)
                oldToMove[i] = toMove[i];

            Position[][] availablePos = GetAvailablePosForEachCar();
            int result = int.MaxValue;
            for (int i = 0; i < cars.Length; i++)
            {
                if (!toMove[i])
                    continue;

                for (int j = 0; j < availablePos[i].Length; j++)
                {
                    Position oldPos = cars[i].Position;

                    cars[i].Move(availablePos[i][j]);

                    Position[][] newAvailablePos = GetAvailablePosForEachCar();
                    for (int k = 0; k < cars.Length; k++)
                        if (newAvailablePos[k].Length > availablePos[k].Length)
                            toMove[k] = true;

                    if (cars[i].Length == shortCarLength)
                    {
                        if (cars[i].Orientation == Orientation.Horizontal)
                        {
                            int diff = cars[i].Position.Row - oldPos.Row;
                            for (int k = shortCarLength; k < diff; k++)
                                foreach (int car in GetCarsPassed(new Position(oldPos.Row + k, oldPos.Column), Orientation.Horizontal))
                                    toMove[car] = true;

                            for (int k = -1; k > diff + 1; k--)
                                foreach (int car in GetCarsPassed(new Position(oldPos.Row + k, oldPos.Column), Orientation.Horizontal))
                                    toMove[car] = true;
                        }
                        else
                        {
                            int diff = cars[i].Position.Column - oldPos.Column;
                            for (int k = shortCarLength; k < diff; k++)
                                foreach (int car in GetCarsPassed(new Position(oldPos.Row, oldPos.Column + k), Orientation.Vertical))
                                    toMove[car] = true;

                            for (int k = -1; k > diff + 1; k--)
                                foreach (int car in GetCarsPassed(new Position(oldPos.Row, oldPos.Column + k), Orientation.Vertical))
                                    toMove[car] = true;
                        }
                    }

                    toMove[i] = false;

                    result = Math.Min(result, Solve(depth + 1, maxDepth, toMove));

                    cars[i].Move(oldPos);

                    toMove = new bool[oldToMove.Length];
                    for (int k = 0; k < oldToMove.Length; k++)
                        toMove[k] = oldToMove[k];
                }
            }

            return result;
        }

        private List<int> GetCarsPassed(Position position, Orientation orientation)
        {
            if(!IsFree(position.Row, position.Column))
                return new List<int>();

            List<int> result = new List<int>();
            Position currentPos = position;
            if(orientation == Orientation.Horizontal)
            {
                while (IsFree(currentPos.Row, currentPos.Column))
                    currentPos.Column++;
                if (currentPos.Column < size)
                    result.Add(GetCarIndex(board[currentPos.Row, currentPos.Column]));

                currentPos = position;
                while (IsFree(currentPos.Row, currentPos.Column))
                    currentPos.Column--;
                if (currentPos.Column >= 0)
                    result.Add(GetCarIndex(board[currentPos.Row, currentPos.Column]));
            }
            else
            {
                while (IsFree(currentPos.Row, currentPos.Column))
                    currentPos.Row++;
                if (currentPos.Row < size)
                    result.Add(GetCarIndex(board[currentPos.Row, currentPos.Column]));

                currentPos = position;
                while (IsFree(currentPos.Row, currentPos.Column))
                    currentPos.Row--;
                if (currentPos.Row >= 0)
                    result.Add(GetCarIndex(board[currentPos.Row, currentPos.Column]));
            }
            return result;
        }

        private int GetCarIndex(Car car)
        {
            int i = 0;
            while (cars[i] != car)
                i++;
            return i;
        }

        private Position[][] GetAvailablePosForEachCar()
        {
            Position[][] availablePos;
            availablePos = new Position[cars.Length][];
            for (int i = 0; i < cars.Length; i++)
                availablePos[i] = cars[i].GetAvailablePosToMove();

            return availablePos;
        }
    }

    public class ConfigurableBoard : Board
    {
        public ConfigurableBoard() : base(1)
        {
            
        }

        public void MoveSpecialCar(Position pos)
        {
            cars[0].Move(pos);
        }

        public void AddCar(Position pos, int length, Orientation orientation)
        {
            Car car = new(length, orientation, nextCarId++, this);
            car.Move(pos);
            Car[] newCars = new Car[cars.Length + 1];
            for (int i = 0; i < cars.Length; i++)
                newCars[i] = cars[i];
            newCars[cars.Length] = car;
            cars = newCars;
        }
    }
}
