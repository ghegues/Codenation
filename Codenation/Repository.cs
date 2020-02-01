using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace Codenation
{
    class Repository
    {
        [JsonPropertyName("numero_casas")]
        public int numero_casas { get; set; }

        [JsonPropertyName("token")]
        public string token { get; set; }

        [JsonPropertyName("cifrado")]
        public string cifrado { get; set; }

        [JsonPropertyName("decifrado")]
        public string decifrado { get; set; }

        [JsonPropertyName("resumo_criptografico")]
        public string resumo_criptografico { get; set; }

        public void Save()
        {
            if (File.Exists(@"answer.json"))
                File.Delete(@"answer.json");
            using (FileStream fs = File.Open(@"answer.json", FileMode.CreateNew))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, this);
            }
        }
    }
}
