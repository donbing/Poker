using PokerPolker.Model;

namespace PokerPolker.Events.BettingRounds
{
    public record RiverCardExpected(Player Dealer) : IEvent;
    public record RiverCardDealt : IEvent;
}