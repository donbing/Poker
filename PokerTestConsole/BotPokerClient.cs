using PokerPolker.Events.GameStart;
using PokerPolker.Model;
using PokerPolker.Model.PlayingCards;

namespace PokerTestConsole;

public class BotPokerClient(Deck deck, IEventBroker events, string botName) 
    : PokerClient(deck, events)
{
    protected override void PlayerNameConfirmed(PlayerAddedToGame obj)
    {
        player = obj.Player;
        Events.Publish(new PlayerReady(obj.Player));
    }

    protected override void BadReservation(ReservationNotFound obj)
    {

    }

    protected override void ConfirmPlayerName(SeatReserved obj) =>
        Events.Publish(new ClaimSeatReservation(ClientId, botName, obj.SeatPosition));

    protected override void SetUserNameAgain(PlayerNameRefused obj) =>
        Events.Publish(new ClaimSeatReservation(ClientId, botName + "1", obj.SeatPosition));
}