using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using neoneopets.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// to allows gz files for unity frames
// I considered using uncompressed unity builds, but this had other transmission problems.
// see https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/static-files?view=aspnetcore-6.0
// and https://github.com/dotnet/aspnetcore/issues/2458
// and https://stackoverflow.com/questions/29156701
app.MapWhen(
    ctx => ctx.Request.Path.ToString().Contains("/unity/"),
    subApp => subApp.UseStaticFiles(new StaticFileOptions {
        OnPrepareResponse = ctx => {
            IHeaderDictionary headers = ctx.Context.Response.Headers;
            string contentType = headers["Content-Type"];
            if(contentType == "application/x-gzip") {
                if(ctx.File.Name.EndsWith("js.gz")) {
                    contentType = "application/javascript";
                }
                else if(ctx.File.Name.EndsWith("wasm.gz")) {
                    contentType = "application/wasm";
                }
                headers.Add("Content-Encoding", "gzip");
                headers["Content-Type"] = contentType;
            }
        }
    })
);
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
