using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.MagpieBridge.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.MagpieBridge;

[MagpieBridgeProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class MagpieBridgeLogicProcessor : AbstractMagpieBridgeProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private MagpieBridgeHandleMgr _commandMgr;

	public MagpieBridgeLogicProcessor()
	{
		_commandMgr = new MagpieBridgeHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		MagpieBridgePackageType magpieBridgePackageType = (MagpieBridgePackageType)packet.ReadByte();
		try
		{
			IMagpieBridgeCommandHadler magpieBridgeCommandHadler = _commandMgr.LoadCommandHandler((int)magpieBridgePackageType);
			if (magpieBridgeCommandHadler != null)
			{
				magpieBridgeCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("MagpieBridgePackageType: {1}, OnGameData is Error: {0}", ex.ToString(), magpieBridgePackageType));
		}
	}
}
