using TrailBlaze.UI.Components;
using TrailBlaze.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddServerSideBlazor(options =>
{
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
});

builder.Services.AddHttpClient("TrailBlazeAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7212/");
});

builder.Services.AddScoped<TrailService>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("TrailBlazeAPI");
    return new TrailService(httpClient);
});

builder.Services.AddScoped<ReviewService>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("TrailBlazeAPI");
    return new ReviewService(httpClient);
});

builder.Services.AddScoped<CloudinaryService>(sp =>
{
    var httpClient = new HttpClient();
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new CloudinaryService(httpClient, configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TrailBlaze.UI.Client._Imports).Assembly);

app.Run();