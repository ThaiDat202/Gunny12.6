using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(188, "场景用户离开")]
public class UseConsortiaReworkNameHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		byte bageType = packet.ReadByte();
		int slot = packet.ReadInt();
		string newNickName = packet.ReadString();
		string text = "";
		if (client.Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		if (itemAt.TemplateID == 11993)
		{
			using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
			ConsortiaInfo consortiaSingle = consortiaBussiness.GetConsortiaSingle(num);
			if (consortiaSingle == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("UseConsortiaReworkNameHandler.Msg1"));
				return 0;
			}
			if (client.Player.PlayerCharacter.ID != consortiaSingle.ChairmanID)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("UseConsortiaReworkNameHandler.Msg2"));
				return 0;
			}
			if (consortiaBussiness.RenameConsortia(num, client.Player.PlayerCharacter.NickName, newNickName))
			{
				inventory.RemoveCountFromStack(itemAt, 1);
			}
			else
			{
				text = LanguageMgr.GetTranslation("UseConsortiaReworkNameHandler.Msg3");
			}
		}
		if (text != "")
		{
			client.Player.SendMessage(text);
		}
		return 0;
	}
}
