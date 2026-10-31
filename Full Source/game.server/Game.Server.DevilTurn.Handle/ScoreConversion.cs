using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(18)]
public class ScoreConversion : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		DevilTreasPointsList devilTreasPointsList = DevilTurnMgr.ListDevilTreasPointsList.Where((DevilTreasPointsList p) => p.ID == id).FirstOrDefault();
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(devilTreasPointsList.TemplateID), 1, 101);
		itemInfo.IsBinds = true;
		player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DevilTurnHandle.ScoreConversion.Success"));
		player.SendItemToMail(itemInfo, LanguageMgr.GetTranslation("DevilTurnHandle.ScoreConversion.Content"), LanguageMgr.GetTranslation("DevilTurnHandle.ScoreConversion.Title"), eMailType.Active);
		List<string> list = new List<string>();
		if (player.PlayerCharacter.DevilTurn.getGiftProgress != "")
		{
			list = player.PlayerCharacter.DevilTurn.getGiftProgress.Split(',').ToList();
		}
		list.Add(id.ToString());
		player.PlayerCharacter.DevilTurn.getGiftProgress = string.Join(",", list);
		DevilTurnMgr.Out.SendUpdateInfo(player);
		return 0;
	}
}
