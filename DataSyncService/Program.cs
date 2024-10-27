using DataSyncService;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLogging();

// serilog
builder.Services.AddSerilog(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
