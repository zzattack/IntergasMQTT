using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace IntergasMQTT
{
    public class DomoticzMqttClient : IDisposable
    {
        private IMqttClient _client;
        private IMqttClientOptions _options;

        private int DomoticzHwIdx = 9;

        public DomoticzMqttClient()
        {
            _options = new MqttClientOptionsBuilder()
                .WithClientId("intergas")
                .WithTcpServer("10.31.45.10", 1883)
                .WithCredentials("intergas", "9k470eekXQ!2")
                .WithCleanSession()
                .Build();

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _client.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER,m RECONNECT IN 2s ###");
                await Task.Delay(TimeSpan.FromSeconds(2));

                try
                {
                    await _client.ConnectAsync(_options, CancellationToken.None);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

            _client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED TO SERVER ###");

                await CreateDevices();

                // Subscribe to a topic
                await _client.SubscribeAsync("domoticz/out");
                // await _client.SubscribeAsync("domoticz/in");
                Console.WriteLine("### SUBSCRIBED ###");
            });

        }

        private async Task CreateDevices()
        {
            //_client.
        }

        public async void Start()
        {
            await _client.ConnectAsync(_options, CancellationToken.None); // Since 3.0.5 with CancellationToken
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
