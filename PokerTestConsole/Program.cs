using PokerPolker;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;

namespace PokerTestConsole
{
    public class Program
    {
        public static readonly Deck deck = Suit.CreateDeck();

        static void Main(string[] args)
        {
            var domainEvents = new Subject<IEvent>();

            EventReporter.WriteEventsToConsole(domainEvents);

            var eventBroker = new EventBroker(domainEvents);
            var prog = new PokerGame(eventBroker);

            var bobBotClient = new PokerClient.BotPokerClient(deck, eventBroker, "bob");
            var daveBotClient = new PokerClient.BotPokerClient(deck, eventBroker, "dave");
            var realPlayerClient = new PokerClient(deck, eventBroker);
            
            while (true)
            {
                // dinada
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

            domainEvents.OnNext(new DealerCardCut(russ, deck.QueenOfSpades));
            domainEvents.OnNext(new DealerCardCut(chris, deck.KingOfSpades));
            domainEvents.OnNext(new DealerCardCut(jak, deck.AceOfSpades));

            domainEvents.OnNext(new SmallBlindAdded(10, russ));
            domainEvents.OnNext(new BigBlindAdded(20, chris));
            domainEvents.OnNext(new PlayerCardsDealt());

            domainEvents.OnNext(new PlayerCalled(20, jak));
            domainEvents.OnNext(new PlayerCalled(10, russ));
            domainEvents.OnNext(new PlayerChecked(chris));

            domainEvents.OnNext(new FlopDealt());
        }
    }
}
