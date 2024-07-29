// See https://aka.ms/new-console-template for more information

using Docker.DotNet;
using Docker.DotNet.Models;
using IMQTTClientRx.Model;
using System.Text;
using MQTTClientRx.Service;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

Console.WriteLine("Hello, World!");

await StartContainers();
await StartEventBus();

async Task StartContainers()
{
    var imageName = "efrecon/mosquitto";

    var client = new DockerClientConfiguration()
        .CreateClient();

    var imageFetchProgress = new Progress<JSONMessage>(json => Console.WriteLine(json.Status));

    await client.Images
        .CreateImageAsync(new() { FromImage = imageName, Tag = "latest", }, null, imageFetchProgress);
    
    var containers = await client.Containers.ListContainersAsync(new());
    if (containers.Any(c => c.Names.Contains("/PokerPolker2")))
    {
        return;
    }

    var containerCreation = await client.Containers
        .CreateContainerAsync(new()
        {
            Name = "PokerPolker2",
            Image = imageName,
            Env = new[]
            {
                "MOSQUITTO_PERSISTENCE=true",
                "MOSQUITTO_PERSISTENCE_LOCATION=/mosquitto/data/",
                "MOSQUITTO_PERSISTENCE_LOG_DEST=/mosquitto/log/mosquitto.log",
                //"MOSQUITTO_PORT=1883",
                //"MOSQUITTO_BIND_ADDRESS=0.0.0.0",
                "MOSQUITTO_ALLOW_ANONYMOUS=true",
                "MOSQUITTO_LISTENER=1883 0.0.0.0",
            },
            HostConfig = new HostConfig
            {
                NetworkMode = "host",
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "1883/tcp", new PortBinding[] { new() { HostPort = "1883", HostIP = "0.0.0.0" } } },
                    { "8883/tcp", new PortBinding[] { new() { HostPort = "8883", HostIP = "0.0.0.0" } } },
                },
            },
        });

    foreach (var creationWarning in containerCreation.Warnings)
    {
        Console.WriteLine(creationWarning);
    }

    await client.Containers.StartContainerAsync(
        containerCreation.ID,
        new ContainerStartParameters()
    );
}

async Task StartEventBus()
{
    // netMQ
    //var subject = new SubjectNetMQ<int>("tcp://127.0.0.1:1883",);
    //subject.Subscribe(Console.Write);
    //subject.OnNext(42); // Sends 42.
    
    //    var eventBusFactory = new MqttFactory();

    //    using var eventBusClient = eventBusFactory
    //        .CreateMqttClient();

    //    var eventBusClientOptions = new MqttClientOptionsBuilder()
    //        .WithTcpServer("localhost", 1883)
    //        .Build();

    //    await eventBusClient
    //        .ConnectAsync(eventBusClientOptions, CancellationToken.None);

    //    await eventBusClient
    //        .DisconnectAsync(new MqttClientDisconnectOptionsBuilder()
    //            .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection).Build());
}

var mqttService = new MQTTService();

var mqttOptions = new opets();

var messageClient = mqttService
    .CreateObservableMQTTClient(
        mqttOptions, 
        willMessage: null);

var disposable = messageClient.observableMessage.Subscribe(
    msg =>
    {
        // Just some color coding to make it easier to see what topic is what
        Console.ForegroundColor = msg.Topic.Contains("PP") 
            ? ConsoleColor.Yellow 
            : ConsoleColor.Blue;

        Console.WriteLine($"{Encoding.UTF8.GetString(msg.Payload)}, " +
                          $"{msg.QualityOfServiceLevel.ToString()}, " +
                          $"Retain: {msg.Retain}, " +
                          $"Topic: {msg.Topic}");
    },
    ex => Console.WriteLine($"{ex.Message} : inner {ex.InnerException.Message}"),
    () => Console.WriteLine("Completed..."));

// IMPORTANT. The is new in version 3.2 and later. You have to connect to the MQTT Server.
//await messageClient.client.ConnectAsync();

public class opets : IClientOptions
{
    public bool UseTls => false;
    public IEnumerable<byte[]> Certificates { get; } = Enumerable.Empty<byte[]>();
    public bool IgnoreCertificateChainErrors { get; } = true;
    public bool IgnoreCertificateRevocationErrors { get; } = true;
    public bool AllowUntrustedCertificates { get; } = true;
    public Uri Uri { get; } = new Uri("mqtt://localhost:1883");
    public string UserName { get; }
    public string Password { get; }
    public string ClientId { get; }
    public bool CleanSession { get; }
    public TimeSpan KeepAlivePeriod { get; }
    public TimeSpan DefaultCommunicationTimeout { get; }
    public ProtocolVersion ProtocolVersion { get; }
    public ConnectionType ConnectionType { get; }
}