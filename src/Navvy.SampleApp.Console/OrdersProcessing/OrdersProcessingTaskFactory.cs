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
                Parameters = new OrdersProcessingParameters(),
                State = new OrdersProcessingState()
            };

            var generatingBatchesCount = 1;
            var processingBatchesCount = 1;

            return new TaskDefinition(
                _generateOrdersStepFactory.Create(generatingBatchesCount, context),
                _processOrdersStepFactory.Create(processingBatchesCount, context),
                _writeSummaryStepFactory.Create(context));
        }
    }
}
