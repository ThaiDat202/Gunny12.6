using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem.Handle;

[EmblemHandle(7)]
public class Sale : IEmblemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		Sys_Users_Emblem emblemInfo = Player.EmblemInfos.FirstOrDefault((Sys_Users_Emblem p) => p.ID == id);
		if (emblemInfo == null)
		{
			return 0;
		}
		TS_Emblem tS_Emblem = EmblemMgr.Emblems.FirstOrDefault((TS_Emblem p) => p.TemplateID == emblemInfo.tempId);
		if (tS_Emblem == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Emblem.Sale.Failed"));
			return 0;
		}
		emblemInfo.isExist = false;
		EmblemMgr.Out.SendUpdateEmblem(Player, emblemInfo);
		Player.AddCoin(tS_Emblem.SaleValue);
		Player.SendMessage(LanguageMgr.GetTranslation("Emblem.Sale.Success", tS_Emblem.SaleValue));
		return 0;
	}
}
