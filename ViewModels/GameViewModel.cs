using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Traffic.Models;
using Traffic.ViewModels.BoardObjects;

namespace Traffic.ViewModels
{
    internal class GameViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly LevelBrowseViewModel levelBrowseViewModel;

        public GameViewModel(BoardCanvas boardCanvas, MainWindowViewModel mainWindowViewModel)
        {
            BoardCanvas = boardCanvas;
            this.mainWindowViewModel = mainWindowViewModel;
            this.levelBrowseViewModel = levelBrowseViewModel;

            ReturnToLevelBrowserCommand = ReactiveCommand.Create(() =>
            {
                mainWindowViewModel.Content = new LevelBrowseViewModel(mainWindowViewModel);
            });
        }

        public ReactiveCommand<Unit, Unit> ReturnToLevelBrowserCommand { get; }

        public BoardCanvas BoardCanvas { get; }
    }
}
