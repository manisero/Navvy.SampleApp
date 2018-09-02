using System.Collections.Generic;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.Core.Models;
using Manisero.Navvy.PipelineProcessing;

namespace Navvy.SampleApp.Console.OrdersProcessing
{
    public class Order
    {
    }

    public class OrderToProcess
    {
        public Order Order { get; set; }

        public decimal Profit { get; set; }
    }

    public class OrdersSummary
    {
    }

    public class OrdersProcessor
    {
        public void Run()
        {
            var generatingBatchesCount = 1;
            var processingBatchesCount = 1;

            var summary = new OrdersSummary();

            var task = new TaskDefinition(
                new PipelineTaskStep<ICollection<Order>>(
                    "GenerateOrders",
                    GenerateOrders(generatingBatchesCount),
                    generatingBatchesCount,
                    new List<PipelineBlock<ICollection<Order>>>
                    {
                        new PipelineBlock<ICollection<Order>>(
                            "WriteOrders",
                            x => WriteOrders(x))
                    }
                ),
                new PipelineTaskStep<ICollection<OrderToProcess>>(
                    "ProcessOrders",
                    ReadOrdersToProcess(processingBatchesCount),
                    processingBatchesCount,
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
                            "UpdateSummary",
                            x => summary = UpdateOrdersSummary(x, summary)),
                        new PipelineBlock<ICollection<OrderToProcess>>(
                            "WriteProfits",
                            x => WriteProfits(x))
                    }),
                new BasicTaskStep(
                    "WriteSummary",
                    () => WriteSummary(summary)));

            var executor = new TaskExecutorBuilder().Build();

            executor.Execute(task);
        }

        private IEnumerable<ICollection<Order>> GenerateOrders(
            int batchesCount)
        {
            for (var i = 0; i < batchesCount; i++)
            {
                yield return new[] { new Order() };
            }
        }

        private void WriteOrders(
            ICollection<Order> orders)
        {
        }

        private IEnumerable<ICollection<OrderToProcess>> ReadOrdersToProcess(
            int batchesCount)
        {
            for (var i = 0; i < batchesCount; i++)
            {
                yield return new[] { new OrderToProcess() };
            }
        }

        private decimal CalculateOrderProfit(
            Order order)
        {
            return 0m;
        }

        private OrdersSummary UpdateOrdersSummary(
            ICollection<OrderToProcess> orders,
            OrdersSummary summary)
        {
            return new OrdersSummary();
        }

        private void WriteProfits(
            ICollection<OrderToProcess> orders)
        {
        }

        private void WriteSummary(
            OrdersSummary summary)
        {
        }
    }
}
