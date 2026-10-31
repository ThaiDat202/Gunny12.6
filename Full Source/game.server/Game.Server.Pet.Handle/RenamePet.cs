using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(9)]
public class RenamePet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int place = packet.ReadInt();
		string name = packet.ReadString();
		int value = Convert.ToInt32(PetMgr.FindConfig("ChangeNameCost").Value);
		if (player.MoneyDirect(value) && player.PetBag.RenamePet(place, name))
		{
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg20"));
		}
		return false;
	}
}
