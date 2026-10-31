using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(330, "客户端日记")]
public class UseLoveChocolateHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (!client.Player.PlayerCharacter.IsMarried)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.UnMerry"));
			return 0;
		}
		eBageType bagType = (eBageType)packet.ReadByte();
		int place = packet.ReadInt();
		int num = packet.ReadInt();
		ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
		if (itemAt == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.ItemNotFound"));
		}
		else
		{
			int itemCount = client.Player.PropBag.GetItemCount(itemAt.TemplateID);
			if (itemCount < num || !itemAt.isLove())
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.ItemNotEnough"));
			}
			else if (client.Player.Extra.CanRingExp(2))
			{
				int num2 = itemAt.Template.Property2 * num;
				client.Player.PropBag.RemoveCountFromStack(itemAt, num);
				client.Player.AddLoveScore(num2, 2);
				client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.UseSuccess", itemAt.Template.Name, num2));
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.UseFail"));
			}
		}
		return 0;
	}
}
