using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HotSpringRooms.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.HotSpringRooms;

[HotSpringRoomsProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class HotSpringRoomsLogicProcessor : AbstractHotSpringRoomsProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HotSpringRoomsHandleMgr _commandMgr;

	public HotSpringRoomsLogicProcessor()
	{
		_commandMgr = new HotSpringRoomsHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HotSpringRoomsPackageType hotSpringRoomsPackageType = (HotSpringRoomsPackageType)packet.ReadByte();
		try
		{
			IHotSpringRoomsCommandHadler hotSpringRoomsCommandHadler = _commandMgr.LoadCommandHandler((int)hotSpringRoomsPackageType);
			if (hotSpringRoomsCommandHadler != null)
			{
				hotSpringRoomsCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("HotSpringRoomsPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), hotSpringRoomsPackageType));
		}
	}
}
