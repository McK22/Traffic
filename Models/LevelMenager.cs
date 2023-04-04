using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Traffic.Models
{
    public class LevelMenager
    {
        private static readonly string savesPath = "saves";

        public static Board CreateBoard(int size, int carCount)
        {
            Board board = new Board(10);
            return board;
        }

        public static Board CreateLevel(int width, int height, int carCount, int movesToSolve)
        {
            return BoardCreator.CreateBoard(width, height, carCount, movesToSolve);
        }

        public static void SerializeBoard(Board board)
        {
            //Create saves directory if doesn't exist
            if (!Directory.Exists(savesPath))
                Directory.CreateDirectory(savesPath);

            //Set the save file name
            string fileName = DateTime.Now.Year.ToString() + '-';
            if (DateTime.Now.Month < 10)
                fileName += '0';
            fileName += DateTime.Now.Month.ToString() + '-';
            if (DateTime.Now.Day < 10)
                fileName += '0';
            fileName += DateTime.Now.Day.ToString() + ' ';
            fileName += DateTime.Now.TimeOfDay.ToString().Substring(0, DateTime.Now.TimeOfDay.ToString().IndexOf('.')).Replace(':', ';') + ".txt";
            string path = Path.Combine(savesPath, fileName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BoardWrapper));
            BoardWrapper boardWrapper = new BoardWrapper(board);
            using FileStream stream = new FileStream(path, FileMode.Create);
            xmlSerializer.Serialize(stream, boardWrapper);
        }

        public static Board DeserializeBoard(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BoardWrapper));
            using FileStream stream = new FileStream(path, FileMode.Open);
            BoardWrapper? boardWrapper = xmlSerializer.Deserialize(stream) as BoardWrapper;
            if (boardWrapper == null)
                throw new NullReferenceException();
            return boardWrapper.GenerateBoard();
        }

        public static List<Board> LoadBoards()
        {
            List<Board> boards = new List<Board>();
            foreach(string saveFile in Directory.EnumerateFiles(savesPath))
                boards.Add(DeserializeBoard(saveFile));
            return boards;
        }
    }
}
