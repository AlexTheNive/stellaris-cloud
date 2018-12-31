using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace ZoksStellarisEmpires
{
    public class Empire
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "rawdata")]
        public string RawData { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string GetHashId()
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(Name)).Take(4);
            return BitConverter.ToString(hash.ToArray()).Replace("-", "").ToLower();
        }
    }
}