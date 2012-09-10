// ----------------------------------------------------------------------------
// This file (LocalDatabase.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui.Database.OldSchemas.v1_1_0_0
{
    using System;

    partial class Game
    {
        partial void OnCreated()
        {
            EnableMatchForm = true;
            EnableNotification = true;
            EnablePublicNotification = true;
            EnableWarMatchForm = true;
            WarMatchFormSaveFiles = true;
            PublicMatchFormSaveFiles = true;
        }
    }

    partial class MatchSessions_Player : IComparable<MatchSessions_Player>, IComparable
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            var other = (MatchSessions_Player)obj;
            if (other == null)
            {
                throw new ArgumentException("Not a MatchSession_Player object");
            }
            return CompareTo(other);
        }

        #endregion

        #region IComparable<MatchSessions_Player> Members

        public int CompareTo(MatchSessions_Player other)
        {
            return PlayerId.CompareTo(other.PlayerId);
        }

        #endregion
    }

    partial class WatchFolder
    {
    }

}