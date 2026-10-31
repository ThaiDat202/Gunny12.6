using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.MagicHouse.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.MagicHouse;

[MagicHouseProcessorAtribute(40, "礼堂逻辑")]
public class MagicHouseLogicProcessor : AbstractMagicHouseProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private MagicHouseHandleMgr _commandMgr;

	public MagicHouseLogicProcessor()
	{
		_commandMgr = new MagicHouseHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		MagicHousePackageTypeIn magicHousePackageTypeIn = (MagicHousePackageTypeIn)packet.ReadInt();
		try
		{
			IMagicHouseCommandHadler magicHouseCommandHadler = _commandMgr.LoadCommandHandler((int)magicHousePackageTypeIn);
			if (magicHouseCommandHadler != null)
			{
				magicHouseCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error($"MagicHousePackageTypeIn: {magicHousePackageTypeIn}");
			log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint));
		}
	}
}
