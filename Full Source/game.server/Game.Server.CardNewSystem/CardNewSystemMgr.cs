using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.CardNewSystem;

public static class CardNewSystemMgr
{
	private static readonly ServerData db = new ServerData();

	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private const int DefaultCardType = 5;

	public static List<TS_CardBooklet> CardBooklets = db.TS_CardBooklet.ToList();

	public static List<TS_CardMain> CardMains = db.TS_CardMain.ToList();

	public static List<TS_CardSuit> CardSuits = db.TS_CardSuit.ToList();

	public static CardNewSystemPacketsOut Out = new CardNewSystemPacketsOut();

	public static List<TS_UpgradeTemplate> UpgradeTemplate = db.TS_UpgradeTemplate.ToList();

	public static bool Init()
	{
		try
		{
			CardBooklets = db.TS_CardBooklet.ToList();
			CardSuits = db.TS_CardSuit.ToList();
			UpgradeTemplate = db.TS_UpgradeTemplate.ToList();
			CardMains = db.TS_CardMain.ToList();
			return true;
		}
		catch (Exception arg)
		{
			Log.ErrorFormat("CardNewSystemMgr.init error: {0}", arg);
			return false;
		}
	}

	public static void GetSuitValue(GamePlayer player, ref int hp, ref int attack, ref int def, ref int agi, ref int luck, ref int mgattack, ref int mgdef, ref int basedame, ref int basedef, bool addPercent = true)
	{
		List<UsersCardInfo> list = player.CardBag.GetCards(5, 100).ToList();
		foreach (UsersCardInfo card in list)
		{
			int profile = 5 - card.CardType;
			TS_CardBooklet tS_CardBooklet = CardBooklets.FirstOrDefault((TS_CardBooklet p) => p.TemplateId == card.TemplateID && p.Profile == profile);
			if (tS_CardBooklet == null)
			{
				throw new NullReferenceException($"Can not found cardbooklet with templateId = {card.TemplateID} and profile = {profile}");
			}
			hp += tS_CardBooklet.Hp;
			attack += tS_CardBooklet.Attack;
			def += tS_CardBooklet.Defence;
			agi += tS_CardBooklet.Agility;
			luck += tS_CardBooklet.Lucky;
			mgattack += tS_CardBooklet.MagicAttack;
			mgdef += tS_CardBooklet.MagicDefence;
			basedame += tS_CardBooklet.Damage;
			basedef += tS_CardBooklet.Armor;
		}
		Sys_Users_CardActive cardActive = player.PlayerCharacter.ActiveCards.FirstOrDefault((Sys_Users_CardActive p) => p.IsActive);
		if (cardActive != null)
		{
			TS_CardSuit tS_CardSuit = CardSuits.FirstOrDefault((TS_CardSuit p) => p.SuitTemplateId == cardActive.MainID);
			if (tS_CardSuit == null)
			{
				throw new NullReferenceException($"Can not found suitCard with SuitTemplateId = {cardActive.MainID}");
			}
			List<int> listIDs = (from p in tS_CardSuit.NeedCardTempIds.Split(',')
				select int.Parse(p)).ToList();
			int num = 5 - (from p in list
				where listIDs.Contains(p.TemplateID)
				orderby p.CardType
				select p.CardType).FirstOrDefault();
			int num2 = int.Parse(tS_CardSuit.AddPercents.Split('|')[num - 1]);
			hp += tS_CardSuit.Hp + tS_CardSuit.Hp * num2 / 1000;
			attack += tS_CardSuit.Attack + tS_CardSuit.Attack * num2 / 1000;
			def += tS_CardSuit.Defence + tS_CardSuit.Defence * num2 / 1000;
			agi += tS_CardSuit.Agility + tS_CardSuit.Agility * num2 / 1000;
			luck += tS_CardSuit.Lucky + tS_CardSuit.Lucky * num2 / 1000;
			mgattack += tS_CardSuit.MagicAttack + tS_CardSuit.MagicAttack * num2 / 1000;
			mgdef += tS_CardSuit.MagicDefence + tS_CardSuit.MagicDefence * num2 / 1000;
			basedame += tS_CardSuit.Damage + tS_CardSuit.Damage * num2 / 1000;
			basedef += tS_CardSuit.Armor + tS_CardSuit.Armor * num2 / 1000;
		}
		if (addPercent)
		{
			TS_UpgradeTemplate tS_UpgradeTemplate = UpgradeTemplate.FirstOrDefault((TS_UpgradeTemplate p) => p.Grades == player.PlayerCharacter.CardInfo.SuitLevel && p.Types == 105);
			if (tS_UpgradeTemplate == null)
			{
				throw new NullReferenceException($"Can not found upgradeTemplate with Grades = {player.PlayerCharacter.CardInfo.SuitLevel}");
			}
			int data = tS_UpgradeTemplate.Data;
			hp += data * hp / 1000;
			attack += data * attack / 1000;
			def += data * def / 1000;
			agi += data * agi / 1000;
			luck += data * luck / 1000;
			mgattack += data * mgattack / 1000;
			mgdef += data * mgdef / 1000;
			basedame += data * basedame / 1000;
			basedef += data * basedef / 1000;
		}
	}
}
