using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic.Models
{
    public class BoardCreator
    {

        public static Task<Board> CreateBoardAsync(int width, int height, int carCount, int movesToSolve)
        {
            return new Task<Board>(() => CreateBoard(width, height, carCount, movesToSolve));
        }

        public static Board CreateBoard(int width, int height, int carCount, int movesToSolve)
        {
            const int randomMoves = 1000;
            Board? board = null;
            do
            {
                while(board is null)
                    try
                    {
                        board = new Board(carCount);
                    }
                    catch (WrongPlacementException)
                    {
                        continue;
                    }

                for (int i = 0; i < randomMoves; i++)
                    board.RandomMove();

            } while (board.Solve() < movesToSolve);
            return board;
        }
    }
}
