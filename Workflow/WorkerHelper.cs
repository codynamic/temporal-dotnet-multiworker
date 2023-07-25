using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Temporalio.Worker;

namespace WorkflowSample;

internal static class WorkerHelper
{
	internal static TemporalWorkerOptions CreateOptions(string taskQueue, IServiceProvider serviceProvider)
	{
		return new TemporalWorkerOptions(taskQueue)
			{
				LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>(),
			}
			.AddWorkflow<BasicWorkflow>()
			.AddAllActivities(new Activities(serviceProvider.GetRequiredService<ILogger<Activities>>(), taskQueue));
	}

	internal static void StartWorker(
		ILogger logger,
		string queueId,
		IWorkerClient temporalClient,
		IServiceProvider serviceProvider,
		CancellationToken cancellationToken)
	{
		var taskQueue = $"task-queue-{queueId}";
		logger.LogInformation("Starting worker for {WorkerTaskQueue}", taskQueue);
		var worker = new TemporalWorker(
			temporalClient,
			CreateOptions(taskQueue, serviceProvider));

		try
		{
			worker.ExecuteAsync(cancellationToken);
		}
		catch (OperationCanceledException)
		{
			Console.WriteLine("Worker cancelled");
		}
	}
}