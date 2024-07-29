using PokerPolker.Events.GameStart;
using PokerPolker.Model;
using PokerPolker.Model.PlayingCards;

namespace PokerPolker.Client;

public class BotPokerClient(Deck deck, IEventBroker events, string botName) 
    : PokerClient(deck, events)
{
    protected override SeatingInteraction SeatingInteraction(IEventBroker events) => 
        new BotSeatingInteraction(events, ClientId, botName);

    class BotSeatingInteraction(IEventBroker events, Guid clientId, string botName)
        : SeatingInteraction(events, clientId)
    {
        protected override void PlayerNameConfirmed(PlayerAddedToGame obj) =>
            Events.Publish(new PlayerReady(obj.Player));

        protected override void ConfirmPlayerName(SeatReserved obj) =>
            Events.Publish(new ClaimSeatReservation(ClientId, botName, obj.SeatPosition));

        protected override void SetUserNameAgain(PlayerNameRefused obj) =>
            Events.Publish(new ClaimSeatReservation(ClientId, botName + "1", obj.SeatPosition));
    }
}
