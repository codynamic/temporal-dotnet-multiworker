using Microsoft.Extensions.Logging;
using Temporalio.Client;

namespace WorkflowSample;

internal static class WorkflowHelper
{
	private static readonly Random Random = new();

	internal static void StartBasicWorkflow(
		ILogger logger,
		string queueId,
		ITemporalClient temporalClient,
		CancellationToken cancellationToken)
	{
		var workflowId = $"workflow-{queueId}::{Guid.NewGuid()}";
		var taskQueue = $"task-queue-{queueId}";
		logger.LogInformation("Starting workflow in {WorkflowTaskQueue} {WorkflowId}", taskQueue, workflowId);
		Task.Run(async () =>
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					await Task.Delay(Random.Next(5000, 10000), cancellationToken);
					await temporalClient.StartWorkflowAsync(
						(BasicWorkflow wf) => wf.RunAsync(taskQueue),
						new WorkflowOptions(workflowId, taskQueue));
				}
			}
			catch (TaskCanceledException)
			{
				logger.LogInformation("Workflow loop {TaskQueue} aborted", taskQueue);
			}
		}, cancellationToken);
	}
}