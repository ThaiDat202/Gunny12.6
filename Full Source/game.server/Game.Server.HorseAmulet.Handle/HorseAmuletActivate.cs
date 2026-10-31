using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HorseAmulet.Handle;

[HorseAmuletHandleAttbute(1)]
public class HorseAmuletActivate : IHorseAmuletCommandHadler
{
	private Random rand = new Random();

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		ItemInfo itemAt = player.HorseAmuletBag.GetItemAt(slot);
		if (itemAt == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletActivate.ItemNotFound"));
			return 0;
		}
		MountTalismansInfo mountTalismansInfo = MountMgr.FindMountTalismans(itemAt.TemplateID);
		if (mountTalismansInfo == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletActivate.ErrorData"));
			return 0;
		}
		if (!mountTalismansInfo.IsCanWash)
		{
			player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletActivate.IsCanWash"));
			return 0;
		}
		if (player.PlayerCharacter.stive < mountTalismansInfo.Consume)
		{
			player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletActivate.StiveNotEnought"));
			return 0;
		}
		player.PlayerCharacter.stive -= mountTalismansInfo.Consume;
		itemAt.Hole1 = rand.Next(mountTalismansInfo.ExtendType1MinValue * mountTalismansInfo.Score, mountTalismansInfo.ExtendType1MaxValue * mountTalismansInfo.Score) / mountTalismansInfo.Score;
		GSPacketIn gSPacketIn = new GSPacketIn(375);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(itemAt.Hole1);
		gSPacketIn.WriteInt(player.PlayerCharacter.stive);
		player.SendTCP(gSPacketIn);
		player.HorseAmuletBag.UpdateItem(itemAt);
		player.EquipBag.UpdatePlayerProperties();
		return 1;
	}
}
