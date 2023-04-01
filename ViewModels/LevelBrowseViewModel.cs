using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traffic.ViewModels.BoardObjects;
using Traffic.Models;
using ReactiveUI;
using System.Reactive;

namespace Traffic.ViewModels
{
    internal class LevelBrowseViewModel : ViewModelBase
    {
        private const double tileSize = 10;

        private readonly MainWindowViewModel mainWindowViewModel;

        private BoardCanvas? selectedLevel = null;

        public LevelBrowseViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            List<Board> boards = LevelMenager.LoadBoards();
            foreach (Board board in boards)
            {
                BoardCanvas boardView = new BoardCanvas(tileSize, board);
                boardView.IsEnabled = false;
                Levels.Add(boardView);
            }
            
            StartLevelCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedLevel is null)
                    return;
                GameViewModel gvm = new GameViewModel(new BoardCanvas(50, SelectedLevel.Board));
                mainWindowViewModel.Content = gvm;
            });
        }

        public ObservableCollection<BoardCanvas> Levels { get; } = new();

        public BoardCanvas? SelectedLevel
        {
            get => selectedLevel;
            set => this.RaiseAndSetIfChanged(ref selectedLevel, value);
        }

        public ReactiveCommand<Unit, Unit> StartLevelCommand { get; }

        public void GenerateLevels()
        {
            Board newBoard = LevelMenager.CreateBoard(1, 10);
            BoardCanvas boardView = new BoardCanvas(tileSize, newBoard);
            boardView.IsEnabled = false;
            Levels.Add(boardView);
            LevelMenager.SerializeBoard(newBoard);
        }
    }
}
