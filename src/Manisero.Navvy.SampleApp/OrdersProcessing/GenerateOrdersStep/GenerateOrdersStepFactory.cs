using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.PipelineProcessing;
using Manisero.Navvy.SampleApp.OrdersProcessing.Models;
using Manisero.Navvy.SampleApp.Utils;
using Manisero.Navvy.Utils;

namespace Manisero.Navvy.SampleApp.OrdersProcessing.GenerateOrdersStep
{
    public class GenerateOrdersStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            int ordersCount,
            int batchSize,
            OrdersProcessingContext context)
        {
            var ordersGenerator = new OrdersGenerator();
            var csvWriter = new Lazy<CsvWriter>(() => new CsvWriter(new StreamWriter(context.Parameters.OrdersFilePath)));

            yield return TaskStepBuilder.Build.Pipeline<ICollection<Order>>("GenerateOrders")
                .WithInput(
                    () => new BatchedPipelineInputItems<Order>(
                        ordersGenerator.GenerateMany(ordersCount),
                        ordersCount,
                        batchSize),
                    "GenerateOrders")
                .WithBlock("WriteOrders", x => csvWriter.Value.WriteRecords(x))
                .Build();

            yield return TaskStepBuilder.Build.Basic(
                "GenerateOrdersCleanup",
                () => csvWriter.ValueIfCreated()?.Dispose(),
                x => true);
        }
    }
}
