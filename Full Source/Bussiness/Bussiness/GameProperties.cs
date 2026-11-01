using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Config;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness;

public abstract class GameProperties
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	[ConfigProperty("Edition", "µ±Ç°ÓÎÏ·°æ±¾", "5498628")]
	public static readonly string EDITION;

	[ConfigProperty("ClientVersion", "ºÏ³ÉÏûºÄ½ð±Ò¼Û\u00b8ñ", 12100)]
	public static readonly int VERSION;

	[ConfigProperty("MustComposeGold", "ºÏ³ÉÏûºÄ½ð±Ò¼Û\u00b8ñ", 1000)]
	public static readonly int PRICE_COMPOSE_GOLD;

	[ConfigProperty("MustFusionGold", "ÈÛÁ¶ÏûºÄ½ð±Ò¼Û\u00b8ñ", 400)]
	public static readonly int PRICE_FUSION_GOLD;

	[ConfigProperty("MustStrengthenGold", "Ç¿»\u00af½ð±ÒÏûºÄ¼Û\u00b8ñ", 1000)]
	public static readonly int PRICE_STRENGHTN_GOLD;

	[ConfigProperty("CheckRewardItem", "ÑéÖ¤Âë½±ÀøÎïÆ·", 11001)]
	public static readonly int CHECK_REWARD_ITEM;

	[ConfigProperty("CheckCount", "×î\u00b4óÑéÖ¤ÂëÊ§°Ü\u00b4ÎÊý", 2)]
	public static readonly int CHECK_MAX_FAILED_COUNT;

	[ConfigProperty("HymenealMoney", "Çó»éµÄ¼Û\u00b8ñ", 300)]
	public static readonly int PRICE_PROPOSE;

	[ConfigProperty("DivorcedMoney", "Àë»éµÄ¼Û\u00b8ñ", 1499)]
	public static readonly int PRICE_DIVORCED;

	[ConfigProperty("DivorcedDiscountMoney", "Àë»éµÄ¼Û\u00b8ñ", 999)]
	public static readonly int PRICE_DIVORCED_DISCOUNT;

	[ConfigProperty("MarryRoomCreateMoney", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "2000,2700,3400")]
	public static readonly string PRICE_MARRY_ROOM;

	[ConfigProperty("LimitGardeSendMailMomey", "LimitGardeSendMailMomey", 0)]
	public static int LimitGardeSendMailMomey;

	[ConfigProperty("MarryRoomSendGiftItemID", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "11900")]
	public static readonly string MARRY_ROOMSEND_GIFT_ITEMID;

	[ConfigProperty("SeniorMarrySuit", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "13705,13706,46060")]
	public static readonly string SENIOR_MARRY_SUIT;

	[ConfigProperty("SeniorMarryBegin", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "2010/1/1 0:00:00")]
	public static readonly string SENIOR_MARRY_BEGIN;

	[ConfigProperty("SeniorMarryEnd", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "2010/1/1 0:00:00")]
	public static readonly string SENIOR_MARRY_END;

	[ConfigProperty("SeniorMarryMoney", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "11000,45000,180000")]
	public static readonly string SENIOR_MARRY_MONEY;

	[ConfigProperty("SeniorMarryMinGiftMoney", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "10,20,30")]
	public static readonly string SENIOR_MARRY_MIN_GIFT_MONEY;

	[ConfigProperty("SeniorMarryMaxGiftMoney", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "20,40,60")]
	public static readonly string SENIOR_MARRY_MAX_GIFT_MONEY;

	[ConfigProperty("SeniorMarryGift", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "1120324,1120325,1120326")]
	public static readonly string SENIOR_MARRY_GIFT;

	[ConfigProperty("BoxAppearCondition", "Ïä×ÓÎïÆ·ÌáÊ¾µÄµÈ¼¶", 4)]
	public static readonly int BOX_APPEAR_CONDITION;

	[ConfigProperty("DisableCommands", "½ûÖ¹Ê¹ÓÃµÄÃüÁî", "")]
	public static readonly string DISABLED_COMMANDS;

	[ConfigProperty("AssState", "·À³ÁÃÔÏµÍ³µÄ¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", false)]
	public static bool ASS_STATE;

	[ConfigProperty("DailyAwardState", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", true)]
	public static bool DAILY_AWARD_STATE;

	[ConfigProperty("Cess", "½»Ò×¿ÛË°", 0.1)]
	public static readonly double Cess;

	[ConfigProperty("BeginAuction", "ÅÄÂòÊ±ÆðÊ¼Ëæ»úÊ±¼ä", 20)]
	public static int BeginAuction;

	[ConfigProperty("EntertainmentModeMoneyAdd", "ÅÄÂòÊ±ÆðÊ¼Ëæ»úÊ±¼ä", 1800)]
	public static int EntertainmentModeMoneyAdd;

	[ConfigProperty("EndAuction", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", 40)]
	public static int EndAuction;

	[ConfigProperty("HotSpringExp", "Kinh nghiệm Spa", "200,240,288,346,415,498,597,717,860,946,1041,1145,1259,1385,1523,1676,1843,2028,2231,2342,2459,2582,2711,2847,2989,3139,3295,3460,3633,3815,3929,4047,4169,4294,4423,4555,4692,4833,4978,5127,5281,5439,5602,5770,5944,6122,6306,6495,6690,6890,7097,7310,7529,7755,7988,8227,8474,8728,8990,9260,9530,9800,10070,10340,10610,10880,11150,11420,11690,11960")]
	public static readonly string HotSpringExp;

	[ConfigProperty("ConsortiaStrengthenEx", "Kinh nghiệm", "1|2")]
	public static readonly string ConsortiaStrengthenEx;

	[ConfigProperty("RuneLevelUpExp", "Kinh nghiệm châu báu", "1|2|4|8|16|32|64|128|384|1152|3456|10368|31104|93312|279936|839808|1679616|6718464|16796160")]
	public static readonly string RuneLevelUpExp;

	[ConfigProperty("VIPExpForEachLv", "VIPExpForEachLv", "1|2")]
	public static readonly string VIPExpForEachLv;

	[ConfigProperty("HoleLevelUpExpList", "HoleLevelUpExpList", "400|600|700|800|800|800")]
	public static readonly string HoleLevelUpExpList;

	[ConfigProperty("VIPStrengthenEx", "VIPStrengthenEx", "1|2")]
	public static readonly string VIPStrengthenEx;

	[ConfigProperty("IsOpenOrCloseEyes", "IsOpenOrCloseEyes", true)]
	public static readonly bool IsOpenOrCloseEyes;

	[ConfigProperty("CustomLimit", "sendattackmail|addaution|PresentGoods|PresentMoney|MarryLargess", "20|20|20|20|20")]
	public static readonly string CustomLimit;

	[ConfigProperty("IsLimitPresentGoods", "IsLimitMail", false)]
	public static readonly bool IsLimitPresentGoods;

	[ConfigProperty("LimitPresentGoods", "LimitMail", 3)]
	public static readonly int LimitPresentGoods;

	[ConfigProperty("IsLimitCount", "IsLimitCount", false)]
	public static readonly bool IsLimitCount;

	[ConfigProperty("LimitCount", "LimitCount", 10)]
	public static readonly int LimitCount;

	[ConfigProperty("IsLimitMoney", "IsLimitMoney", false)]
	public static readonly bool IsLimitMoney;

	[ConfigProperty("LimitMoney", "LimitMoney", 999000)]
	public static readonly int LimitMoney;

	[ConfigProperty("IsLimitAuction", "IsLimitAuction", false)]
	public static readonly bool IsLimitAuction;

	[ConfigProperty("LimitAuction", "LimitAuction", 3)]
	public static readonly int LimitAuction;

	[ConfigProperty("IsLimitMail", "IsLimitMail", false)]
	public static readonly bool IsLimitMail;

	[ConfigProperty("LimitMail", "LimitMail", 3)]
	public static readonly int LimitMail;

	[ConfigProperty("TestActive", "TestActive", false)]
	public static readonly bool TestActive;

	[ConfigProperty("WishBeadLimitLv", "WishBeadLimitLv", 12)]
	public static readonly int WishBeadLimitLv;

	[ConfigProperty("IsWishBeadLimit", "IsWishBeadLimit", false)]
	public static readonly bool IsWishBeadLimit;

	[ConfigProperty("BagMailEnable", "BagMailEnable", true)]
	public static readonly bool BagMailEnable;

	[ConfigProperty("FreeMoney", "µ±Ç°ÓÎÏ·°æ±¾", 9990000)]
	public static readonly int FreeMoney;

	[ConfigProperty("FreeExp", "µ±Ç°ÓÎÏ·°æ±¾", "11901|1")]
	public static readonly string FreeExp;

	[ConfigProperty("BigExp", "µ±Ç°ÓÎÏ·°æ±¾", "11906|99")]
	public static readonly string BigExp;

	[ConfigProperty("PetExp", "µ±Ç°ÓÎÏ·°æ±¾", "334103|999")]
	public static readonly string PetExp;

	[ConfigProperty("IsActiveMoney", "IsActiveMoney", true)]
	public static readonly bool IsActiveMoney;

	[ConfigProperty("IsDDTMoneyActive", "IsDDTMoneyActive", true)]
	public static readonly bool IsDDTMoneyActive;

	[ConfigProperty("NewChickenBeginTime", "NewChickenBeginTime", "2013/12/17 0:00:00")]
	public static readonly string NewChickenBeginTime;

	[ConfigProperty("NewChickenEndTime", "NewChickenEndTime", "2123/12/25 0:00:00")]
	public static readonly string NewChickenEndTime;

	[ConfigProperty("NewChickenEagleEyePrice", "NewChickenEagleEyePrice", "3000, 2000, 1000")]
	public static readonly string NewChickenEagleEyePrice;

	[ConfigProperty("NewChickenOpenCardPrice", "NewChickenOpenCardPrice", "2500, 2000, 1500, 1000, 500")]
	public static readonly string NewChickenOpenCardPrice;

	[ConfigProperty("NewChickenFlushPrice", "NewChickenFlushPrice", 10000)]
	public static readonly int NewChickenFlushPrice;

	[ConfigProperty("DiceBeginTime", "DiceBeginTime", "2013/12/17 0:00:00")]
	public static readonly string DiceBeginTime;

	[ConfigProperty("DiceEndTime", "DiceEndTime", "2122/12/25 0:00:00")]
	public static readonly string DiceEndTime;

	[ConfigProperty("DiceRefreshPrice", "DiceRefreshPrice", 40000)]
	public static readonly int DiceRefreshPrice;

	[ConfigProperty("CommonDicePrice", "CommonDicePrice", 30000)]
	public static readonly int CommonDicePrice;

	[ConfigProperty("DoubleDicePrice", "DoubleDicePrice", 40000)]
	public static readonly int DoubleDicePrice;

	[ConfigProperty("BigDicePrice", "BigDicePrice", 50000)]
	public static readonly int BigDicePrice;

	[ConfigProperty("SmallDicePrice", "SmallDicePrice", 60000)]
	public static readonly int SmallDicePrice;

	[ConfigProperty("PyramidBeginTime", "PyramidBeginTime", "2013/12/17 0:00:00")]
	public static readonly string PyramidBeginTime;

	[ConfigProperty("PyramidEndTime", "PyramidEndTime", "2122/12/25 0:00:00")]
	public static readonly string PyramidEndTime;

	[ConfigProperty("PyramidRevivePrice", "PyramidRevivePrice", "10000, 30000, 50000")]
	public static readonly string PyramidRevivePrice;

	[ConfigProperty("PyramydTurnCardPrice", "PyramydTurnCardPrice", 5000)]
	public static readonly int PyramydTurnCardPrice;

	[ConfigProperty("DragonBoatBeginDate", "DragonBoatBeginDate", "2013/12/19 0:00:00")]
	public static readonly string DragonBoatBeginDate;

	[ConfigProperty("DragonBoatEndDate", "DragonBoatEndDate", "2113/12/26 0:00:00")]
	public static readonly string DragonBoatEndDate;

	[ConfigProperty("FightFootballTime", "FightFootballTime", "19|60")]
	public static readonly string FightFootballTime;

	[ConfigProperty("KingBuffPrice", "KingBuffPrice", "475,1425,2500")]
	public static readonly string KingBuffPrice;

	[ConfigProperty("RunePackageID", "RunePackageID", "311100|311200|311300|311400")]
	public static readonly string RunePackageID;

	[ConfigProperty("OpenRunePackageMoney", "OpenRunePackageMoney", "10|20|50|100")]
	public static readonly string OpenRunePackageMoney;

	[ConfigProperty("OpenRunePackageRange", "OpenRunePackageRange", "1,6|1,5|1,4")]
	public static readonly string OpenRunePackageRange;

	[ConfigProperty("VIPQuestFinishDirect", "VIPQuestFinishDirect", "0|0|0|0|0|2|2|2|2|3|3|4")]
	public static readonly string VIPQuestFinishDirect;

	[ConfigProperty("MustTransferGold", "MustTransferGold", 40000)]
	public static readonly int MustTransferGold;

	[ConfigProperty("LuckStarActivityBeginDate", "LuckStarActivityBeginDate", "2013/12/1 0:00:00")]
	public static readonly string LuckStarActivityBeginDate;

	[ConfigProperty("LuckStarActivityEndDate", "LuckStarActivityEndDate", "2114/12/24 0:00:00")]
	public static readonly string LuckStarActivityEndDate;

	[ConfigProperty("MinUseNum", "MinUseNum", 1000)]
	public static readonly int MinUseNum;

	[ConfigProperty("YearMonsterBeginDate", "YearMonsterBeginDate", "2014/1/17 0:00:00")]
	public static readonly string YearMonsterBeginDate;

	[ConfigProperty("YearMonsterEndDate", "YearMonsterEndDate", "2122/2/25 0:00:00")]
	public static readonly string YearMonsterEndDate;

	[ConfigProperty("YearMonsterBoxInfo", "YearMonsterBoxInfo", "112370,5|112371,30|112372,90|112373,150|112374,300")]
	public static readonly string YearMonsterBoxInfo;

	[ConfigProperty("YearMonsterBuffMoney", "YearMonsterBuffMoney", 300)]
	public static readonly int YearMonsterBuffMoney;

	[ConfigProperty("YearMonsterFightNum", "YearMonsterFightNum", 1)]
	public static readonly int YearMonsterFightNum;

	[ConfigProperty("YearMonsterHP", "YearMonsterHP", 3000000)]
	public static readonly int YearMonsterHP;

	[ConfigProperty("YearMonsterOpenLevel", "YearMonsterOpenLevel", 15)]
	public static readonly int YearMonsterOpenLevel;

	[ConfigProperty("DiceGameAwardAndCount", "DiceGameAwardAndCount", "32|16|8|4|2|1")]
	public static readonly string DiceGameAwardAndCount;

	[ConfigProperty("LightRiddleAnswerScore", "LightRiddleAnswerScore", "29|9")]
	public static readonly string LightRiddleAnswerScore;

	[ConfigProperty("LightRiddleBeginDate", "LightRiddleBeginDate", "2014/2/13 0:00:00")]
	public static readonly string LightRiddleBeginDate;

	[ConfigProperty("LightRiddleEndDate", "LightRiddleEndDate", "2114/2/28 0:00:00")]
	public static readonly string LightRiddleEndDate;

	[ConfigProperty("LightRiddleBeginTime", "LightRiddleBeginTime", "2014/2/13 12:30:00")]
	public static readonly string LightRiddleBeginTime;

	[ConfigProperty("LightRiddleEndTime", "LightRiddleEndTime", "2114/2/13 13:00:00")]
	public static readonly string LightRiddleEndTime;

	[ConfigProperty("LightRiddleFreeHitNum", "LightRiddleFreeHitNum", 2)]
	public static readonly int LightRiddleFreeHitNum;

	[ConfigProperty("LightRiddleFreeComboNum", "LightRiddleFreeComboNum", 2)]
	public static readonly int LightRiddleFreeComboNum;

	[ConfigProperty("LightRiddleComboMoney", "LightRiddleComboMoney", 30)]
	public static readonly int LightRiddleComboMoney;

	[ConfigProperty("LightRiddleHitMoney", "LightRiddleHitMoney", 30)]
	public static readonly int LightRiddleHitMoney;

	[ConfigProperty("LightRiddleAnswerTime", "LightRiddleAnswerTime", 15)]
	public static readonly int LightRiddleAnswerTime;

	[ConfigProperty("LightRiddleOpenLevel", "LightRiddleOpenLevel", 15)]
	public static readonly int LightRiddleOpenLevel;

	[ConfigProperty("WarriorFamRaidPricePerMin", "WarriorFamRaidPricePerMin", 10)]
	public static readonly int WarriorFamRaidPricePerMin;

	[ConfigProperty("WarriorFamRaidPriceSmall", "WarriorFamRaidPriceSmall", 30000)]
	public static readonly int WarriorFamRaidPriceSmall;

	[ConfigProperty("WarriorFamRaidPriceBig", "WarriorFamRaidPriceBig ", 40000)]
	public static readonly int WarriorFamRaidPriceBig;

	[ConfigProperty("WarriorFamRaidDDTPrice", "WarriorFamRaidDDTPrice", 5000)]
	public static readonly int WarriorFamRaidDDTPrice;

	[ConfigProperty("WarriorFamRaidTimeRemain", "WarriorFamRaidTimeRemain", 120)]
	public static readonly int WarriorFamRaidTimeRemain;

	[ConfigProperty("ChristmasBeginDate", "ChristmasBeginDate", "2013/12/17 0:00:00")]
	public static readonly string ChristmasBeginDate;

	[ConfigProperty("ChristmasEndDate", "ChristmasEndDate", "2123/12/25 0:00:00")]
	public static readonly string ChristmasEndDate;

	[ConfigProperty("ChristmasGifts", "ChristmasGifts", "201148,10|201149,35|201150,70|201151,120|201152,220|201153,370|201154,650|201155,1000|201156,100")]
	public static readonly string ChristmasGifts;

	[ConfigProperty("ChristmasGiftsMaxNum", "ChristmasGiftsMaxNum", 1000)]
	public static readonly int ChristmasGiftsMaxNum;

	[ConfigProperty("ChristmasBuildSnowmanDoubleMoney", "ChristmasBuildSnowmanDoubleMoney", 10)]
	public static readonly int ChristmasBuildSnowmanDoubleMoney;

	[ConfigProperty("ChristmasBuyTimeMoney", "ChristmasBuyTimeMoney", 150)]
	public static readonly int ChristmasBuyTimeMoney;

	[ConfigProperty("ChristmasMinute", "ChristmasMinute", 60)]
	public static readonly int ChristmasMinute;

	[ConfigProperty("ChristmasBuyMinute", "ChristmasBuyMinute", 10)]
	public static readonly int ChristmasBuyMinute;

	[ConfigProperty("DragonBoatByMoney", "DragonBoatByMoney", "100:10,10")]
	public static readonly string DragonBoatByMoney;

	[ConfigProperty("DragonBoatByProps", "DragonBoatByProps", "1:10,10")]
	public static readonly string DragonBoatByProps;

	[ConfigProperty("DragonBoatMaxScore", "DragonBoatMaxScore", 30000)]
	public static readonly int DragonBoatMaxScore;

	[ConfigProperty("DragonBoatMinScore", "DragonBoatMinScore", 13000)]
	public static readonly int DragonBoatMinScore;

	[ConfigProperty("DragonBoatAreaMinScore", "DragonBoatAreaMinScore", 20000)]
	public static readonly int DragonBoatAreaMinScore;

	[ConfigProperty("DragonBoatProp", "DragonBoatProp", 11690)]
	public static readonly int DragonBoatProp;

	[ConfigProperty("DragonBoatConvertHours", "DragonBoatConvertHours", 72)]
	public static readonly int DragonBoatConvertHours;

	[ConfigProperty("PromotePackagePrice", "PromotePackagePrice", 3600)]
	public static readonly int PromotePackagePrice;

	[ConfigProperty("IsPromotePackageOpen", "IsPromotePackageOpen", false)]
	public static readonly bool IsPromotePackageOpen;

	[ConfigProperty("SearchGoodsFreeLimit", "SearchGoodsFreeLimit", 0)]
	public static readonly int SearchGoodsFreeLimit;

	[ConfigProperty("SearchGoodsPayMoney", "SearchGoodsPayMoney", 20)]
	public static readonly int SearchGoodsPayMoney;

	[ConfigProperty("SearchGoodsFreeCount", "SearchGoodsFreeCount", 3)]
	public static readonly int SearchGoodsFreeCount;

	[ConfigProperty("SearchGoodsTakeCardMoney", "SearchGoodsTakeCardMoney", "0|50|120")]
	public static readonly string SearchGoodsTakeCardMoney;

	[ConfigProperty("MaxMissionEnergy", "MaxMissionEnergy", 1500)]
	public static readonly int MaxMissionEnergy;

	[ConfigProperty("MissionEnergyAdd", "MissionEnergyAdd", 300)]
	public static readonly int MissionEnergyAdd;

	[ConfigProperty("OpenMagicBoxMoney", "OpenMagicBoxMoney", "300,30")]
	public static readonly string OpenMagicBoxMoney;

	[ConfigProperty("FastGrowSubTime", "FastGrowSubTime", 30)]
	public static readonly int FastGrowSubTime;

	[ConfigProperty("FastGrowNeedMoney", "FastGrowNeedMoney", 30)]
	public static readonly int FastGrowNeedMoney;

	[ConfigProperty("HalloweenMinNum", "HalloweenMinNum", 5000)]
	public static readonly int HalloweenMinNum;

	[ConfigProperty("HalloweenBeginDate", "HalloweenBeginDate", "2013/12/17 0:00:00")]
	public static readonly string HalloweenBeginDate;

	[ConfigProperty("HalloweenEndDate", "HalloweenEndDate", "2113/12/17 0:00:00")]
	public static readonly string HalloweenEndDate;

	[ConfigProperty("DanDanBuffPrice", "DanDanBuffPrice", 3000)]
	public static readonly int DanDanBuffPrice;

	[ConfigProperty("ArenaChallengeCD", "ArenaChallengeCD", 10)]
	public static readonly int ArenaChallengeCD;

	[ConfigProperty("ArenaRankMaxNum", "ArenaRankMaxNum", 5000)]
	public static readonly int ArenaRankMaxNum;

	[ConfigProperty("ArenaChallengeMaxNum", "ArenaChallengeMaxNum", 10)]
	public static readonly int ArenaChallengeMaxNum;

	[ConfigProperty("ArenaCleanCDMoney", "ArenaCleanCDMoney", 50)]
	public static readonly int ArenaCleanCDMoney;

	[ConfigProperty("ArenaChallengeBuyNum", "ArenaChallengeBuyNum", 10)]
	public static readonly int ArenaChallengeBuyNum;

	[ConfigProperty("ArenaChallengeBuyMoney", "ArenaChallengeBuyMoney", 80)]
	public static readonly int ArenaChallengeBuyMoney;

	[ConfigProperty("ArenaRewardIntervalSecond", "ArenaRewardIntervalSecond", 3)]
	public static readonly int ArenaRewardIntervalSecond;

	[ConfigProperty("ArenaRewardNumPerInterval", "ArenaRewardNumPerInterval", 50)]
	public static readonly int ArenaRewardNumPerInterval;

	[ConfigProperty("CryptBossOpenDay", "CryptBossOpenDay", "1,1|2,2|3,3|4,4|5,5,6,0|6,5,6,0")]
	public static readonly string CryptBossOpenDay;

	[ConfigProperty("MagicRoomJuniorWeaponList", "MagicRoomJuniorWeaponList", "7080,70801,70802,70803,70804|7093,70931,70932,70933,70934|7100,71001,71002,71003,71004")]
	public static readonly string MagicRoomJuniorWeaponList;

	[ConfigProperty("MagicRoomMidWeaponList", "MagicRoomMidWeaponList", "7127,71271,71272,71273,71274|7126,71261,71262,71263,71264|7131,71311,71312,71313,71314")]
	public static readonly string MagicRoomMidWeaponList;

	[ConfigProperty("MagicRoomSeniorWeaponList", "MagicRoomSeniorWeaponList", "7099,70991,70992,70993,70994|7116,71161,71162,71163,71164|7129,71291,71292,71293,71294")]
	public static readonly string MagicRoomSeniorWeaponList;

	[ConfigProperty("MagicRoomJuniorAddAttribute", "MagicRoomJuniorAddAttribute", "3,3,0|1,1,0|1,1,0|1,1,0|2,2,0|2,2,0")]
	public static readonly string MagicRoomJuniorAddAttribute;

	[ConfigProperty("MagicRoomMidAddAttribute", "MagicRoomMidAddAttribute", "0,3,2|0,1,1|0,1,1|0,2,1|0,3,1|0,3,1")]
	public static readonly string MagicRoomMidAddAttribute;

	[ConfigProperty("MagicRoomSeniorAddAttribute", "MagicRoomSeniorAddAttribute", "5,0,2|1,0,1|1,0,1|2,0,1|3,0,1|3,0,1")]
	public static readonly string MagicRoomSeniorAddAttribute;

	[ConfigProperty("MagicRoomBoxNeedMoney", "MagicRoomBoxNeedMoney", 500)]
	public static readonly int MagicRoomBoxNeedMoney;

	[ConfigProperty("MagicRoomLevelUpCount", "MagicRoomLevelUpCount", "800|2400|3200|4800|6400")]
	public static readonly string MagicRoomLevelUpCount;

	[ConfigProperty("MagicRoomOpenNeedMoney", "MagicRoomOpenNeedMoney", "500|10")]
	public static readonly string MagicRoomOpenNeedMoney;

	[ConfigProperty("WorshipMoonBeginDate", "WorshipMoonBeginDate", "2014/11/7 9:00:00")]
	public static readonly string WorshipMoonBeginDate;

	[ConfigProperty("WorshipMoonEndDate", "WorshipMoonEndDate", "2122/11/13 22:30:00")]
	public static readonly string WorshipMoonEndDate;

	[ConfigProperty("WorshipMoonProb", "WorshipMoonProb", "35|35|20|5|3|2")]
	public static readonly string WorshipMoonProb;

	[ConfigProperty("WorshipMoonReward", "WorshipMoonReward", "1120233|1120234|1120235|1120236|1120237|1120238")]
	public static readonly string WorshipMoonReward;

	[ConfigProperty("WorshipMoonGift", "WorshipMoonGift", "200|1120239")]
	public static readonly string WorshipMoonGift;

	[ConfigProperty("WorshipMoonGiftShowList", "WorshipMoonGiftShowList", "11972|11973|11974|11975|11976|11977|11978|7093|7100|7126|13668|17000")]
	public static readonly string WorshipMoonGiftShowList;

	[ConfigProperty("WorshipMoonPriceInfo", "WorshipMoonPriceInfo", "3|100")]
	public static readonly string WorshipMoonPriceInfo;

	[ConfigProperty("SummerAcitveBeginTime", "SummerAcitveBeginTime", "2014/12/11 9:00:00")]
	public static readonly string SummerAcitveBeginTime;

	[ConfigProperty("SummerAcitveEndTime", "SummerAcitveEndTime", "2114/12/28 23:59:59")]
	public static readonly string SummerAcitveEndTime;

	[ConfigProperty("SummerAcitveGifts", "SummerAcitveGifts", "1120179,1000|1120180,3000|1120181,10000|1120182,20000|1120183,50000|1120184,100000|1120185,200000")]
	public static readonly string SummerAcitveGifts;

	[ConfigProperty("AreaSummerAcitveTitle", "AreaSummerAcitveTitle", "626|618|618|629|629|629|629|629|629|629")]
	public static readonly string AreaSummerAcitveTitle;

	[ConfigProperty("SummerAcitveTitle", "SummerAcitveTitle", "620|619|619|621|621|621|621|621|621|621")]
	public static readonly string SummerAcitveTitle;

	[ConfigProperty("SummerAcitveConvertHours", "SummerAcitveConvertHours", 48)]
	public static readonly int SummerAcitveConvertHours;

	[ConfigProperty("SummerAcitveMonsters", "SummerAcitveMonsters", "10012|10013|10014")]
	public static readonly string SummerAcitveMonsters;

	[ConfigProperty("TanabataActiveEventNum", "TanabataActiveEventNum", "17,2,2|19,2,3|19,3,3")]
	public static readonly string TanabataActiveEventNum;

	[ConfigProperty("TanabataActivePrice", "TanabataActivePrice", 100)]
	public static readonly int TanabataActivePrice;

	[ConfigProperty("TanabataActiveDestinationAWardID", "TanabataActiveDestinationAWardID", "1120156|1120157|1120158")]
	public static readonly string TanabataActiveDestinationAWardID;

	[ConfigProperty("TanabataActiveBeginTime", "TanabataActiveBeginTime", "2015/3/19 9:00:00")]
	public static readonly string TanabataActiveBeginTime;

	[ConfigProperty("TanabataActiveEndTime", "TanabataActiveEndTime", "2122/3/22 23:59:59")]
	public static readonly string TanabataActiveEndTime;

	[ConfigProperty("IsTanabataTreasure", "IsTanabataTreasure", 48)]
	public static readonly int IsTanabataTreasure;

	[ConfigProperty("TanabataTreasureLimit", "TanabataTreasureLimit", 25)]
	public static readonly int TanabataTreasureLimit;

	[ConfigProperty("MysteryShopOpenTime", "MysteryShopOpenTime", "12|0")]
	public static readonly string MysteryShopOpenTime;

	[ConfigProperty("MysteryShopFreshTime", "MysteryShopFreshTime", 18)]
	public static readonly int MysteryShopFreshTime;

	[ConfigProperty("HorseGameBuffConfig", "HorseGameBuffConfig", "1,3|2,5|3,5|4,5|5,5|6,5|7,2|8,5")]
	public static readonly string HorseGameBuffConfig;

	[ConfigProperty("HorseGamePlayerCount", "HorseGamePlayerCount", 1)]
	public static readonly int HorseGamePlayerCount;

	[ConfigProperty("HorseGameEachDayMaxCount", "HorseGameEachDayMaxCount", 1)]
	public static readonly int HorseGameEachDayMaxCount;

	[ConfigProperty("HorseGameUsePapawMoney", "HorseGameUsePapawMoney", 100)]
	public static readonly int HorseGameUsePapawMoney;

	[ConfigProperty("HorseGameBeginTime", "HorseGameBeginTime", "2015/01/03 0:00:00")]
	public static readonly string HorseGameBeginTime;

	[ConfigProperty("HorseGameEndTime", "HorseGameEndTime", "2115/06/03 0:00:00")]
	public static readonly string HorseGameEndTime;

	[ConfigProperty("HorseGameCostMoneyCount", "HorseGameCostMoneyCount", 200)]
	public static readonly int HorseGameCostMoneyCount;

	[ConfigProperty("HorseGameEachDayMaxCostCount", "HorseGameEachDayMaxCostCount", 5)]
	public static readonly int HorseGameEachDayMaxCostCount;

	[ConfigProperty("HorseGameBeginWeek", "HorseGameBeginWeek", "1,2,3,4,5,6,0")]
	public static readonly string HorseGameBeginWeek;

	[ConfigProperty("TreasureHuntStarCount", "TreasureHuntStarCount", "25,1,2,1,2|28,1,2,1,2|30,2,1,2,2")]
	public static readonly string TreasureHuntStarCount;

	[ConfigProperty("TreasureHuntEventTools", "TreasureHuntEventTools", "20150,11165,11166,201520")]
	public static readonly string TreasureHuntEventTools;

	[ConfigProperty("TreasureHuntConfig", "TreasureHuntConfig", "5,50|10,1")]
	public static readonly string TreasureHuntConfig;

	[ConfigProperty("TreasureHuntDicePrice", "TreasureHuntDicePrice", "100,150,250,350,450,550,650,750,850,900")]
	public static readonly string TreasureHuntDicePrice;

	[ConfigProperty("TreasureHuntEnergyPrice", "TreasureHuntEnergyPrice", "50,150,250,350,500")]
	public static readonly string TreasureHuntEnergyPrice;

	[ConfigProperty("TreasureHuntDropPara", "TreasureHuntDropPara", 10021)]
	public static readonly int TreasureHuntDropPara;

	[ConfigProperty("TreasureHuntConvertCount", "TreasureHuntConvertCount", 30)]
	public static readonly int TreasureHuntConvertCount;

	[ConfigProperty("TreasureHuntEventAward", "TreasureHuntEventAward", 201487)]
	public static readonly int TreasureHuntEventAward;

	[ConfigProperty("TreasureHuntEndDate", "TreasureHuntEndDate", "2122/3/1 0:00:00")]
	public static readonly string TreasureHuntEndDate;

	[ConfigProperty("TreasureHuntNpc", "TreasureHuntNpc", "10015,4,8|10016,4,10")]
	public static readonly string TreasureHuntNpc;

	[ConfigProperty("HomeLvLimit", "HomeLvLimit", 25)]
	public static readonly int HomeLvLimit;

	[ConfigProperty("HomeTDHomeBlood", "HomeTDHomeBlood", 5)]
	public static readonly int HomeTDHomeBlood;

	[ConfigProperty("HomeTDRevivePrice", "HomeTDRevivePrice", 140)]
	public static readonly int HomeTDRevivePrice;

	[ConfigProperty("HomeBarDrinkNum", "HomeBarDrinkNum", 6)]
	public static readonly int HomeBarDrinkNum;

	[ConfigProperty("HomeBarDrinkPower", "HomeBarDrinkPower", 20)]
	public static readonly int HomeBarDrinkPower;

	[ConfigProperty("HomeBarPourPower", "HomeBarPourPower", 30)]
	public static readonly int HomeBarPourPower;

	[ConfigProperty("HomeBarTastePower", "HomeBarTastePower", 5)]
	public static readonly int HomeBarTastePower;

	[ConfigProperty("HomeBarCleanCDPrice", "HomeBarCleanCDPrice", 70)]
	public static readonly int HomeBarCleanCDPrice;

	[ConfigProperty("HomeBarPourBasePrice", "HomeBarPourBasePrice", 50)]
	public static readonly int HomeBarPourBasePrice;

	[ConfigProperty("HouseUpgradeUse", "HouseUpgradeUse", "1,11865,1,11867,150|2,11866,2,11867,500")]
	public static readonly string HouseUpgradeUse;

	[ConfigProperty("FishDailyOnlineTime", "FishDailyOnlineTime", 50)]
	public static readonly int FishDailyOnlineTime;

	[ConfigProperty("FishSecondRandom", "FishSecondRandom", "7|26,1|7,1|7,1|7,1|7,1|7")]
	public static readonly string FishSecondRandom;

	[ConfigProperty("FishOdds", "FishOdds", "45,45,45,45,45,45")]
	public static readonly string FishOdds;

	[ConfigProperty("FishBuyCost", "FishBuyCost", 50)]
	public static readonly int FishBuyCost;

	[ConfigProperty("FishDailyCount", "FishDailyCount", 30)]
	public static readonly int FishDailyCount;

	[ConfigProperty("FishLostRandom", "FishLostRandom", "0,0,5,10,20,25,0")]
	public static readonly string FishLostRandom;

	[ConfigProperty("RedGiftTime", "RedGiftTime", 1440)]
	public static readonly int RedGiftTime;

	[ConfigProperty("WealthTreeConfigList", "WealthTreeConfigList", "1120303,600,200|1120300,480,200|11963,120,200|1120302,240,200")]
	public static readonly string WealthTreeConfigList;

	[ConfigProperty("WealthTreeRedBagPro", "WealthTreeRedBagPro", "90,9,1")]
	public static readonly string WealthTreeRedBagPro;

	[ConfigProperty("DDPlayBeginDate", "DDPlayBeginDate", "2015/01/03 0:00:00")]
	public static readonly string DDPlayBeginDate;

	[ConfigProperty("DDPlayEndDate", "DDPlayEndDate", "2122/03/03 0:00:00")]
	public static readonly string DDPlayEndDate;

	[ConfigProperty("DDPlayMoney", "DDPlayMoney", 20000)]
	public static readonly int DDPlayMoney;

	[ConfigProperty("ExchangeFold", "ExchangeFold", 1000)]
	public static readonly int ExchangeFold;

	[ConfigProperty("CloudBuyLotteryEndDate", "CloudBuyLotteryEndDate", "2015/03/03 0:00:00")]
	public static readonly string CloudBuyLotteryEndDate;

	[ConfigProperty("CloudBuyLotteryExchangeScore", "CloudBuyLotteryExchangeScore", 50000)]
	public static readonly int CloudBuyLotteryExchangeScore;

	[ConfigProperty("DDTMoneyActive", "DDTMoneyActive", false)]
	public static readonly bool DDTMoneyActive;

	[ConfigProperty("GoldHour", "GoldHour", "20|22,2500")]
	public static readonly string GoldHour;

	[ConfigProperty("DisibleBonusMoneyHour", "DisibleBonusMoneyHour", "00|06")]
	public static readonly string DisibleBonusMoneyHour;

	[ConfigProperty("DoubleMoneyActive", "DoubleMoneyActive", false)]
	public static readonly bool DoubleMoneyActive;

	[ConfigProperty("DoubleMoneyRate", "DoubleMoneyRate", 2)]
	public static readonly int DoubleMoneyRate;

	[ConfigProperty("DoubleDDTMoneyActive", "DoubleDDTMoneyActive", false)]
	public static readonly bool DoubleDDTMoneyActive;

	[ConfigProperty("DoubleDDTMoneyRate", "DoubleDDTMoneyRate", 2)]
	public static readonly int DoubleDDTMoneyRate;

	[ConfigProperty("DoubleExpActive", "DoubleExpActive", false)]
	public static readonly bool DoubleExpActive;

	[ConfigProperty("DoubleExpRate", "DoubleExpRate", 2)]
	public static readonly int DoubleExpRate;

	[ConfigProperty("VirtualName", "VirtualName", "Doraemon,Shrek,WallE,Nobita,Robot,Chibi,Star,Eyeshield")]
	public static readonly string VirtualName;

	[ConfigProperty("RemoteEnable", "RemoteEnable", false)]
	public static readonly bool RemoteEnable;

	[ConfigProperty("NewbieEnable", "NewbieEnable", true)]
	public static readonly bool NewbieEnable;

	[ConfigProperty("BeginLevel", "BeginLevel", 40)]
	public static readonly int BeginLevel;

	[ConfigProperty("NeedBox1GoguAward", "NeedBox1GoguAward", 10)]
	public static readonly int NeedBox1GoguAward;

	[ConfigProperty("NeedBox2GoguAward", "NeedBox2GoguAward", 25)]
	public static readonly int NeedBox2GoguAward;

	[ConfigProperty("NeedBox3GoguAward", "NeedBox3GoguAward", 30)]
	public static readonly int NeedBox3GoguAward;

	[ConfigProperty("FindMinePrice", "FindMinePrice", 1000)]
	public static readonly int FindMinePrice;

	[ConfigProperty("RevivePrice", "RevivePrice", 2500)]
	public static readonly int RevivePrice;

	[ConfigProperty("ResetPrice", "ResetPrice", 5000)]
	public static readonly int ResetPrice;

	[ConfigProperty("resetCount", "resetCount", 5)]
	public static readonly int resetCount;

	[ConfigProperty("ItemDevelopPrice", "ItemDevelopPrice", 35)]
	public static readonly int ItemDevelopPrice;

	[ConfigProperty("IsMissionEnergyEnable", "IsMissionEnergyEnable", true)]
	public static readonly bool IsMissionEnergyEnable;

	[ConfigProperty("AlwaysShowNoticeUpdate", "AlwaysShowNoticeUpdate", false)]
	public static readonly bool AlwaysShowNoticeUpdate;

	[ConfigProperty("ToyMachinePrice", "ToyMachinePrice", "200|1800|1000|9500")]
	public static readonly string ToyMachinePrice;

	[ConfigProperty("AlwaysOpen1vs5", "AlwaysOpen1vs5", false)]
	public static readonly bool AlwaysOpen1vs5;

	[ConfigProperty("IsOpenPetScore", "IsOpenPetScore", true)]
	public static readonly bool IsOpenPetScore;

	[ConfigProperty("CardResetSoulValue", "CardResetSoulValue", 50)]
	public static readonly int CardResetSoulValue;

	[ConfigProperty("CardLockAttrMoney", "CardLockAttrMoney", "280|520|1800")]
	public static readonly string CardLockAttrMoney;

	[ConfigProperty("HighCardResetSoulValue", "HighCardResetSoulValue", 2000)]
	public static readonly int HighCardResetSoulValue;

	[ConfigProperty("HomeTempPracticeSmallLuck", "HomeTempPracticeSmallLuck", "100,1500")]
	public static readonly string HomeTempPracticeSmallLuck;

	[ConfigProperty("HomeTempPracticeBigLuck", "HomeTempPracticeBigLuck", "50,2000")]
	public static readonly string HomeTempPracticeBigLuck;

	[ConfigProperty("MemoryGameEndTime", "MemoryGameEndTime", "2117/2/16 0:00:00")]
	public static readonly string MemoryGameEndTime;

	[ConfigProperty("MemoryGameAwardBox", "MemoryGameAwardBox", 11204280)]
	public static readonly int MemoryGameAwardBox;

	[ConfigProperty("MemoryGameResetCount", "MemoryGameResetCount", 5)]
	public static readonly int MemoryGameResetCount;

	[ConfigProperty("SanXiaoStartTime", "SanXiaoStartTime", "2016/2/13 0:00:00")]
	public static readonly string SanXiaoStartTime;

	[ConfigProperty("SanXiaoEndTime", "SanXiaoEndTime", "2122/2/16 0:00:00")]
	public static readonly string SanXiaoEndTime;

	[ConfigProperty("SanXiaoResetCount", "SanXiaoResetCount", 5)]
	public static readonly int SanXiaoResetCount;

	[ConfigProperty("ThreeCleanAwardBox", "ThreeCleanAwardBox", 1120428)]
	public static readonly int ThreeCleanAwardBox;

	[ConfigProperty("ThreeCleanBuyCost", "ThreeCleanBuyCost", 100)]
	public static readonly int ThreeCleanBuyCost;

	[ConfigProperty("ThreeClearCrossCost", "ThreeClearCrossCost", 350)]
	public static readonly int ThreeClearCrossCost;

	[ConfigProperty("ThreeClearRectCost", "ThreeClearRectCost", 300)]
	public static readonly int ThreeClearRectCost;

	[ConfigProperty("ThreeClearColourCleanCost", "ThreeClearColourCleanCost", 500)]
	public static readonly int ThreeClearColourCleanCost;

	[ConfigProperty("ThreeClearColourChangeCost", "ThreeClearColourChangeCost", 350)]
	public static readonly int ThreeClearColourChangeCost;

	[ConfigProperty("ThreeClearCrossPoint", "ThreeClearCrossPoint", 25)]
	public static readonly int ThreeClearCrossPoint;

	[ConfigProperty("ThreeClearRectPoint", "ThreeClearRectPoint", 20)]
	public static readonly int ThreeClearRectPoint;

	[ConfigProperty("ThreeClearColourCleanPoint", "ThreeClearColourCleanPoint", 40)]
	public static readonly int ThreeClearColourCleanPoint;

	[ConfigProperty("ThreeClearColourChangePoint", "ThreeClearColourChangePoint", 25)]
	public static readonly int ThreeClearColourChangePoint;

	[ConfigProperty("RouletteOpenOrClose", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", true)]
	public static bool RouletteOpenOrClose;

	[ConfigProperty("RouletteRate", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", "1.2|1.5|1.7|1.9|2.0")]
	public static string RouletteRate;

	[ConfigProperty("MaxLevelResetCost", "MaxLevelResetCost", 1000)]
	public static readonly int MaxLevelResetCost;

	[ConfigProperty("MaxLevelAllResetCost", "MaxLevelAllResetCost", 10000)]
	public static readonly int MaxLevelAllResetCost;

	[ConfigProperty("FightSpiritSwitch", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", true)]
	public static bool FightSpiritSwitch;

	[ConfigProperty("FightSpiritMaxLevel", "FightSpiritMaxLevel", 6)]
	public static readonly int FightSpiritMaxLevel;

	[ConfigProperty("FightSpiritLevelAddDamage", "FightSpiritLevelAddDamage", "6,2|12,4")]
	public static readonly string FightSpiritLevelAddDamage;

	[ConfigProperty("EventBombKingWeek", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", false)]
	public static bool EventBombKingWeek;

	[ConfigProperty("MaxWellSignBuff", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", 50)]
	public static int MaxWellSignBuff;

	[ConfigProperty("FightPowerSignData", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", "100000|500000|1000000|2000000|5000000|10000000")]
	public static string FightPowerSignData;

	[ConfigProperty("PairUpCardCost", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", 100)]
	public static int PairUpCardCost;

	[ConfigProperty("PairUpResetCost", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", 150)]
	public static int PairUpResetCost;

	[ConfigProperty("BeautyVoteCost", "BeautyVoteCost", 300)]
	public static readonly int BeautyVoteCost;

	[ConfigProperty("GuildBattleStartTime", "GuildBattleStartTime", "2016/1/1 20:00:00")]
	public static readonly string GuildBattleStartTime;

	[ConfigProperty("GuildBattleStartDay", "GuildBattleStartDay", "Saturday")]
	public static readonly string GuildBattleStartDay;

	[ConfigProperty("LotteryPoolCountDefault", "LotteryPoolCountDefault", 1000000)]
	public static readonly int LotteryPoolCountDefault;

	[ConfigProperty("LotteryTicketEndTime", "LotteryTicketEndTime", "2122/1/1 20:00:00")]
	public static readonly string LotteryTicketEndTime;

	[ConfigProperty("LotteryTicketCloseTicketTime", "LotteryTicketCloseTicketTime", "20:50:00")]
	public static readonly string LotteryTicketCloseTicketTime;

	[ConfigProperty("LotteryTicketShowPoolTime", "LotteryTicketShowPoolTime", "21:00:00")]
	public static readonly string LotteryTicketShowPoolTime;

	[ConfigProperty("LotteryTicketPrice", "LotteryTicketPrice", 100)]
	public static readonly int LotteryTicketPrice;

	[ConfigProperty("AmuletBuyDustCountAndNeedMoney", "AmuletBuyDustCountAndNeedMoney", "30|150|150")]
	public static readonly string AmuletBuyDustCountAndNeedMoney;

	[ConfigProperty("MountAmuletOpenLvl", "MountAmuletOpenLvl", "1,0|2,0|3,3|4,6|5,9|6,12|7,15|8,18|9,21")]
	public static readonly string MountAmuletOpenLvl;

	[ConfigProperty("AmuletBuyDustMax", "AmuletBuyDustMax", 20)]
	public static readonly int AmuletBuyDustMax;

	[ConfigProperty("GodCardOpenOneTimeMoney", "GodCardOpenOneTimeMoney", 100)]
	public static readonly int GodCardOpenOneTimeMoney;

	[ConfigProperty("GodCardOpenFiveTimeMoney", "GodCardOpenFiveTimeMoney", 500)]
	public static readonly int GodCardOpenFiveTimeMoney;

	[ConfigProperty("ConsortiaRichesPlus", "ConsortiaRichesPlus", 2)]
	public static readonly int ConsortiaRichesPlus;

	[ConfigProperty("ChickOrAntTreasureTimes", "ChickOrAntTreasureTimes", 5)]
	public static readonly int ChickOrAntTreasureTimes;

	[ConfigProperty("CityBattleBeginTime", "CityBattleBeginTime", "1990/01/01 08:00:00")]
	public static readonly string CityBattleBeginTime;

	[ConfigProperty("CityBattleEndTime", "CityBattleEndTime", "1990/01/01 22:00:00")]
	public static readonly string CityBattleEndTime;

	[ConfigProperty("CityOccupationAddPrice", "CityOccupationAddPrice", "10,1000|50,4500|200,8000|500,14000")]
	public static readonly string CityOccupationAddPrice;

	[ConfigProperty("TrailEliteAwardLevel", "TrailEliteAwardLevel", "50501|1120819,50502|1120820,50503|1120821,50504|1120822,50505|1120823")]
	public static readonly string TrailEliteAwardLevel;

	[ConfigProperty("QiYuanEndTime", "QiYuanEndTime", "2119/2/16 0:00:00")]
	public static readonly string QiYuanEndTime;

	[ConfigProperty("QiYuanJoinLeastOfferTimes", "Luot dang toi thieu de nhan qua tham gia dang", 10)]
	public static readonly int QiYuanJoinLeastOfferTimes;

	[ConfigProperty("QiYuanRankLeastOfferTimes", "Luot dang it nhat de duoc vao bxh", 600)]
	public static readonly int QiYuanRankLeastOfferTimes;

	[ConfigProperty("QiYuanOfferTimesPerBaoZhu", "So chuc phuc toi da se reset", 2000)]
	public static readonly int QiYuanOfferTimesPerBaoZhu;

	[ConfigProperty("QiYuanOfferTimesPerTreasureBox", "Luot dang toan sv toi thieu de duoc 1 ruong kho bau", 10)]
	public static readonly int QiYuanOfferTimesPerTreasureBox;

	[ConfigProperty("DdtLuckWorshipMoney", "DdtLuckWorshipMoney", "100|900")]
	public static readonly string DdtLuckWorshipMoney;

	[ConfigProperty("DdtLuckOpenBoxMoney", "DdtLuckOpenBoxMoney", 350)]
	public static readonly int DdtLuckOpenBoxMoney;

	[ConfigProperty("OnlineArmCostEnergy", "OnlineArmCostEnergy", 3)]
	public static readonly int OnlineArmCostEnergy;

	[ConfigProperty("NormalDungeonCostEnergy", "NormalDungeonCostEnergy", 10)]
	public static readonly int NormalDungeonCostEnergy;

	[ConfigProperty("ElementDungeonCostEnergy", "ElementDungeonCostEnergy", 20)]
	public static readonly int ElementDungeonCostEnergy;

	[ConfigProperty("OnlineArmRefreshItemTime", "OnlineArmRefreshItemTime", "1,2,3,4,5,6,7|9:0:0")]
	public static readonly string OnlineArmRefreshItemTime;

	[ConfigProperty("OnlineArmPerTimeDig", "OnlineArmPerTimeDig", 5)]
	public static readonly int OnlineArmPerTimeDig;

	[ConfigProperty("BlessBuffRefreshPrice", "BlessBuffRefreshPrice", 100)]
	public static readonly int BlessBuffRefreshPrice;

	[ConfigProperty("BlessBuffAddBuffPrice", "BlessBuffAddBuffPrice", 500)]
	public static readonly int BlessBuffAddBuffPrice;

	[ConfigProperty("BlessBuffAddRate", "BlessBuffAddRate", 20)]
	public static readonly int BlessBuffAddRate;

	[ConfigProperty("VIPLevelGiftID", "VIPLevelGiftID", "0|1120965|1120966|1120967|1120968|1120969|1120970|1120971|1120972|1120973|1120974|1120975|1120976|1120977|1120978")]
	public static readonly string VIPLevelGiftID;

	[ConfigProperty("VIPSendFireRed", "VIPSendFireRed", "0|0|0|0|0|0|0|0|0|0|0|0|1120995|1120996|1120997")]
	public static readonly string VIPSendFireRed;

	[ConfigProperty("ghostlevel", "ghostlevel", "0|0|0|0|0|0|0|0|0|0")]
	public static readonly string ghostlevel;

	[ConfigProperty("TrailEliteWinScore", "TrailEliteWinScore", 15)]
	public static readonly int TrailEliteWinScore;

	[ConfigProperty("TrailEliteLostScore", "TrailEliteLostScore", 5)]
	public static readonly int TrailEliteLostScore;

	[ConfigProperty("EngraveVaultsConfig", "EngraveVaultsConfig", "1,1000,1|10,9000,10")]
	public static readonly string EngraveVaultsConfig;

	[ConfigProperty("EngraveVaultsTime", "EngraveVaultsTime", 10)]
	public static readonly int EngraveVaultsTime;

	[ConfigProperty("EngraveVaultsFreeTimes", "EngraveVaultsFreeTimes", 5)]
	public static readonly int EngraveVaultsFreeTimes;

	[ConfigProperty("EngraveLimitLevel", "EngraveLimitLevel", 48)]
	public static readonly int EngraveLimitLevel;

	[ConfigProperty("EngraveSaleStarConfig", "EngraveSaleStarConfig", 10)]
	public static readonly int EngraveSaleStarConfig;

	[ConfigProperty("EngraveSaleTemperConsumeConfig", "EngraveSaleTemperConsumeConfig", 10)]
	public static readonly int EngraveSaleTemperConsumeConfig;

	[ConfigProperty("contasPorIp", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", 2)]
	public static int contasPorIp;

	[ConfigProperty("batismoNoPvp", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", true)]
	public static bool batismoNoPvp;

	[ConfigProperty("batismoNoDesafio", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", true)]
	public static bool batismoNoDesafio;

	[ConfigProperty("SealItems", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", "5,5,50")]
	public static string SealItems;

	[ConfigProperty("SealLockItems", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", "1,15|2,45|3,75|4,115")]
	public static string SealLockItems;

	[ConfigProperty("AdvSealLockItems", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", "1,50|2,150|3,250|4,350")]
	public static string AdvSealLockItems;

	[ConfigProperty("reduzirFC", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", true)]
	public static bool reduzirFC;

	[ConfigProperty("servidor", "", "default")]
	public static string servidor;

	[ConfigProperty("expNoPvp", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", true)]
	public static bool expNoPvp;

	[ConfigProperty("ExaltRatewith15", "ExaltRatewith15", 333333)]
	public static int ExaltRatewith15;

	[ConfigProperty("ExaltRatewith16", "ExaltRatewith16", 2500000)]
	public static int ExaltRatewith16;

	[ConfigProperty("ExaltRatewith17", "ExaltRatewith17", 6000000)]
	public static int ExaltRatewith17;

	[ConfigProperty("HappyRechargeOpenClose", "HappyRechargeOpenClose", true)]
	public static bool HappyRechargeOpenClose;

	[ConfigProperty("HappyRechargeBeginDate", "HappyRechargeBeginDate", "2013/01/01 0:00:00")]
	public static string HappyRechargeBeginDate;

	[ConfigProperty("HappyRechargeEndDate", "HappyRechargeEndDate", "2129/01/01 0:00:00")]
	public static string HappyRechargeEndDate;

	[ConfigProperty("ShopVipScore", "ShopVipScore", "500|600|700|800|1000|1200|1400|1600|1800|2800|3800|4800|5800|6800|7800")]
	public static readonly string ShopVipScore;

	[ConfigProperty("CardGrooveRevert", "CardGrooveRevert", 300)]
	public static int CardGrooveRevert;

	[ConfigProperty("EmblemComposeRandom", "EmblemComposeRandom", "500,600")]
	public static string EmblemComposeRandom;

	[ConfigProperty("ElfFourItems", "ElfFourItems", "1123667,1|1123670,1|1122190,1|1122193,1|1122196,1|1122199,1|1122189,1|1122192,1|1122195,1|1122198,1|1122188,1|1122191,1|1122194,1|1122197,1|12246,1|12245,1|12244,1|12243,1|12247,1|1122253,1")]
	public static readonly string ElfFourItems;

	[ConfigProperty("ElfThreeItems", "ElfThreeItems", "1123666,1|1123669,1|1122189,1|1122192,1|1122195,1|1122198,1|1122188,1|1122191,1|1122194,1|1122197,1|12246,1|12245,1|12244,1|12243,1|12247,1|1122253,1")]
	public static readonly string ElfThreeItems;

	[ConfigProperty("ElfBuyItemMoneys", "ElfBuyItemMoneys", "180,1600|800,7200")]
	public static readonly string ElfBuyItemMoneys;

	[ConfigProperty("ElfIntimacyDayMax", "ElfIntimacyDayMax", "10,30|10,30|10,20")]
	public static readonly string ElfIntimacyDayMax;

	[ConfigProperty("ElfLockSkillPayMoney", "ElfLockSkillPayMoney", "200,500,1500,3000,5000,8500,12000")]
	public static readonly string ElfLockSkillPayMoney;

	[ConfigProperty("ElfInitSkillRandoms", "ElfInitSkillRandoms", "500,300,0,0|0,300,200,0|0,0,200,50")]
	public static readonly string ElfInitSkillRandoms;

	[ConfigProperty("ElfLearnSkillRandoms", "ElfLearnSkillRandoms", "100,50,30,20,0")]
	public static readonly string ElfLearnSkillRandoms;

	[ConfigProperty("DevilTreasureOneCost", "DevilTreasureOneCost", 100)]
	public static readonly int DevilTreasureOneCost;

	[ConfigProperty("DevilTreasureTenCost", "DevilTreasureTenCost", 900)]
	public static readonly int DevilTreasureTenCost;

	[ConfigProperty("DevilTreasureFreeLotteryCount", "DevilTreasureFreeLotteryCount", 10)]
	public static readonly int DevilTreasureFreeLotteryCount;

	[ConfigProperty("DevilTreasureFreeLotteryCD", "DevilTreasureFreeLotteryCD", 600)]
	public static readonly int DevilTreasureFreeLotteryCD;

	[ConfigProperty("DevilTreasurePrizePool", "DevilTreasurePrizePool", 50000)]
	public static readonly int DevilTreasurePrizePool;

	[ConfigProperty("DevilTreasurePrizePoolMax", "DevilTreasurePrizePoolMax", 100000)]
	public static readonly int DevilTreasurePrizePoolMax;

	[ConfigProperty("DevilTreasureRankCount", "DevilTreasureRankCount", 2000)]
	public static readonly int DevilTreasureRankCount;

	[ConfigProperty("DevilTreasureBoxExpriyMinutes", "DevilTreasureBoxExpriyMinutes", 240)]
	public static readonly int DevilTreasureBoxExpriyMinutes;

	[ConfigProperty("DevilTreasureTemplateID", "DevilTreasureTemplateID", 11169)]
	public static readonly int DevilTreasureTemplateID;

	[ConfigProperty("DevilTreasureBeginDate", "DevilTreasureBeginDate", "2020/4/26 10:00:00")]
	public static readonly string DevilTreasureBeginDate;

	[ConfigProperty("DevilTreasureEndDate", "DevilTreasureEndDate", "2050/4/26 23:59:59")]
	public static readonly string DevilTreasureEndDate;

	[ConfigProperty("DevilTreasureCfgBox", "DevilTreasureCfgBox", "12711|12712|12713")]
	public static readonly string DevilTreasureCfgBox;

	[ConfigProperty("CreateTeamMoney", "CreateTeamMoney", 3000)]
	public static readonly int CreateTeamMoney;

	[ConfigProperty("MoneyRichesOffer", "MoneyRichesOffer", 10)]
	public static readonly int MoneyRichesOffer;

	[ConfigProperty("AcademyMasterFreezeHours", "AcademyMasterFreezeHours", 48)]
	public static readonly int AcademyMasterFreezeHours;

	[ConfigProperty("AcademyApprenticeFreezeHours", "AcademyApprenticeFreezeHours", 24)]
	public static readonly int AcademyApprenticeFreezeHours;

	[ConfigProperty("EntertainmentScore", "EntertainmentScore", "75|201252,125|201253,250|201254,375|201255,500|201256,625|201257,750|201258,875|201259,1000|201260,1125|201261")]
	public static readonly string EntertainmentScore;

	public static List<int[]> GetEntertainmentScoreReward()
	{
		List<int[]> list = new List<int[]>();
		string[] array = EntertainmentScore.Split(',');
		foreach (string text in array)
		{
			string[] array2 = text.Split('|');
			List<int> list2 = new List<int>();
			string[] array3 = array2;
			foreach (string s in array3)
			{
				list2.Add(int.Parse(s));
			}
			list.Add(list2.ToArray());
		}
		return list;
	}

	private static void Load(Type type)
	{
		using ServiceBussiness sb = new ServiceBussiness();
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsStatic)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), inherit: false);
				if (customAttributes.Length != 0)
				{
					ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
					fieldInfo.SetValue(null, LoadProperty(attrib, sb));
				}
			}
		}
	}

	private static void Save(Type type)
	{
		using ServiceBussiness sb = new ServiceBussiness();
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsStatic)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), inherit: false);
				if (customAttributes.Length != 0)
				{
					ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
					SaveProperty(attrib, sb, fieldInfo.GetValue(null));
				}
			}
		}
	}

	public static Dictionary<int, int> ConvertIntDict(string value, char splitChar, char subSplitChar)
	{
		string[] array = value.Split(splitChar);
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(subSplitChar);
			if (!dictionary.ContainsKey(int.Parse(array2[0])))
			{
				dictionary.Add(int.Parse(array2[0]), int.Parse(array2[1]));
			}
		}
		return dictionary;
	}

	private static object LoadProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb)
	{
		string key = attrib.Key;
		ServerProperty serverProperty = sb.GetServerPropertyByKey(key);
		if (serverProperty == null)
		{
			serverProperty = new ServerProperty();
			serverProperty.Name = key;
			serverProperty.Value = attrib.DefaultValue.ToString();
			log.Info("Cannot find server property " + key + ",keep it default value!");
			SaveProperty(attrib, sb, serverProperty.Value);
		}
		try
		{
			return Convert.ChangeType(serverProperty.Value, attrib.DefaultValue.GetType());
		}
		catch (Exception exception)
		{
			log.Error("Exception in GameProperties Load: ", exception);
			return null;
		}
	}

	private static void SaveProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb, object value)
	{
		try
		{
			sb.UpdateServerPropertyByKey(attrib.Key, value.ToString());
		}
		catch (Exception exception)
		{
			log.Error("Exception in GameProperties Save: ", exception);
		}
	}

	public static void Refresh()
	{
		log.Info("Refreshing game properties!");
		Load(typeof(GameProperties));
	}

	public static List<int> getProp(string prop)
	{
		List<int> list = new List<int>();
		string[] array = prop.Split('|');
		string[] array2 = array;
		foreach (string value in array2)
		{
			list.Add(Convert.ToInt32(value));
		}
		return list;
	}

	public static int LimitLevel(int index)
	{
		string[] array = CustomLimit.Split('|');
		return Convert.ToInt32(array[index]);
	}

	public static bool ShortFoldDress()
	{
		switch (VERSION)
		{
		case 7600:
		case 7700:
		case 7900:
		case 8100:
			return false;
		default:
			return true;
		}
	}

	public static List<int> VIPExp()
	{
		return getProp(VIPExpForEachLv);
	}

	public static List<int> VIPLevelGiftList()
	{
		return getProp(VIPLevelGiftID);
	}

	public static List<int> VIPSendFireRedList()
	{
		return getProp(VIPSendFireRed);
	}

	public static List<int> RuneExp()
	{
		return getProp(RuneLevelUpExp);
	}

	public static int ConsortiaStrengExp(int Lv)
	{
		return getProp(ConsortiaStrengthenEx)[Lv];
	}

	public static int VIPStrengthenExp(int vipLv)
	{
		return getProp(VIPStrengthenEx)[vipLv];
	}

	public static int HoleLevelUpExp(int lv)
	{
		return getProp(HoleLevelUpExpList)[lv];
	}

	public static List<int[]> EngraveVaults()
	{
		List<int[]> list = new List<int[]>();
		string[] array = EngraveVaultsConfig.Split('|');
		foreach (string text in array)
		{
			string[] array2 = text.Split(',');
			List<int> list2 = new List<int>();
			string[] array3 = array2;
			foreach (string s in array3)
			{
				list2.Add(int.Parse(s));
			}
			list.Add(list2.ToArray());
		}
		return list;
	}

	public static int[] ConvertStringArrayToIntArray(string str)
	{
		List<int> list = new List<int>();
		string[] array = new string[3] { "99999", "999999", "9999999" };
		switch (str)
		{
		case "NewChickenEagleEyePrice":
			array = NewChickenEagleEyePrice.Split(',');
			break;
		case "NewChickenOpenCardPrice":
			array = NewChickenOpenCardPrice.Split(',');
			break;
		case "PyramidRevivePrice":
			array = PyramidRevivePrice.Split(',');
			break;
		}
		string[] array2 = array;
		foreach (string value in array2)
		{
			list.Add(Convert.ToInt32(value));
		}
		return list.ToArray();
	}

	public static void Save()
	{
		log.Info("Saving game properties into db!");
	}
}
