using Etiket.DtoClient;
using Etiket.DtoClient.Models;
using Etiket.MAUI.Reports;
using System.Collections.ObjectModel;
#if ANDROID || IOS || WINDOWS
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
#endif
namespace Etiket.MAUI.Pages;

public partial class PrinterConfig : ContentPage
{
	private readonly ApiClientService _apiClientService;
	public PrinterConfig(ApiClientService apiClientService)
	{
        _apiClientService = apiClientService;
        InitializeComponent();
        
	}
    public async Task LoadReportData(string topNo,string printerIp)
    {
        try
        {
            var kumasTopList = await _apiClientService.GetKumasTop(topNo);

            // Eðer veri boþsa bir hata mesajý gösterebilirsiniz
            if (kumasTopList == null || !kumasTopList.Any())
            {
                await DisplayAlert("No Data", "No data found for the provided TopNo.", "OK");
                return;
            }

            GenerateAndPrintReport(kumasTopList,printerIp);
        }
        catch(HttpRequestException ex)
        {
        
            await DisplayAlert("Error", ex.Message, "OK");
        }
        
    }
    private async void OnPrintClicked(object sender, EventArgs e)
    {
        string printerIp = PrinterIpEntry.Text;
        if(string.IsNullOrEmpty(printerIp))
        {
            await DisplayAlert("Error", "Please enter printer IP address.", "OK");
            return;
        }
        await LoadReportData("S60200001",printerIp);
    }
#if ANDROID || IOS || WINDOWS
    public void GenerateAndPrintReport(IEnumerable<KumasTopDto> kumasTopList, string printerIp)
    {
        var report = new KumasReport(); // XtraReport sýnýfýnýz

        // Veriyi raporun DataSource'una baðlayýn
        report.DataSource = kumasTopList;

        using (var stream = new MemoryStream())
        {
            // Raporu PDF olarak export et
            report.ExportToPdf(stream);

            // Stream'i baþa sar
            stream.Seek(0, SeekOrigin.Begin);

            // Zebra yazýcýsýna baðlan
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "KumasReport.pdf");
            var connection = new TcpConnection(printerIp, 9100);
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            try
            {
                connection.Open();
                // Yazýcýya PDF verisini gönder
                connection.Write(stream.ToArray());
            }
            catch (ConnectionException ex)
            {
                DisplayAlert("Connection Error", ex.Message, "OK").Wait();
            }
            finally
            {
                Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
                connection.Close();
            }
        }
    }
#else
        // MacCatalyst gibi desteklenmeyen platformlarda kullanmak isterseniz, alternatif bir iþlem ekleyebilirsiniz.
        public void GenerateAndPrintReport(IEnumerable<KumasTopDto> kumasTopList, string printerIp)
        {
            // Alternatif iþlem
            DisplayAlert("Error", "Printing is not supported on this platform.", "OK");
        }
#endif



}