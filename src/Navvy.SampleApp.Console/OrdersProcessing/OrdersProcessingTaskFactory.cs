using System.IO;
using System.Linq;
using Manisero.Navvy.Core.Models;
using Navvy.SampleApp.Console.OrdersProcessing.GenerateOrdersStep;
using Navvy.SampleApp.Console.OrdersProcessing.Models;
using Navvy.SampleApp.Console.OrdersProcessing.ProcessOrdersStep;
using Navvy.SampleApp.Console.OrdersProcessing.WriteSummaryStep;

namespace Navvy.SampleApp.Console.OrdersProcessing
{
    public class OrdersProcessingTaskFactory
    {
        private readonly GenerateOrdersStepFactory _generateOrdersStepFactory = new GenerateOrdersStepFactory();
        private readonly ProcessOrdersStepFactory _processOrdersStepFactory = new ProcessOrdersStepFactory();
        private readonly WriteSummaryStepFactory _writeSummaryStepFactory = new WriteSummaryStepFactory();

        public TaskDefinition Create()
        {
            var context = new OrdersProcessingContext
            {
                Parameters = new OrdersProcessingParameters
                {
                    OrdersFilePath = Path.GetTempFileName(),
                    ProfitsFilePath = Path.GetTempFileName()
                },
                State = new OrdersProcessingState()
            };

            var generatingBatchesCount = 2;
            var generatingBatchSize = 3;
            var processingBatchesCount = 2;
            var processingBatchSize = 3;

            var generateOrdersSteps = _generateOrdersStepFactory.Create(generatingBatchesCount, generatingBatchSize, context);
            var processOrdersSteps = _processOrdersStepFactory.Create(processingBatchSize, processingBatchesCount, context);
            var writeSummarySteps = _writeSummaryStepFactory.Create(context);

            return new TaskDefinition(
                generateOrdersSteps
                    .Concat(processOrdersSteps)
                    .Concat(writeSummarySteps)
                    .ToList());
        }
    }
}
