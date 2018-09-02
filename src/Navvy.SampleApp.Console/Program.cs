using Navvy.SampleApp.Console.OrdersProcessing;

namespace Navvy.SampleApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var ordersProcessor = new OrdersProcessor();
            ordersProcessor.Run();
        }
    }
}
