using System;
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
            var ordersCsvReader = new Lazy<CsvReader>(() => new CsvReader(new StreamReader(context.Parameters.OrdersFilePath)));
            var profitsCsvWriter = new Lazy<CsvWriter>(() => new CsvWriter(new StreamWriter(context.Parameters.ProfitsFilePath)));

            yield return new PipelineTaskStep<ICollection<OrderToProcess>>(
                "ProcessOrders",
                ReadOrdersToProcess(ordersCsvReader, batchSize),
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
                        x => WriteProfits(x, profitsCsvWriter.Value)),
                    new PipelineBlock<ICollection<OrderToProcess>>(
                        "UpdateStats",
                        x => context.State.Stats = UpdateOrdersStats(x, context.State.Stats))
                });

            yield return new BasicTaskStep(
                "ProcessOrdersCleanup",
                () =>
                {
                    ordersCsvReader.ValueIfCreated()?.Dispose();
                    profitsCsvWriter.ValueIfCreated()?.Dispose();
                },
                x => true);
        }

        private IEnumerable<ICollection<OrderToProcess>> ReadOrdersToProcess(
            Lazy<CsvReader> csvReader,
            int batchSize)
        {
            csvReader.Value.Read();
            csvReader.Value.ReadHeader();
            var orders = csvReader.Value.GetRecords<Order>();
            
            foreach (var batch in orders.Batch(batchSize))
            {
                yield return batch
                    .Select(order => new OrderToProcess { Order = order })
                    .ToArray();
            }
        }

        private decimal CalculateOrderProfit(
            Order order)
        {
            return order.Price * (1m - (decimal)order.CostRate);
        }

        private void WriteProfits(
            ICollection<OrderToProcess> orders,
            CsvWriter csvWriter)
        {
            csvWriter.WriteRecords(
                orders
                .Select(
                    x => new
                    {
                        x.Order.OrderId,
                        x.Order.Price,
                        x.Order.CostRate,
                        x.Profit
                    }));
        }

        private OrdersStats UpdateOrdersStats(
            ICollection<OrderToProcess> orders,
            OrdersStats stats)
        {
            var price = 0m;
            var costRate = 0f;
            var profit = 0m;

            foreach (var order in orders)
            {
                price += order.Order.Price;
                costRate += order.Order.CostRate;
                profit += order.Profit;
            }

            return new OrdersStats
            {
                OrdersCount = stats.OrdersCount + orders.Count,
                TotalPrice = stats.TotalPrice + price,
                TotalCostRate = stats.TotalCostRate + costRate,
                TotalProfit = stats.TotalProfit + profit
            };
        }
    }
}
