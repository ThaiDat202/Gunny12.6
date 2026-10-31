using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HorseAmulet.Handle;

[HorseAmuletHandleAttbute(2)]
public class HorseAmuletSmash : IHorseAmuletCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		ItemInfo itemInfo = null;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			itemInfo = player.HorseAmuletBag.GetItemAt(packet.ReadInt());
			if (itemInfo != null && !itemInfo.cellLocked)
			{
				num2 += MountMgr.FindMountTalismans(itemInfo.TemplateID)?.ShatterNum ?? 0;
				player.HorseAmuletBag.RemoveItemAt(itemInfo.Place);
			}
		}
		player.PlayerCharacter.stive += num2;
		GSPacketIn gSPacketIn = new GSPacketIn(375);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(player.PlayerCharacter.stive);
		player.SendTCP(gSPacketIn);
		return 1;
	}
}
