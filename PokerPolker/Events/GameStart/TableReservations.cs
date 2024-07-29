using System;
using PokerPolker.Model;

namespace PokerPolker.Events.GameStart
{
    public record ClaimSeatReservation(Guid ClientId, string PlayerName, int SeatPosition) : IEvent;

    public record PlayerAddedToGame(Guid ClientId, Player Player) : IEvent;

    public record AllPlayersReady : IEvent;

    public record PlayerReady(Player Player) : IEvent;

    public record SeatRequested(Guid ClientId, int? SeatPosition) : IEvent;

    public record SeatReserved(Guid ClientId, int SeatPosition) : IEvent;

    public record ReservationNotFound(Guid ClientId) : IEvent;

    public record PlayerNameRefused(Guid ClientId, int SeatPosition) : IEvent;
}