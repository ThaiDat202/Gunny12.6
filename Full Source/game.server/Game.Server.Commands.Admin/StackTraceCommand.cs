using System;
using Game.Base;

namespace Game.Server.Commands.Admin;

[Cmd("&trace", ePrivLevel.Admin, "显示服务的线程堆栈信息", new string[] { "eg: /trace -r      RoomMgr的线程", "   /trace  -g      GameMgr的线程" })]
public class StackTraceCommand : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		throw new NotImplementedException();
	}
}
