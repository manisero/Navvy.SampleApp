using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.Core.Models;
using Manisero.Navvy.PipelineProcessing;
using Navvy.SampleApp.Console.OrdersProcessing.Models;
using Navvy.SampleApp.Console.Utils;

namespace Navvy.SampleApp.Console.OrdersProcessing.ProcessOrdersStep
{
    public class ProcessOrdersStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            int batchSize,
            int expectedBatchesCount,
            OrdersProcessingContext context)
        {
            var csvReader = new CsvReader(new StreamReader(context.Parameters.OrdersFilePath));
            var orders = csvReader.GetRecords<Order>();

            yield return new PipelineTaskStep<ICollection<OrderToProcess>>(
                "ProcessOrders",
                ReadOrdersToProcess(orders, batchSize),
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
                        "WriteProfits",
                        x => WriteProfits(x)),
                    new PipelineBlock<ICollection<OrderToProcess>>(
                        "UpdateStats",
                        x => context.State.Stats = UpdateOrdersStats(x, context.State.Stats))
                });

            yield return new BasicTaskStep(
                "ProcessOrdersCleanup",
                () => csvReader.Dispose(),
                x => true);
        }

        private IEnumerable<ICollection<OrderToProcess>> ReadOrdersToProcess(
            IEnumerable<Order> orders,
            int batchSize)
        {
            return orders
                .Batch(batchSize)
                .Select(
                    batch => batch.Select(order => new OrderToProcess
                          {
                              Order = order
                          })
                          .ToArray());
        }

        private decimal CalculateOrderProfit(
            Order order)
        {
            return order.Price * (1m - (decimal)order.CostRate);
        }

        private void WriteProfits(
            ICollection<OrderToProcess> orders)
        {
        }

        private OrdersStats UpdateOrdersStats(
            ICollection<OrderToProcess> orders,
            OrdersStats stats)
        {
            return new OrdersStats();
        }
    }
}
