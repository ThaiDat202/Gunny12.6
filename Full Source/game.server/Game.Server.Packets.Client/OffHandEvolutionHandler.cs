using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(384, "副手进化")]
public class OffHandEvolutionHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = 0;
		int num3 = -1;
		int num4 = 0;
		int num5 = 0;
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
		if (itemAt == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("OffHandEvolutionHandler.ItemNotFound"));
			return 0;
		}
		SubWeaponEvolutionTemplateInfo subWeaponEvolutionTemplateInfo = SubWeaponEvolutionTemplateMgr.FindSubWeaponEvolutionByGp(itemAt.curExp);
		if (subWeaponEvolutionTemplateInfo != null && subWeaponEvolutionTemplateInfo.Level >= 20)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("OffHandEvolutionHandler.MaxLevel"));
			return 0;
		}
		for (int i = 0; i < num; i++)
		{
			num2 = packet.ReadInt();
			num3 = packet.ReadInt();
			num4 = packet.ReadInt();
			ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)num2, num3);
			if (itemAt2.StrengthenLevel <= 0 && itemAt2.StrengthenExp <= 0 && itemAt2 != null && num4 > 0 && num4 <= client.Player.GetItemCount(itemAt2.TemplateID))
			{
				num5 += itemAt2.Template.Property2 * num4;
				subWeaponEvolutionTemplateInfo = SubWeaponEvolutionTemplateMgr.FindSubWeaponEvolutionByGp(itemAt.curExp + num5);
				if (subWeaponEvolutionTemplateInfo != null && subWeaponEvolutionTemplateInfo.Level >= 20)
				{
					num4 = (itemAt.curExp + num5 - subWeaponEvolutionTemplateInfo.Exp) / itemAt2.Template.Property2;
					client.Player.RemoveTempate((eBageType)num2, itemAt2.Template, (num4 <= 0) ? 1 : num4);
					break;
				}
				client.Player.RemoveTempate((eBageType)num2, itemAt2.Template, num4);
			}
		}
		if (num5 > 0)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(384);
			gSPacketIn.WriteBoolean(val: true);
			client.Player.SendTCP(gSPacketIn);
			itemAt.curExp += num5;
			itemAt.IsBinds = true;
			if (subWeaponEvolutionTemplateInfo != null && subWeaponEvolutionTemplateInfo.Level >= 20)
			{
				itemAt.curExp = subWeaponEvolutionTemplateInfo.Exp;
			}
			client.Player.StoreBag.UpdateItem(itemAt);
		}
		else
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("OffHandEvolutionHandler.Materials.Empty"));
		}
		return 1;
	}
}
