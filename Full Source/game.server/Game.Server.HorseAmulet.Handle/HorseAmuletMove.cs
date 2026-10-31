using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HorseAmulet.Handle;

[HorseAmuletHandleAttbute(3)]
public class HorseAmuletMove : IHorseAmuletCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		if (player.Client.Server.Configuration.DebugMode)
		{
			Console.WriteLine(" HorseAmuletActivate =>fromPlace:{0}, toPlace:{1}", num, num2);
		}
		ItemInfo itemAt = player.HorseAmuletBag.GetItemAt(num);
		if (itemAt == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletMove.ItemNotFound"));
			return 1;
		}
		if (num2 < 20)
		{
			MountTalismansInfo mountTalismansInfo = MountMgr.FindMountTalismans(itemAt.TemplateID);
			if (mountTalismansInfo == null)
			{
				player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletMove.ItemNotFound"));
				return 1;
			}
			if (mountTalismansInfo.BaseType1 == 0)
			{
				player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletMove.CanNotEquip"));
			}
			else
			{
				if (num2 == 19)
				{
					player.HorseAmuletBag.MoveItem(num, num2, 1);
					return 1;
				}
				if (player.HorseAmuletBag.CanPutInEquipAmulet() != -1)
				{
					if (player.HorseAmuletBag.CanEquipAmulet(itemAt.TemplateID))
					{
						player.HorseAmuletBag.MoveItem(num, num2, 1);
						player.EquipBag.UpdatePlayerProperties();
					}
					else
					{
						player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletMove.WrongEquip"));
					}
				}
				else
				{
					player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletMove.MaxPutInEquip"));
				}
			}
		}
		else
		{
			player.HorseAmuletBag.MoveItem(num, num2, 1);
		}
		return 1;
	}
}
