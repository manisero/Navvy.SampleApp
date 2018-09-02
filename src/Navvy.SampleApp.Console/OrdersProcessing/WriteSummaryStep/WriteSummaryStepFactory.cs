﻿using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.Core.Models;
using Navvy.SampleApp.Console.OrdersProcessing.Models;

namespace Navvy.SampleApp.Console.OrdersProcessing.WriteSummaryStep
{
    public class WriteSummaryStepFactory
    {
        public ITaskStep Create(
            OrdersProcessingContext context)
        {
            return new BasicTaskStep(
                "WriteSummary",
                () => WriteSummary(context.State.Stats));
        }

        private void WriteSummary(
            OrdersStats stats)
        {
        }
    }
}