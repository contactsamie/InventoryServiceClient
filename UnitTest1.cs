using Akka.Actor;

using InventoryService.Messages;
using InventoryService.Messages.Models;
using InventoryService.Messages.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using InventoryService.AkkaInMemoryServer;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
             var initialInventory = new RealTimeInventory("ticketsections-100", 10, 0, 0);

            using (new InventoryServiceServer())
            {
                var mySystem = Akka.Actor.ActorSystem.Create("mySystem");
                var address = ConfigurationManager.AppSettings["RemoteActorAddress"];
                var inventoryActor = mySystem.ActorSelection(address);

                var result = inventoryActor.Ask<IInventoryServiceCompletedMessage>(new ReserveMessage(initialInventory.ProductId, 20)).Result;
                if (result.Successful)
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
                else
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
            }
         
            using (var server = new InventoryServiceServer(initialInventory))
            {
                var result = server.inventoryActor.Ask<IInventoryServiceCompletedMessage>(new ReserveMessage(initialInventory.ProductId, 20)).Result;
                if (result.Successful)
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
                else
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
            }

            using (var server = new InventoryServiceServer())
            {
                var result = server.inventoryActor.Ask<IInventoryServiceCompletedMessage>(new ReserveMessage(initialInventory.ProductId, 20)).Result;
                if (result.Successful)
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
                else
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
            }

            using (var server = new InventoryServiceServer())
            {
                var result = server.inventoryActor.Ask<IInventoryServiceCompletedMessage>(new ReserveMessage(initialInventory.ProductId, 20)).Result;
                if (result.Successful)
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
                else
                {
                    Console.WriteLine(result.RealTimeInventory);
                }
            }
        }
    }
}