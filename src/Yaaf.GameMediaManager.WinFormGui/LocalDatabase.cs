// ----------------------------------------------------------------------------
// This file (LocalDatabase.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Yaaf.GameMediaManager.Primitives;
    partial class Player : Helpers.ILinqEntity
    {
        public override string ToString()
        {
            return string.Format("Player(MyId: {0}, MyTags: {1}, Id: {2}, Name: {3}, EslPlayerId: {4})", MyId, MyTags, Id, Name, EslPlayerId);
        }

        partial void OnLoaded()
        {
            if (Id != 0)
            {
                SetOriginal();
            }
        }
        public void SetOriginal()
        {
            isOriginal = true;
        }

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

        private string myTags = null;

        private MyIdHelper myId;
        
        private bool isOriginal;

        public string MyTags
        {
            get
            {
                if (myTags == null)
                {
                    if (isOriginal)
                    {
                        myTags = string.Join(",", Player_Tag.Select(mt => mt.Tag.Name).ToArray());
                    }
                }
                return myTags ?? "";
            }
            set
            {
                myTags = value;
                if (isOriginal)
                {
                    var playerTags = (myTags ?? "").Split(',');
                    Player_Tag.Load();
                    var toRemove = this.Player_Tag.ToDictionary(t => t.Tag.Name, t => t);
                    var othercontext = FSharpInterop.Interop.GetNewContext();
                    var currentContext = this.GetContext();
                    foreach (var tag in playerTags)
                    {
                        toRemove.Remove(tag);
                        if (!(from assoc in Player_Tag where assoc.Tag.Name == tag select assoc).Any())
                        {
                            var association = new Player_Tag();
                            association.Player = this;
                            var newTag = othercontext.GetTag(tag); 
                            association.Tag = (from t in currentContext.Tags where t.Id == newTag.Id select t).Single();
                            
                            Player_Tag.Add(association);
                        }
                    }

                    
                    foreach (var playertag in toRemove.Values)
                    {
                        playertag.Player.Player_Tag.Remove(playertag);
                        playertag.Tag.Player_Tag.Remove(playertag);
                        currentContext.Player_Tags.DeleteOnSubmit(playertag);
                    }
                }
            }
        }
    }

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

    partial class MatchSession : Helpers.ILinqEntity
    {
        public override string ToString()
        {
            return string.Format("MatchSession(MyId: {0}, MyTags: {1}, Id: {2}, GameId: {3}, Startdate: {4}, Duration: {5}, EslMatchLink: {6}, Name: {7})", MyId, MyTags, Id, GameId, Startdate, Duration, EslMatchLink, Name);
        }

        private MyIdHelper myId;

        private string myTags;
        
        private bool isOriginal;

        private Game myGame;

        partial void OnLoaded()
        {
            if (Id != 0)
            {
                SetOriginal();
                myGame = Game;
            }
        }

        public void SetOriginal()
        {
            isOriginal = true;
        }

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

        public string MyTags
        {
            get
            {
                if (myTags == null)
                {
                    if (isOriginal)
                    {
                        myTags = string.Join(",", MatchSessions_Tag.Select(mt => mt.Tag.Name).ToArray());
                    }
                }
                return myTags ?? "";
            }
            set
            {
                myTags = value;
                if (isOriginal)
                {
                    var mediaTags = (myTags ?? "").Split(',');
                    MatchSessions_Tag.Load();
                    var toRemove = this.MatchSessions_Tag.ToDictionary(t => t.Tag.Name, t => t);
                    var context = FSharpInterop.Interop.GetNewContext();
                    var currentContext = this.GetContext();
                    foreach (var tag in mediaTags)
                    {
                        toRemove.Remove(tag);
                        if (!(from assoc in MatchSessions_Tag where assoc.Tag.Name == tag select assoc).Any())
                        {
                            var association = new MatchSessions_Tag();
                            association.MatchSession = this;
                            var newTag = context.GetTag(tag);
                            association.Tag = (from t in currentContext.Tags where t.Id == newTag.Id select t).Single();
                            MatchSessions_Tag.Add(association);
                        }
                    }

                    foreach (var matchSessionsTag in toRemove.Values)
                    {
                        matchSessionsTag.Tag.MatchSessions_Tag.Remove(matchSessionsTag);
                        matchSessionsTag.MatchSession.MatchSessions_Tag.Remove(matchSessionsTag);
                        currentContext.MatchSessions_Tags.DeleteOnSubmit(matchSessionsTag);
                    }
                }
            }
        }


        public Game MyGame
        {
            get
            {
                return myGame;
            }
            set
            {
                myGame = value;
                if (isOriginal)
                {
                    Game = myGame;
                }
            }
        }
    }

    partial class Matchmedia : Helpers.ILinqEntity
    {
        public override string ToString()
        {
            return string.Format("Matchmedia(MyId: {0}, MyTags: {1}, Id: {2}, Name: {3}, Type: {4}, Map: {5}, MatchSessionId: {6}, Path: {7}, Created: {8}, PlayerId: {9})", MyId, MyTags, Id, Name, Type, Map, MatchSessionId, Path, Created, PlayerId);
        }

        private MyIdHelper myId ;

        private string myTags;
        
        private MatchSessions_Player myMatchsessionPlayer;

        private bool isOriginal;
        
        partial void OnLoaded()
        {
            if (Id != 0)
            {
                SetOriginal();
                MyMatchSessionsPlayer = MatchSessions_Player;
            }
        }
        public void SetOriginal()
        {
            isOriginal = true;
        }

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

        public string MyTags
        {
            get
            {
                if (myTags == null)
                {
                    if (isOriginal)
                    {
                        myTags = string.Join(",", Matchmedia_Tag.Select(mt => mt.Tag.Name).ToArray());
                    }
                }
                return myTags ?? "";
            }
            set
            {
                myTags = value;
                if (isOriginal)
                {
                    var mediaTags = (myTags ?? "").Split(',');
                    Matchmedia_Tag.Load();
                    var toRemove = this.Matchmedia_Tag.ToDictionary(t => t.Tag.Name, t => t);
                    var context = FSharpInterop.Interop.GetNewContext();
                    var currentContext = this.GetContext();
                    foreach (var tag in mediaTags)
                    {
                        toRemove.Remove(tag);
                        if (!(from assoc in Matchmedia_Tag where assoc.Tag.Name == tag select assoc).Any())
                        {
                            var association = new Matchmedia_Tag();
                            association.Matchmedia = this;
                            var newTag = context.GetTag(tag);
                            association.Tag = (from t in currentContext.Tags where t.Id == newTag.Id select t).Single();
                            Matchmedia_Tag.Add(association);
                        }
                    }

                    foreach (var tagToRemove in toRemove.Values)
                    {
                        tagToRemove.Matchmedia.Matchmedia_Tag.Remove(tagToRemove);
                        tagToRemove.Tag.Matchmedia_Tag.Remove(tagToRemove);
                        currentContext.Matchmedia_Tags.DeleteOnSubmit(tagToRemove);
                    }
                }
            }
        }

        public MatchSessions_Player MyMatchSessionsPlayer
        {
            get
            {
                return myMatchsessionPlayer;
            }
            set
            {
                myMatchsessionPlayer = value;
                if (isOriginal)
                {
                    MatchSessions_Player = myMatchsessionPlayer;
                }
            }
        }
    }

    partial class Game
    {
        public override string ToString()
        {
            return string.Format("Game(Id: {0}, Name: {1}, Shortname: {2}, EnableNotification: {3}, EnableMatchForm: {4}, EnablePublicNotification: {5}, EnableWarMatchForm: {6}, WarMatchFormSaveFiles: {7}, PublicMatchFormSaveFiles: {8})", Id, Name, Shortname, EnableNotification, EnableMatchForm, EnablePublicNotification, EnableWarMatchForm, WarMatchFormSaveFiles, PublicMatchFormSaveFiles);
        }

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
        private MatchSession myMatchSession;

        private bool isOriginal;

        public override string ToString()
        {
            return string.Format("MatchSessions_Player(MyTeam: {0}, MySkill: {1}, MatchSessionId: {2}, PlayerId: {3}, Team: {4}, Skill: {5}, Description: {6}, Cheating: {7})", MyTeam, MySkill, MatchSessionId, PlayerId, Team, Skill, Description, Cheating);
        }

        partial void OnLoaded()
        {
            if (PlayerId != 0 && MatchSessionId != 0)
            {
                SetOriginal();
                MyMatchSession = MatchSession;
            }
        }

        private void SetOriginal()
        {
            isOriginal = true;
        }

        public MatchSession MyMatchSession
        {
            get
            {
                return myMatchSession;
            }
            set
            {
                myMatchSession = value;
                if (isOriginal)
                {
                    MatchSession = myMatchSession;
                }
            }
        }

        partial void OnCreated()
        {
        }

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
        public override string ToString()
        {
            return string.Format("WatchFolder(MyId: {0}, Id: {1}, GameId: {2}, Folder: {3}, NotifyOnInactivity: {4}, Filter: {5})", MyId, Id, GameId, Folder, NotifyOnInactivity, Filter);
        }

        private MyIdHelper myId;

        private Game myGame;

        private bool isOriginal;

        partial void OnCreated()
        {
            myId = new MyIdHelper(() => Id);
        }
        partial void OnLoaded()
        {
            if(Id != 0)
            {
                SetOriginal();
                MyGame = Game;
            }
        }

        public void SetOriginal()
        {
            isOriginal = true;
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

        public Game MyGame
        {
            get
            {
                return myGame;
            }
            set
            {
                myGame = value; 
                if (isOriginal)
                {
                    if (value != null)
                    {
                        Game = value;
                    }
                }
            }
        }
    }

    partial class LocalDataContext
    {
    }
}