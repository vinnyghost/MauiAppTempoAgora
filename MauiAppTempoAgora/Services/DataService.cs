using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net; // Importação necessária para o HttpStatusCode

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            // Parte 2: Verifica conexão com a internet
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new Exception("Sem conexão com a internet. Verifique sua rede e tente novamente.");
            }

            Tempo? t = null;
            string chave = "6135072afe7f6cec1537d5cb08a5a1a2";

            // Adicionado o parâmetro &lang=pt_br na URL
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&lang=pt_br&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();
                    var rascunho = JObject.Parse(json);

                    // CORREÇÃO DA DATA: Iniciando em 1 de Janeiro de 1970 (Padrão Unix)
                    DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                    DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                    t = new()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],

                        // Arredondando as temperaturas para remover as casas decimais (Opcional, mas fica melhor na UI)
                        temp_min = Math.Round((double)rascunho["main"]["temp_min"]),
                        temp_max = Math.Round((double)rascunho["main"]["temp_max"]),

                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString("dd/MM/yyyy HH:mm:ss"), // Formatando para ficar bonito
                        sunset = sunset.ToString("dd/MM/yyyy HH:mm:ss"),
                    };
                }
                // Parte 2: Tratamento de erro quando a cidade não é encontrada (Erro 404)
                else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("Cidade não encontrada. Verifique o nome digitado e tente novamente.");
                }
                // Outros erros genéricos de servidor ou API
                else
                {
                    throw new Exception($"Erro de comunicação com o servidor: {resp.StatusCode}");
                }
            }

            return t;
        }
    }
}