var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddHostedService<CupidBackgroundWorker>();
var app = builder.Build();

app.MapHub<CupidHub>("/cupid");

app.Run();
