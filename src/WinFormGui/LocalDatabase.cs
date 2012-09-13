// ----------------------------------------------------------------------------
// This file (LocalDatabase.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Yaaf.WirePlugin.Primitives;

    class MyIdHelper
    {
        private readonly Func<int> id;

        public MyIdHelper(Func<int> id)
        {
            this.id = id;
        }

        private int? myId;
        public int MyId
        {
            get
            {
                if (!myId.HasValue)
                {
                    var readId = id();
                    myId = readId;
                    return readId;
                }

                return myId.Value;
            }
            set
            {
                myId = value;
            }
        }
    }

    partial class MatchSession
    {
        private MyIdHelper myId;

        private string myTags;

        private bool isLoaded;

        partial void OnLoaded()
        {
            if (Id != 0)
            {
                MyTags = string.Join(",", MatchSessions_Tag.Select(mt => mt.Tag.Name).ToArray());
                isLoaded = true;
            }
        }
        partial void OnCreated()
        {
            myId = new MyIdHelper(() => Id);
            MyTags = "";
        }

        public int MyId
        {
            get
            {
                return myId.MyId;
            }
            set
            {
                myId.MyId = value;
            }
        }

        public string MyTags
        {
            get
            {
                return myTags;
            }
            set
            {
                myTags = value;
                if (isLoaded)
                {
                    var media_tags = (myTags ?? "").Split(',');
                    var toRemove = this.MatchSessions_Tag.ToDictionary(t => t.Tag.Name, t => t);
                    var context = FSharpInterop.Interop.GetNewContext();
                    foreach (var tag in media_tags)
                    {
                        toRemove.Remove(tag);
                        if (!(from assoc in MatchSessions_Tag where assoc.Tag.Name == tag select assoc).Any())
                        {
                            var association = new MatchSessions_Tag();
                            association.MatchSession = this;
                            var newTag = context.GetTag(tag);
                            association.TagId = newTag.Id;
                            MatchSessions_Tag.Add(association);
                        }
                    }

                    foreach (var matchSessionsTag in toRemove.Values)
                    {
                        MatchSessions_Tag.Remove(matchSessionsTag);
                    }
                }
            }
        }
    }

    partial class Matchmedia
    {
        
        private MyIdHelper myId ;

        private string myTags;

        private bool isLoaded;
        partial void OnLoaded()
        {
            if (Id != 0)
            {
                MyTags = string.Join(",", Matchmedia_Tag.Select(mt => mt.Tag.Name).ToArray());
                isLoaded = true;
            }
        }
        partial void OnCreated()
        {
            myId = new MyIdHelper(() => Id);
            MyTags = "";
        }

        public int MyId
        {
            get
            {
                return myId.MyId;
            }
            set
            {
                myId.MyId = value;
            }
        }

        public string MyTags
        {
            get
            {
                return myTags;
            }
            set
            {
                myTags = value;
                if (isLoaded)
                {
                    var mediaTags = (myTags ?? "").Split(',');
                    var toRemove = this.Matchmedia_Tag.ToDictionary(t => t.Tag.Name, t => t);
                    var context = FSharpInterop.Interop.GetNewContext();
                    foreach (var tag in mediaTags)
                    {
                        toRemove.Remove(tag);
                        if (!(from assoc in Matchmedia_Tag where assoc.Tag.Name == tag select assoc).Any())
                        {
                            var association = new Matchmedia_Tag();
                            association.Matchmedia = this;
                            var newTag = context.GetTag(tag);
                            association.TagId = newTag.Id;
                            Matchmedia_Tag.Add(association);
                        }
                    }

                    foreach (var matchSessionsTag in toRemove.Values)
                    {
                        Matchmedia_Tag.Remove(matchSessionsTag);
                    }
                }
            }
        }
    }

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

    partial class MatchSessions_Player
    {
        private MyIdHelper myPlayerId;

        private MyIdHelper myMatchSessionId;

        private string myTags;

        private bool isLoaded;


        partial void OnLoaded()
        {
            if (PlayerId != 0 && MatchSessionId != 0)
            {
                SetLoaded();
            }
        }

        public void SetLoaded()
        {
            if (isLoaded) return;
            MyTags = string.Join(",", this.Player.Player_Tag.Select(mt => mt.Tag.Name).ToArray());
            MyName = Player.Name;
            MyEslId = Player.EslPlayerId;
            isLoaded = true;
            removeTags = new List<Player_Tag>();
        }

        partial void OnCreated()
        {
            myPlayerId = new MyIdHelper(() => PlayerId);
            myMatchSessionId = new MyIdHelper(() => MatchSessionId);

            MyTags = "";
            MyName = "unknown";
            MyEslId = null;
        }

        public int MyPlayerId
        {
            get
            {
                return myPlayerId.MyId;
            }
            set
            {
                myPlayerId.MyId = value;
            }
        }

        public int MyMatchSessionId
        {
            get
            {
                return myMatchSessionId.MyId;
            }
            set
            {
                myMatchSessionId.MyId = value;
            }
        }
        private List<Player_Tag> removeTags = null;
        public string MyTags
        {
            get
            {
                return myTags;
            }
            set
            {
                myTags = value;
                if (isLoaded)
                {
                    var playerTags = (myTags ?? "").Split(',');
                    var toRemove = this.Player.Player_Tag.ToDictionary(t => t.Tag.Name, t => t);
                    var context = FSharpInterop.Interop.GetNewContext();
                    foreach (var tag in playerTags)
                    {
                        toRemove.Remove(tag);
                        if (!(from assoc in Player.Player_Tag where assoc.Tag.Name == tag select assoc).Any())
                        {
                            var association = new Player_Tag();
                            association.Player = Player;
                            var newTag = context.GetTag(tag);
                            association.TagId = newTag.Id;
                            Player.Player_Tag.Add(association);
                        }
                    }
                    RemoveTags.AddRange(toRemove.Values);
                }
            }
        }

        public string MyName { get; set; }
        public int? MyEslId { get; set; }
        public PlayerTeam MyTeam
        {
            get
            {
                var team = Team;
                if (team == 0)
                {
                    return PlayerTeam.Team11;
                }
                
                if (team > 11) team = 11;
                return (PlayerTeam)team;
            }
            set
            {
                this.Team = (byte)value;
            }
        }

        public PlayerSkill MySkill
        {
            get
            {
                if (!this.Skill.HasValue)
                {
                    return PlayerSkill.Unknown;
                }
                else
                {
                    var mod = (Skill.Value / 10) * 10;
                    if (mod > 100) mod = 100;
                    return (PlayerSkill)mod;
                }
            }
            set
            {
                if (value == PlayerSkill.Unknown)
                {
                    this.Skill = null;
                }
                else
                {
                    this.Skill = (byte)value;
                }
            }
        }

        internal List<Player_Tag> RemoveTags
        {
            get
            {
                return removeTags;
            }
        }
    }

    public enum PlayerSkill : byte
    {
        Unknown = 0,
        UUlow = 10,
        Ulow = 20,
        Low = 30,
        LowMid = 40,
        Mid = 50,
        MidHigh = 60,
        High = 70,
        Eas = 70,
        Eps = 80,
        Godlike = 90,
        JuckNorris = 100,
    }

    public enum PlayerTeam : byte
    {
        Team1 = 1,
        Team2 = 2,
        Team3 = 3,
        Team4 = 4,
        Team5 = 5,
        Team6 = 6,
        Team7 = 7,
        Team8 = 8,
        Team9 = 9,
        Team10 = 10,
        Team11 = 11
    }

    partial class WatchFolder
    {
        private MyIdHelper myId;
        partial void OnCreated()
        {
            myId = new MyIdHelper(() => Id);
        }
        public int MyId
        {
            get
            {
                return myId.MyId;
            }
            set
            {
                myId.MyId = value;
            }
        }
    }

    partial class LocalDataContext
    {
    }
}