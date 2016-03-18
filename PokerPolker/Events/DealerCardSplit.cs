using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PokerPolker
{
    public class DealerCutDrawn : IEvent
    {
        public  List<Player> players;

        public DealerCutDrawn(List<KeyValuePair<Player, Card>> list)
        {
            this.players = list.Select(l => l.Key).ToList();
        }
    }
    public class DealerChosen : IEvent
    {
        public Player dealer;

        public DealerChosen(Player dealer)
        {
            this.dealer = dealer;
        }
    }
    public class SmallBlindAdded : IEvent
    {
        public Player player;
        public int v;
        
        public SmallBlindAdded(int v, Player player) 
        {
            this.v = v;
            this.player = player;
        }
    }
    public class BigBlindAdded : IEvent
    {
        public Player player;
        public int v;
        
        public BigBlindAdded(int v, Player player)
        {
            this.v = v;
            this.player = player;
        }
    }

    public class DealerCardCut : IEvent
    {
        public Card card;
        public Player player;

        public DealerCardCut(Player player, Card card)
        {
            this.player = player;
            this.card = card;
        }
    }
}