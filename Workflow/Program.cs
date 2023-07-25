
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Temporalio.Client;
using WorkflowSample;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(options => options.AddConsole().SetMinimumLevel(LogLevel.Information));

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

var temporalClient = await TemporalClient.ConnectAsync(new("localhost:7233"));

using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
	tokenSource.Cancel();
	eventArgs.Cancel = true;
};

var queues = new[] { "A", "B", "C" };

foreach (var queue in queues)
{
	WorkerHelper.StartWorker(logger, queue, temporalClient, host.Services, tokenSource.Token);
	WorkflowHelper.StartBasicWorkflow(logger, queue, temporalClient, tokenSource.Token);
}

await host.RunAsync(tokenSource.Token);
