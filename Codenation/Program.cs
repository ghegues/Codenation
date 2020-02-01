using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Codenation
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string TOKEN = "a062c8db87bf1851e0b465ccee0dab9c31f7ff96";
        static async Task Main(string[] args)
        {
            var repo = await ProcessRepositories();
            repo.decifrado = JulioCesarCriptography(repo.cifrado, repo.numero_casas);
            repo.resumo_criptografico = CalculateSHA1(repo.decifrado);
            try
            {
                repo.Save();
                HttpPost();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao salvar arquivo: {e.Message}");
            }
        }
       

        private static async Task<Repository> ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");

            var streamTask = await client.GetStreamAsync($"https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token={TOKEN}");
            var repository = await JsonSerializer.DeserializeAsync<Repository>(streamTask);
            return repository;
        }
        public static string JulioCesarCriptography(string CriptographyMessage, int NumeroCasas)
        {
            string descripto = "";
            string alfabeto = "abcdefghijklmnopqrstuvwxyz";
            foreach (var item in CriptographyMessage.ToCharArray())
            {
                var letter = item.ToString().ToLower();
                if (alfabeto.Contains(letter))
                {
                    int i = alfabeto.IndexOf(letter);
                    i = (i + NumeroCasas) % 26;
                    descripto += alfabeto[i].ToString();
                }
                else
                {
                    descripto += letter;
                }
            }
            Console.WriteLine(descripto);
            return descripto;
        }
        public static string CalculateSHA1(string text)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(text);
                System.Security.Cryptography.SHA1CryptoServiceProvider cryptoTransformSHA1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
                return hash.ToLower();
            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }
        }

        private static void HttpPost()
        {
            try
            {
                byte[] arquivoByte = File.ReadAllBytes(@"answer.json");
                string requestURL = $"https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token={TOKEN}";

                using (MultipartFormDataContent tipoConteudo = new MultipartFormDataContent())
                {
                    tipoConteudo.Add(new StreamContent(new MemoryStream(arquivoByte)), "answer", "answer.json");
                    HttpResponseMessage resposta = new HttpClient().PostAsync(requestURL, tipoConteudo).Result;
                    var resp = resposta.Content.ReadAsStringAsync().Result;

                    Console.WriteLine(resp);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

        }

    }
}
