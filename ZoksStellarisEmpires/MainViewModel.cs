using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ZoksStellarisEmpires
{
    public class MainViewModel : ViewModelBase
    {
        private EmpireFile _empireFile;

        public EmpireFile EmpireFile
        {
            get => _empireFile;
            set => Set(ref _empireFile, value);
        }

        private Empire _selectedLocalEmpire;

        public Empire SelectedLocalEmpire
        {
            get => _selectedLocalEmpire;
            set => Set(ref _selectedLocalEmpire, value);
        }

        private Empire _selectedOnlineEmpire;

        public Empire SelectedOnlineEmpire
        {
            get => _selectedOnlineEmpire;
            set => Set(ref _selectedOnlineEmpire, value);
        }

        private EmpireOnline _empireOnline;

        public EmpireOnline EmpireOnline
        {
            get => _empireOnline;
            set => Set(ref _empireOnline, value);
        }

        public ICommand UploadEmpiresCommand => new RelayCommand(UploadEmpires);
        public ICommand DeleteOnlineEmpireCommand => new RelayCommand<Empire>(RemoveOnlineEmpire);
        public ICommand SaveLocalEmpiresCommand => new RelayCommand(SaveLocalEmpires);

        public ICommand DownloadEmpiresCommand => new RelayCommand(DownloadEmpires);

        public ICommand RefreshEmpiresCommand => new RelayCommand(RefreshEmpires);

        private async void RefreshEmpires()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                EmpireFile = new EmpireFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    @"Paradox Interactive\Stellaris\user_empire_designs.txt"));
                EmpireOnline = new EmpireOnline();
            });
            await EmpireFile.Load();
            await EmpireOnline.Load();
        }

        private void DownloadEmpires()
        {
            var firstOrDefault = EmpireFile.Empires.FirstOrDefault(empire => empire.GetHashId() == SelectedOnlineEmpire.GetHashId());
            if (firstOrDefault == null)
                EmpireFile.Empires.Add(SelectedOnlineEmpire);
            else
            {
                firstOrDefault.RawData = SelectedOnlineEmpire.RawData;
            }
        }

        private void SaveLocalEmpires()
        {
            EmpireFile.Save();
        }

        private void RemoveOnlineEmpire(Empire empire)
        {
            if (empire != null)
            {
                EmpireOnline.Remove(empire);
                EmpireOnline.Load();
            }
        }

        private void UploadEmpires()
        {
            if (SelectedLocalEmpire != null)
            {
                EmpireOnline.Add(SelectedLocalEmpire);
                EmpireOnline.Load();
            }
            
        }

        public MainViewModel()
        {
            RefreshEmpires();
        }
    }
}