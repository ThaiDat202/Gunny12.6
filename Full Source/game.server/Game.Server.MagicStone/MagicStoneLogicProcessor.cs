using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.MagicStone.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.MagicStone;

[MagicStoneProcessorAtribute(40, "礼堂逻辑")]
public class MagicStoneLogicProcessor : AbstractMagicStoneProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private MagicStoneHandleMgr _commandMgr;

	public MagicStoneLogicProcessor()
	{
		_commandMgr = new MagicStoneHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		MagicStonePackageType code = (MagicStonePackageType)packet.ReadByte();
		try
		{
			IMagicStoneCommandHadler magicStoneCommandHadler = _commandMgr.LoadCommandHandler((int)code);
			if (magicStoneCommandHadler != null)
			{
				magicStoneCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint));
		}
	}
}
