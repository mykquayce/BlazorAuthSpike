using BlazorAuthSpike.BlazorApp1.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DuoConfig>(builder.Configuration.GetSection(nameof(DuoConfig)));
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<DuoUniversal.ClientBuilder>(provider =>
{
	var (clientId, clientSecret, apiHost, redirectUri) = provider.GetRequiredService<IOptions<DuoConfig>>().Value;
	return new DuoUniversal.ClientBuilder(
		clientId: clientId,
		clientSecret: clientSecret,
		apiHost: apiHost,
		redirectUri: redirectUri);
});
builder.Services.AddSingleton(provider =>
{
	var builder = provider.GetRequiredService<DuoUniversal.ClientBuilder>();
	return builder.Build();
});
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, DuoAuthenticationStateProvider>();
builder.Services.TryAddScoped<Session>(_ => new Session { State = Guid.NewGuid(), });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
