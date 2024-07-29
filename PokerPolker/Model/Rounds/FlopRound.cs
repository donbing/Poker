using System;
using System.Collections.Generic;
using System.Linq;
using PokerPolker.Events.BettingRounds;

namespace PokerPolker.Model.Rounds;

public class FlopRound : Round
{
    public FlopRound(Player dealer, IList<Player> playersInSeatingOrder, IEventBroker domainEvents) 
        : base(dealer, playersInSeatingOrder, domainEvents)
    {
        domainEvents.Publish(new FlopExpected(dealer));
        Subscribe<FlopDealt>(PromptFirstPlayerToBet);
    }

    private void PromptFirstPlayerToBet(FlopDealt obj)
    {
        var playerToBet = PlayersInDealingOrder().First();
        domainEvents.Publish(new BetExpected(playerToBet, 0));
    }
}
