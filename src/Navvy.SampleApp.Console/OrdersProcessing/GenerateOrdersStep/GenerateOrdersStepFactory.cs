using System.Collections.Generic;
using Manisero.Navvy.Core.Models;
using Manisero.Navvy.PipelineProcessing;
using Navvy.SampleApp.Console.OrdersProcessing.Models;

namespace Navvy.SampleApp.Console.OrdersProcessing.GenerateOrdersStep
{
    public class GenerateOrdersStepFactory
    {
        public ITaskStep Create(
            int batchesCount,
            OrdersProcessingContext context)
        {
            var ordersGenerator = new OrdersGenerator();

            return new PipelineTaskStep<ICollection<Order>>(
                "GenerateOrders",
                GenerateOrders(ordersGenerator, batchesCount),
                batchesCount,
                new List<PipelineBlock<ICollection<Order>>>
                {
                    new PipelineBlock<ICollection<Order>>(
                        "WriteOrders",
                        x => WriteOrders(x))
                }
            );
        }

        private IEnumerable<ICollection<Order>> GenerateOrders(
            OrdersGenerator ordersGenerator,
            int batchesCount)
        {
            for (var i = 0; i < batchesCount; i++)
            {
                yield return new[] { ordersGenerator.GenerateNext() };
            }
        }

        private void WriteOrders(
            ICollection<Order> orders)
        {
        }
    }
}
