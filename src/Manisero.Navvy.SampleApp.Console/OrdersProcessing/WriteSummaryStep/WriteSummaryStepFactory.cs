﻿using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.Core.Models;
using Manisero.Navvy.SampleApp.Console.OrdersProcessing.Models;

namespace Manisero.Navvy.SampleApp.Console.OrdersProcessing.WriteSummaryStep
{
    public class WriteSummaryStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            OrdersProcessingContext context)
        {
            yield return new BasicTaskStep(
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
