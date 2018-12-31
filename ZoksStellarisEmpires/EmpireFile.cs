using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using GalaSoft.MvvmLight;

namespace ZoksStellarisEmpires
{
    public class EmpireFile : IEmpireService
    {
        public string FileName { get; set; }

        public ObservableCollection<Empire> Empires { get; set; } = new ObservableCollection<Empire>();

        private readonly Regex EmpireStart = new Regex(@"^\x22(.+)\x22");
        private readonly Regex RawDataStart = new Regex(@"^=\{");
        private readonly Regex EmpireEnd = new Regex(@"^}");

        public EmpireFile(string fileName)
        {
            FileName = fileName;
        }

        public async Task Load()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Empires.Clear();
            });
            using (var reader = File.OpenText(FileName))
            {
                string line;
                Empire currentEmpire = null;
                while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    var match = EmpireStart.Match(line);
                    if (match.Success)
                    {
                        currentEmpire = new Empire {Name = match.Groups[1].Value};
                        continue;
                    }

                    //match = RawDataStart.Match(line);
                    //if (match.Success)
                    //{
                    //    currentEmpire.Name = match.Groups[1].Value;
                    //    continue;
                    //}
                    if (currentEmpire == null)
                        continue;
                    currentEmpire.RawData += line + "\r\n";
                    match = EmpireEnd.Match(line);
                    if (match.Success)
                    {
                        var empire = currentEmpire;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Empires.Add(empire);
                        });
                        currentEmpire = null;
                        continue;
                    }

                }
            }
        }

        public async Task Save()
        {
            try
            {
                File.Delete(FileName + ".bak");
                File.Move(FileName, FileName + ".bak");
                using (var writer = File.CreateText(FileName))
                {
                    foreach (var empire in Empires)
                    {
                        await writer.WriteLineAsync('"' + empire.Name + '"');
                        await writer.WriteAsync(empire.RawData);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}