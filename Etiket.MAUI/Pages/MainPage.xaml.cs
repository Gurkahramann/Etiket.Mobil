using Etiket.DtoClient;
using Etiket.MAUI.Pages;

namespace Etiket.MAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiClientService _apiClientService;
        public MainPage(ApiClientService apiClientService)
        {
            InitializeComponent();
            _apiClientService = apiClientService;
        }
        private async void OnOpenPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrinterConfig(_apiClientService));
        }
        private void OnEtiketYazdirClicked(object sender, EventArgs e)
        {
            // Burada etiket yazdırma işlemleri yapılacak
        }

        private void OnCikisClicked(object sender, EventArgs e)
        {
            // Uygulamadan çıkış işlemi yapılacak
        }
    }

}
