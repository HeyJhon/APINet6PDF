var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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