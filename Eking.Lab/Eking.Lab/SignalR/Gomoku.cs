using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignalR.Hubs;
using System.Linq;

namespace Eking.Lab.SignalR
{
    public class Gomoku : Hub, IDisconnect, IConnected
    {
        public class GomokuData
        {
            private static GomokuData _i;
            internal static GomokuData I
            {
                get
                {
                    return _i ?? (_i = new GomokuData());
                }
            }

            public HashSet<Player> Players = new HashSet<Player>();

            public HashSet<Match> Matches = new HashSet<Match>();
        }

        public const int MAP_SIZE = 1000;
        public class Match
        {
            public Player Player1 { get; set; }

            public Player Player2 { get; set; }

            public string Id { get; set; }

            public int Turn { get; set; }

            public const int PLAYING = 1;

            public const int NONE = 0;

            public const int PLAYER1_TURN = 0;

            public const int PLAYER2_TURN = 1;

            //public HashSet<Cell> Cells = new HashSet<Cell>();

            public Cell[,] CellMap = new Cell[MAP_SIZE, MAP_SIZE];
        }

        public class Player
        {
            public string Name { get; set; }

            // None, Playing
            public int Status { get; set; }
            public string ClientContextId { get; set; }
            public string Id
            {
                get { return ClientContextId; }
            }
        }

        public class Cell
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Player Player { get; set; }
        }

        // JOIN MATCH
        public void RequestMatch(string playerId)
        {
            var pl = GomokuData.I.Players.Single(p => p.Id == playerId);
            if (pl.Status == Match.PLAYING)
                throw new Exception("This player is joining another match");
            Clients[pl.ClientContextId].onMatchRequest(GomokuData.I.Players.Single(p => p.ClientContextId == Context.ConnectionId));
        }

        public void AnswerMatch(string vsPlayerId, string answer)
        {
            var p2 = GomokuData.I.Players.Single(p => p.Id == Context.ConnectionId);
            var p1 = GomokuData.I.Players.Single(p => p.Id == vsPlayerId);
            if (answer == "yes")
            {
                var m = new Match
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Player1 = p1,
                        Player2 = p2
                    };

                GomokuData.I.Matches.Add(m);
                m.Player1.Status = Match.PLAYING;
                m.Player2.Status = Match.PLAYING;
                m.Turn = Match.PLAYER1_TURN;

                Clients[m.Player1.ClientContextId].onMatchAnswered(m, "yes");
                Clients[m.Player2.ClientContextId].onMatchAnswered(m, "yes");

                Clients.notifyPlayersStatus(GomokuData.I.Players);
            }
            else
            {
                Clients[p1.ClientContextId].onMatchAnswered(null, "no");
            }
        }

        public void QuitMatch(string matchId)
        {
            // Clean up
            var ma = GomokuData.I.Matches.Single(m => m.Id == matchId);
            ma.Player1.Status = Match.NONE;
            ma.Player2.Status = Match.NONE;
            GomokuData.I.Matches.Remove(ma);
            Clients[ma.Player1.ClientContextId].onQuitMatch(ma);
            Clients[ma.Player2.ClientContextId].onQuitMatch(ma);
            Clients.notifyPlayersStatus(GomokuData.I.Players);
        }

        // FIGHTING
        public void Tick(Cell cell, string matchId)
        {
            if (cell.X < 0 || cell.Y >= MAP_SIZE)
                throw new Exception("Invalid CELL. Check programmation!");

            var match = GomokuData.I.Matches.Single(m => m.Id == matchId);
            //var chk = match.Cells.SingleOrDefault(c => c.X == cell.X && c.Y == cell.Y);
            var chk = match.CellMap[cell.X, cell.Y];

            if (chk != null)
                throw new Exception("Cell was already checked");
            if (match.Player1.ClientContextId == Context.ConnectionId)
                cell.Player = match.Player1;
            else if (match.Player2.ClientContextId == Context.ConnectionId)
                cell.Player = match.Player2;
            else
                throw new Exception("This player doesn't play this match");

            //match.Cells.Add(cell);
            match.CellMap[cell.X, cell.Y] = cell;

            var isOver = GameLogic(match, cell);
            if (isOver)
            {
                Clients[match.Player1.ClientContextId].notifyOver(cell.Player.ClientContextId);
                Clients[match.Player2.ClientContextId].notifyOver(cell.Player.ClientContextId);
                GomokuData.I.Matches.Remove(match);
            }

            Clients[match.Player1.ClientContextId].notifyTick(cell);
            Clients[match.Player2.ClientContextId].notifyTick(cell);
        }

        private bool GameLogic(Match match, Cell toCheck)
        {
            var x1 = toCheck.X - 5;
            var y1 = toCheck.Y - 5;
            var x2 = toCheck.X + 5;
            var y2 = toCheck.Y + 5;

            var counter = 0;
            for (var y = y1; y < y2; y++)
            {
                if (y < 0 || y > MAP_SIZE)
                {
                    counter = 0;
                    continue;
                }
                if (match.CellMap[toCheck.X, y] == null || match.CellMap[toCheck.X, y].Player != toCheck.Player)
                {
                    counter = 0;
                    continue;
                }

                counter++;
                if (counter == 5)
                    return true;
            }

            counter = 0;
            for (var x = x1; x < x2; x++)
            {
                if (x < 0 || x > MAP_SIZE)
                {
                    counter = 0;
                    continue;
                }

                if (match.CellMap[x, toCheck.X] == null || match.CellMap[x, toCheck.X].Player != toCheck.Player)
                {
                    counter = 0;
                    continue;
                }

                counter++;
                if (counter == 5)
                    return true;
            }

            counter = 0;
            for (var i = 0; i < 10; i++)
            {
                var x = x1 + i;
                var y = y1 + i;

                if (x < 0 || x > MAP_SIZE || y < 0 || y > MAP_SIZE)
                {
                    counter = 0;
                    continue;
                }


                if (match.CellMap[x, y] == null || match.CellMap[x, y].Player != toCheck.Player)
                {
                    counter = 0;
                    continue;
                }

                counter++;
                if (counter == 5)
                    return true;
            }


            counter = 0;
            for (var i = 0; i < 10; i++)
            {
                var x = x1 + i;
                var y = y2 - i;

                if (x < 0 || x > MAP_SIZE || y < 0 || y > MAP_SIZE)
                {
                    counter = 0;
                    continue;
                }


                if (match.CellMap[x, y] == null || match.CellMap[x, y].Player != toCheck.Player)
                {
                    counter = 0;
                    continue;
                }

                counter++;
                if (counter == 5)
                    return true;
            }



            return false;
        }


        // INFO
        public void SetPlayerName(string name)
        {
            var pl = GomokuData.I.Players.SingleOrDefault(p => p.Name == name);
            if (pl != null)
                throw new Exception("This player already existed! Please choose another name");

            pl = GomokuData.I.Players.Single(p => p.ClientContextId == Context.ConnectionId);
            pl.Name = name;
            Clients.notifyPlayersStatus(GomokuData.I.Players);
        }

        public Task Disconnect()
        {
            var pl = GomokuData.I.Players.SingleOrDefault(p => p.ClientContextId == Context.ConnectionId);
            if (pl != null)
                GomokuData.I.Players.Remove(pl);
            Clients.notifyPlayersStatus(GomokuData.I.Players);

            return null;
        }

        public Task Connect()
        {
            Clients[Context.ConnectionId].onReceiveId(Context.ConnectionId);
            var pl = GomokuData.I.Players.SingleOrDefault(p => p.ClientContextId == Context.ConnectionId);
            if (pl == null)
            {
                GomokuData.I.Players.Add(new Player { ClientContextId = Context.ConnectionId });
                Clients.notifyPlayersStatus(GomokuData.I.Players);
            }
            return null;
        }

        public Task Reconnect(IEnumerable<string> groups)
        {
            //Clients.onReceiveId(Context.ConnectionId);
            //var pl = GomokuData.I.Players.SingleOrDefault(p => p.ClientContextId == Context.ConnectionId);
            //if (pl == null)
            //{
            //    GomokuData.I.Players.Add(new Player { ClientContextId = Context.ConnectionId });
            //    Clients.notifyPlayersStatus(GomokuData.I.Players);
            //}

            return null;
        }
    }
}