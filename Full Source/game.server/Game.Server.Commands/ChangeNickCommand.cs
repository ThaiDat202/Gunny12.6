using System.Text.RegularExpressions;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Commands;

internal class ChangeNickCommand
{
	public static void Execute(string command, GamePlayer player)
	{
		Regex regex = new Regex("[~]|[!]|[@]|[#]|[$]|[%]|[\\^]|[&]|[*]|[(]|[)]|[<]|[>]|[']|[\"]|[_]|[+]|[-]|[=]|[{]|[}]|[[]|[]]|[:]|[\"]|[|]|[;]|[']|[ ]|[?]|[,]|[.]|[/]|[~]|[！]|[@]|[#]|[￥]|[%]|[…]|[…]|[&]|[*]|[(]|[)]|[：]|[“]|[|]|[《]|[》]|[？]|[{][}]|[—]|[—]|[+]|[-]|[=]|[·]|[【]|[】]|[；]|[‘]|[、]|[。]|[、]|[,]|[.]", RegexOptions.IgnoreCase);
		string[] array = command.Split(';');
		if (array.Length == 3)
		{
			if (player.PlayerCharacter.Money < 100000)
			{
				player.Out.SendMessage(eMessageType.ALERT, "你没有足够的点券");
				return;
			}
			if (array[1].Length >= 28 && array[1].Length < 6)
			{
				player.Out.SendMessage(eMessageType.ALERT, "新昵称必须包含至少4个且不超过28个字符.");
				return;
			}
			if (array[1] != array[2])
			{
				player.Out.SendMessage(eMessageType.ALERT, "与原始昵称重复了或已存在");
				return;
			}
			MatchCollection matchCollection = regex.Matches(array[1]);
			if (matchCollection.Count == 0)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					if (playerBussiness.RenameNick(player.PlayerCharacter.UserName, player.PlayerCharacter.NickName, array[1]))
					{
						PlayerInfo playerInfo = WorldMgr.GetClientByPlayerNickName(player.PlayerCharacter.SpouseName).PlayerCharacter;
						if (playerInfo == null)
						{
							playerInfo = playerBussiness.GetUserSingleByNickName(player.PlayerCharacter.SpouseName);
						}
						playerInfo.SpouseName = array[1];
						playerBussiness.UpdatePlayer(playerInfo);
						GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(player.PlayerCharacter.SpouseName);
						if (clientByPlayerNickName != null)
						{
							clientByPlayerNickName.PlayerCharacter.SpouseName = array[1];
						}
						player.RemoveMoney(100000);
						player.Out.SendMessage(eMessageType.ALERT, "您已成功更改昵称");
					}
					else
					{
						player.Out.SendMessage(eMessageType.ALERT, "更改昵称时发生错误...");
					}
					return;
				}
			}
			player.Out.SendMessage(eMessageType.ALERT, "新昵称包含无效字符");
		}
		else
		{
			player.Out.SendMessage(eMessageType.ALERT, "并非所有命令参数都已指定./n例如:!ChangeNick; NEW; RETURN");
		}
	}
}
