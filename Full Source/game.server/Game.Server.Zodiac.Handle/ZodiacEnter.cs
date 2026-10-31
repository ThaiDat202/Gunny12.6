using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Zodiac.Handle;

[ZodiacHandleAttbute(3)]
public class ZodiacEnter : IZodiacCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		return true;
	}
}
