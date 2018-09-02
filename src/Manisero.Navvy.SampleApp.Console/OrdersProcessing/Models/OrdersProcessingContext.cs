namespace Manisero.Navvy.SampleApp.Console.OrdersProcessing.Models
{
    public class OrdersProcessingContext
    {
        public OrdersProcessingParameters Parameters { get; set; }

        public OrdersProcessingState State { get; set; }
    }

    public class OrdersProcessingParameters
    {
        public string OrdersFilePath { get; set; }
        public string ProfitsFilePath { get; set; }
        public string SummaryFilePath { get; set; }
    }

    public class OrdersProcessingState
    {
        public OrdersStats Stats { get; set; } = new OrdersStats();
    }
}
