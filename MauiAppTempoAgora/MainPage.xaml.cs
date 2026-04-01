using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if(t != null) 
                    {
                        string dados_previsao = "";

                        // Parte 1: Inclusão da descrição, velocidade do vento e visibilidade
                        dados_previsao = $"Descrição: {t.description} \n" +
                                         $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp Máx: {t.temp_max}°C \n" +
                                         $"Temp Min: {t.temp_min}°C \n" +
                                         $"Velocidade do Vento: {t.speed} m/s \n" +
                                         $"Visibilidade: {t.visibility} metros \n";

                        lbl_res.Text = dados_previsao;
                    } 
                    else
                    {
                        lbl_res.Text = "Sem dados de Previsão";
                    }
                } 
                else
                {
                    await DisplayAlert("Aviso", "Por favor, preencha o nome da cidade.", "OK");
                }
            } 
            catch(Exception ex)
            {
                // Limpa o texto da label em caso de erro para não confundir o usuário
                lbl_res.Text = ""; 
                // Exibe o erro tratado na classe DataService
                await DisplayAlert("Atenção", ex.Message, "OK");
            }
        }
    }
}