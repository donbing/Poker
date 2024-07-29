using System.Collections.Generic;
using PokerPolker.Model;
using PokerPolker.Model.PlayingCards;

namespace PokerPolker.Events.GameStart
{
    public record CutRequiresTieBreak(Card Card, IEnumerable<Player> Players) : IEvent;

    public record PlayerCutTheDeck(Player Player, Card Card) : IEvent;

    public record DealerChosen(Card Card, Player Player) : IEvent;

    public record PlayerTriedToCutMultipleTimes(Player Player) : IEvent;

    public record PlayerCutRequired(Player Player) : IEvent;
}