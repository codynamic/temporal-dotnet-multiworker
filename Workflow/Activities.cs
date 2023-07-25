using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace WorkflowSample;

public class Activities
{
	private readonly ILogger<Activities> _logger;
	private readonly string _workerTaskQueue;

	public Activities(ILogger<Activities> logger, string workerTaskQueue)
	{
		_logger = logger;
		_workerTaskQueue = workerTaskQueue;
	}

	[Activity]
	public Task DoSomething(string workflowTaskQueue)
	{
		_logger.LogInformation(
			"Executing activity owned by worker for {WorkerTaskQueue} with input from {WorkflowTaskQueue} (ActivityExecutionContext: {{ TaskQueue: {ContextTaskQueue}, WorkflowID: {ContextWorkflowId} }})",
			_workerTaskQueue, workflowTaskQueue, ActivityExecutionContext.Current.Info.TaskQueue, ActivityExecutionContext.Current.Info.WorkflowID);
		return Task.CompletedTask;
	}
}