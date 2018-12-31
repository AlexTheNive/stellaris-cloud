using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ZoksStellarisEmpires
{
    public interface IEmpireService
    {
        ObservableCollection<Empire> Empires { get; set; }

        Task Load();
    }   
}