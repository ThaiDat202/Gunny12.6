using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.BombKing.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.BombKing;

[BombKingProcessorAtribute(40, "礼堂逻辑")]
public class BombKingLogicProcessor : AbstractBombKingProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private BombKingHandleMgr _commandMgr;

	public BombKingLogicProcessor()
	{
		_commandMgr = new BombKingHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		BombKingPackageType bombKingPackageType = (BombKingPackageType)packet.ReadByte();
		try
		{
			IBombKingCommandHadler bombKingCommandHadler = _commandMgr.LoadCommandHandler((int)bombKingPackageType);
			if (bombKingCommandHadler != null)
			{
				bombKingCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("BombKingLogicProcessor PackageType {0} not found!", bombKingPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception arg)
		{
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("BombKingLogicProcessor Error {0}", arg);
			Console.WriteLine("_______________END_______________");
		}
	}
}
