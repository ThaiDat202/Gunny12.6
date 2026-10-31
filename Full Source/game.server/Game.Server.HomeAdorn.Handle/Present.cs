using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(2)]
public class Present : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Player.SendMessage(LanguageMgr.GetTranslation("Present.FailByPermission"));
		return true;
	}
}
