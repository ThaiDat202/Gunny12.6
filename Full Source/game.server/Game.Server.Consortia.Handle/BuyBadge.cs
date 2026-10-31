using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(28)]
public class BuyBadge : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		bool flag = false;
		int num = packet.ReadInt();
		ConsortiaBadgeConfigInfo consortiaBadgeConfigInfo = ConsortiaExtraMgr.FindConsortiaBadgeConfig(num);
		if (consortiaBadgeConfigInfo == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("BuyBadge.BadgeNotFound"));
			return 0;
		}
		string msg = "BuyBadgeHandler.Fail";
		int validDate = 30;
		string badgeBuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaInfo consortiaSingle = consortiaBussiness.GetConsortiaSingle(Player.PlayerCharacter.ConsortiaID);
			if (consortiaSingle == null)
			{
				return 0;
			}
			if (consortiaSingle.Riches < consortiaBadgeConfigInfo.Cost)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("ConsortiaBussiness.Riches.Msg3"));
				return 0;
			}
			consortiaSingle.BadgeID = num;
			consortiaSingle.ValidDate = validDate;
			consortiaSingle.BadgeBuyTime = badgeBuyTime;
			if (consortiaBussiness.BuyBadge(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, consortiaSingle, ref msg))
			{
				msg = "BuyBadgeHandler.Success";
				flag = true;
			}
		}
		if (flag)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(Player.PlayerCharacter.ConsortiaID);
			ConsortiaUserInfo[] array = allMemberByConsortia;
			foreach (ConsortiaUserInfo consortiaUserInfo in array)
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(consortiaUserInfo.UserID);
				if (playerById != null && playerById.PlayerId != Player.PlayerCharacter.ID)
				{
					playerById.UpdateBadgeId(num);
					playerById.SendMessage(LanguageMgr.GetTranslation("Consortia.Msg7"));
					playerById.UpdateProperties();
				}
			}
		}
		Player.Out.sendBuyBadge(Player.PlayerCharacter.ConsortiaID, num, validDate, flag, badgeBuyTime, Player.PlayerCharacter.ID);
		Player.SendMessage(LanguageMgr.GetTranslation(msg));
		Player.UpdateBadgeId(num);
		Player.UpdateProperties();
		if (flag)
		{
			using ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness();
			consortiaBussiness2.UpdateConsortiaRiches(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, consortiaBadgeConfigInfo.Cost, ref msg);
		}
		return 0;
	}
}
