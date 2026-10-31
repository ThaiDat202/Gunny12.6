using System;
using System.Collections.Generic;
using System.Linq;

namespace Center.Server
{
	public class ZoneLoginMgr
	{
		private Dictionary<int, Player> m_players = new Dictionary<int, Player>();

		private object syc_obj = new object();

		public void CreatePlayer(Player player)
		{
			Player player2 = null;
			lock (syc_obj)
			{
				player.LastTime = DateTime.Now.Ticks;
				if (m_players.ContainsKey(player.Id))
				{
					player2 = m_players[player.Id];
					player.State = player2.State;
					player.CurrentServer = player2.CurrentServer;
					m_players[player.Id] = player;
				}
				else
				{
					player2 = GetPlayerByName(player.Name);
					if (player2 != null && m_players.ContainsKey(player2.Id))
					{
						m_players.Remove(player2.Id);
					}
					player.State = ePlayerState.NotLogin;
					m_players.Add(player.Id, player);
				}
			}
			if (player2 != null && player2.CurrentServer != null)
			{
				player2.CurrentServer.SendKitoffUser(player2.Id);
			}
		}

		public bool TryLoginPlayer(int id, ServerClient server)
		{
			lock (syc_obj)
			{
				if (!m_players.ContainsKey(id))
				{
					return false;
				}
				Player player = m_players[id];
				if (player.CurrentServer == null)
				{
					player.CurrentServer = server;
					player.State = ePlayerState.Logining;
					return true;
				}
				if (player.State == ePlayerState.Play)
				{
					player.CurrentServer.SendKitoffUser(id);
				}
				return false;
			}
		}

		public Player PlayerLogined(int id, ServerClient server)
		{
			lock (syc_obj)
			{
				if (!m_players.ContainsKey(id))
				{
					return null;
				}
				Player player = m_players[id];
				if (player == null)
				{
					return null;
				}
				player.CurrentServer = server;
				player.State = ePlayerState.Play;
				return player;
			}
		}

		public void PlayerLoginOut(int id, ServerClient server)
		{
			lock (syc_obj)
			{
				if (m_players.ContainsKey(id))
				{
					Player player = m_players[id];
					if (player != null && player.CurrentServer == server)
					{
						player.CurrentServer = null;
						player.State = ePlayerState.NotLogin;
					}
				}
			}
		}

		public Player GetPlayerByName(string name)
		{
			Player[] allPlayer = GetAllPlayer();
			if (allPlayer != null)
			{
				Player[] array = allPlayer;
				foreach (Player player in array)
				{
					if (player.Name == name)
					{
						return player;
					}
				}
			}
			return null;
		}

		public Player[] GetAllPlayer()
		{
			lock (syc_obj)
			{
				return m_players.Values.ToArray();
			}
		}

		public void RemovePlayer(int playerId)
		{
			lock (syc_obj)
			{
				if (m_players.ContainsKey(playerId))
				{
					m_players.Remove(playerId);
				}
			}
		}

		public void RemovePlayer(List<Player> players)
		{
			lock (syc_obj)
			{
				foreach (Player player in players)
				{
					m_players.Remove(player.Id);
				}
			}
		}

		public Player GetPlayer(int playerId)
		{
			lock (syc_obj)
			{
				if (m_players.ContainsKey(playerId))
				{
					return m_players[playerId];
				}
			}
			return null;
		}

		public ServerClient GetServerClient(int playerId)
		{
			return GetPlayer(playerId)?.CurrentServer;
		}

		public int GetOnlineCount()
		{
			Player[] allPlayer = GetAllPlayer();
			int num = 0;
			Player[] array = allPlayer;
			foreach (Player player in array)
			{
				if (player.State != 0)
				{
					num++;
				}
			}
			return num;
		}

		public Dictionary<int, int> GetOnlineForLine()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Player[] allPlayer = GetAllPlayer();
			foreach (Player player in allPlayer)
			{
				if (player.CurrentServer != null)
				{
					if (dictionary.ContainsKey(player.CurrentServer.Info.ID))
					{
						dictionary[player.CurrentServer.Info.ID]++;
					}
					else
					{
						dictionary.Add(player.CurrentServer.Info.ID, 1);
					}
				}
			}
			return dictionary;
		}

		public List<Player> GetServerPlayers(ServerClient server)
		{
			List<Player> list = new List<Player>();
			Player[] allPlayer = GetAllPlayer();
			foreach (Player player in allPlayer)
			{
				if (player.CurrentServer == server)
				{
					list.Add(player);
				}
			}
			return list;
		}
	}
}
