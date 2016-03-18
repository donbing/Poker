using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PokerPolker
{
    public class DealerCutRound
    {
        private IEventBroker domainEvents;
        private IList<Player> players;
        IDictionary<Player, Card> dealerCuts = new ConcurrentDictionary<Player, Card>();
        private IDisposable checkCutsComplete;
        private IDisposable onFinish;
        IDisposable onDraw;

        public DealerCutRound(IList<Player> players, IEventBroker domainEvents)
        {
            this.players = players;
            this.domainEvents = domainEvents;

            checkCutsComplete = domainEvents.Subscribe<DealerCardCut>(c =>
            {
                dealerCuts[c.player] = c.card;
                // check if we've all cut 
                if ((players.Count - dealerCuts.Count) == 0)
                {
                    // find the winner so we can make them dealer, deal with DRAW!!
                    var cutsGroupedByCardAndOrderedHighestFirst = dealerCuts.GroupBy(x => x.Value).OrderByDescending(x => x.Key);
                    var CutsWithHighCard = cutsGroupedByCardAndOrderedHighestFirst.First();
                    if (CutsWithHighCard.Count() > 1)
                    {
                        domainEvents.Publish(new DealerCutDrawn(CutsWithHighCard.ToList()));
                    }
                    else {
                        var dealer = cutsGroupedByCardAndOrderedHighestFirst.First().Single().Key;
                        domainEvents.Publish(new DealerChosen(dealer));
                    }
                }
            });

            onDraw = domainEvents.Subscribe<DealerCutDrawn>(e =>
            {
                onDraw.Dispose();
                onFinish.Dispose();
                checkCutsComplete.Dispose();
                var dealerCutRound = new DealerCutRound(e.players, domainEvents);
            });

            onFinish = domainEvents.Subscribe<DealerChosen>(e =>
            {
                onDraw.Dispose();
                onFinish.Dispose();
                checkCutsComplete.Dispose();
            });
        }
    }
}