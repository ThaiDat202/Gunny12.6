using System.Linq;
using System.Text;
using Bussiness.Managers;
using Game.Base;
using Game.Server.Managers;

namespace Game.Server.Commands.Admin;

[Cmd("&load", ePrivLevel.Player, "   相关文件重新加载.", new string[] { "       /load /config     :加载配置文件.", "       /load /shop       :加载商城文件.", "       /load /rate       :加载倍率.", "       /load /item       :加载模板.", "       /load /property   :刷新游戏道具." })]
public class ReloadCommand : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		if (args.Length > 1)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			if (args.Contains("/cmd"))
			{
				CommandMgr.LoadCommands();
				DisplayMessage(client, "Command load success!");
				stringBuilder.Append("/cmd,");
			}
			if (args.Contains("/config"))
			{
				GameServer.Instance.Configuration.Refresh();
				DisplayMessage(client, "Application config file load success!");
				stringBuilder.Append("/config,");
			}
			if (args.Contains("/item"))
			{
				if (ItemMgr.ReLoad())
				{
					DisplayMessage(client, "Items load success!");
					stringBuilder.Append("/item,");
				}
				else
				{
					DisplayMessage(client, "Items load failed!");
					stringBuilder2.Append("/item,");
				}
			}
			if (args.Contains("/shop"))
			{
				if (ShopMgr.ReLoad())
				{
					DisplayMessage(client, "Shops load success!");
					stringBuilder.Append("/shop,");
				}
				else
				{
					DisplayMessage(client, "Shops load failed!");
					stringBuilder2.Append("/shop,");
				}
			}
			if (args.Contains("/rate"))
			{
				if (RateMgr.ReLoad())
				{
					DisplayMessage(client, "Rates load success!");
					stringBuilder.Append("/rate,");
				}
				else
				{
					DisplayMessage(client, "Rates load failed!");
					stringBuilder2.Append("/rate,");
				}
			}
			bool flag;
			if (stringBuilder.Length == 0 && stringBuilder2.Length == 0)
			{
				DisplayMessage(client, "Nothing executed!");
				DisplaySyntax(client);
			}
			else
			{
				DisplayMessage(client, "Success Options:    " + stringBuilder);
				if (stringBuilder2.Length > 0)
				{
					DisplayMessage(client, "Faile Options:      " + stringBuilder2);
					flag = false;
					return false;
				}
			}
			flag = true;
		}
		else
		{
			DisplaySyntax(client);
			bool flag = true;
		}
		return true;
	}
}
