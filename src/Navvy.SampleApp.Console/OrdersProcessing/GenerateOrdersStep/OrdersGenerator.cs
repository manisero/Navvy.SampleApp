using System;
using Navvy.SampleApp.Console.OrdersProcessing.Models;

namespace Navvy.SampleApp.Console.OrdersProcessing.GenerateOrdersStep
{
    public class OrdersGenerator
    {
        private readonly Random _random = new Random();

        private int _orderId;

        public Order GenerateNext()
        {
            return new Order
            {
                OrderId = ++_orderId,
                Price = _random.Next(5, 1000),
                CostRate = _random.Next(3, 15)
            };
        }
    }
}
