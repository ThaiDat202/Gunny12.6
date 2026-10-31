using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(661, "项链精铸")]
public class NecklaceCastHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		TS_NecklaceCasting tS_NecklaceCasting = NecklaceStrengthMgr.ts_NecklaceCasting.FirstOrDefault((TS_NecklaceCasting p) => p.Level == client.Player.PlayerCharacter.necklaceCastLevel);
		if (tS_NecklaceCasting.NeedItemCount1 == 0 && tS_NecklaceCasting.NeedItemCount2 == 0)
		{
			client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("NecklaceStrength.MaxLevel"));
			return 0;
		}
		List<ItemInfo> allItemByTemplateID = client.Player.PropBag.GetAllItemByTemplateID(0, 11160);
		if (allItemByTemplateID.Select((ItemInfo e) => e.Count).Sum() < tS_NecklaceCasting.NeedItemCount1)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.NoNormalItem"));
			return 0;
		}
		List<ItemInfo> allItemByTemplateID2 = client.Player.PropBag.GetAllItemByTemplateID(0, 12240);
		if (allItemByTemplateID2.Select((ItemInfo e) => e.Count).Sum() < tS_NecklaceCasting.NeedItemCount2)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.NoAffinazhItem"));
			return 0;
		}
		if (!client.Player.RemoveTemplate(11160, tS_NecklaceCasting.NeedItemCount1))
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.WTF1"));
			return 0;
		}
		if (!client.Player.RemoveTemplate(12240, tS_NecklaceCasting.NeedItemCount2))
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.WTF2"));
			return 0;
		}
		client.Player.PlayerCharacter.necklaceCastLevel = tS_NecklaceCasting.Level + 1;
		NecklaceStrengthMgr.SendNecklaceCastInfo(client.Player);
		client.Player.EquipBag.UpdatePlayerProperties();
		return 0;
	}
}
