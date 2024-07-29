using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PokerPolker.Model.PlayingCards
{
    public struct Suit(string v)
    {
        public static Suit Diamonds = new Suit("Diamonds");
        public static Suit Hearts = new Suit("Hearts");
        public static Suit Spades = new Suit("Spades");
        public static Suit Clubs = new Suit("Clubs");

        public static Suit[] All = { Diamonds, Hearts, Spades, Clubs };

        public static Deck CreateDeck() =>
            new Deck(All.SelectMany(CardValue.RangeForSuit));

        public override string ToString() =>
            v;
    }

    public struct CardValue(string v) : IComparable<CardValue>
    {
        public static CardValue Ace = new CardValue("Ace");
        public static CardValue Two = new CardValue("Two");
        public static CardValue Three = new CardValue("Three");
        public static CardValue Four = new CardValue("Four");
        public static CardValue Five = new CardValue("Five");
        public static CardValue Six = new CardValue("Six");
        public static CardValue Seven = new CardValue("Seven");
        public static CardValue Eight = new CardValue("Eight");
        public static CardValue Nine = new CardValue("Nine");
        public static CardValue Ten = new CardValue("Ten");
        public static CardValue Jack = new CardValue("J");
        public static CardValue Queen = new CardValue("Q");
        public static CardValue King = new CardValue("K");

        static readonly List<CardValue> Order = new() { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, };

        internal static IEnumerable<Card> RangeForSuit(Suit s) =>
            Order.Select(v => new Card(v, s));

        public int CompareTo(CardValue other) =>
            Order.IndexOf(this).CompareTo(Order.IndexOf(other));

        public override string ToString() =>
            v;
    }

    public class Deck(IEnumerable<Card> cards) : IEnumerable<Card>
    {
        private readonly IList<Card> cards = cards.ToList();

        // todo: 
        public Card AceOfSpades =>
            cards.Single(c => c.Matches(Suit.Spades, CardValue.Ace));

        public Card KingOfSpades =>
            cards.Single(c => c.Matches(Suit.Spades, CardValue.King));

        public Card QueenOfSpades =>
            cards.Single(c => c.Matches(Suit.Spades, CardValue.Queen));

        public Card PopRandom()
        {
            var card = cards.MinBy(e => Guid.NewGuid());
            cards.Remove(card);
            return card;
        }

        public IEnumerator<Card> GetEnumerator() =>
            cards.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            cards.GetEnumerator();
    }
}