#if ANDROID
using Android.Views.InputMethods;
#endif
using Etiket.DtoClient;
using Etiket.DtoClient.Models;
using Etiket.MAUI.Pages;
using Microsoft.Maui.Handlers;
using System.Text;
#if ANDROID || IOS || WINDOWS
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
#endif

namespace Etiket.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiClientService _apiClientService;
        public string PrinterIp { get; set; } = string.Empty;
        private bool isFirstConnection = true;
        public MainPage(ApiClientService apiClientService)
        {
            InitializeComponent();
            PrinterIp = Preferences.Get("PrinterIp", string.Empty);

            _apiClientService = apiClientService;
            EntryField.Focus(); // Entry'e odaklan
            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => { HideKeyboard(); };
            this.Content.GestureRecognizers.Add(tapGesture);
            // Sadece MainPage üzerinde klavyenin açýlmasýný engelle
            ApplyNoKeyboardEntry();
        }

        private void ApplyNoKeyboardEntry()
        {
            EntryHandler.Mapper.AppendToMapping("NoKeyboardEntry", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.ShowSoftInputOnFocus = false;
                handler.PlatformView.EditorAction += (sender, args) =>
                {
                    if (args.ActionId == ImeAction.Done)
                    {
                        var inputMethodManager = (InputMethodManager)handler.PlatformView.Context.GetSystemService(Android.Content.Context.InputMethodService);
                        inputMethodManager.HideSoftInputFromWindow(handler.PlatformView.WindowToken, HideSoftInputFlags.None);
                    }
                };
#endif
            });
        }
        private void HideKeyboard()
        {
#if ANDROID
            var activity = Platform.CurrentActivity;
            var inputMethodManager = (InputMethodManager)activity.GetSystemService(Android.Content.Context.InputMethodService);
            var token = activity.CurrentFocus?.WindowToken;
            if (token != null)
            {
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            }
#endif
        }
        private void OnOpenKeyboardClicked(object sender, EventArgs e)
        {
            EntryField.Focus();
        }
        private async void OnOpenPageClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///PrinterConfig");
        }

        private async void OnEtiketYazdirClicked(object sender, EventArgs e)
        {
            PrinterIp = Preferences.Get("PrinterIp", string.Empty);
            if (string.IsNullOrEmpty(PrinterIp))
            {
                await DisplayAlert("Uyarý", "Yazýcý IP'si girilmeden iþlem yapýlamaz!", "Tamam");
                return;
            }

            var kumasTopBool = await _apiClientService.KumasTopCheck(EntryField.Text);
            if (!kumasTopBool)
            {
                await DisplayAlert("Uyarý", "Böyle bir TopNo bulunmamaktadýr!", "Tamam");
                return;
            }
            if (isFirstConnection)
            {
                loadingOverlay.IsVisible = true;
            }
            var kumasTop = await _apiClientService.GetKumasTop(EntryField.Text);
            GenerateAndPrintReport(kumasTop, PrinterIp);
            loadingOverlay.IsVisible = false;
            isFirstConnection = false;
            EntryField.CursorPosition = 0; // Ýmleci baþa al
            EntryField.SelectionLength = EntryField.Text.Length; // Tüm metni seç
        }

#if ANDROID || IOS || WINDOWS
        public async void GenerateAndPrintReport(IEnumerable<KumasTopDto> kumasTopList, string printerIp)
        {
            var connection = new TcpConnection(printerIp, 9100);
            try
            {
                connection.Open();

                foreach (var kumasTop in kumasTopList)
                {
                    string formattedNetMt = kumasTop.NetMt.ToString("F2");
                    string formattedBrutKg = kumasTop.BrutKg.ToString("F2");
                    string formattedNetKg = kumasTop.NetKg.ToString("F2");

                    string zpl = $@"
                        ^XA
                        ^PW800
                        ^LL800

                        ^FO30,130^A0N,30,30^FDStock Code: ^FS
                        ^FO180,130^A0N,30,30^FD{kumasTop.StokKodu}^FS

                        ^FO30,190^A0N,30,30^FDProduct Code: ^FS
                        ^FO200,190^A0N,30,30^FD{kumasTop.TicariStokKodu}^FS

                        ^FO30,250^A0N,30,30^FDWidth: ^FS
                        ^FO120,250^A0N,30,30^FD{kumasTop.En}^FS

                        ^FO30,310^A0N,30,30^FDMeter: ^FS
                        ^FO120,310^A0N,30,30^FD{formattedNetMt}^FS

                        ^FO20,370^A0N,30,30^FDGross / Net (kg): ^FS
                        ^FO220,370^A0N,30,30^FD{formattedBrutKg} / {formattedNetKg}^FS
                        ^FO640,130^BY2,3,20^BCB,70,N,N,N
                        ^FD{kumasTop.TopNo}^FS
                        ^FO720,190^A0B,30,30^FD{kumasTop.TopNo}^FS
                        ^XZ";

                    connection.Write(Encoding.UTF8.GetBytes(zpl));
                }
            }
            catch (ConnectionException ex)
            {
                await DisplayAlert("Uyarý", "Yazýcýya baðlanýlamadý! IP Adresini Kontrol Edin", "Tamam");
                EntryField.Text = string.Empty;
            }
            finally
            {
                connection.Close();
            }
        }
#else
        public void GenerateAndPrintReport(IEnumerable<KumasTopDto> kumasTopList, string printerIpö)
        {
            DisplayAlert("Error", "Printing is not supported on this platform.", "OK");
        }
#endif

        private async void OnCikisClicked(object sender, EventArgs e)
        {
            bool isExit = await DisplayAlert("Uyarý", "Uygulamadan Çýkýþ Yapmak Ýstiyor Musunuz??", "Evet", "Hayýr");
            if (isExit)
            {
                Application.Current.Quit();
            }
            else
            {
                return;
            }
        }
    }
}

