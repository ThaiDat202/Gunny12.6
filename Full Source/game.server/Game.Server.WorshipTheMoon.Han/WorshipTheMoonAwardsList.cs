using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.WorshipTheMoon.Handle;

[WorshipTheMoonHandleAttbute(4)]
public class WorshipTheMoonAwardsList : IWorshipTheMoonCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		string[] array = GameProperties.WorshipMoonGiftShowList.Split('|');
		GSPacketIn gSPacketIn = new GSPacketIn(281);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(int.Parse(GameProperties.WorshipMoonGift.Split('|')[1]));
		gSPacketIn.WriteInt(array.Length);
		string[] array2 = array;
		foreach (string s in array2)
		{
			gSPacketIn.WriteInt(int.Parse(s));
		}
		player.SendTCP(gSPacketIn);
		return false;
	}
}
