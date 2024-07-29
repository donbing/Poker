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
    readonly ObservableCollection<Guid> reservableSeats;
    readonly CompositeDisposable d = new();
    readonly ConcurrentDictionary<int, Player> seatedPlayers = new();
    readonly HashSet<Player> readyPlayers = new();

    public IEnumerable<Player> Players =>
        seatedPlayers
            .OrderBy(x => x.Key)
            .Select(x => x.Value);

    public TableConcierge(IEventBroker gameEvents, int minPlayers = 3, int maxPlayers = 10)
    {
        reservableSeats = new ObservableCollection<Guid>(new List<Guid>(maxPlayers));

        d.Add(gameEvents.Subscribe<NextFreeSeatRequested>(p =>
        {
            reservableSeats.Add(p.ClientId);
            gameEvents.Publish(new SeatReserved(p.ClientId, reservableSeats.IndexOf(p.ClientId)));
        }));

        d.Add(gameEvents.Subscribe<SpecificSeatRequested>(requested =>
        {
            var seatIsNotFree = requested.SeatPosition >= maxPlayers
                             || requested.SeatPosition < 0
                             || reservableSeats.ElementAtOrDefault(requested.SeatPosition) != default;
            if (seatIsNotFree)
            {
                gameEvents.Publish(new SeatReservationRefused(requested.ClientId, requested.SeatPosition));
                return;
            }

            reservableSeats.Insert(requested.SeatPosition, requested.ClientId);
            gameEvents.Publish(new SeatReserved(requested.ClientId, requested.SeatPosition));
        }));

        d.Add(gameEvents.Subscribe<ClaimSeatReservation>(e =>
        {
            if (!reservableSeats.Contains(e.ClientId))
            {
                gameEvents.Publish(new ReservationNotFound(e.ClientId));
            }
            else if (Players.Any(x => x.Name == e.PlayerName))
            {
                gameEvents.Publish(new PlayerNameRefused(e.ClientId, reservableSeats.IndexOf(e.ClientId)));
            }
            else
            {
                var player = seatedPlayers[reservableSeats.IndexOf(e.ClientId)] = new Player(e.PlayerName);

                gameEvents.Publish(new PlayerAddedToGame(e.ClientId, player));
            }
        }));

        d.Add(gameEvents.Subscribe<PlayerReady>(p =>
        {
            if (Players.Contains(p.Player))
            {
                readyPlayers.Add(p.Player);
            }

            if (readyPlayers.Count >= minPlayers && readyPlayers.Count == Players.Count())
            {
                gameEvents.Publish(new AllPlayersReady());
                Dispose();
            }
        }));
    }

    public void Dispose() => 
        d.Dispose();
}
