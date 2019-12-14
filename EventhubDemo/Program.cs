using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace EventhubDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //SendMessageToEventHubs();
            ReceiveMessages(args).GetAwaiter().GetResult();
        }

        private static async Task ReceiveMessages(string[] args)
        {
            const string listenEventHubConnectionString = "Endpoint=sb://brianexperiment.servicebus.windows.net/;SharedAccessKeyName=ListenOnly;SharedAccessKey=YZEenPYwQBrky0ks6BMCaEFQYeszfXeet4qc8bHUDOQ=";
            const string eventHubName = "brianeventhub";
            const string storageContainerName = "eventhubcontainer";
            const string storageAccountName = "brianblobeventhubs";
            const string storageAccountKey = "RxmlN8ZSD1AgbW+Jwq1n9UdCPjHVLMI5PoishBrm28EdLkP203A5bn0MQ90Hi1WehDE3B/CuZj7UIZKLgU7aXQ==";

            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";

            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                eventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                listenEventHubConnectionString,
                storageConnectionString,
                storageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving.");

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }

        private static void SendMessageToEventHubs()
        {
            const string sendEventHubConnectionString = "Endpoint=sb://brianexperiment.servicebus.windows.net/;SharedAccessKeyName=SendOnlyPolicy;SharedAccessKey=jK+jJuxyEk/mZqO1YvbcsnPAWOvHwLbW5qSOMbsUWbo=;EntityPath=brianeventhub";

            var ehClient = EventHubClient.CreateFromConnectionString(sendEventHubConnectionString);

            for (int i = 0; i < 100; i++)
            {
                var eventData = new EventData(Encoding.UTF8.GetBytes($"My awesome message #{i}"));
                ehClient.SendAsync(eventData);
                Console.WriteLine($"Message #{i} sent");
                Thread.Sleep(50);
            }

            Console.WriteLine("Messages sent");
        }
    }
}
