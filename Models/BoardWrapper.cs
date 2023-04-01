using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic.Models;

public class BoardWrapper
{
    public struct Car
    {
        public int Row;
        public int Column;
        public Orientation Orientation;
        public int Id;
        public int Length;
    }

    public int Rows { get; set; } = -1;
    public int Columns { get; set; } = -1;
    public Car[] Cars { get; set; } = new Car[0];
    public Position SpecialCarPos { get; set; } = new Position(-1, -1);

    private BoardWrapper() { }

    public BoardWrapper(Board board)
    {
        Rows = board.Rows;
        Columns = board.Columns;
        Cars = new Car[board.Cars.Length - 1];
        for (int i = 0; i < Cars.Length; i++)
        {
            Cars[i] = new Car();
            Cars[i].Row = board.Cars[i + 1].Position.Row;
            Cars[i].Column = board.Cars[i + 1].Position.Column;
            Cars[i].Orientation = board.Cars[i + 1].Orientation;
            Cars[i].Id = board.Cars[i + 1].Id;
            Cars[i].Length = board.Cars[i + 1].Length;
        }
        SpecialCarPos = board.Cars[0].Position;
    }

    public Board GenerateBoard()
    {
        ConfigurableBoard board = new ConfigurableBoard();
        board.MoveSpecialCar(new Position(SpecialCarPos.Row, SpecialCarPos.Column));
        foreach(Car car in Cars)
            board.AddCar(new Position(car.Row, car.Column), car.Length, car.Orientation);
        return board;
    }
}
