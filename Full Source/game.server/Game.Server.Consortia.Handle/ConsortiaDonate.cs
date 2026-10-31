using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(23)]
public class ConsortiaDonate : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		packet.ReadInt();
		int num = packet.ReadInt();
		if (Player.PlayerCharacter.DDTMoney >= num)
		{
			Player.RemoveGiftToken(num);
			Player.OnDonateRiches(num, 1);
		}
		return 0;
	}
}
