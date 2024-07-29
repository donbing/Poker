using System.Reactive.Disposables;
using System.Reactive.Linq;
using PokerPolker.Events;
using PokerPolker.Events.GameStart;
using PokerPolker.Model;

namespace PokerPolker.Client;

public class ConciergeInteraction(IEventBroker game, Guid clientId) : IInteraction<ISeatReservationRequest>
{
    private readonly CompositeDisposable d = new();

    public void RequestSpecificSeat(int seat)
    {
        Console.WriteLine($"client {clientId} wants seat {seat}");
        d.Clear();
        var response = Publish<IReservationRequestResponse, SpecificSeatRequested>(new SpecificSeatRequested(clientId, seat));
        d.Add(response.Where(x => x.ClientId == clientId).Subscribe(x =>
        {
            x.Visit(this);
        }));
    }

    public void RequestAnySeat()
    {
        Console.WriteLine($"client {clientId} wants the next seat");
        d.Clear();
        var response = Publish<IReservationRequestResponse, NextFreeSeatRequested>(new NextFreeSeatRequested(clientId));
        d.Add(response.Where(x => x.ClientId == clientId).Subscribe(x =>
        {
            x.Visit(this);
        }));
    }

    public void SeatRefused(Guid requestedClientId, int requestedSeatPosition)
    {
        Console.WriteLine("seat was taken");
        Console.WriteLine("please request another seat, or press enter to have one chosen for you");

        var keypress = Console.ReadKey();
        {
            if (keypress.Key == ConsoleKey.Enter)
            {
                RequestAnySeat();
            }
            else
            {
                RequestSpecificSeat(int.Parse(keypress.KeyChar.ToString()));
            }
        }
    }

    public void SeatReserved(Guid clientId, int seat)
    {
        Console.WriteLine($"client {clientId} reserved seat {seat}");
    }

    public IObservable<TResponse> Publish<TResponse, TRequest>(TRequest request) where TRequest : IRequest<TResponse>, IEvent
    {
        game.Publish(request);
        return game.OfType<TResponse>();
    }
}