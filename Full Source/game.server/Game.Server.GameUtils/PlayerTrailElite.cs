using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerTrailElite
{
	protected GamePlayer m_player;

	protected object m_lock = new object();

	private bool m_saveToDb;

	private UserTrailEliteInfo m_data;

	public GamePlayer Player => m_player;

	public UserTrailEliteInfo Data
	{
		get
		{
			return m_data;
		}
		set
		{
			m_data = value;
		}
	}

	public PlayerTrailElite(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
	}

	private void SetupData()
	{
		lock (m_lock)
		{
			m_data = new UserTrailEliteInfo();
			m_data.UserID = Player.PlayerCharacter.ID;
		}
	}

	public void LoadFromDatabase()
	{
		if (m_data != null)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		try
		{
			m_data = playerBussiness.GetSigleUserTrailElite(Player.PlayerCharacter.ID);
			if (m_data == null)
			{
				SetupData();
				ResetTrailElite();
			}
		}
		finally
		{
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (m_data != null && m_data.IsDirty)
			{
				if (m_data.ID > 0)
				{
					playerBussiness.UpdateUserTrailElite(m_data);
				}
				else
				{
					playerBussiness.AddUserTrailElite(m_data);
				}
			}
		}
	}

	public void SendTrailEliteInfo()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84);
		gSPacketIn.WriteInt(14);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteInt(Data.trailEliteLevel);
		gSPacketIn.WriteInt(Data.trailEliteScore);
		gSPacketIn.WriteInt(Data.trailEliteTotal);
		gSPacketIn.WriteInt(Data.trailEliteWin);
		gSPacketIn.WriteInt(Data.trailEliteRankTotal);
		gSPacketIn.WriteInt(Data.trailEliteRankWin);
		gSPacketIn.WriteInt(Data.trailEliteIsUpRank ? 1 : 0);
		gSPacketIn.WriteInt((int)(WorldMgr.EndTrailEliteDate - DateTime.Now.Date).TotalDays);
		m_player.SendTCP(gSPacketIn);
	}

	public void ResetTrailElite()
	{
		lock (m_lock)
		{
			m_data.trailEliteLevel = 0;
			m_data.trailEliteScore = 0;
			m_data.trailEliteTotal = 0;
			m_data.trailEliteWin = 0;
			m_data.trailEliteRankTotal = 0;
			m_data.trailEliteRankWin = 0;
			m_data.trailEliteIsUpRank = false;
		}
	}

	public void UpdateTrailElite(bool isWin)
	{
		lock (m_lock)
		{
			m_data.trailEliteTotal++;
			if (m_data.trailEliteIsUpRank)
			{
				bool flag = false;
				m_data.trailEliteRankTotal++;
				if (isWin)
				{
					m_data.trailEliteRankWin++;
				}
				if (m_data.trailEliteRankWin >= 3)
				{
					m_data.trailEliteLevel++;
					flag = true;
					List<ItemInfo> list = new List<ItemInfo>();
					string[] array = WorldMgr.TrailEliteAward[m_data.trailEliteLevel - 1].Split('|');
					foreach (string s in array)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(int.Parse(s)), 1, 105);
						itemInfo.ValidDate = 30;
						itemInfo.IsBinds = true;
						list.Add(itemInfo);
					}
					Player.SendItemsToMail(list, LanguageMgr.GetTranslation("GameServer.TrailElite.Mail.Content"), LanguageMgr.GetTranslation("GameServer.TrailElite.Mail.Title"), eMailType.BuyItem);
					Player.SendMessage(LanguageMgr.GetTranslation("GameServer.TrailElite.PassLevel.Msg"));
				}
				else if (m_data.trailEliteRankTotal >= 5)
				{
					flag = true;
					Player.SendMessage(LanguageMgr.GetTranslation("GameServer.TrailElite.PassLevel.Fail"));
				}
				else
				{
					Player.SendMessage(LanguageMgr.GetTranslation("GameServer.TrailElite.PassLevel.Success", m_data.trailEliteRankWin, m_data.trailEliteRankTotal));
				}
				if (flag)
				{
					m_data.trailEliteIsUpRank = false;
					m_data.trailEliteRankTotal = 0;
					m_data.trailEliteRankWin = 0;
					m_data.trailEliteScore = 0;
				}
			}
			else
			{
				string text = "";
				if (isWin)
				{
					m_data.trailEliteWin++;
					m_data.trailEliteScore += GameProperties.TrailEliteWinScore;
					if (m_data.trailEliteScore > 500)
					{
						m_data.trailEliteScore = 500;
					}
					text = LanguageMgr.GetTranslation("GameServer.TrailElite.Score.Win", GameProperties.TrailEliteWinScore);
				}
				else
				{
					m_data.trailEliteScore -= GameProperties.TrailEliteLostScore;
					if (m_data.trailEliteScore < 0)
					{
						m_data.trailEliteScore = 0;
					}
					text = LanguageMgr.GetTranslation("GameServer.TrailElite.Score.Lost", GameProperties.TrailEliteLostScore);
				}
				if (m_data.trailEliteScore >= 500)
				{
					m_data.trailEliteIsUpRank = true;
					text = LanguageMgr.GetTranslation("GameServer.TrailElite.JoinPassLevel.Msg");
				}
				Player.SendMessage(text);
			}
		}
		SendTrailEliteInfo();
		Player.PlayerCharacter.trailEliteLevel = Data.trailEliteLevel;
	}
}
