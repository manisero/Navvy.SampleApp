using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.PipelineProcessing;
using Manisero.Navvy.SampleApp.OrdersProcessing.Models;
using Manisero.Navvy.SampleApp.Utils;

namespace Manisero.Navvy.SampleApp.OrdersProcessing.GenerateOrdersStep
{
    public class GenerateOrdersStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            int batchesCount,
            int batchSize,
            OrdersProcessingContext context)
        {
            var ordersGenerator = new OrdersGenerator();
            var csvWriter = new Lazy<CsvWriter>(() => new CsvWriter(new StreamWriter(context.Parameters.OrdersFilePath)));

            yield return PipelineTaskStep
                .Builder<ICollection<Order>>("GenerateOrders")
                .WithInput(GenerateOrders(ordersGenerator, batchesCount, batchSize), batchesCount)
                .WithBlock("WriteOrders", x => csvWriter.Value.WriteRecords(x))
                .Build();

            yield return new BasicTaskStep(
                "GenerateOrdersCleanup",
                () => csvWriter.ValueIfCreated()?.Dispose(),
                x => true);
        }

        private IEnumerable<ICollection<Order>> GenerateOrders(
            OrdersGenerator ordersGenerator,
            int batchesCount,
            int batchSize)
        {
            for (var batchIndex = 0; batchIndex < batchesCount; batchIndex++)
            {
                var batch = new Order[batchSize];

                for (var orderIndex = 0; orderIndex < batchSize; orderIndex++)
                {
                    batch[orderIndex] = ordersGenerator.GenerateNext();
                }

                yield return batch;
            }
        }
    }
}
