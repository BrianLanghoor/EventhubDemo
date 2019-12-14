using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace EventhubDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            SendMessageToEventHubs();
            //ReceiveMessages(args).GetAwaiter().GetResult();
        }

        private static void SendMessageToEventHubs()
        {
            const string sendEventHubConnectionString = "<<<Sender Event Hub ConnectionString>>>";

            var ehClient = EventHubClient.CreateFromConnectionString(sendEventHubConnectionString);

            for (var i = 0; i < 100; i++)
            {
                var eventData = new EventData(Encoding.UTF8.GetBytes($"My awesome message #{i}"));
                ehClient.SendAsync(eventData);
                Console.WriteLine($"Message #{i} sent");
                Thread.Sleep(50);
            }

            Console.WriteLine("Messages sent");
        }

        private static async Task ReceiveMessages(string[] args)
        {
            const string listenEventHubConnectionString = "<<<Listen Event Hub ConnectionString>>>";
            const string eventHubName = "<<<Event Hub Name>>>";
            const string storageContainerName = "<<<Storage Container Name>>>";
            const string storageAccountName = "<<<Storage Account Name>>>";
            const string storageAccountKey = "<<<StorageAccountKey>>>";

            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";

            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                eventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                listenEventHubConnectionString,
                storageConnectionString,
                storageContainerName);

            var processorOptions = new EventProcessorOptions{};
            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(new EventProcessorOptions());

            Console.WriteLine("Receiving.");

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}