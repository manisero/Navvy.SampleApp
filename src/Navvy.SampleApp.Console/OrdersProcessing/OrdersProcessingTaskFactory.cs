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
                    ProfitsFilePath = Path.GetTempFileName(),
                    SummaryFilePath = Path.GetTempFileName()
                },
                State = new OrdersProcessingState()
            };

            var batchesCount = 3;
            var batchSize = 10000;

            var generateOrdersSteps = _generateOrdersStepFactory.Create(batchesCount, batchSize, context);
            var processOrdersSteps = _processOrdersStepFactory.Create(batchSize, batchesCount, context);
            var writeSummarySteps = _writeSummaryStepFactory.Create(context);

            return new TaskDefinition(
                generateOrdersSteps
                    .Concat(processOrdersSteps)
                    .Concat(writeSummarySteps)
                    .ToList());
        }
    }
}
