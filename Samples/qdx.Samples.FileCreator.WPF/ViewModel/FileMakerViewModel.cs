using GalaSoft.MvvmLight.Command;
using Quotidian.Diagnostics.Source;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace qdx.Samples.FileCreator.WPF.ViewModel
{
    public class FileMakerViewModel : INotifyPropertyChanged
    {
        public FileMakerViewModel(FileCreator.Creator creator)
        {
            Creator = creator ?? throw new System.ArgumentNullException(nameof(creator));
        }

        public Creator Creator { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private string directory;
        public string SelectedDirectory
        {
            get => directory;
            set
            {
                if (directory == value)
                    return;
                if (value == null)
                    return;

                directory = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDirectory)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibleDirectories)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Files)));
            }
        }

        public IEnumerable<string> VisibleDirectories
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var dir in SelectedDirectory.Split("\\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries))
                {
                    builder.Append(dir).Append('\\');
                    yield return builder.ToString();
                }

                foreach (var dir in Directory.GetDirectories(SelectedDirectory))
                    yield return $"{dir.TrimEnd('\\')}\\";
            }
        }

        private int numberFiles;
        public int NumberFiles
        {
            get => numberFiles;
            set
            {
                if (numberFiles == value)
                    return;

                numberFiles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberFiles)));
            }
        }

        private TraceLevel traceLevel;
        public TraceLevel TraceLevel
        {
            get => traceLevel;
            set
            {
                if (traceLevel == value)
                    return;

                traceLevel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TraceLevel)));

            }
        }

        public IEnumerable<FileInfo> Files => new DirectoryInfo(SelectedDirectory).GetFiles();

        public ICommand CreateFiles => new RelayCommand(DoCreation);

        private void DoCreation()
        {
            Creator.MakeFiles(SelectedDirectory, NumberFiles);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Files)));
        }
    }
}
