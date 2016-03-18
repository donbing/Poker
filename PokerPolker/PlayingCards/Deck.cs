using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PokerPolker
{
    public struct Suit
    {
        public static Suit Diamonds = new Suit("Diamonds");
        public static Suit Hearts = new Suit("Hearts");
        public static Suit Spades = new Suit("Spades");
        public static Suit Clubs = new Suit("Clubs");
        public static Suit[] All = { Diamonds, Hearts, Spades, Clubs };

        string v;

        public Suit(string v)
        {
            this.v = v;
        }

        public override string ToString() => v;
        public static Deck CreateDeck() => new Deck(All.SelectMany(CardValue.RangeForSuit));
    }

    public struct CardValue : IComparable<CardValue>
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

        static readonly List<CardValue> Order = new List<CardValue> { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, };

        private string v;

        public CardValue(string v)
        {
            this.v = v;
        }

        internal static IEnumerable<Card> RangeForSuit(Suit s) => Order.Select(v => new Card(v, s));
        public int CompareTo(CardValue other) => Order.IndexOf(this).CompareTo(Order.IndexOf(other));
        public override string ToString() => v;
    }

    public class Deck : IEnumerable<Card>
    {
        private IList<Card> enumerable;

        public Deck(IEnumerable<Card> enumerable)
        {
            this.enumerable = enumerable.ToList();
        }

        public Card AceOfSpades => enumerable.Single(c => c.Matches(Suit.Spades, CardValue.Ace));
        public Card KingOfSpades => enumerable.Single(c => c.Matches(Suit.Spades, CardValue.King));
        public Card QueenOfSpades => enumerable.Single(c => c.Matches(Suit.Spades, CardValue.Queen));

        public Card PopRandom()
        {
            var card = enumerable.OrderBy(e => Guid.NewGuid()).First();
            enumerable.Remove(card);
            return card;
        }

        public IEnumerator<Card> GetEnumerator() => enumerable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
    }
}