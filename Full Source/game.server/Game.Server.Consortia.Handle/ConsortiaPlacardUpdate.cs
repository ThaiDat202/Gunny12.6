using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(15)]
public class ConsortiaPlacardUpdate : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		string text = packet.ReadString();
		if (Encoding.Default.GetByteCount(text) > 300)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaPlacardUpdateHandler.Long"));
			return 1;
		}
		bool val = false;
		string msg = "ConsortiaPlacardUpdateHandler.Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (consortiaBussiness.UpdateConsortiaPlacard(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, text, ref msg))
			{
				msg = "ConsortiaPlacardUpdateHandler.Success";
				val = true;
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(15);
		gSPacketIn.WriteString(text);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
