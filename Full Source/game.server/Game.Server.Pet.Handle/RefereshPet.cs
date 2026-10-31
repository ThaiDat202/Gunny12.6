using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(5)]
public class RefereshPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int num = Convert.ToInt32(PetMgr.FindConfig("AdoptRefereshCost").Value);
		int templateId = Convert.ToInt32(PetMgr.FindConfig("FreeRefereshID").Value);
		ItemInfo itemByTemplateID = player.PropBag.GetItemByTemplateID(0, templateId);
		List<UsersPetInfo> list = player.PetBag.GetAdoptPet(player.PlayerCharacter.VIPLevel).ToList();
		if (flag)
		{
			bool flag2 = true;
			if (player.Extra.UseKingBless(2))
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg12", num));
				flag2 = false;
			}
			else if (itemByTemplateID != null)
			{
				player.PropBag.RemoveTemplate(templateId, 1);
				flag2 = false;
			}
			else if (!player.MoneyDirect(num))
			{
				return false;
			}
			if (flag2)
			{
				Console.WriteLine("refereshCost {0}, div {1}", num, (int)((double)num * 0.1));
				player.AddPetScore((int)((double)num * 0.1));
			}
			if (GameProperties.VERSION < 7600)
			{
				list = PetMgr.CreateAdoptList(player.PlayerCharacter.ID, player.PetBag.MaxLevelByGrade, player.PlayerCharacter.VIPLevel);
			}
		}
		player.Out.SendRefreshPet(player, list.ToArray(), null, flag);
		return false;
	}
}
