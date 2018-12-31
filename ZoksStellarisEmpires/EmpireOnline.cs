using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace ZoksStellarisEmpires
{
    public class EmpireOnline : IEmpireService
    {
        public ObservableCollection<Empire> Empires { get; set; } = new ObservableCollection<Empire>();

        private const string API = "https://zok.xyz/stellarisapi";
        //private const string API = "http://localhost:8000/";

        public async Task Load()
        {
            Empires.Clear();
            var result = await API
                .AppendPathSegment("empire")
                .GetJsonAsync<EmpireList>().ConfigureAwait(true);

            foreach (var empire in result.Empires)
            {
                Empires.Add(empire);
            }
        }

        public async Task Add(Empire empire)
        {
            var result = await API
                .AppendPathSegments("empire", empire.GetHashId())
                .PostJsonAsync(empire);
        }

        public async Task Remove(Empire empire)
        {
            var result = await API
                .AppendPathSegments("empire", empire.GetHashId())
                .DeleteAsync();
        }
    }
}
