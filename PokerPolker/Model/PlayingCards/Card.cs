using System;

namespace PokerPolker.Model.PlayingCards
{
    public record Card(CardValue Value, Suit Suit) : IComparable<Card>
    {
        public bool Matches(Suit suit, CardValue value) => 
            Value.Equals(value) && Suit.Equals(suit);

        public override string ToString() =>
            Value + " " + Suit;

        public int CompareTo(Card other) => 
            Value.CompareTo(other.Value);
    }
}