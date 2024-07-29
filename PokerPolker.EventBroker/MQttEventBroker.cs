using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using PokerPolker.Events;
using PokerPolker.Model;

namespace PokerPolker.EventBroker;

public class MQttEventBroker : IEventBroker
{
    MqttClientOptionsBuilder clientOptions = new MqttClientOptionsBuilder()
        .WithClientId("Client1")
        .WithTcpServer("localhost")
        .WithCredentials("bud", "%spencer%")
        .WithProtocolVersion(MqttProtocolVersion.V500)
        .WithCleanSession();

    private readonly IMqttClient mqttClient;
    private IObservable<MqttApplicationMessageReceivedEventArgs> rawEvents;
    private IObservable<IEvent> mappedEvents;

    public MQttEventBroker()
    {
        mqttClient = new MqttFactory().CreateMqttClient();
    }

    public async Task Connect()
    {
        await mqttClient.ConnectAsync(clientOptions.Build());
        await mqttClient.SubscribeAsync(new MqttTopicFilter { Topic = "MQTTClientRx/Test" });

        rawEvents = Observable
            .FromEvent<Func<MqttApplicationMessageReceivedEventArgs, Task>, MqttApplicationMessageReceivedEventArgs>(
                a => e =>
                {
                    a(e);
                    return Task.CompletedTask;
                },
                h => mqttClient.ApplicationMessageReceivedAsync += h,
                h => mqttClient.ApplicationMessageReceivedAsync -= h
            );
        mappedEvents = rawEvents.Select(MapEvent2);
    }

    private IEvent MapEvent2(MqttApplicationMessageReceivedEventArgs arg)
    {
        var eventTypes = typeof(IEvent).Assembly
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IEvent)))
            .ToDictionary(x => x.AssemblyQualifiedName);

        var evtType = eventTypes[arg.ApplicationMessage.ContentType];

        var json = arg.ApplicationMessage.Payload;

        var deserialize = System.Text.Json.JsonSerializer.Deserialize(json, evtType);

        return (IEvent)deserialize; 
    }

    public IObservable<T> Subscribe<T>() =>
        mappedEvents.OfType<T>();

    public async Task Publish<T>(T message)
    {
        Console.WriteLine($"publishing {message}");

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("MQTTClientRx/Test")
            .WithContentType(typeof(T).AssemblyQualifiedName)
            .WithPayload(System.Text.Json.JsonSerializer.Serialize(message))
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    }

    public IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> onConsume) where TEvent : IEvent =>
        mappedEvents
            .OfType<TEvent>()
            .Where(filter)
            .Subscribe(onConsume);

    public IDisposable Subscribe<TEvent>(Action<TEvent> onConsume) where TEvent : IEvent =>
        mappedEvents
            .OfType<TEvent>()
            .Subscribe(onConsume);

    public async Task DisconnectAsync() => 
        await mqttClient.DisconnectAsync();

    public IDisposable Subscribe(IObserver<IEvent> observer)
    {
        return mappedEvents.Subscribe(observer);
    }

    IEventBroker IEventBroker.Publish<TEvent>(TEvent @event)
    {
        var observable = Publish(@event).ToObservable();
        return this;
    }
}