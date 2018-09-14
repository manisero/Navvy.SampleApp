using Manisero.Navvy.Core;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.PipelineProcessing.Events;

namespace Manisero.Navvy.SampleApp.Console
{
    public class TaskExecutorFactory
    {
        public ITaskExecutor Create()
        {
            var taskEvents = new TaskExecutionEvents(
                taskStarted: x => System.Console.WriteLine("Task started."),
                taskEnded: x => System.Console.WriteLine($"Task ended after {x.Duration.TotalMilliseconds}ms."),
                stepStarted: x => System.Console.WriteLine($"{x.Step.Name}:"),
                stepEnded: x => System.Console.WriteLine($"{x.Step.Name} took {x.Duration.TotalMilliseconds}ms."),
                stepSkipped: x => System.Console.WriteLine($"{x.Step.Name} skipped"),
                stepCanceled: x => System.Console.WriteLine($"{x.Step.Name} canceled"),
                stepFailed: x => System.Console.WriteLine($"{x.Step.Name} failed"));

            var pipelineEvents = new PipelineExecutionEvents(
                itemStarted: x => System.Console.WriteLine($"  Item {x.ItemNumber} (materialized in {x.MaterializationDuration.TotalMilliseconds}ms):"),
                itemEnded: x => System.Console.WriteLine($"  Item {x.ItemNumber} ended after {x.Duration.TotalMilliseconds}ms."),
                blockStarted: x => System.Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber}..."),
                blockEnded: x => System.Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber} took {x.Duration.TotalMilliseconds}ms."),
                pipelineEnded: x =>
                {
                    System.Console.WriteLine($"      Materialization: {x.TotalInputMaterializationDuration.TotalMilliseconds}ms");

                    foreach (var blockDuration in x.TotalBlockDurations)
                    {
                        System.Console.WriteLine($"      {blockDuration.Key}: {blockDuration.Value.TotalMilliseconds}ms");
                    }
                });

            return new TaskExecutorBuilder()
                   .RegisterDataflowExecution()
                   .RegisterEvents(taskEvents)
                   .RegisterEvents(pipelineEvents)
                   .Build();
        }
    }
}
