// See https://aka.ms/new-console-template for more information

using Docker.DotNet;
using Docker.DotNet.Models;
using PokerPolker.Client;
using PokerPolker.EventBroker;
using PokerPolker.Model;
using PokerPolker.Model.PlayingCards;

Console.WriteLine("Hello, World!");

await StartContainers();

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
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    ["1883/tcp"] = new[] { new PortBinding() { HostPort = "1883",  } },
                    ["8883/tcp"] = new[] { new PortBinding() { HostPort = "8883",  } },
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

var broker = new MQttEventBroker();
await broker.Connect();
Console.WriteLine("client connected.");
var game = new PokerGame(broker);

var deck = Suit.CreateDeck();
var bobBotClient = new BotPokerClient(deck, broker, "bob");
bobBotClient.JoinGameAtSeat(0);

var daveBotClient = new BotPokerClient(deck, broker, "dave");
daveBotClient.JoinGameAtSeat(1);

var realPlayerClient = new PokerClient(deck, broker);
realPlayerClient.JoinGameAtSeat(2);

Console.WriteLine("publish...");

await Task.Delay(20000);
await broker.DisconnectAsync();

Console.WriteLine("connect...");