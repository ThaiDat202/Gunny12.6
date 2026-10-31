using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Fighting.Server.GameObjects;

public class ProxyPlayer : IGamePlayer
{
	private ServerClient m_client;

	private PlayerInfo m_character;

	private UserMatchInfo m_matchInfo;

	private ItemInfo m_currentWeapon;

	private ItemInfo m_secondWeapon;

	private ItemInfo m_healstone;

	private UsersPetInfo m_pet;

	private bool m_canUseProp;

	private int m_gamePlayerId;

	private double GPRate;

	private double OfferRate;

	public double m_antiAddictionRate;

	public List<BufferInfo> Buffers;

	public int m_serverid;

	public string RedFightFootballStyle;

	public string BlueFightFootballStyle;

	private int[] m_horseSkillEquip;

	private int[] m_petSealSkillEquip;

	private string m_tcpEndPoint;

	private bool m_dragonBoatOpen;

	private int m_dragonBoatAddExpPlus;

	private bool m_isCrossZone;

	private List<int> m_questColect;

	private int m_zoneId;

	private string m_zoneName;

	private List<BufferInfo> m_fightBuffs;

	private ItemInfo m_weapon3D;

	private ItemInfo m_amulet;

	private BatleConfigInfo m_batleConfig;

	private double m_baseAglilty;

	private double m_baseAttack;

	private double m_baseDefence;

	private double m_baseBlood;

	private List<ItemInfo> m_equipEffect;

	private List<ItemInfo> m_horseAmuletEquipEffect;

	public int ServerID
	{
		get
		{
			return m_serverid;
		}
		set
		{
			m_serverid = value;
		}
	}

	public int[] HorseSkillEquip => m_horseSkillEquip;

	public int[] PetSealSkillEquip => m_petSealSkillEquip;

	public int GamePlayerId
	{
		get
		{
			return m_gamePlayerId;
		}
		set
		{
			m_gamePlayerId = value;
			m_client.SendGamePlayerId(this);
		}
	}

	public UserPvePowerBuffInfo PvePowerBuff { get; set; }

	public int BombKingDelay { get; set; }

	public int battleId { get; set; }

	public List<int> QuestColect
	{
		get
		{
			return m_questColect;
		}
		set
		{
			m_questColect = value;
		}
	}

	public bool IsCrossZone
	{
		get
		{
			return m_isCrossZone;
		}
		set
		{
			m_isCrossZone = value;
		}
	}

	public int ZoneId => m_zoneId;

	public string ZoneName => m_zoneName;

	public PlayerInfo PlayerCharacter => m_character;

	public List<BufferInfo> FightBuffs => m_fightBuffs;

	public UserMatchInfo MatchInfo => m_matchInfo;

	public ItemInfo MainWeapon => m_currentWeapon;

	public ItemInfo SecondWeapon => m_secondWeapon;

	public ItemInfo Weapon3D => m_weapon3D;

	public ItemInfo Healstone => m_healstone;

	public ItemInfo Amulet => m_amulet;

	public UsersPetInfo Pet => m_pet;

	public BatleConfigInfo BatleConfig => m_batleConfig;

	public long WorldbossBood => 0L;

	public long AllWorldDameBoss => 0L;

	public double BaseAttack => m_baseAttack;

	public double BaseDefence => m_baseDefence;

	public string ProcessLabyrinthAward { get; set; }

	public bool CanUseProp
	{
		get
		{
			return m_canUseProp;
		}
		set
		{
			m_canUseProp = value;
		}
	}

	public List<ItemInfo> EquipEffect => m_equipEffect;

	public List<ItemInfo> HorseAmuletEquipEffect => m_horseAmuletEquipEffect;

	public ProxyPlayer(ServerClient client, PlayerInfo character, UserMatchInfo matchInfo, ProxyPlayerInfo proxyPlayer, UsersPetInfo pet, List<BufferInfo> infos, List<ItemInfo> euipEffects, List<ItemInfo> horseAmuletEffects, List<BufferInfo> fightBuffs, List<int> horseSkill, BatleConfigInfo batleConfig, List<int> petSealSkillEquip)
	{
		m_client = client;
		m_character = character;
		m_matchInfo = matchInfo;
		m_pet = pet;
		m_serverid = proxyPlayer.ServerId;
		m_baseAttack = proxyPlayer.BaseAttack;
		m_baseDefence = proxyPlayer.BaseDefence;
		m_baseAglilty = proxyPlayer.BaseAgility;
		m_baseBlood = proxyPlayer.BaseBlood;
		m_currentWeapon = proxyPlayer.GetItemTemplateInfo();
		m_secondWeapon = proxyPlayer.GetSecondWeaponItemInfo();
		m_weapon3D = proxyPlayer.GetItem3DInfo();
		m_healstone = proxyPlayer.GetHealstone();
		m_amulet = proxyPlayer.Amulet;
		GPRate = proxyPlayer.GPAddPlus;
		m_zoneId = proxyPlayer.ZoneId;
		m_zoneName = proxyPlayer.ZoneName;
		OfferRate = proxyPlayer.OfferAddPlus;
		m_antiAddictionRate = proxyPlayer.AntiAddictionRate;
		m_equipEffect = euipEffects;
		m_horseAmuletEquipEffect = horseAmuletEffects;
		Buffers = infos;
		m_fightBuffs = fightBuffs;
		RedFightFootballStyle = proxyPlayer.RedFootballStyle;
		BlueFightFootballStyle = proxyPlayer.BlueFootballStyle;
		m_horseSkillEquip = horseSkill.ToArray();
		m_tcpEndPoint = proxyPlayer.TcpEndPoint;
		m_dragonBoatAddExpPlus = proxyPlayer.DragonBoatAddExpPlus;
		m_dragonBoatOpen = proxyPlayer.DragonBoatOpen;
		m_isCrossZone = false;
		m_batleConfig = batleConfig;
		m_questColect = new List<int>();
		m_petSealSkillEquip = petSealSkillEquip.ToArray();
	}

	public int AddSummerScore(int value)
	{
		return value;
	}

	public double GetBaseAgility()
	{
		return m_baseAglilty;
	}

	public bool UseKingBlessHelpStraw(eRoomType roomType)
	{
		return false;
	}

	public bool RemoveMissionEnergy(int value)
	{
		return false;
	}

	public bool MissionEnergyEmpty(int value)
	{
		return false;
	}

	public void ClearFightBuffOneMatch()
	{
	}

	public void UpdateDungeonCount(int mapId)
	{
	}

	public int AddScoreCampBattle(int value)
	{
		return 0;
	}

	public int AddScoreCityBattle(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddScoreCityBattle(m_character.ID, value);
		}
		return value;
	}

	public void UpdateCampBattleResult(eGameType gameType, int bossId, int totalPlayersTeam, bool isWin)
	{
	}

	public string GetFightFootballStyle(int team)
	{
		if (team == 1)
		{
			return RedFightFootballStyle;
		}
		return BlueFightFootballStyle;
	}

	public void UpdatePveResult(string type, int value, bool isWin)
	{
	}

	public double GetBaseBlood()
	{
		return m_baseBlood;
	}

	public void UpdateBarrier(int barrier, string pic)
	{
	}

	public int DragonBoatAddExpPlus()
	{
		return m_dragonBoatAddExpPlus;
	}

	public bool DragonBoatOpen()
	{
		return m_dragonBoatOpen;
	}

	public string TcpEndPoint()
	{
		return m_tcpEndPoint;
	}

	public void UpdateUserMoney(int value, int type)
	{
		m_client.SendPlayerUpdateUserMoney(m_character.ID, value, type);
	}

	public int AddEnterModePoint(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddEnterModePoint(m_character.ID, value);
		}
		return value;
	}

	public void CreateRandomEnterModeItem()
	{
		m_client.SendCreateEnterModeItem(PlayerCharacter.ID);
	}

	public void FootballTakeOut(bool isWin)
	{
		m_client.SendFootballTakeOut(PlayerCharacter.ID, isWin);
	}

	public int AddBoatScore(int score)
	{
		if (score > 0)
		{
			m_client.SendPlayerBoatScore(PlayerCharacter.ID, score);
		}
		return score;
	}

	public int AddCupomPVP(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddMoneyPVP(m_character.ID, value);
		}
		return value;
	}

	public int AddGP(int gp)
	{
		if (gp > 0)
		{
			m_client.SendPlayerAddGP(PlayerCharacter.ID, gp);
		}
		return (int)(GPRate * (double)gp);
	}

	public int RemoveGP(int gp)
	{
		m_client.SendPlayerRemoveGP(PlayerCharacter.ID, gp);
		return gp;
	}

	public int AddGold(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddGold(PlayerCharacter.ID, value);
		}
		return value;
	}

	public int AddHonor(int value)
	{
		return value;
	}

	public int AddDamageScores(int value)
	{
		return value;
	}

	public int RemoveGold(int value)
	{
		m_client.SendPlayerRemoveGold(m_character.ID, value);
		return 0;
	}

	public bool ClearPropItem()
	{
		return true;
	}

	public void ResetRoom(bool isWin, string parram)
	{
	}

	public int AddScoreBombKing(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddScoreBombKing(m_character.ID, value);
		}
		return value;
	}

	public int AddMoney(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddMoney(m_character.ID, value);
		}
		return value;
	}

	public int AddActiveMoney(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddActiveMoney(m_character.ID, value);
		}
		return value;
	}

	public int RemoveMoney(int value)
	{
		m_client.SendPlayerRemoveMoney(m_character.ID, value);
		return 0;
	}

	public int AddGiftToken(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddGiftToken(m_character.ID, value);
		}
		return value;
	}

	public bool RemoveHealstone()
	{
		m_client.SendPlayerRemoveHealstone(m_character.ID);
		return false;
	}

	public int AddMedal(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddMedal(m_character.ID, value);
		}
		return value;
	}

	public int AddLeagueMoney(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddLeagueMoney(m_character.ID, value);
		}
		return value;
	}

	public void AddPrestige(bool isWin, eRoomType roomType)
	{
		m_client.SendPlayerAddPrestige(m_character.ID, isWin, roomType);
	}

	public void RingstationResult(bool isWin)
	{
		m_client.SendRingstationResult(m_character.ID, isWin);
	}

	public void UpdateRestCount()
	{
		m_client.SendUpdateRestCount(m_character.ID);
	}

	public int RemoveGiftToken(int value)
	{
		return 0;
	}

	public int RemoveMedal(int value)
	{
		return 0;
	}

	public int AddHardCurrency(int value)
	{
		return 0;
	}

	public bool isDoubleAward()
	{
		return false;
	}

	public void UpdateLabyrinth(int currentFloor, int m_missionInfoId, bool bigAward)
	{
	}

	public void OutLabyrinth()
	{
	}

	public int AddOffer(int baseoffer)
	{
		if (baseoffer < 0)
		{
			return baseoffer;
		}
		return (int)((double)baseoffer * OfferRate * m_antiAddictionRate);
	}

	public int RemoveOffer(int value)
	{
		m_client.SendPlayerRemoveOffer(m_character.ID, value);
		return value;
	}

	public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
	{
	}

	public void UpdatePublicPlayer(string tempStyle = "")
	{
		m_client.SendUpdatePublicPlayer(m_character.ID, tempStyle);
	}

	public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
	{
		m_client.SendPlayerUsePropInGame(PlayerCharacter.ID, bag, place, templateId, isLiving);
		game.Pause(500);
		return false;
	}

	public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
	{
		m_client.SendPlayerOnGameOver(PlayerCharacter.ID, game.Id, isWin, gainXp);
	}

	public void Disconnect()
	{
		m_client.SendDisconnectPlayer(m_character.ID);
	}

	public void SendTCP(GSPacketIn pkg)
	{
		m_client.SendPacketToPlayer(m_character.ID, pkg);
	}

	public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
	{
		m_client.SendPlayerOnKillingLiving(m_character.ID, game, type, id, isLiving, demage);
	}

	public void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int turnNum)
	{
		m_client.SendPlayerOnMissionOver(m_character.ID, game, isWin, MissionID, turnNum);
	}

	public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
	{
		m_client.SendPlayerConsortiaFight(m_character.ID, consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth);
		return 0;
	}

	public void SendConsortiaFight(int consortiaID, int riches, string msg)
	{
		m_client.SendPlayerSendConsortiaFight(m_character.ID, consortiaID, riches, msg);
	}

	public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eGameView typeGet)
	{
		m_client.SendPlayerAddTemplate(m_character.ID, cloneItem, bagType, count);
		return true;
	}

	public void OutLabyrinth(bool isWin)
	{
	}

	public void SendMessage(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendHideMessage(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public bool IsPvePermission(int missionId, eHardLevel hardLevel)
	{
		return true;
	}

	public bool SetPvePermission(int missionId, eHardLevel hardLevel)
	{
		return true;
	}

	public void SendInsufficientMoney(int type)
	{
	}

	public bool ClearTempBag()
	{
		return true;
	}

	public bool ClearFightBag()
	{
		return true;
	}

	public void UpdateConsortiaBattle(int leftBlood, bool isWin, int tieStatus)
	{
	}
}
