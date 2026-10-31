using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(95, "项链炼化")]
public class NecklaceStrengthHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (packet.ReadByte() == 1)
		{
			NecklaceStrengthMgr.UpdateNecklaceInfo(client.Player);
			NecklaceStrengthMgr.SendNecklaceCastInfo(client.Player);
		}
		else
		{
			List<ItemInfo> allItemByTemplateID = client.Player.PropBag.GetAllItemByTemplateID(0, 11160);
			if (allItemByTemplateID.Count <= 0)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.NoNormalItem"));
				return 0;
			}
			int necklaceLevelByGP = NecklaceStrengthMgr.GetNecklaceLevelByGP(client.Player.PlayerCharacter.necklaceExp);
			if (necklaceLevelByGP >= StrengthenMgr.NECKLACE_MAX_LEVEL)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.MaxNormalLv"));
				return 0;
			}
			int num = NecklaceStrengthMgr.FindStrengThenExpInfo(necklaceLevelByGP + 1).NecklaceStrengthExp - client.Player.PlayerCharacter.necklaceExp;
			int num2 = (int)Math.Ceiling((float)num / (float)allItemByTemplateID.First().Template.Property2);
			bool flag = true;
			if (num2 > allItemByTemplateID.Select((ItemInfo e) => e.Count).Sum())
			{
				num2 = allItemByTemplateID.Select((ItemInfo e) => e.Count).Sum();
				num = num2 * allItemByTemplateID.First().Template.Property2;
				flag = false;
			}
			if (!client.Player.RemoveTemplate(11160, num2))
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("NecklaceStrength.WTF"));
				return 0;
			}
			client.Player.PlayerCharacter.necklaceExp += num;
			if (flag)
			{
				client.Player.EquipBag.UpdatePlayerProperties();
			}
			NecklaceStrengthMgr.UpdateNecklaceInfo(client.Player);
		}
		return 0;
	}
}
