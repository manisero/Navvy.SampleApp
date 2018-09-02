namespace Navvy.SampleApp.Console.OrdersProcessing.Models
{
    public class OrdersProcessingContext
    {
        public OrdersProcessingParameters Parameters { get; set; }

        public OrdersProcessingState State { get; set; }
    }

    public class OrdersProcessingParameters
    {
    }

    public class OrdersProcessingState
    {
        public OrdersStats Stats { get; set; } = new OrdersStats();
    }
}
