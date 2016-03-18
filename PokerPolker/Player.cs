using System.Diagnostics.Contracts;

namespace PokerPolker
{
    public struct Player
    {
        public string Name;

        public Player(string v)
        {
            Contract.Requires(v != null);
            this.Name = v;
        }

        public override string ToString() => Name;
    }
}