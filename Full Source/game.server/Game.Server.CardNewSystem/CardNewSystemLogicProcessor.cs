using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.CardNewSystem.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.CardNewSystem;

public class CardNewSystemLogicProcessor : AbstractCardNewSystemProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private CardNewSystemHandleMgr _commandMgr;

	public CardNewSystemLogicProcessor()
	{
		_commandMgr = new CardNewSystemHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		CardEPackageType cardEPackageType = (CardEPackageType)packet.ReadByte();
		try
		{
			ICardNewSystemCommandHandler cardNewSystemCommandHandler = _commandMgr.LoadCommandHandler((int)cardEPackageType);
			if (cardNewSystemCommandHandler != null)
			{
				cardNewSystemCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("CardNewSystemLogicProcessor CardNewSystemPackageType.{0} not found!", cardEPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("CardNewSystemLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), cardEPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}
