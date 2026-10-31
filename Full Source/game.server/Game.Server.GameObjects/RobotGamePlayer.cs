using System;
using System.Collections.Generic;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.Achievements;
using Game.Server.Buffer;
using Game.Server.GameUtils;
using Game.Server.LotteryTicket;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;

namespace Game.Server.GameObjects;

public class RobotGamePlayer : GamePlayer
{
	public class NullOut : IPacketLib
	{
		public void SendTotemUpgradeUpdate(int templateId, int level)
		{
			Nothing();
		}

		public void SendPolarRegion(bool isOpen)
		{
			Nothing();
		}

		public void SendTotemUpgradeInfo(Dictionary<int, int> infos)
		{
			Nothing();
		}

		public GSPacketIn SendUpdateSoulCrystal(UsersExtraInfo extra)
		{
			return null;
		}

		public void SendPvePowerGetBuff(UserPvePowerBuffInfo pveBuffInfo)
		{
			Nothing();
		}

		public void SendPvePowerBuffRefresh(UserPvePowerBuffInfo pveBuffInfo)
		{
			Nothing();
		}

		public void SendPvePowerBuffResponse(UserPvePowerBuffInfo pveBuffInfo)
		{
			Nothing();
		}

		public GSPacketIn SendUpdateRoomBorden(List<ItemInfo> list, bool isAdd, int currentItemId)
		{
			return null;
		}

		public GSPacketIn SendMinesShopInfo()
		{
			return null;
		}

		public void SendMinesLevelUpTool(UserMinesInfo info)
		{
			Nothing();
		}

		public void SendMinesLevelUpArm(UserMinesInfo info)
		{
			Nothing();
		}

		public void SendOpenMines(UserMinesInfo info)
		{
			Nothing();
		}

		public void SendOpenNoviceActive(int channel, int activeId, int condition, int awardGot, DateTime startTime, DateTime endTime)
		{
			Nothing();
		}

		public void SendOpenOrCloseStock(bool isOpen)
		{
			Nothing();
		}

		public GSPacketIn SendLoginDeviceCheck(GamePlayer p, bool isLogin)
		{
			return null;
		}

		public void SendOpenOrCloseChristmas(int lastPacks, bool isOpen)
		{
			Nothing();
		}

		public GSPacketIn SendUpdateOneKeyFinish(PlayerInfo player)
		{
			return null;
		}

		public GSPacketIn SendNecklaceStrength(PlayerInfo player)
		{
			return null;
		}

		public GSPacketIn SendMissionEnergy(UsersExtraInfo extra)
		{
			return null;
		}

		public void SendPlayerCardEquip(PlayerInfo player, List<UsersCardInfo> cards)
		{
			Nothing();
		}

		public GSPacketIn SendUpdateGoodsCount(PlayerInfo player, ShopItemInfo[] BagList, ShopItemInfo[] ConsoList)
		{
			return null;
		}

		public GSPacketIn SendUserRanks(int Id, List<UserRankInfo> ranks)
		{
			return null;
		}

		public GSPacketIn SendUpdateAchievements(GamePlayer player, BaseAchievement[] infos)
		{
			return null;
		}

		public GSPacketIn SendAchievementDatas(GamePlayer player, BaseAchievement[] infos)
		{
			return null;
		}

		public GSPacketIn SendDiceActiveClose(int ID)
		{
			return null;
		}

		public GSPacketIn SendDiceReceiveData(PlayerDice Dice)
		{
			return null;
		}

		public GSPacketIn SendDiceReceiveResult(PlayerDice Dice)
		{
			return null;
		}

		public GSPacketIn SendDiceActiveOpen(PlayerDice Dice)
		{
			return null;
		}

		public GSPacketIn SendGrowthPackageIsOpen(int ID)
		{
			return null;
		}

		public GSPacketIn SendGrowthPackageOpen(int ID, int isBuy)
		{
			return null;
		}

		public GSPacketIn SendGrowthPackageUpadte(int ID, int isBuy)
		{
			return null;
		}

		public GSPacketIn SendChickenBoxOpen(int ID, int flushPrice, int[] openCardPrice, int[] eagleEyePrice)
		{
			return null;
		}

		public GSPacketIn SendKingBlessMain(PlayerExtra Extra)
		{
			return null;
		}

		public GSPacketIn SendKingBlessUpdateBuffData(int UserID, int data, int value)
		{
			return null;
		}

		public GSPacketIn SendDeedMain(PlayerExtra Extra)
		{
			return null;
		}

		public GSPacketIn SendDeedUpdateBuffData(int UserID, int data, int value)
		{
			return null;
		}

		public GSPacketIn SendRuneOpenPackage(GamePlayer player, int rand)
		{
			return null;
		}

		public GSPacketIn SendPlayerDrill(int ID, Dictionary<int, UserDrillInfo> drills)
		{
			return null;
		}

		public GSPacketIn SendLuckStoneEnable(int ID)
		{
			return null;
		}

		public GSPacketIn SendBattleGoundOpen(int ID)
		{
			return null;
		}

		public GSPacketIn SendBattleGoundOver(int ID)
		{
			return null;
		}

		public GSPacketIn SendActivityList(int ID)
		{
			return null;
		}

		public void SendExpBlessedData(int PlayerId)
		{
			Nothing();
		}

		public GSPacketIn SendFindBackIncome(int ID)
		{
			return null;
		}

		public void SendWeaklessGuildProgress(PlayerInfo player)
		{
			Nothing();
		}

		public GSPacketIn SendSevenDoubleOpenClose(int ID, bool result)
		{
			return null;
		}

		public GSPacketIn SendConsortiaBattleOpenClose(int ID, bool result)
		{
			return null;
		}

		public GSPacketIn SendFightFootballTimeOpenClose(int ID, bool result)
		{
			return null;
		}

		public GSPacketIn SendGetBoxTime(int ID, int receiebox, bool result)
		{
			return null;
		}

		public GSPacketIn SendMessage(eMessageType type, string message)
		{
			return null;
		}

		public GSPacketIn sendBuyBadge(int consortiaID, int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaApplyStatusOut(bool state, bool result, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaUpdatePlacard(string description, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaChangeChairman(string nick, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaOut(int id, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaEquipConstrol(bool result, List<int> riches, int playerid)
		{
			return null;
		}

		public GSPacketIn SendConsortiaMemberGrade(int id, bool update, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn SendConsortiaCreate(string name1, bool result, int id, string name2, string msg, int dutyLevel, string DutyName, int dutyRight, int playerid)
		{
			return null;
		}

		public GSPacketIn SendConsortiaRichesOffer(int money, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn SendConsortiaInvite(string username, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaInvitePass(int id, bool result, int consortiaid, string consortianame, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaInviteDel(int id, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn SendConsortiaLevelUp(byte type, byte level, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaTryIn(int id, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaTryInPass(int id, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaTryInDel(int id, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn sendConsortiaUpdateDescription(string description, bool result, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn SendFarmPoultryLevel(int PlayerID, UserFarmInfo farm, int nextLevel)
		{
			return null;
		}

		public GSPacketIn SendFarmLandInfo(PlayerFarm farm)
		{
			return null;
		}

		public GSPacketIn SendHelperSwitchField(PlayerInfo Player, UserFarmInfo farm)
		{
			return null;
		}

		public GSPacketIn SendPayFields(GamePlayer Player, List<int> fieldIds)
		{
			return null;
		}

		public GSPacketIn SendSeeding(PlayerInfo Player, UserFieldInfo field)
		{
			return null;
		}

		public GSPacketIn SendKillCropField(PlayerInfo Player, UserFieldInfo field)
		{
			return null;
		}

		public GSPacketIn SendtoGather(PlayerInfo Player, UserFieldInfo field)
		{
			return null;
		}

		public GSPacketIn SenddoMature(PlayerFarm farm)
		{
			return null;
		}

		public GSPacketIn SendEnterFarm(PlayerInfo Player, UserFarmInfo farm, UserFieldInfo[] fields)
		{
			return null;
		}

		public void SendPetGuildOptionChange()
		{
			Nothing();
		}

		public GSPacketIn SendRefreshPet(GamePlayer player, UsersPetInfo[] pets, ItemInfo[] items, bool refreshBtn)
		{
			return null;
		}

		public GSPacketIn SendGameMissionPrepare()
		{
			return null;
		}

		public void SendOpenWorldBoss(int pX, int pY)
		{
			Nothing();
		}

		public void SendLittleGameActived()
		{
			Nothing();
		}

		public void SendTCP(GSPacketIn packet)
		{
			Nothing();
		}

		public void SendLoginSuccess()
		{
			Nothing();
		}

		public void SendCheckCode()
		{
			Nothing();
		}

		public void SendNotEnoughEnergyBuy(bool MustBuy)
		{
			Nothing();
		}

		public void SendUpdateCountHorseRace(int count)
		{
			Nothing();
		}

		public void SendUpdateAreaInfo()
		{
			Nothing();
		}

		public void SendEdictumVersion()
		{
			Nothing();
		}

		public GSPacketIn SendGameMissionStart()
		{
			return null;
		}

		public void SendLoginFailed(string msg)
		{
			Nothing();
		}

		public void SendKitoff(string msg)
		{
			Nothing();
		}

		public GSPacketIn SendOpenHoleComplete(GamePlayer player, int type, bool result)
		{
			return null;
		}

		public void SendEditionError(string msg)
		{
			Nothing();
		}

		public GSPacketIn SendOpenVIP(PlayerInfo player)
		{
			return null;
		}

		public void SendOpenEntertainmentMode()
		{
			Nothing();
		}

		public void SendUesedFinishTime(int uesedFinishTime)
		{
			Nothing();
		}

		public void SendOpenWorshipTheMoon(bool open)
		{
			Nothing();
		}

		public void SendDateTime()
		{
			Nothing();
		}

		public void SendchangeHorse(int curIndex)
		{
			Nothing();
		}

		public void SendPicCherishInfo(int playerId, MountDrawDataInfo[] draws)
		{
			Nothing();
		}

		public void SendHorseInitAllData(MountDataInfo info, MountSkillDataInfo[] skills)
		{
			Nothing();
		}

		public void SendUpHorse(bool isHasLevelUp, int grade, int exp)
		{
			Nothing();
		}

		public GSPacketIn SendDailyAward(PlayerInfo player)
		{
			return null;
		}

		public void SendPingTime(GamePlayer player)
		{
			Nothing();
		}

		public void SendDragonBoat(PlayerInfo info)
		{
			Nothing();
		}

		public void SendSignBuffInfo(ActiveSystemInfo activeUser)
		{
			Nothing();
		}

		public void SendGMBuff(ActiveSystemInfo activeUser)
		{
			Nothing();
		}

		public void SendLotteryTicketInfo(int userId, LotteryTicketMgr lottInfo)
		{
			Nothing();
		}

		public void SendLeagueNotice(int id, int restCount, int maxCount, byte type)
		{
			Nothing();
		}

		public void SendQQtips(QQtipsMessagesInfo QQTips)
		{
			Nothing();
		}

		public void SendCSMBox(int UserID)
		{
			Nothing();
		}

		public void SendPyramidOpenClose(PyramidConfigInfo info)
		{
			Nothing();
		}

		public void SendGuildMemberWeekOpenClose(PyramidConfigInfo info)
		{
			Nothing();
		}

		public void SendSuperWinnerOpen(int playerID, bool isOpen)
		{
			Nothing();
		}

		public void SendCatchBeastOpen(int playerID, bool isOpen)
		{
			Nothing();
		}

		public void SendLanternriddlesOpen(int playerID, bool isOpen)
		{
			Nothing();
		}

		public void SendTreasureHunting(PyramidConfigInfo info)
		{
			Nothing();
		}

		public GSPacketIn SendLuckStarOpen(int ID)
		{
			return null;
		}

		public void SendUpdatePrivateInfo(PlayerInfo info)
		{
			Nothing();
		}

		public GSPacketIn SendUpdatePlayerProperty(PlayerInfo info, PlayerProperty prop, Sys_Users_CardInfo cardInfo)
		{
			return null;
		}

		public GSPacketIn SendOtherPlayerProperty(PlayerInfo player, PlayerProperty prop, Sys_Users_CardInfo cardInfo)
		{
			return null;
		}

		public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info, UserMatchInfo matchInfo)
		{
			return null;
		}

		public GSPacketIn sendOneOnOneTalk(int receiverID, bool isAutoReply, string SenderNickName, string msg, int playerid)
		{
			return null;
		}

		public GSPacketIn SendConsortiaMail(bool result, int playerid)
		{
			return null;
		}

		public GSPacketIn SendNetWork(GamePlayer player, long delay)
		{
			return null;
		}

		public GSPacketIn SendPetLacrarInfo(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendUserEquip(PlayerInfo info, List<ItemInfo> pets, List<UserGemStone> UserGemStone, List<ItemInfo> beadItems, List<ItemInfo> magicStone, ExplorerManualInfo explorerInfo, List<MarkChipItemInfo> playerMark, List<UsersPetLacrarInfo> lacres, List<Sys_Users_Emblem> emblemInfos, ICollection<Sys_Users_Elf> elfinfo, Sys_Users_Academy academyInfo)
		{
			return null;
		}

		public GSPacketIn SendPlayerCardReset(PlayerInfo player, List<int> points)
		{
			return null;
		}

		public GSPacketIn SendGetPlayerCard(int playerId)
		{
			return null;
		}

		public void SendPlayerCardSlot(PlayerInfo info, List<UsersCardInfo> cardslots)
		{
			Nothing();
		}

		public void SendCardAchievementUpdate(int playerId, int[] Property, List<CardAchievementDataInfo> cardAchievements)
		{
			Nothing();
		}

		public GSPacketIn SendPlayerCardSlot(PlayerInfo player, UsersCardInfo card)
		{
			return null;
		}

		public GSPacketIn SendPlayerCardSoul(PlayerInfo player, bool isSoul, int soul)
		{
			return null;
		}

		public GSPacketIn SendGetCard(PlayerInfo info, UsersCardInfo card)
		{
			return null;
		}

		public void SendPlayerCardInfo(CardInventory bag, int[] updatedSlots)
		{
			Nothing();
		}

		public GSPacketIn SendPetInfo(int id, int zoneId, UsersPetInfo[] pets, EatPetsInfo eatpet)
		{
			return null;
		}

		public GSPacketIn SendUpdateUserPet(PetInventory bag, int[] slots)
		{
			return null;
		}

		public GSPacketIn SendEatPetsInfo(EatPetsInfo info)
		{
			return null;
		}

		public void SendWaitingRoom(bool result)
		{
			Nothing();
		}

		public GSPacketIn SendUpdateRoomList(List<BaseRoom> room)
		{
			return null;
		}

		public GSPacketIn SendSceneAddPlayer(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendRoomCreate(BaseRoom room)
		{
			return null;
		}

		public GSPacketIn SendSingleRoomCreate(BaseRoom room)
		{
			return null;
		}

		public GSPacketIn SendSingleRoomCreate(BaseRoom room, GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendRoomLoginResult(bool result)
		{
			return null;
		}

		public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
		{
			return null;
		}

		public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
		{
			return null;
		}

		public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendRoomPairUpStart(BaseRoom room)
		{
			return null;
		}

		public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
		{
			return null;
		}

		public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
		{
			return null;
		}

		public GSPacketIn SendGameRoomSetupChange(BaseRoom room)
		{
			return null;
		}

		public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isBind, int MinValid)
		{
			return null;
		}

		public GSPacketIn SendSetDressModelArr(int playerId, Dictionary<int, List<ItemInfo>> dresses)
		{
			return null;
		}

		public GSPacketIn SendCurentDressModel(int playerId, int current)
		{
			return null;
		}

		public GSPacketIn SendAvatarColectionAllInfo(int PlayerId, Dictionary<int, UserAvatarColectionInfo> infos)
		{
			return null;
		}

		public GSPacketIn SendFusionResult(GamePlayer player, bool result)
		{
			return null;
		}

		public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item)
		{
			return null;
		}

		public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
		{
			Nothing();
		}

		private void Nothing()
		{
		}

		public GSPacketIn SendAddFriend(PlayerInfo user, int relation, bool state)
		{
			return null;
		}

		public GSPacketIn SendFriendRemove(int FriendID)
		{
			return null;
		}

		public GSPacketIn SendFriendState(int playerID, int state, byte typeVip, int viplevel)
		{
			return null;
		}

		public GSPacketIn SendUpdateAllData(PlayerInfo player)
		{
			return null;
		}

		public GSPacketIn SendGetSpree(PlayerInfo player)
		{
			return null;
		}

		public GSPacketIn SendUpdateUpCount(PlayerInfo player)
		{
			return null;
		}

		public GSPacketIn SendPlayerRefreshTotem(PlayerInfo player)
		{
			return null;
		}

		public GSPacketIn SendLabyrinthUpdataInfoOld(int ID, UserLabyrinthInfo laby)
		{
			return null;
		}

		public GSPacketIn SendLabyrinthUpdataInfo(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendPlayerFigSpiritinit(int ID, List<UserGemStone> gems)
		{
			return null;
		}

		public GSPacketIn SendPlayerFigSpiritUp(int ID, UserGemStone gem, bool isUp, bool isMaxLevel, bool isFall, int num, int dir)
		{
			return null;
		}

		public GSPacketIn SendTrusteeshipStart(int ID)
		{
			return null;
		}

		public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)
		{
			return null;
		}

		public GSPacketIn SendUpdateConsotiaBuffer(GamePlayer player, Dictionary<int, BufferInfo> bufflist)
		{
			return null;
		}

		public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
		{
			return null;
		}

		public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] quests)
		{
			return null;
		}

		public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
		{
			return null;
		}

		public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item)
		{
			return null;
		}

		public GSPacketIn SendIDNumberCheck(bool result)
		{
			return null;
		}

		public GSPacketIn SendAASState(bool result)
		{
			return null;
		}

		public GSPacketIn SendAASInfoSet(bool result)
		{
			return null;
		}

		public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
		{
			return null;
		}

		public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
		{
			return null;
		}

		public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
		{
			return null;
		}

		public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
		{
			return null;
		}

		public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int ID)
		{
			return null;
		}

		public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
		{
			return null;
		}

		public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int ID)
		{
			return null;
		}

		public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result, int failType)
		{
			return null;
		}

		public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
		{
			return null;
		}

		public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
		{
			return null;
		}

		public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
		{
			return null;
		}

		public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
		{
			return null;
		}

		public GSPacketIn SendInsufficientMoney(GamePlayer player, int type)
		{
			return null;
		}

		public GSPacketIn SendUserBankUpdate(UserBankInfo bank)
		{
			return null;
		}

		public GSPacketIn SendUserSyncEquipGhost(GamePlayer p)
		{
			return null;
		}

		public void SendOpenCloseBraveDoor(GamePlayer gamePlayer, bool isOpen)
		{
			Nothing();
		}

		public void SendRenshenOpenClose(bool isOpen)
		{
			Nothing();
		}

		public void SendRenshenOpenClose()
		{
			Nothing();
		}

		public GSPacketIn SendUpdateCoin(UsersExtraInfo info)
		{
			return null;
		}

		public GSPacketIn SendSummonResult(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendGourdExpStorage(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendLabyrinthSweepInfo(GamePlayer player, List<ItemInfo> items, int exp, int money)
		{
			return null;
		}

		public GSPacketIn SendLabyrinthStartSweep(GamePlayer player)
		{
			return null;
		}

		public GSPacketIn SendAcademySystemNotice(string text, bool isAlert)
		{
			return null;
		}

		public GSPacketIn SendAcademyApprenticeState(PlayerInfo player, int removeUserId)
		{
			return null;
		}

		public void SendAcademyGradute(GamePlayer app, int type)
		{
			Nothing();
		}

		public void SendLabyrinthUpdataInfo(int id, UserLabyrinthInfo laby)
		{
			throw new NotImplementedException();
		}

		public GSPacketIn SendRoomRobotInfo(bool result)
		{
			throw new NotImplementedException();
		}
	}

	public static IPacketLib NULLOUT = new NullOut();

	public override bool IsActive => true;

	public override IPacketLib Out => NULLOUT;

	public override bool IsRobot => true;

	public RobotGamePlayer(int playerId, PlayerInfo info)
		: base(playerId, "", null, info)
	{
	}

	public override void SendTCP(GSPacketIn pkg)
	{
	}

	public override bool LoadFromDatabase()
	{
		return true;
	}

	public override bool SaveIntoDatabase()
	{
		return true;
	}

	public void Equip(int equipid, int strength, int attCompose, int defCompose, int agiCompose, int luckCompose)
	{
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(equipid), 1, 0);
		itemInfo.StrengthenLevel = strength;
		itemInfo.AttackCompose = attCompose;
		itemInfo.DefendCompose = defCompose;
		itemInfo.AgilityCompose = agiCompose;
		itemInfo.LuckCompose = luckCompose;
		base.EquipBag.AddItem(itemInfo, base.EquipBag.FindItemEpuipSlot(itemInfo.Template));
	}
}
