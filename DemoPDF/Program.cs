using PuppeteerReportCsharp;
using PuppeteerSharp;
using Wkhtmltopdf.NetCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddWkhtmltopdf();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapPost("/reporte", () =>
{
    //Logica que obtenga los datos de la BD
    //Convertir información a HTML
    var htmlCode = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Template\\invoice1.html");
    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
    SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlCode);
    //doc.Save(AppDomain.CurrentDomain.BaseDirectory + "Template\\invoice1.pdf");
    byte[]data = doc.Save();
    var result = Convert.ToBase64String(data);
    doc.Close();
    //{status = "ok", data = "result"};
    return result;
});

app.MapPost("/reporte-puppeteer", async () =>
{
    //Logica que obtenga los datos de la BD
    //Convertir información a HTML
    var htmlCode = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Template\\invoice1.html");

    //await new BrowserFetcher().DownloadAsync();
    var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath=""});
    var page = await browser.NewPageAsync();
    await page.SetContentAsync(htmlCode);

    var puppeteer = new PuppeteerReport();
    byte[] data = await puppeteer.PDFPage(page, new PuppeteerSharp.PdfOptions
    {
        Format = PuppeteerSharp.Media.PaperFormat.A4,
        PreferCSSPageSize = true,
        MarginOptions = new PuppeteerSharp.Media.MarginOptions
        {
            Top = "10mm",
            Left = "10mm",
            Right = "10mm",
            Bottom = "10mm"
        }
    });
    
    var result = Convert.ToBase64String(data);    

    return result;
});

app.MapPost("/reporte-rotativa",(IGeneratePdf generatePdf) =>
{
    //Logica que obtenga los datos de la BD
    //Convertir información a HTML
    var htmlCode = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Template\\invoice1.html");
    var result = generatePdf.GetPDF(htmlCode);
    return result;
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}