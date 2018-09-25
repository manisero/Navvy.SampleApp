using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.PipelineProcessing;
using Manisero.Navvy.SampleApp.OrdersProcessing.Models;
using Manisero.Navvy.SampleApp.Utils;

namespace Manisero.Navvy.SampleApp.OrdersProcessing.ProcessOrdersStep
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

            yield return PipelineTaskStep
                .Builder<ICollection<OrderToProcess>>("ProcessOrders")
                .WithInput(
                    () => ReadOrdersToProcess(ordersCsvReader.Value, batchSize),
                    () => expectedBatchesCount)
                .WithBlock(
                    "CalculateProfits",
                    x =>
                    {
                        foreach (var orderToProcess in x)
                        {
                            orderToProcess.Profit = CalculateOrderProfit(orderToProcess.Order);
                        }
                    })
                .WithBlock("WriteProfits", x => WriteProfits(x, profitsCsvWriter.Value))
                .WithBlock("UpdateStats", x => context.State.Stats = UpdateOrdersStats(x, context.State.Stats))
                .Build();

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
            CsvReader csvReader,
            int batchSize)
        {
            csvReader.Read();
            csvReader.ReadHeader();

            return csvReader
                .GetRecords<Order>()
                .Select(order => new OrderToProcess { Order = order })
                .Batch(batchSize);
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
                orders.Select(
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
