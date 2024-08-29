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

            // E�er veri bo�sa bir hata mesaj� g�sterebilirsiniz
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
        var report = new KumasReport(); // XtraReport s�n�f�n�z

        // Veriyi raporun DataSource'una ba�lay�n
        report.DataSource = kumasTopList;

        using (var stream = new MemoryStream())
        {
            // Raporu PDF olarak export et
            report.ExportToPdf(stream);

            // Stream'i ba�a sar
            stream.Seek(0, SeekOrigin.Begin);

            // Zebra yaz�c�s�na ba�lan
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "KumasReport.pdf");
            var connection = new TcpConnection(printerIp, 9100);
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            try
            {
                connection.Open();
                // Yaz�c�ya PDF verisini g�nder
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
        // MacCatalyst gibi desteklenmeyen platformlarda kullanmak isterseniz, alternatif bir i�lem ekleyebilirsiniz.
        public void GenerateAndPrintReport(IEnumerable<KumasTopDto> kumasTopList, string printerIp)
        {
            // Alternatif i�lem
            DisplayAlert("Error", "Printing is not supported on this platform.", "OK");
        }
#endif



}