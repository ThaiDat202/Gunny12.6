using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(134)]
public class CatchInsectCakeStatus : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		ItemInfo itemByTemplateID = Player.GetItemByTemplateID(11958);
		bool val = false;
		if (itemByTemplateID != null)
		{
			val = true;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(134);
		gSPacketIn.WriteBoolean(val);
		Player.Out.SendTCP(gSPacketIn);
		return true;
	}
}
