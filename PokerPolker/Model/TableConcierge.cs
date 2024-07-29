using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using PokerPolker.Events.GameStart;

namespace PokerPolker.Model;

public class TableConcierge
{
    readonly ObservableCollection<Guid> reservedSeats;
    readonly CompositeDisposable subscriptions = new();
    readonly ConcurrentDictionary<int, Player> playerSeatingPositions = new();
    readonly HashSet<Player> readyPlayers = new();

    public IEnumerable<Player> Players =>
        playerSeatingPositions
            .OrderBy(x => x.Key)
            .Select(x => x.Value);

    public TableConcierge(IEventBroker domainEvents, int minPlayers = 3, int maxPlayers = 10)
    {
        reservedSeats = new ObservableCollection<Guid>(new List<Guid>(maxPlayers));
        subscriptions.Add(domainEvents.Subscribe<SeatRequested>(p =>
        {
            if (p.SeatPosition.HasValue &&
                p.SeatPosition.Value < reservedSeats.Count &&
                reservedSeats[p.SeatPosition.Value] != default)
            {
                reservedSeats.Insert(p.SeatPosition.Value, p.ClientId);
            }
            else
            {
                reservedSeats.Add(p.ClientId);
            }

            domainEvents.Publish(new SeatReserved(p.ClientId, reservedSeats.IndexOf(p.ClientId)));
        }));

        subscriptions.Add(domainEvents.Subscribe<ClaimSeatReservation>(e =>
        {
            if (!reservedSeats.Contains(e.ClientId))
            {
                domainEvents.Publish(new ReservationNotFound(e.ClientId));
            }
            else if (Players.Any(x => x.Name == e.PlayerName))
            {
                domainEvents.Publish(new PlayerNameRefused(e.ClientId, reservedSeats.IndexOf(e.ClientId)));
            }
            else
            {
                var player = playerSeatingPositions[reservedSeats.IndexOf(e.ClientId)] = new Player(e.PlayerName);

                domainEvents.Publish(new PlayerAddedToGame(e.ClientId, player));
            }
        }));

        subscriptions.Add(domainEvents.Subscribe<PlayerReady>(p =>
        {
            if (Players.Contains(p.Player))
            {
                readyPlayers.Add(p.Player);
            }

            if (readyPlayers.Count >= minPlayers && readyPlayers.Count == Players.Count())
            {
                domainEvents.Publish(new AllPlayersReady());
            }
        }));
    }

    public void Dispose() => 
        subscriptions.Dispose();
}