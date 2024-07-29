using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using PokerPolker.Client;
using PokerPolker.Events;
using PokerPolker.Events.BettingRounds;
using PokerPolker.Events.GameStart;
using PokerPolker.Model;
using PokerPolker.Model.PlayingCards;
using PokerTestConsole.EventStore;

namespace PokerTestConsole
{
    public class Program
    {
        public static readonly Deck deck = Suit.CreateDeck();

        static void Main()
        {
            var gameEvents = new Subject<IEvent>();

            EventReporter.WriteEventsToConsole(gameEvents);

            var eventBroker = new MemoryEventBroker(gameEvents);
            var prog = new PokerGame(eventBroker);

            var bobBotClient = new BotPokerClient(deck, eventBroker, "bob");
            bobBotClient.JoinGameAtSeat(0);

            var daveBotClient = new BotPokerClient(deck, eventBroker, "dave");
            daveBotClient.JoinGameAtSeat(1);

            var realPlayerClient = new PokerClient(deck, eventBroker);
            realPlayerClient.JoinGameAtSeat(2);

            while (true)
            {
                // todont: do this
            }
        }

        static void PlayExample3PlayerGame(Subject<IEvent> gameEvents)
        {
            var russ = new Player("Russ");
            var chris = new Player("Chris");
            var jak = new Player("Jak");

            gameEvents.OnNext(new PlayerReady(russ));
            gameEvents.OnNext(new PlayerReady(chris));
            gameEvents.OnNext(new PlayerReady(jak));

            gameEvents.OnNext(new AllPlayersReady());

            gameEvents.OnNext(new PlayerCutTheDeck(russ, deck.QueenOfSpades));
            gameEvents.OnNext(new PlayerCutTheDeck(chris, deck.KingOfSpades));
            gameEvents.OnNext(new PlayerCutTheDeck(jak, deck.AceOfSpades));

            gameEvents.OnNext(new SmallBlindAdded(russ, 10));
            gameEvents.OnNext(new BigBlindAdded(chris, 20));
            gameEvents.OnNext(new PlayerCardsDealt());

            gameEvents.OnNext(new PlayerCalled(jak, 20));
            gameEvents.OnNext(new PlayerCalled(russ, 10));
            gameEvents.OnNext(new PlayerChecked(chris));

            gameEvents.OnNext(new FlopDealt());
        }
    }
}
