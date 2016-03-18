using System.Collections.Generic;

namespace PokerPolker
{
    public class RoundStarted : IEvent
    {
        public IList<Player> players;

        public RoundStarted(IList<Player> playingOrder)
        {
            this.players = playingOrder;
        }
    }

    public class PlayerFolded : IEvent
    {
        public Player Player;

        public PlayerFolded(Player player)
        {
            this.Player = player;
        }
    }

    public class PlayerChecked : IEvent
    {
        public Player Player;

        public PlayerChecked(Player player)
        {
            this.Player = player;
        }
    }

    public class PlayerCalled : IEvent
    {
        public Player Player;
        public int v;

        public PlayerCalled(int v, Player player)
        {
            this.v = v;
            this.Player = player;
        }
    }

    public class PlayerCardsDealt : IEvent
    {
        public PlayerCardsDealt()
        {
        }
    }
}