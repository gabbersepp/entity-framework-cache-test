using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int orderId;

            using (var ctx = new TestContext())
            {
                if (!ctx.Orders.Any())
                {
                    ctx.Orders.Add(new Order { Name = "Test" });
                    ctx.SaveChanges();
                }

                var order = ctx.Orders.First();
                order.Name = "Test";
                ctx.SaveChanges();
                orderId = order.Id;
            }

            var t1 = Task.Run(() =>
            {
                try
                {
                    using (var ctx = new TestContext())
                    {
                        var order = ctx.Orders.First(x => x.Id == orderId);
                        Thread.Sleep(5000);
                        order = ctx.Orders.First(x => x.Id == orderId);
                        Console.WriteLine($"READER: Order has name: {order.Name}");
                    }

                    using (var ctx = new TestContext())
                    {
                        var order = ctx.Orders.First(x => x.Id == orderId);
                        Console.WriteLine($"READER WITH NEW CONTEXT: Order has name: {order.Name}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"READER: Error {e}");
                }
            });

            var t2 = Task.Run(() =>
            {
                try
                {
                    using (var ctx = new TestContext())
                    {
                        Thread.Sleep(3000);
                        var order = ctx.Orders.First(x => x.Id == orderId);
                        order.Name = "TestTest";
                        ctx.SaveChanges();
                        Console.WriteLine("WRITER: write order");
                    }

                    using (var ctx = new TestContext())
                    {
                        var order = ctx.Orders.First(x => x.Id == orderId);
                        Console.WriteLine($"WRITER: read order: {order.Name}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"WRITER: Error {e}");
                }
            });

            t1.Wait();
            t2.Wait();

            Console.ReadKey();
        }
    }
}
