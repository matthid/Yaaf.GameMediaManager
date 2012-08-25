namespace Yaaf.WirePlugin.WinFormGui.Database
{
    using System;

    partial class Game
    {
        partial void OnCreated()
        {
            this.EnableMatchForm = true;
            this.EnableNotification = true;
            this.EnablePublicNotification = true;
            this.EnableWarMatchForm = true;
            this.WarMatchFormSaveFiles = true;
            this.PublicMatchFormSaveFiles = true;
        }
    }

    partial class MatchSessions_Player : IComparable<MatchSessions_Player>, IComparable
    {
        public int CompareTo(MatchSessions_Player other)
        {
            return this.PlayerId.CompareTo(other.PlayerId);
        }

        public int CompareTo(object obj)
        {
            var other = (MatchSessions_Player)obj;
            if (other == null)
            {
                throw new ArgumentException("Not a MatchSession_Player object");
            }
            return CompareTo(other);
        }
    }

    partial class WatchFolder
    {
    }

    partial class LocalDatabaseDataContext
    {
    }
}
