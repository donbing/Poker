using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PokerPolker
{
    public class PokerGame
    {
        readonly ObservableCollection<Guid> reservedSeats;
        readonly ConcurrentDictionary<int, Player> playerSeatingPositions = new ConcurrentDictionary<int, Player>();
        readonly HashSet<Player> readyPlayers = new HashSet<Player>();
        IEnumerable<Player> Players => playerSeatingPositions.OrderBy(x => x.Key).Select(x => x.Value);

        public PokerGame(IEventBroker domainEvents, int minPlayers = 3)
        {
            reservedSeats = new ObservableCollection<Guid>();

            var seatButler = domainEvents.Subscribe<SeatRequested>(p =>
            {
                if (p.SeatPosition.HasValue)
                    reservedSeats.Insert(p.SeatPosition.Value, p.ClientId);
                else
                    reservedSeats.Add(p.ClientId);

                domainEvents.Publish(new SeatReserved(p.ClientId, reservedSeats.IndexOf(p.ClientId)));
            });

            var addPlayerName = domainEvents.Subscribe<AddPlayer>(e =>
            {
                if (!reservedSeats.Contains(e.ClientId))
                {
                    domainEvents.Publish(new ReservationNotFound(e.ClientId));
                }
                else if (Players.Any(x => x.Name == e.PlayerName))
                {
                    domainEvents.Publish(new PlayerNameRefused(e.ClientId));
                }
                else
                {
                    var player = playerSeatingPositions[reservedSeats.IndexOf(e.ClientId)] = new Player(e.PlayerName);
                    
                    domainEvents.Publish(new PlayerAdded(e.ClientId, player));
                }
            });

            var waitForPlayersToBeReady = domainEvents.Subscribe<PlayerReady>(p =>
            {
                if (Players.Contains(p.player) && !readyPlayers.Contains(p.player))
                    readyPlayers.Add(p.player);

                if(readyPlayers.Count >= minPlayers && readyPlayers.Count == Players.Count())
                    domainEvents.Publish(new AllPlayersReady());
            });

            var cutForDealerWhenAllPlayersReady = domainEvents.Subscribe<AllPlayersReady>(p =>
            {
                var dealerCutRound = new DealerCutRound(Players.ToList(), domainEvents);
            });

            var startRoundOnceDealerChosen = domainEvents.Subscribe<DealerChosen>(c =>
            {
                var blindsRound = new RoundWithBlinds(c.dealer, Players.ToList(), domainEvents);
            });
        }
    }
}