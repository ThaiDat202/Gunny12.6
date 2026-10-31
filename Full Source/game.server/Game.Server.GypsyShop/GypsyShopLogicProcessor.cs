using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GypsyShop.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.GypsyShop;

[GypsyShopProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class GypsyShopLogicProcessor : AbstractGypsyShopProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private GypsyShopHandleMgr _commandMgr;

	public GypsyShopLogicProcessor()
	{
		_commandMgr = new GypsyShopHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		GypsyShopPackageType gypsyShopPackageType = (GypsyShopPackageType)packet.ReadByte();
		try
		{
			IGypsyShopCommandHadler gypsyShopCommandHadler = _commandMgr.LoadCommandHandler((int)gypsyShopPackageType);
			if (gypsyShopCommandHadler != null)
			{
				gypsyShopCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("GypsyShopPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), gypsyShopPackageType));
		}
	}
}
