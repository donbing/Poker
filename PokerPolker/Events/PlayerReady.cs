using System;
using System.Diagnostics.Contracts;

namespace PokerPolker
{
    public class AddPlayer : IEvent
    {
        public AddPlayer(Guid clientId, string playerName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(playerName));
            Contract.Requires(clientId != Guid.Empty);
            this.ClientId = clientId;
            this.PlayerName = playerName;
        }

        public string PlayerName { get; set; }
        public Guid ClientId { get; set; }
    }

    public class PlayerAdded : IEvent
    {
        public PlayerAdded(Guid clientId, Player player)
        {
            this.ClientId = clientId;
            Player = player;
        }
        public Guid ClientId { get; set; }
        public Player Player { get; set; }
    }

    public class PlayerReady : IEvent
    {
        public Player player;

        public PlayerReady(Player player)
        {
            this.player = player;
        }
    }

    public class SeatRequested : IEvent
    {
        public SeatRequested(Guid clientId, int? seatPostition = null)
        {
            this.ClientId = clientId;
            this.SeatPosition = seatPostition;
        }

        public int? SeatPosition { get; set; }
        public Guid ClientId { get; set; }
    }

    public class SeatReserved : IEvent
    {
        public Guid ClientId;
        public readonly int SeatPosition;

        public SeatReserved(Guid clientId, int seatPosition)
        {
            this.ClientId = clientId;
            this.SeatPosition = seatPosition;
        }
    }

    public class ReservationNotFound : IEvent
    {
        public Guid ClientId;

        public ReservationNotFound(Guid clientId)
        {
            this.ClientId = clientId;
        }
    }
    public class PlayerNameRefused : IEvent
    {
        public Guid ClientId;

        public PlayerNameRefused(Guid clientId)
        {
            this.ClientId = clientId;
        }
    }

}