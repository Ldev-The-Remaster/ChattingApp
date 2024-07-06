using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend.Client.Models; 

var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();
