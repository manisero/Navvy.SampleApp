using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.SampleApp.OrdersProcessing.Models;

namespace Manisero.Navvy.SampleApp.OrdersProcessing.WriteSummaryStep
{
    public class WriteSummaryStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            OrdersProcessingContext context)
        {
            yield return TaskStepBuilder.Build.Basic(
                "WriteSummary",
                () =>
                {
                    WriteSummary(
                        context.State.Stats,
                        context.Parameters.SummaryFilePath);
                });
        }

        private void WriteSummary(
            OrdersStats stats,
            string toPath)
        {
            var summary = new
            {
                stats.OrdersCount,
                stats.TotalPrice,
                AvgPrice = stats.TotalPrice / stats.OrdersCount,
                AvgCostRate = stats.TotalCostRate / stats.OrdersCount,
                stats.TotalProfit,
                AvgProfit = stats.TotalProfit / stats.OrdersCount
            };

            using (var csvWriter = new CsvWriter(new StreamWriter(toPath)))
            {
                csvWriter.WriteRecords(new[] { summary });
            }
        }
    }
}
