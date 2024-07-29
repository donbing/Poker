using PokerPolker.Events.BettingRounds;
using System.Collections.Generic;
using System.Linq;

namespace PokerPolker.Model.Rounds;

public class TurnCardRound : Round
{
    public TurnCardRound(Player dealer, IList<Player> playersInSeatingOrder, IEventBroker domainEvents) : base(dealer, playersInSeatingOrder, domainEvents)
    {
        domainEvents.Publish(new TurnCardExpected(dealer));
        Subscribe<TurnCardDealt>(PromptFirstPlayerToBet);
    }

    private void PromptFirstPlayerToBet(TurnCardDealt obj)
    {
        var playerToBet = PlayersInDealingOrder().First();
        domainEvents.Publish(new BetExpected(playerToBet, 0));
    }
}