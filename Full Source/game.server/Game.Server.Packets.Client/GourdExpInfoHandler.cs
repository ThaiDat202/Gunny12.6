using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(658, "经验葫芦")]
public class GourdExpInfoHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadInt())
		{
		case 1:
			if (client.Player.PlayerCharacter.GourdExpInfo.TemplateID != 0)
			{
				client.Player.Out.SendGourdExpStorage(client.Player);
			}
			break;
		case 2:
			client.Player.PlayerCharacter.GourdExpInfo.TemplateID = 0;
			client.Player.PlayerCharacter.GourdExpInfo.Stage = 0;
			client.Player.PlayerCharacter.GourdExpInfo.Exp = 0;
			client.Player.SendMessage("葫芦停止成功.");
			client.Player.Out.SendGourdExpStorage(client.Player);
			break;
		}
		return 0;
	}

	public static void AddGourdExp(GamePlayer Player, int value)
	{
		Player.PlayerCharacter.GourdExpInfo.Exp += value;
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(Player.PlayerCharacter.GourdExpInfo.TemplateID);
		if (Player.PlayerCharacter.GourdExpInfo.Exp >= itemTemplateInfo.Property2)
		{
			int templateID = Player.PlayerCharacter.GourdExpInfo.TemplateID;
			Player.PlayerCharacter.GourdExpInfo.TemplateID = 0;
			Player.PlayerCharacter.GourdExpInfo.Stage = 0;
			int gp = Player.PlayerCharacter.GourdExpInfo.Exp - itemTemplateInfo.Property2;
			Player.PlayerCharacter.GourdExpInfo.Exp = 0;
			Player.AddGP(gp);
			int templateId = 0;
			switch (templateID)
			{
			case 11192:
				templateId = 11196;
				break;
			case 11193:
				templateId = 11197;
				break;
			case 11194:
				templateId = 11198;
				break;
			case 11195:
				templateId = 11199;
				break;
			case 12207:
				templateId = 12211;
				break;
			case 12258:
				templateId = 12259;
				break;
			case 12260:
				templateId = 12261;
				break;
			}
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 101);
			itemInfo.IsBinds = true;
			if (Player.PlayerCharacter.GourdExpInfo.TemplateID == 12207)
			{
				itemInfo.IsBinds = false;
			}
			Player.AddTemplate(itemInfo);
			Player.SendMessage("葫芦经验已满,已转换成经验瓶,请使用下一个葫芦.");
		}
		Player.Out.SendGourdExpStorage(Player);
	}
}
