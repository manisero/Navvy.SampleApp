using System;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.Logging;
using Manisero.Navvy.PipelineProcessing.Events;
using Manisero.Navvy.Reporting;

namespace Manisero.Navvy.SampleApp
{
    public class TaskExecutorFactory
    {
        public ITaskExecutor Create(
            Func<TaskDefinition, string> taskReportsFolderPathFactory)
        {
            var taskEvents = new TaskExecutionEvents(
                taskStarted: x => Console.WriteLine("Task started."),
                taskEnded: x => Console.WriteLine($"Task ended after {x.Duration.TotalMilliseconds}ms."),
                stepStarted: x => Console.WriteLine($"{x.Step.Name}:"),
                stepEnded: x => Console.WriteLine($"{x.Step.Name} took {x.Duration.TotalMilliseconds}ms."),
                stepSkipped: x => Console.WriteLine($"{x.Step.Name} skipped"),
                stepCanceled: x => Console.WriteLine($"{x.Step.Name} canceled"),
                stepFailed: x => Console.WriteLine($"{x.Step.Name} failed"));

            var pipelineEvents = new PipelineExecutionEvents(
                itemMaterialized: x => Console.WriteLine($"  Item {x.ItemNumber} (materialized in {x.MaterializationDuration.TotalMilliseconds}ms):"),
                itemEnded: x => Console.WriteLine($"  Item {x.ItemNumber} ended after {x.Duration.TotalMilliseconds}ms."),
                blockStarted: x => Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber}..."),
                blockEnded: x => Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber} took {x.Duration.TotalMilliseconds}ms."),
                pipelineEnded: x =>
                {
                    Console.WriteLine($"      Materialization: {x.TotalInputMaterializationDuration.TotalMilliseconds}ms");

                    foreach (var blockDuration in x.TotalBlockDurations)
                    {
                        Console.WriteLine($"      {blockDuration.Key}: {blockDuration.Value.TotalMilliseconds}ms");
                    }
                });

            return new TaskExecutorBuilder()
                .UseDataflowPipelineExecution()
                .UseTaskExecutionLogger()
                .UseTaskExecutionReporter(taskReportsFolderPathFactory)
                .RegisterEvents(taskEvents, pipelineEvents)
                .Build();
        }
    }
}
