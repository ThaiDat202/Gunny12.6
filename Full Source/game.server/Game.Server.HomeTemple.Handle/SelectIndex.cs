using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeTemple.Handle;

[HomeTempleHandleAttbute(2)]
public class SelectIndex : IHomeTempleCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (player.Actives.Info == null)
		{
			return 0;
		}
		int num2 = HomeTempPracticeMgr.MaxLevel();
		if (num > num2)
		{
			num = num2;
		}
		player.Actives.Info.CurrentSelectIndex = ((num > 0) ? num : 0);
		return 1;
	}
}
