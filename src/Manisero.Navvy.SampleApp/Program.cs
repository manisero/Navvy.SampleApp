using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Manisero.Navvy.Reporting;
using Manisero.Navvy.SampleApp.OrdersProcessing;

namespace Manisero.Navvy.SampleApp
{
    class Program
    {
        private static readonly TaskExecutorFactory TaskExecutorFactory = new TaskExecutorFactory();
        private static readonly OrdersProcessingTaskFactory OrdersProcessingTaskFactory = new OrdersProcessingTaskFactory();

        static void Main(string[] args)
        {
            var artifactsFolderPath = Path.Combine(Path.GetTempPath(), "Navvy_SampleApp");

            // Create TaskExecutor. It can be used to execute multiple tasks (it's stateless, can be singleton).
            var taskExecutor = TaskExecutorFactory.Create(
                x => Path.Combine(artifactsFolderPath, x.Name, "reports"));

            // Create task. Generally given task should be executed only once (task can have state).
            var task = OrdersProcessingTaskFactory.Create(artifactsFolderPath);   

            using (var cancellationSource = new CancellationTokenSource())
            {
                Task.Run(() => WaitForCancellation(cancellationSource));

                // Execute the task.
                var taskResult = taskExecutor.Execute(task, cancellationSource.Token);

                Console.WriteLine();
                Console.WriteLine($"Task outcome: {taskResult.Outcome}.");
                Console.WriteLine($"Artifacts saved to: {task.Extras.Get<string>(OrdersProcessingTaskFactory.ArtifactsFolderPathExtraKey)}");
                Console.WriteLine($"Reports saved to: {task.GetExecutionReportsPath()}");
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
