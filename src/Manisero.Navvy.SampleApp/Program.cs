using System;
using System.Threading;
using System.Threading.Tasks;
using Manisero.Navvy.SampleApp.OrdersProcessing;

namespace Manisero.Navvy.SampleApp
{
    class Program
    {
        private static readonly TaskExecutorFactory TaskExecutorFactory = new TaskExecutorFactory();
        private static readonly OrdersProcessingTaskFactory OrdersProcessingTaskFactory = new OrdersProcessingTaskFactory();

        static void Main(string[] args)
        {
            // Create TaskExecutor. It can be used to execute multiple tasks (it's stateless, can be singleton).
            var taskExecutor = TaskExecutorFactory.Create();

            // Create task. Generally given task should be executed only once (task can have state).
            var task = OrdersProcessingTaskFactory.Create();   

            using (var cancellationSource = new CancellationTokenSource())
            {
                var progress = new Progress<TaskProgress>(
                    x => Console.WriteLine($"{x.StepName}: {x.ProgressPercentage}%"));

                Task.Run(() => WaitForCancellation(cancellationSource));

                // Execute the task.
                var taskResult = taskExecutor.Execute(task, progress, cancellationSource.Token);

                Console.WriteLine($"Task outcome: {taskResult.Outcome}.");
            }
        }

        private static void WaitForCancellation(
            CancellationTokenSource cancellationSource)
        {
            var input = (char)Console.Read();

            if (input == 'c')
            {
                cancellationSource.Cancel();
            }
        }
    }
}
