﻿using System;
using System.Collections.Generic;
using Manisero.Navvy.SampleApp.OrdersProcessing.Models;

namespace Manisero.Navvy.SampleApp.OrdersProcessing.GenerateOrdersStep
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
                CostRate = _random.Next(3, 15) / 100f
            };
        }

        public IEnumerable<Order> GenerateMany(
            int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return GenerateNext();
            }
        }
    }
}
