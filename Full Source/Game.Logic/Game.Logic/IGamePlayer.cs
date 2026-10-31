using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic;

public interface IGamePlayer
{
	int ZoneId { get; }

	string ZoneName { get; }

	int BombKingDelay { get; set; }

	int battleId { get; set; }

	PlayerInfo PlayerCharacter { get; }

	UserMatchInfo MatchInfo { get; }

	BatleConfigInfo BatleConfig { get; }

	UserPvePowerBuffInfo PvePowerBuff { get; set; }

	ItemInfo MainWeapon { get; }

	ItemInfo SecondWeapon { get; }

	ItemInfo Weapon3D { get; }

	ItemInfo Healstone { get; }

	ItemInfo Amulet { get; }

	string ProcessLabyrinthAward { get; set; }

	UsersPetInfo Pet { get; }

	int[] HorseSkillEquip { get; }

	int[] PetSealSkillEquip { get; }

	bool CanUseProp { get; set; }

	bool IsCrossZone { get; set; }

	int GamePlayerId { get; set; }

	long WorldbossBood { get; }

	long AllWorldDameBoss { get; }

	int ServerID { get; set; }

	List<ItemInfo> EquipEffect { get; }

	List<ItemInfo> HorseAmuletEquipEffect { get; }

	List<BufferInfo> FightBuffs { get; }

	List<int> QuestColect { get; set; }

	double BaseAttack { get; }

	double BaseDefence { get; }

	int AddCupomPVP(int value);

	int AddSummerScore(int value);

	bool RemoveHealstone();

	void UpdateUserMoney(int value, int type);

	int DragonBoatAddExpPlus();

	void UpdateDungeonCount(int mapId);

	bool DragonBoatOpen();

	string TcpEndPoint();

	bool isDoubleAward();

	void ClearFightBuffOneMatch();

	bool ClearPropItem();

	double GetBaseBlood();

	void UpdateBarrier(int barrier, string pic);

	void UpdateConsortiaBattle(int leftBlood, bool isWin, int tieStatus);

	void FootballTakeOut(bool isWin);

	int AddGP(int gp);

	int RemoveGP(int gp);

	bool UseKingBlessHelpStraw(eRoomType roomType);

	bool RemoveMissionEnergy(int value);

	int AddGold(int value);

	int RemoveGold(int value);

	bool MissionEnergyEmpty(int value);

	int AddMoney(int value);

	int AddActiveMoney(int value);

	int RemoveMoney(int value);

	string GetFightFootballStyle(int team);

	int AddGiftToken(int value);

	int RemoveGiftToken(int value);

	int AddHardCurrency(int value);

	int AddScoreCityBattle(int value);

	int AddMedal(int value);

	int RemoveMedal(int value);

	int AddHonor(int value);

	int AddBoatScore(int value);

	int AddDamageScores(int value);

	int AddLeagueMoney(int value);

	int AddScoreBombKing(int value);

	int AddScoreCampBattle(int value);

	void AddPrestige(bool isWin, eRoomType roomType);

	void RingstationResult(bool isWin);

	int AddOffer(int value);

	int AddEnterModePoint(int value);

	int RemoveOffer(int value);

	bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView);

	void UpdatePveResult(string type, int value, bool isWin);

	void UpdateCampBattleResult(eGameType gameType, int bossId, int totalPlayersTeam, bool isWin);

	bool ClearTempBag();

	bool ClearFightBag();

	void ResetRoom(bool isWin, string parram);

	bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving);

	void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage);

	void OnGameOver(AbstractGame game, bool isWin, int gainXp);

	void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int TurnNum);

	int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count);

	void SendConsortiaFight(int consortiaID, int riches, string msg);

	void CreateRandomEnterModeItem();

	bool SetPvePermission(int missionId, eHardLevel hardLevel);

	bool IsPvePermission(int missionId, eHardLevel hardLevel);

	void Disconnect();

	void UpdateRestCount();

	void SendInsufficientMoney(int type);

	void UpdateLabyrinth(int currentFloor, int mMissionInfoId, bool bigAward);

	void OutLabyrinth(bool isWin);

	void SendMessage(string msg);

	void SendHideMessage(string msg);

	void SendTCP(GSPacketIn pkg);

	void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int spareMoney);

	void UpdatePublicPlayer(string tempStyle = "");
}
