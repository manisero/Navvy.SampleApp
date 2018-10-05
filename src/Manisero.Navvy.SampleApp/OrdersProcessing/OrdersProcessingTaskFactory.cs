using System;
using System.IO;
using System.Linq;
using Manisero.Navvy.SampleApp.OrdersProcessing.GenerateOrdersStep;
using Manisero.Navvy.SampleApp.OrdersProcessing.Models;
using Manisero.Navvy.SampleApp.OrdersProcessing.ProcessOrdersStep;
using Manisero.Navvy.SampleApp.OrdersProcessing.WriteSummaryStep;

namespace Manisero.Navvy.SampleApp.OrdersProcessing
{
    public class OrdersProcessingTaskFactory
    {
        private readonly GenerateOrdersStepFactory _generateOrdersStepFactory = new GenerateOrdersStepFactory();
        private readonly ProcessOrdersStepFactory _processOrdersStepFactory = new ProcessOrdersStepFactory();
        private readonly WriteSummaryStepFactory _writeSummaryStepFactory = new WriteSummaryStepFactory();

        public TaskDefinition Create(
            string artifactsFolderPath,
            int batchesCount = 10,
            int batchSize = 100000)
        {
            var taskName = $"OrdersProcessing_{DateTime.Now:yyyyMMdd_hhmmss}";

            var taskArtifactsFolderPath = Path.Combine(artifactsFolderPath, taskName);
            Directory.CreateDirectory(taskArtifactsFolderPath);

            var context = new OrdersProcessingContext
            {
                Parameters = new OrdersProcessingParameters
                {
                    OrdersFilePath = Path.Combine(taskArtifactsFolderPath, "orders.csv"),
                    ProfitsFilePath = Path.Combine(taskArtifactsFolderPath, "profits.csv"),
                    SummaryFilePath = Path.Combine(taskArtifactsFolderPath, "summary.csv")
                },
                State = new OrdersProcessingState()
            };

            var generateOrdersSteps = _generateOrdersStepFactory.Create(batchesCount, batchSize, context);
            var processOrdersSteps = _processOrdersStepFactory.Create(batchSize, batchesCount, context);
            var writeSummarySteps = _writeSummaryStepFactory.Create(context);
            
            return new TaskDefinition(
                taskName,
                generateOrdersSteps
                    .Concat(processOrdersSteps)
                    .Concat(writeSummarySteps)
                    .ToList());
        }
    }
}
    