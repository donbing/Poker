using System.Reactive.Disposables;
using PokerPolker.Events.GameStart;
using PokerPolker.Model;

namespace PokerPolker.Client;

public class SeatingInteraction
{
    private CompositeDisposable d = new();
    protected readonly IEventBroker Events;
    protected readonly Guid ClientId;

    public SeatingInteraction(IEventBroker events, Guid clientId)
    {
        Events = events;
        ClientId = clientId;
        d.Add(Events.Subscribe<SeatReserved>(e => e.ClientId == ClientId, ConfirmPlayerName));
        d.Add(Events.Subscribe<ReservationNotFound>(e => e.ClientId == ClientId, BadReservation));
        d.Add(Events.Subscribe<PlayerNameRefused>(e => e.ClientId == ClientId, SetUserNameAgain));
        d.Add(Events.Subscribe<PlayerAddedToGame>(e => e.ClientId == ClientId, PlayerNameConfirmed));
    }

    protected virtual void PlayerNameConfirmed(PlayerAddedToGame obj)
    {
        var player = obj.Player;
        Console.WriteLine($"Game joined as {obj.Player.Name}: press any key when ready.");
        //Console.ReadKey();
        Events.Publish(new PlayerReady(player));
    }

    protected virtual void BadReservation(ReservationNotFound obj)
    {
        Console.WriteLine("server seems to have forgotten about us :(, please restart and try again");
    }

    protected virtual void SetUserNameAgain(PlayerNameRefused obj)
    {
        Console.WriteLine("that name is taken!");
        Console.WriteLine("Please Enter your player name");
        var name = "chris";
        Events.Publish(new ClaimSeatReservation(ClientId, name, obj.SeatPosition));
    }

    protected virtual void ConfirmPlayerName(SeatReserved obj)
    {
        Console.WriteLine($"you are in seat {obj.SeatPosition}");
        Console.WriteLine("Please Enter your player name");
        var name = "chris1";
        Events.Publish(new ClaimSeatReservation(ClientId, name, obj.SeatPosition));
    }
}