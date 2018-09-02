using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.Core.Models;
using Navvy.SampleApp.Console.OrdersProcessing.Models;
using Navvy.SampleApp.Console.Utils;

namespace Navvy.SampleApp.Console.OrdersProcessing.WriteSummaryStep
{
    public class WriteSummaryStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            OrdersProcessingContext context)
        {
            var csvWriter = new Lazy<CsvWriter>(() => new CsvWriter(new StreamWriter(context.Parameters.SummaryFilePath)));

            yield return new BasicTaskStep(
                "WriteSummary",
                () => WriteSummary(context.State.Stats, csvWriter.Value));

            yield return new BasicTaskStep(
                "WriteSummaryCleanup",
                () => csvWriter.ValueIfCreated()?.Dispose(),
                x => true);
        }

        private void WriteSummary(
            OrdersStats stats,
            CsvWriter csvWriter)
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

            csvWriter.WriteRecords(new[] { summary });
        }
    }
}
