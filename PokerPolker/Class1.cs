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


    public class GameDemo
    {
        private ObservableCollection<object> events = new ObservableCollection<object>();

        public GameDemo()
        {
            var deck = Suit.CreateDeck();

            var russ = new Player("Russ");
            var chris = new Player("Chris");

            var players = new Player[] {
                russ,
                chris,
            };

            // players are ready
            events.Add(new PlayerReady(chris));
            events.Add(new PlayerReady(russ));

            // split for dealer
            // assert that dealer is chris after split
            events.Add(new DealerCardCut(russ, deck.KingOfSpades));
            events.Add(new DealerCardCut(chris, deck.AceOfSpades));

            // enter blinds
            // assert blinds added
            events.Add(new BetPlaced(russ, 20m));
            events.Add(new BetPlaced(chris, 40m));

            // deal cards
            // assert cant do this until blinds are in
            events.Add(new PlayerCardsDealt());

            // russ calls my big blind
            events.Add(new BetPlaced(russ, 20m));
            // i check
            // test this cant be done at wrong time
            events.Add(new Check(chris));

            // we see the flop
            events.Add(new FlopDealt());

            // russ raises it 40
            events.Add(new BetPlaced(russ, 40m));
            // i call
            events.Add(new BetPlaced(chris, 40m));
            // turn card is dealt
            events.Add(new TurnCardDealt());

            // russ raises it 40
            events.Add(new BetPlaced(russ, 40m));
            // i reraise 40
            events.Add(new BetPlaced(chris, 80m));
            // russ calls
            events.Add(new BetPlaced(russ, 40m));

            // river card is dealt
            events.Add(new RiverCardDealt());
            // both check
            events.Add(new Check(russ));
            events.Add(new Check(chris));

            // game over!
        }
    }
}
