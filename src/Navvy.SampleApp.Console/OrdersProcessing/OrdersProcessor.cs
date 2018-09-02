using Manisero.Navvy;
using Manisero.Navvy.Core;

namespace Navvy.SampleApp.Console.OrdersProcessing
{
    public class OrdersProcessor
    {
        private readonly ITaskExecutor _executor = new TaskExecutorBuilder().Build();

        private readonly OrdersProcessingTaskFactory _ordersProcessingTaskFactory = new OrdersProcessingTaskFactory();
        
        public void Run()
        {
            var task = _ordersProcessingTaskFactory.Create();

            _executor.Execute(task);
        }
    }
}
