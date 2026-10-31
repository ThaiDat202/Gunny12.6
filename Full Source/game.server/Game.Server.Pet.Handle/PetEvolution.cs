using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(23)]
public class PetEvolution : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		if (num != 11163)
		{
			return false;
		}
		bool flag = player.Extra.UseDeed(11);
		int num3 = 0;
		if (flag)
		{
			num3 = 10;
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg24"));
		}
		ItemInfo itemByTemplateID = player.GetItemByTemplateID(num);
		if (!flag && itemByTemplateID != null && num2 > 0 && num == 11163)
		{
			if (itemByTemplateID.Count < num2)
			{
				num2 = itemByTemplateID.Count;
			}
			num3 = itemByTemplateID.Template.Property2 * num2;
		}
		if (num3 > 0)
		{
			bool val = false;
			int evolutionGrade = player.PlayerCharacter.evolutionGrade;
			int evolutionExp = player.PlayerCharacter.evolutionExp;
			int num4 = player.PlayerCharacter.evolutionExp + num3;
			int evolutionMax = PetMgr.GetEvolutionMax();
			for (int i = evolutionGrade; i <= evolutionMax; i++)
			{
				PetFightPropertyInfo petFightPropertyInfo = PetMgr.FindFightProperty(i + 1);
				if (petFightPropertyInfo != null && petFightPropertyInfo.Exp <= num4)
				{
					player.PlayerCharacter.evolutionGrade = i + 1;
					val = true;
				}
			}
			player.PlayerCharacter.evolutionExp = num4;
			player.PropBag.RemoveTemplate(num, num2);
			player.EquipBag.UpdatePlayerProperties();
			player.SendUpdatePublicPlayer();
			GSPacketIn gSPacketIn = new GSPacketIn(68);
			gSPacketIn.WriteByte(23);
			gSPacketIn.WriteBoolean(val);
			player.SendTCP(gSPacketIn);
		}
		return false;
	}
}
