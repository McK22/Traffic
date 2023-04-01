using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traffic.Models;
using Traffic.ViewModels.BoardObjects;

namespace Traffic.ViewModels
{
    internal class GameViewModel : ViewModelBase
    {
        public GameViewModel(BoardCanvas boardCanvas)
        {
            BoardCanvas = boardCanvas;
        }

        public BoardCanvas BoardCanvas { get; }
    }
}
