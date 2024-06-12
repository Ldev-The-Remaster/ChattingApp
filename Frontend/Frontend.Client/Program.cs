using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend.Client.Models; 

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<WebSocketService>();

await builder.Build().RunAsync();
