using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(171, "场景玩家离开")]
public class UseReworkNameHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte bageType = packet.ReadByte();
		int slot = packet.ReadInt();
		string text = packet.ReadString();
		string text2 = LanguageMgr.GetTranslation("UseReworkNameHandler.Msg");
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		if (text.IndexOfAny("%!@#$%^&*()?/>.<,:;'\\|}]{[_~`+=-\"".ToCharArray()) != -1)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("UseReworkNameHandler.HasSpecialCharacters"));
			return 0;
		}
		if (itemAt.TemplateID == 11994)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			if (playerBussiness.RenameNick(client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.NickName, text))
			{
				inventory.RemoveCountFromStack(itemAt, 1);
				text2 = "";
			}
		}
		if (text2 != "")
		{
			client.Player.SendMessage(text2);
		}
		return 0;
	}
}
