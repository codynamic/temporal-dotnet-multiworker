using Microsoft.Extensions.Logging;
using Temporalio.Workflows;

namespace WorkflowSample;

[Workflow]
public class BasicWorkflow
{
	[WorkflowRun]
	public async Task RunAsync(string workflowTaskQueue)
	{
		Workflow.Logger.LogInformation("Running workflow in {WorkflowTaskQueue}", workflowTaskQueue);
		await Workflow.DelayAsync(Workflow.Random.Next(1000, 5000));
		await Workflow.ExecuteActivityAsync<Activities>(
			activities => activities.DoSomething(workflowTaskQueue),
			new() { StartToCloseTimeout = TimeSpan.FromMinutes(5) });
	}
}