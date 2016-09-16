using Akka.Actor;

using InventoryService.Messages;
using InventoryService.Messages.Models;
using InventoryService.Messages.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Akka.Configuration;
using InventoryService.AkkaInMemoryServer;
using InventoryService.Storage;
using InventoryService.Storage.InMemoryLib;

namespace UnitTestProject1
{
   
        [TestClass]
        public class UnitTest1
        {
            [TestMethod]
            public void TestMethod1()
            {
                var initialInventory = new RealTimeInventory("ticketsections-100", 10, 0, 0);
                var serverOptions = new InventoryServerOptions()
                {
                    InitialInventory = initialInventory,
                    InventoryActorAddress = "akka.tcp://InventoryService-Server@localhost:10000/user/InventoryActor",
                    ServerEndPoint = "http://*:10088/",
                    StorageType = typeof(InMemory),
                    ServerActorSystemName = "InventoryService-Server",
                    ServerActorSystemConfig = @"
                  akka.actor{provider= ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""}
                  akka.remote.helios.tcp {
                      transport-class =""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                      port = 10000
                      transport-protocol = tcp
                      hostname = ""localhost""
                  }
              "
                };


                using (var server = new InventoryServiceServer(serverOptions))
                {
                    var mySystem = Akka.Actor.ActorSystem.Create("mySystem", ConfigurationFactory.ParseString(@"
                  akka.actor{provider= ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""}
                  akka.remote.helios.tcp {
                      transport-class =""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                      port = 0
                      transport-protocol = tcp
                      hostname = ""localhost""
                  }
              "));
                    var inventoryActor = mySystem.ActorSelection(serverOptions.InventoryActorAddress);


                    var result =
                     server.inventoryActor.Ask<IInventoryServiceCompletedMessage>(new ReserveMessage(
                            initialInventory.ProductId, 20));

                    result.ConfigureAwait(false);

                    Task.WaitAll(result);

                    if (result.Result.Successful)
                    {
                        Console.WriteLine(result.Result.RealTimeInventory);
                    }
                    else
                    {
                        Console.WriteLine(result.Result.RealTimeInventory);
                    }
                }
            }
        }

        public class BadInventoryStorage : IInventoryStorage
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public Task<StorageOperationResult<List<string>>> ReadAllInventoryIdAsync()
            {
                throw new NotImplementedException();
            }

            public Task<StorageOperationResult<IRealTimeInventory>> ReadInventoryAsync(string productId)
            {
                throw new NotImplementedException();
            }

            public Task<StorageOperationResult> WriteInventoryAsync(IRealTimeInventory inventoryObject)
            {
                throw new NotImplementedException();
            }

            public Task<bool> FlushAsync()
            {
                throw new NotImplementedException();
            }
        }

        public class GoodInventoryStorage:IInventoryStorage
        {

            Dictionary<string,IRealTimeInventory> Data=new Dictionary<string, IRealTimeInventory>();

            public void Dispose()
            {
                Data=new Dictionary<string, IRealTimeInventory>();
            }

            public async Task<StorageOperationResult<List<string>>> ReadAllInventoryIdAsync()
            {
                return await Task.FromResult(new StorageOperationResult < List < string >> (Data.Select(x => x.Key).ToList()));
            }

            public async Task<StorageOperationResult<IRealTimeInventory>> ReadInventoryAsync(string productId)
            {
                return await Task.FromResult(new StorageOperationResult<IRealTimeInventory>(Data.First(x => x.Key == productId).Value));
            }

            public async Task<StorageOperationResult> WriteInventoryAsync(IRealTimeInventory inventoryObject)
            {
                Data[inventoryObject.ProductId] = inventoryObject;
                return  await Task.FromResult(new StorageOperationResult());
            }

            public async Task<bool> FlushAsync()
            {
                return await Task.FromResult(true);
            }
        }
    }
