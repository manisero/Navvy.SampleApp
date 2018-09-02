using System.Collections.Generic;
using Manisero.Navvy.Core.Models;
using Manisero.Navvy.PipelineProcessing;
using Navvy.SampleApp.Console.OrdersProcessing.Models;

namespace Navvy.SampleApp.Console.OrdersProcessing.ProcessOrdersStep
{
    public class ProcessOrdersStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            int expectedBatchesCount,
            OrdersProcessingContext context)
        {
            yield return new PipelineTaskStep<ICollection<OrderToProcess>>(
                "ProcessOrders",
                ReadOrdersToProcess(),
                expectedBatchesCount,
                new List<PipelineBlock<ICollection<OrderToProcess>>>
                {
                    new PipelineBlock<ICollection<OrderToProcess>>(
                        "CalculateProfits",
                        x =>
                        {
                            foreach (var orderToProcess in x)
                            {
                                orderToProcess.Profit = CalculateOrderProfit(orderToProcess.Order);
                            }
                        }),
                    new PipelineBlock<ICollection<OrderToProcess>>(
                        "UpdateStats",
                        x => context.State.Stats = UpdateOrdersStats(x, context.State.Stats)),
                    new PipelineBlock<ICollection<OrderToProcess>>(
                        "WriteProfits",
                        x => WriteProfits(x))
                });
        }

        private IEnumerable<ICollection<OrderToProcess>> ReadOrdersToProcess()
        {
            yield return new[] { new OrderToProcess() };
        }

        private decimal CalculateOrderProfit(
            Order order)
        {
            return 0m;
        }

        private OrdersStats UpdateOrdersStats(
            ICollection<OrderToProcess> orders,
            OrdersStats stats)
        {
            return new OrdersStats();
        }

        private void WriteProfits(
            ICollection<OrderToProcess> orders)
        {
        }
    }
}
