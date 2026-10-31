using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem.Handle;

[EmblemHandle(4)]
public class Forgive : IEmblemCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int scrollId = packet.ReadInt();
		int emblemId = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		TS_Scroll scroll = EmblemMgr.Scrolls.FirstOrDefault((TS_Scroll p) => p.TemplateID == scrollId);
		if (scroll == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("Emblem.Forgive.ScrollNotFound"));
			return 0;
		}
		Sys_Users_Scroll sys_Users_Scroll = player.ScrollInfos.FirstOrDefault((Sys_Users_Scroll p) => p.TemplateId == scrollId && p.Count > 0);
		if (sys_Users_Scroll == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("Emblem.Forgive.ItemNotFound"));
			return 0;
		}
		TS_Emblem tS_Emblem = EmblemMgr.Emblems.FirstOrDefault((TS_Emblem p) => p.TemplateID == emblemId && p.Profile == scroll.Profile);
		if (tS_Emblem == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("Emblem.Forgive.EmblemNotFound"));
			return 0;
		}
		if (!player.PropBag.RemovePropItem(player, tS_Emblem.NeedItem1, tS_Emblem.ItemCount1) || !player.PropBag.RemovePropItem(player, tS_Emblem.NeedItem2, tS_Emblem.ItemCount2) || !player.PropBag.RemovePropItem(player, tS_Emblem.NeedItem3, tS_Emblem.ItemCount3))
		{
			player.SendMessage(LanguageMgr.GetTranslation("Emblem.Forgive.CanNotRemoveItems"));
			return 0;
		}
		sys_Users_Scroll.Count--;
		EmblemMgr.Out.SendUpdateScroll(player);
		int id = EmblemMgr.AddItem(player, tS_Emblem.TemplateID);
		EmblemMgr.Out.SendForgiveSuccess(player, id);
		player.SendMessage(LanguageMgr.GetTranslation("Emblem.Forgive.Success"));
		return 1;
	}
}
