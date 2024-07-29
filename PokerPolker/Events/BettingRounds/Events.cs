using System.Collections.Generic;
using PokerPolker.Model;

namespace PokerPolker.Events.BettingRounds
{
    public record PlayerCardsDealt : IEvent;

    public interface IBlind
    {
        int Amount { get; }
        Player Player { get; }
    };

    // delete?
    public record RoundStarted(Player Dealer, IList<Player> Players) : IEvent;

    public record BigBlindExpected(Player Player, int Amount) : IEvent;

    public record SmallBlindExpected(Player Player, int Amount) : IEvent;

    public record SmallBlindAdded(Player Player, int Amount) : IEvent, IBlind;

    public record BigBlindAdded(Player Player, int Amount) : IEvent, IBlind;

    public record PlayerCardsRequired(Player Dealer) : IEvent;

    public record BetExpected(Player Player, int MinBet) : IEvent;

    public record PlayerTriedToCheckButHasNotBetEnough(Player Player) : IEvent;

    public record PlayerTriedToCallButHasNotBetEnough(Player Player) : IEvent;

    public record PlayerTriedToRaiseButHasNotBetEnough(Player Player) : IEvent;

    public record PlayerFolded(Player Player) : IEvent;

    public record PlayerRaised(Player Player, int Amount) : IEvent;

    public record PlayerChecked(Player Player) : IEvent;

    public record PlayerCalled(Player Player, int Amount) : IEvent;

    public record RoundComplete(Player Dealer, IList<Bet> Bets) : IEvent;

    public record PlayerPlayedOutOfTurn(Player Player) : IEvent;

    public record FlopExpected(Player Dealer) : IEvent;

    public record GameWon(Player Player, IList<Bet> Bets) : IEvent;
}