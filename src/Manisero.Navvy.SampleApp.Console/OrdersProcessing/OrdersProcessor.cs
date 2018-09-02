using System;
using System.Threading;
using System.Threading.Tasks;
using Manisero.Navvy.Core;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Core.Models;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.PipelineProcessing.Events;

namespace Manisero.Navvy.SampleApp.Console.OrdersProcessing
{
    public class OrdersProcessor
    {
        private readonly IProgress<TaskProgress> _progress =
            new Progress<TaskProgress>(x => System.Console.WriteLine($"{x.StepName}: {x.ProgressPercentage}%"));

        private readonly ITaskExecutor _executor;

        private readonly OrdersProcessingTaskFactory _ordersProcessingTaskFactory =
            new OrdersProcessingTaskFactory();

        public OrdersProcessor()
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
                itemStarted: x => System.Console.WriteLine($"  Item {x.ItemNumber}:"),
                itemEnded: x => System.Console.WriteLine($"  Item {x.ItemNumber} ended after {x.Duration.TotalMilliseconds}ms."),
                blockStarted: x => System.Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber}..."),
                blockEnded: x => System.Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber} took {x.Duration.TotalMilliseconds}ms."));

            _executor = new TaskExecutorBuilder()
                .RegisterDataflowExecution()
                .RegisterEvents(taskEvents)
                .RegisterEvents(pipelineEvents)
                .Build();
        }

        public TaskResult Run()
        {
            var task = _ordersProcessingTaskFactory.Create();

            using (var cancellationSource = new CancellationTokenSource())
            {
                Task.Run(() => WaitForCancellation(cancellationSource));

                return _executor.Execute(task, _progress, cancellationSource.Token);
            }
        }

        private static void WaitForCancellation(CancellationTokenSource cancellationSource)
        {
            var input = (char)System.Console.Read();

            if (input == 'c')
            {
                cancellationSource.Cancel();
            }
        }
    }
}
