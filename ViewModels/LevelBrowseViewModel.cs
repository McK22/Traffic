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
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Traffic.ViewModels
{
    internal class LevelBrowseViewModel : ViewModelBase
    {
        private const double tileSize = 10;

        private readonly MainWindowViewModel mainWindowViewModel;

        private BoardCanvas? selectedLevel = null;

        private bool startingLevelPossible = false;

        public LevelBrowseViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            List<Board> boards = LevelMenager.LoadBoards();
            foreach (Board board in boards)
            {
                BoardCanvas boardView = new(tileSize * board.Columns, tileSize * board.Rows)
                {
                    Board = board,
                    IsEnabled = false
                };
                Levels.Add(boardView);
            }
            
            StartLevelCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedLevel is null)
                    return;
                GameViewModel gvm = new(new BoardCanvas(50 * SelectedLevel.Board!.Columns, 50 * SelectedLevel.Board.Rows)
                                    { Board = SelectedLevel.Board }, mainWindowViewModel);
                mainWindowViewModel.Content = gvm;
            });
        }

        public ObservableCollection<BoardCanvas> Levels { get; } = new();

        public BoardCanvas? SelectedLevel
        {
            get => selectedLevel;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedLevel, value);
                StartingLevelPossible = selectedLevel == null ? false : selectedLevel.Board is not null;
            }
        }

        public bool StartingLevelPossible
        {
            get => startingLevelPossible;
            set => this.RaiseAndSetIfChanged(ref startingLevelPossible, value);
        }

        public ReactiveCommand<Unit, Unit> StartLevelCommand { get; }

        public void GenerateLevelsAsync()
        {
            const int rows = 6;
            const int columns = 6;
            const int movesToSolve = 15;
            const int carCount = 10;
            Task.Run(() =>
            {
                BoardCanvas? boardView = null;

                Dispatcher.UIThread.Post(() =>
                {
                    boardView = new(tileSize * columns, tileSize * rows) { IsEnabled = false };
                    boardView.Draw();
                    Levels.Add(boardView);
                });

                Board newBoard = LevelMenager.CreateLevel(6, 6, carCount, movesToSolve);
                LevelMenager.SerializeBoard(newBoard);

                Dispatcher.UIThread.Post(() =>
                {
                    boardView!.Board = newBoard;
                    if (SelectedLevel == boardView)
                        StartingLevelPossible = true;
                });
            });
        }
    }
}
