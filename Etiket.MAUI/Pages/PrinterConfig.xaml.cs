using Etiket.DtoClient;
using Etiket.DtoClient.Models;
using System.Collections.ObjectModel;
using System.Text;

namespace Etiket.MAUI.Pages;

public partial class PrinterConfig : ContentPage
{
    private readonly ApiClientService _apiClientService;
    public event Action<string> OnIpAddressSaved;

    public PrinterConfig(ApiClientService apiClientService)
    {
        _apiClientService = apiClientService;
        InitializeComponent();
        var printerIp = Preferences.Get("PrinterIp", string.Empty);
        PrinterIpEntry.Text = printerIp;
        PrinterIpEntry.Focus();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var ipAddress = PrinterIpEntry.Text;
        if (string.IsNullOrEmpty(ipAddress))
        {
            await DisplayAlert("Error", "Ip Adresi bo� b�rak�lamaz!", "OK");
            return;
        }
        var ipPattern = @"^(\d{1,3}\.){3}\d{1,3}$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(ipAddress, ipPattern))
        {
            await DisplayAlert("Error", "Ge�ersiz IP Adresi format�!", "OK");
            return;
        }

        // IP adresindeki her say�n�n 0-255 aras�nda olup olmad���n� kontrol et
        var parts = ipAddress.Split('.');
        foreach (var part in parts)
        {
            if (!int.TryParse(part, out int num) || num < 0 || num > 255)
            {
                await DisplayAlert("Error", "Ge�ersiz IP Adresi!", "OK");
                return;
            }
        }

        OnIpAddressSaved?.Invoke(ipAddress);
        Preferences.Set("PrinterIp", ipAddress); // IP adresini kaydet
        await Shell.Current.GoToAsync("///MainPage");
    }
    private void OnEntryFocused(object sender, FocusEventArgs e)
    {
        PrinterIpEntry.Focus();
    }
    private async void OnBackToMainPageClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MainPage");
    }
}
