using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.HomeTemple.Handle;

[HomeTempleHandleAttbute(3)]
public class Promote : IHomeTempleCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.Actives.Info == null)
		{
			return 0;
		}
		TS_HomeTempAdvance_Template tS_HomeTempAdvance_Template = HomeTempleMgr.TS_HomeTempAdvance_Template.FirstOrDefault((TS_HomeTempAdvance_Template p) => p.Level == player.Actives.Info.promoteLevel);
		if (tS_HomeTempAdvance_Template == null)
		{
			throw new NullReferenceException($"nextlv is null {player.Actives.Info.promoteLevel}");
		}
		if (tS_HomeTempAdvance_Template.Count1 == 0 && tS_HomeTempAdvance_Template.Count2 == 0)
		{
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("NecklaceStrength.MaxLevel"));
			return 0;
		}
		List<ItemInfo> allItemByTemplateID = player.PropBag.GetAllItemByTemplateID(0, 11177);
		if (allItemByTemplateID.Select((ItemInfo e) => e.Count).Sum() < tS_HomeTempAdvance_Template.Count1)
		{
			player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.NoNormalItem"));
			return 0;
		}
		List<ItemInfo> allItemByTemplateID2 = player.PropBag.GetAllItemByTemplateID(0, 12248);
		if (allItemByTemplateID2.Select((ItemInfo e) => e.Count).Sum() < tS_HomeTempAdvance_Template.Count2)
		{
			player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.NoAffinazhItem"));
			return 0;
		}
		if (!player.RemoveTemplate(11177, tS_HomeTempAdvance_Template.Count1))
		{
			player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.WTF1"));
			return 0;
		}
		if (!player.RemoveTemplate(12248, tS_HomeTempAdvance_Template.Count2))
		{
			player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.WTF2"));
			return 0;
		}
		player.Actives.Info.promoteLevel = tS_HomeTempAdvance_Template.Level + 1;
		player.EquipBag.UpdatePlayerProperties();
		HomeTempleMgr.Out.SendHomeTemplePromote(player);
		HomeTempleMgr.Out.SendHomeTempleInfo(player);
		return 1;
	}
}
