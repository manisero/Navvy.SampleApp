using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.Core.Models;
using Manisero.Navvy.PipelineProcessing;
using Navvy.SampleApp.Console.OrdersProcessing.Models;

namespace Navvy.SampleApp.Console.OrdersProcessing.GenerateOrdersStep
{
    public class GenerateOrdersStepFactory
    {
        public IEnumerable<ITaskStep> Create(
            int batchesCount,
            int batchSize,
            OrdersProcessingContext context)
        {
            var ordersGenerator = new OrdersGenerator();
            var csvWriter = new CsvWriter(new StreamWriter(context.Parameters.OrdersFilePath));

            yield return new PipelineTaskStep<ICollection<Order>>(
                "GenerateOrders",
                GenerateOrders(ordersGenerator, batchesCount, batchSize),
                batchesCount,
                new List<PipelineBlock<ICollection<Order>>>
                {
                    new PipelineBlock<ICollection<Order>>(
                        "WriteOrders",
                        x => csvWriter.WriteRecords(x))
                }
            );

            yield return new BasicTaskStep(
                "GenerateOrdersCleanup",
                () => csvWriter.Dispose(),
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
