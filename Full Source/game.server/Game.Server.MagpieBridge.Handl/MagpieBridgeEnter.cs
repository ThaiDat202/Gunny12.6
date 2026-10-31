using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MagpieBridge.Handle;

[MagpieBridgeHandleAttbute(1)]
public class MagpieBridgeEnter : IMagpieBridgeCommandHadler
{
	private static ThreadSafeRandom rand = new ThreadSafeRandom();

	private readonly int[] mapID = new int[3] { 1, 2, 3 };

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.Extra.Info.NowPositionMB >= 35)
		{
			Player.Extra.CreateMagpieBridgeItems(Refresh: true);
		}
		rand.Shuffer(mapID);
		Player.Extra.GetMagpieBridgeItemsDb();
		int num = rand.Next(mapID.Length);
		Player.Extra.MapId = mapID[num];
		Player.Extra.SendMagpieBridgeEnter();
		return false;
	}
}
