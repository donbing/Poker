using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using PokerPolker.Events;
using PokerPolker.Events.BettingRounds;
using PokerPolker.Events.GameStart;
using PokerPolker.Model.Rounds;

namespace PokerPolker.Model
{
    public class PokerGame
    {
        public IEventBroker Events { get; }
        readonly ObservableCollection<IRound> rounds = new();

        public PokerGame(IEventBroker domainEvents, int minPlayers = 3, int maxPlayers = 10)
        {
            Events = domainEvents;

            var concierge = new TableConcierge(domainEvents);
            // todo: disposes!
            var cutForDealerWhenAllPlayersReady = domainEvents.Subscribe<AllPlayersReady>(p =>
            {
                var dealerCutRound = new CutForDealerRound(concierge.Players.ToList(), domainEvents);
                rounds.Add(dealerCutRound);
            });

            // start new round of cuts on draw result
            domainEvents.Subscribe<CutRequiresTieBreak>(e =>
            {
                rounds.Last().Dispose();
                var dealerCutRound = new CutForDealerRound(e.Players.ToList(), domainEvents);
                rounds.Add(dealerCutRound);
            });

            var startBlindRoundWhenDealerChosen = domainEvents.Subscribe<DealerChosen>(dc =>
            {
                rounds.Last().Dispose();
                rounds.Add(new BlindsRound(dc.Player, concierge.Players.ToList(), domainEvents));

                var startFlopRoundOnBlindRoundComplete = domainEvents.Subscribe<RoundComplete>(rc =>
                {
                    rounds.Last().Dispose();
                    switch (rounds.Last())
                    {
                        case BlindsRound:
                            rounds.Add(new FlopRound(rc.Dealer, concierge.Players.ToList(), domainEvents));
                            break;
                        case FlopRound:
                            rounds.Add(new TurnCardRound(rc.Dealer, concierge.Players.ToList(), domainEvents));
                            break;
                        case TurnCardRound:
                            rounds.Add(new RiverCardRound(rc.Dealer, concierge.Players.ToList(), domainEvents));
                            break;
                        case RiverCardRound:
                            // game over, so who won?
                            break;
                    }
                });
            });
        }
    }
}