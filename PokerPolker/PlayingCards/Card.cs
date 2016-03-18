using System;

namespace PokerPolker
{
    public struct Card  : IComparable<Card>
    {
        private readonly Suit s;
        private readonly CardValue v;

        public Card(CardValue v, Suit s)
        {
            this.v = v;
            this.s = s;
        }

        public bool Matches(Suit suit, CardValue value) => v.Equals(value) && s.Equals(suit);
        public override string ToString() => v + " " + s;
        public int CompareTo(Card other) => v.CompareTo(other.v);
    }
}