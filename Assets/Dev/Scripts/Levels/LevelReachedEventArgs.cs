using System;

namespace Dev.Scripts.Levels
{
    public class LevelReachedEventArgs : EventArgs
    {
        public int Number { get; private set; }

        public LevelReachedEventArgs(int number)
        {
            Number = number;
        }
    }
}