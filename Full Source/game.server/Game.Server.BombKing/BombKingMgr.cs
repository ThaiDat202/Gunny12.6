using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.BombKing;

public class BombKingMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static int _battleStage;

	private static int _turn;

	private static int _status;

	private static bool IsOpen = false;

	private static bool _IsBombKingOpen;

	private static int _Week;

	private static List<BombKingPlayerInfo> PlayerInfo_;

	private static List<BombKingRewardInfo> BombKingReward_;

	private static DateTime _BombKingBeginTime;

	private static DateTime _BombKingEndTime;

	public static int battleStage
	{
		get
		{
			return _battleStage;
		}
		set
		{
			_battleStage = value;
		}
	}

	public static int turn
	{
		get
		{
			return _turn;
		}
		set
		{
			_turn = value;
		}
	}

	public static int status
	{
		get
		{
			return _status;
		}
		set
		{
			_status = value;
		}
	}

	public static bool IsBombKingOpen
	{
		get
		{
			return _IsBombKingOpen;
		}
		set
		{
			_IsBombKingOpen = value;
		}
	}

	public static int Week
	{
		get
		{
			return _Week;
		}
		set
		{
			_Week = value;
		}
	}

	public static DateTime BombKingBeginTime
	{
		get
		{
			return _BombKingBeginTime;
		}
		set
		{
			_BombKingBeginTime = value;
		}
	}

	public static DateTime BombKingEndTime
	{
		get
		{
			return _BombKingEndTime;
		}
		set
		{
			_BombKingEndTime = value;
		}
	}

	public static bool LoadPlayer()
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			PlayerInfo_ = produceBussiness.GetBombKingPlayer().ToList();
			BombKingReward_ = produceBussiness.GetBombKingReward().ToList();
		}
		return true;
	}

	public static bool Init()
	{
		bool result = false;
		try
		{
			PlayerInfo_ = new List<BombKingPlayerInfo>();
			BombKingReward_ = new List<BombKingRewardInfo>();
			battleStage = 0;
			status = 1;
			turn = 0;
			IsOpen = false;
			IsBombKingOpen = false;
			BombKingBeginTime = new DateTime(2016, 1, 1);
			BombKingBeginTime = new DateTime(2016, 2, 1);
			LoadPlayer();
			result = true;
		}
		catch (Exception exception)
		{
			log.Error("BombKingMgr Init", exception);
		}
		return result;
	}

	public static void sendBombKingOpenClose(bool value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(263);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteBoolean(value);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			gamePlayer.Out.SendTCP(gSPacketIn);
		}
	}

	public static void SendUpdateBombKingFrame()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(263);
		gSPacketIn.WriteByte(7);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			gamePlayer.Out.SendTCP(gSPacketIn);
		}
	}

	public static void UpdatePlayer(BombKingPlayerInfo info)
	{
		BombKingPlayerInfo bombKingPlayerInfo = PlayerInfo_.SingleOrDefault((BombKingPlayerInfo s) => s.UserID == info.UserID && s.AreaID == info.AreaID);
		if (bombKingPlayerInfo != null)
		{
			bombKingPlayerInfo.Place = info.Place;
			bombKingPlayerInfo.myRank = info.myRank;
			bombKingPlayerInfo.myScore = info.myScore;
			bombKingPlayerInfo.Score = info.Score;
			bombKingPlayerInfo.RankType = info.RankType;
			bombKingPlayerInfo.status = info.status;
			bombKingPlayerInfo.style = info.style;
			bombKingPlayerInfo.color = info.color;
			bombKingPlayerInfo.sex = info.sex;
			bombKingPlayerInfo.smg = info.smg;
		}
		else
		{
			PlayerInfo_.Add(info);
		}
	}

	public static void ClearPlayer()
	{
		PlayerInfo_.Clear();
	}

	public static void AddScore(GamePlayer Player, int score, bool win)
	{
		BombKingPlayerInfo bombKingPlayerInfo = new BombKingPlayerInfo();
		bombKingPlayerInfo.UserID = Player.PlayerCharacter.ID;
		bombKingPlayerInfo.NickName = Player.PlayerCharacter.NickName;
		bombKingPlayerInfo.AreaID = Player.ZoneId;
		bombKingPlayerInfo.AreaName = Player.ZoneName;
		bombKingPlayerInfo.vipType = Player.PlayerCharacter.typeVIP;
		bombKingPlayerInfo.vipLevel = Player.PlayerCharacter.VIPLevel;
		bombKingPlayerInfo.style = Player.PlayerCharacter.Style;
		bombKingPlayerInfo.color = Player.PlayerCharacter.Colors;
		bombKingPlayerInfo.sex = Player.PlayerCharacter.Sex;
		GameServer.Instance.LoginServer.SendBombKingUpdatePlayer(bombKingPlayerInfo, score, win);
	}

	public static void GetBombKingPlayerInfo(int UserId, ref int rank, ref int Score, int type)
	{
		rank = 0;
		Score = 0;
		if (type == 0)
		{
			Score = ((PlayerInfo_.SingleOrDefault((BombKingPlayerInfo s) => s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId) != null) ? PlayerInfo_.SingleOrDefault((BombKingPlayerInfo s) => s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId).myScore : 0);
			if (Score > 0)
			{
				List<BombKingPlayerInfo> list = PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.myScore).ToList();
				rank = list.FindIndex((BombKingPlayerInfo s) => s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId) + 1;
			}
			return;
		}
		Score = ((PlayerInfo_.SingleOrDefault((BombKingPlayerInfo s) => s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId) != null) ? PlayerInfo_.SingleOrDefault((BombKingPlayerInfo s) => s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId).Score : 0);
		if (Score > 0)
		{
			List<BombKingPlayerInfo> list2 = PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.Score).ToList();
			rank = list2.FindIndex((BombKingPlayerInfo s) => s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId) + 1;
		}
	}

	public static BombKingPlayerInfo GetBombKingPlayerInfo(int UserId)
	{
		return PlayerInfo_.SingleOrDefault((BombKingPlayerInfo s) => s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId);
	}

	public static List<BombKingPlayerInfo> GetListTop100BombKingPlayerInfo()
	{
		IEnumerable<BombKingPlayerInfo> source = PlayerInfo_.OrderByDescending((BombKingPlayerInfo s) => s.myScore).Take(100);
		return source.ToList();
	}

	public static List<BombKingPlayerInfo> GetListTop16BombKingPlayerInfo()
	{
		IEnumerable<BombKingPlayerInfo> source = (from s in PlayerInfo_
			where s.Score > 0
			orderby s.Score descending
			select s).Take(16);
		return source.ToList();
	}

	public static BombKingPlayerInfo GetTopBombKing(int Rank)
	{
		if (Week == 0 || (Week == 3 && DateTime.Now.DayOfWeek != 0))
		{
			int RankType = 4;
			switch (Rank)
			{
			case 0:
				RankType = 6;
				break;
			case 1:
				RankType = 5;
				break;
			}
			return (from s in PlayerInfo_
				where s.RankType == RankType
				orderby s.Place descending
				select s).Take(1).SingleOrDefault();
		}
		return null;
	}

	public static bool Checkbattle(int UserId)
	{
		if (Week == 2)
		{
			return (from s in GetListTop100BombKingPlayerInfo()
				where s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId
				select s).SingleOrDefault() != null;
		}
		if (Week == 3)
		{
			return (from s in GetListTop16BombKingPlayerInfo()
				where s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId
				select s).SingleOrDefault() != null && (from s in GetListTop16BombKingPlayerInfo()
				where s.UserID == UserId && s.AreaID == GameServer.Instance.Configuration.AreaId
				select s).SingleOrDefault().RankType == battleStage - 1;
		}
		return true;
	}

	public static int GetBattlaId(int UserId, int RankType)
	{
		if (RankType < 2 || Week != 3)
		{
			return 0;
		}
		List<BombKingPlayerInfo> list = (from s in PlayerInfo_
			where s.RankType == RankType
			orderby s.Score descending
			select s).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].UserID != UserId || list[i].AreaID != GameServer.Instance.Configuration.ServerID)
			{
				continue;
			}
			if (i % 2 == 0)
			{
				if (i + 1 < list.Count)
				{
					return list[i + 1].UserID;
				}
				return -1;
			}
			return list[i - 1].UserID;
		}
		return -1;
	}

	public static void AddNewRank(int ID, int valid, int UserId)
	{
		if (WorldMgr.GetPlayerById(UserId) != null)
		{
			WorldMgr.GetPlayerById(UserId).Rank.AddNewRank(ID, valid);
		}
		else
		{
			log.Error("///BOMB_KING ERROR FIND USERID " + UserId + " TO ADD NEWTITLE " + ID);
		}
	}
}
