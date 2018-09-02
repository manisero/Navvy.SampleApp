using Manisero.Navvy.SampleApp.Console.OrdersProcessing;

namespace Manisero.Navvy.SampleApp.Console
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
