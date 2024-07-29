using PokerPolker.Events.BettingRounds;
using System.Collections.Generic;
using System.Linq;
using PokerPolker.Events;

namespace PokerPolker.Model.Rounds;

public class RiverCardRound : Round
{
    public RiverCardRound(Player dealer, IList<Player> playersInSeatingOrder, IEventBroker domainEvents) 
        : base(dealer, playersInSeatingOrder, domainEvents)
    {
        domainEvents.Publish(new RiverCardExpected(dealer));
        Subscribe<RiverCardDealt>(PromptFirstPlayerToBet);
    }

    private void PromptFirstPlayerToBet(RiverCardDealt obj)
    {
        var playerToBet = PlayersInDealingOrder().First();
        domainEvents.Publish(new BetExpected(playerToBet, 0));
    }
}
