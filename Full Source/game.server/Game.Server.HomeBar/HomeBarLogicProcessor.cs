using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HomeBar.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.HomeBar;

[HomeBarProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class HomeBarLogicProcessor : AbstractHomeBarProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HomeBarHandleMgr _commandMgr;

	public HomeBarLogicProcessor()
	{
		_commandMgr = new HomeBarHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HomeBarPackageType homeBarPackageType = (HomeBarPackageType)packet.ReadByte();
		try
		{
			IHomeBarCommandHadler homeBarCommandHadler = _commandMgr.LoadCommandHandler((int)homeBarPackageType);
			if (homeBarCommandHadler != null)
			{
				homeBarCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("HomeBarPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), homeBarPackageType));
		}
	}
}
