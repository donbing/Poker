using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
// prompt for player name, assume playing order is as entered
// until first split added, then ask for all players splits
// once all players have cut, announce dealer
// prompt for player blinds and dealer to deal cards
// prompt betting round <- saga
// if(allallin) prompt for cards to be shown and 
//  then prompt flop, 
//  then prompt turn/river cards
// else
//  prompt dealer to flop
//  prompt player betting round <- saga
//  prompt dealer to show turn card
//  prompt player betting round <- saga
//  prompt dealer to show river card
//  prompt player betting round <- saga
//  announce winner!
namespace PokerPolker
{
    // i enter player names
    // it prompt for split
    // i enter splits
    // it shows dealer and prompts for blinds
    // 

    // states
    // awaiting more players
    // dealer split process
    // in play

    // blinds
    // cards
    // betting round
    // turn
    // betting
    // river
    // betting 
    // conclusion

    class Dealer
    {
        GameState gameState = GameState.WaitingForPlayers;
        private IList<Player> players = new List<Player>();
        private IDictionary<Player, Card> dealerCuts = new Dictionary<Player, Card>();

        void AddPlayer(string name)
        {
            players.Add(new Player(name));
        }

        void CutForDealer(Player player, Card card)
        {
            if (gameState != GameState.CuttingForDealer)
            {
                gameState = GameState.CuttingForDealer;
            }
            if (dealerCuts.Keys.Contains(player))
            {
                // raise player already Cut Event!
            }
            if (dealerCuts.Values.Contains(card))
            {
                // raise card already Cut Event!
            }
            dealerCuts[player] = card;
        }

        Dealer CalculateDealer()
        {
            gameState = GameState.PlayingHands;
            return new Dealer();
        }
    }
}
