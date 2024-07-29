using System;
using PokerPolker.Model;

namespace PokerPolker.Events.GameStart
{
    public interface IInteraction<T> : IInteractionState
    {
        void SeatRefused(Guid requestedClientId, int requestedSeatPosition);
        void SeatReserved(Guid clientId, int seat);
    }

    public interface IInteractionState
    {
    }

    public interface ISeatReservationRequest : IEvent, IRequest<IReservationRequestResponse>;

    public interface IReservationRequestResponse : IEvent
    {
        void Visit(IInteraction<ISeatReservationRequest> conciergeInteraction);
        Guid ClientId { get; }
    }

    public interface IRequest<out T> ;

    public record NextFreeSeatRequested(Guid ClientId) : ISeatReservationRequest;

    public record SpecificSeatRequested(Guid ClientId, int SeatPosition) : ISeatReservationRequest;

    public record SeatReservationRefused(Guid ClientId, int requestedSeatPosition) : IReservationRequestResponse
    {
        public void Visit(IInteraction<ISeatReservationRequest> conciergeInteraction)
        {
            conciergeInteraction.SeatRefused(ClientId, requestedSeatPosition);
        }
    }

    public record SeatReserved(Guid ClientId, int SeatPosition) : IReservationRequestResponse
    {
        public void Visit(IInteraction<ISeatReservationRequest> conciergeInteraction)
        {
            conciergeInteraction.SeatReserved(ClientId, SeatPosition);
        }
    }

    public record ClaimSeatReservation(Guid ClientId, string PlayerName, int SeatPosition) : IEvent;

    public record AllPlayersReady : IEvent;

    public record PlayerReady(Player Player) : IEvent;
    
    public record ReservationNotFound(Guid ClientId) : IEvent;

    public record PlayerNameRefused(Guid ClientId, int SeatPosition) : IEvent;

    public record PlayerAddedToGame(Guid ClientId, Player Player) : IEvent;
}