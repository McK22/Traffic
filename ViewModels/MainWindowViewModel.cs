using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Traffic.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase content;
        
        public MainWindowViewModel()
        {
            //content = new GameViewModel();
            content = new LevelBrowseViewModel(this);
            //content = new GameViewModel(new BoardObjects.BoardCanvas(40, new Models.Board(10)));
        }

        public void SwitchContent(ViewModelBase vm)
        {
            this.RaiseAndSetIfChanged(ref content, vm);
        }

        public ViewModelBase Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }
    }
}
