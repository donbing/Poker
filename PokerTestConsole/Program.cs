using System.Reactive.Subjects;
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
            var domainEvents = new Subject<IEvent>();

            EventReporter.WriteEventsToConsole(domainEvents);

            var eventBroker = new EventBroker(domainEvents);
            var prog = new PokerGame(eventBroker);

            var bobBotClient = new BotPokerClient(deck, eventBroker, "bob");
            bobBotClient.JoinGame(prog);

            var daveBotClient = new BotPokerClient(deck, eventBroker, "dave");
            daveBotClient.JoinGame(prog);

            var realPlayerClient = new PokerClient(deck, eventBroker);
            realPlayerClient.JoinGame(prog);

            while (true)
            {
                // pause for events
            }
        }

        static void PlayExample3PlayerGame(Subject<IEvent> domainEvents)
        {
            var russ = new Player("Russ");
            var chris = new Player("Chris");
            var jak = new Player("Jak");

            domainEvents.OnNext(new PlayerReady(russ));
            domainEvents.OnNext(new PlayerReady(chris));
            domainEvents.OnNext(new PlayerReady(jak));

            domainEvents.OnNext(new AllPlayersReady());

            domainEvents.OnNext(new PlayerCutTheDeck(russ, deck.QueenOfSpades));
            domainEvents.OnNext(new PlayerCutTheDeck(chris, deck.KingOfSpades));
            domainEvents.OnNext(new PlayerCutTheDeck(jak, deck.AceOfSpades));

            domainEvents.OnNext(new SmallBlindAdded(russ, 10));
            domainEvents.OnNext(new BigBlindAdded(chris, 20));
            domainEvents.OnNext(new PlayerCardsDealt());

            domainEvents.OnNext(new PlayerCalled(jak, 20));
            domainEvents.OnNext(new PlayerCalled(russ, 10));
            domainEvents.OnNext(new PlayerChecked(chris));

            domainEvents.OnNext(new FlopDealt());
        }
    }
}
