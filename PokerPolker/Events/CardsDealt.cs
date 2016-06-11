using System.Collections.Generic;

namespace PokerPolker
{
    public interface IPLayerAction : IEvent
    {

    }

    public class RoundStarted : IEvent
    {
        public IList<Player> players;

        public RoundStarted(IList<Player> playingOrder)
        {
            this.players = playingOrder;
        }
    }

    public class PlayerFolded : IPLayerAction
    {
        public Player Player;

        public PlayerFolded(Player player)
        {
            this.Player = player;
        }
    }

    public class PlayerRaised : IPLayerAction
    {
        public Player Player;

        public PlayerRaised(Player player)
        {
            this.Player = player;
        }
    }

    public class PlayerChecked : IPLayerAction
    {
        public Player Player;

        public PlayerChecked(Player player)
        {
            this.Player = player;
        }
    }

    public class PlayerCalled : IPLayerAction
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