using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HomeAdorn.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.HomeAdorn;

[HomeAdornProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class HomeAdornLogicProcessor : AbstractHomeAdornProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HomeAdornHandleMgr _commandMgr;

	public HomeAdornLogicProcessor()
	{
		_commandMgr = new HomeAdornHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HomeAdornPackageType homeAdornPackageType = (HomeAdornPackageType)packet.ReadByte();
		try
		{
			IHomeAdornCommandHadler homeAdornCommandHadler = _commandMgr.LoadCommandHandler((int)homeAdornPackageType);
			if (homeAdornCommandHadler != null)
			{
				homeAdornCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("HomeAdornPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), homeAdornPackageType));
		}
	}
}
