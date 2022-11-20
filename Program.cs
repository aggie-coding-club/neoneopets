using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NeoNeoPets.Data;
using InternalAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<MongoConnector>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Handle gzip compressed unity assets
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
// Create APIHandler Object
MongoConnector mongo = new MongoConnector();
InternalApi.DBLink.UserStore userStore = new InternalApi.DBLink.UserStore(mongo);
APIHandler apiHandler = new APIHandler(userStore);
// Route api requests to APIHandler
app.MapWhen(
    ctx => ctx.Request.Path.ToString().Contains("/api/"),
    subApp => subApp.Run(apiHandler.HandleRequest)
);
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
