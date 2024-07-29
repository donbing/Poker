using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using PokerPolker.Events.GameStart;
using PokerPolker.Model.PlayingCards;

namespace PokerPolker.Model.Rounds
{
    public class CutForDealerRound : IRound
    {
        CompositeDisposable subscriptions = new();
        IDictionary<Player, Card> dealerCuts = new ConcurrentDictionary<Player, Card>();
        
        public CutForDealerRound(IList<Player> players, IEventBroker domainEvents)
        {
            subscriptions.Add(domainEvents.Subscribe<PlayerCutTheDeck>(c =>
            {
                // watch for players that might try to cheat  (ick, where'Suit the state)
                if (dealerCuts.ContainsKey(c.Player))
                {
                    domainEvents.Publish(new PlayerTriedToCutMultipleTimes(c.Player));
                    return;
                }
                
                // remember the cut, so we can see who wins (this should be an event projection?)
                dealerCuts[c.Player] = c.Card;

                // keep waiting if all players have not cut 
                if (players.Count - dealerCuts.Count != 0)
                {
                    var nextPlayer = players[players.IndexOf(c.Player) + 1];

                    domainEvents.Publish(new PlayerCutRequired(nextPlayer));
                    return;
                }

                // find the winner so we can make them dealer, or declare a draw
                var highCardHoldingPlayers = dealerCuts
                    .GroupBy(x => x.Value, x => x.Key)
                    .OrderByDescending(x => x.Key)
                    .First();

                var highCard = highCardHoldingPlayers.Key;

                if (highCardHoldingPlayers.Count() is 1)
                {
                    domainEvents.Publish(new DealerChosen(highCard, highCardHoldingPlayers.Single()));
                }
                else
                {
                    domainEvents.Publish(new CutRequiresTieBreak(highCard, highCardHoldingPlayers));
                }
            }));

            domainEvents.Publish(new PlayerCutRequired(players.First()));
        }

        public void Dispose() => 
            subscriptions.Dispose();
    }
}