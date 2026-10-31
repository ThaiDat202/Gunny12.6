using System;
using Game.Base;

namespace Game.Server.Commands.Admin;

[Cmd("&time", ePrivLevel.Player, "   显示当前系统时间", new string[] { "       /time :  显示当前系统时间", "                   " })]
public class DataTimeCommand : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		DisplayMessage(client, "data time:");
		DisplayMessage(client, "-------------------------------");
		DisplayMessage(client, DateTime.Now.ToString());
		return true;
	}
}
