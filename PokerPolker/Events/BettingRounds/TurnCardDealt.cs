using PokerPolker.Model;

namespace PokerPolker.Events.BettingRounds
{
    public record TurnCardDealt : IEvent;
    public record TurnCardExpected(Player Dealer) : IEvent;
}