using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.MagicStone.Handle;

[MagicStoneHandleAttbute(6)]
public class Clock : IMagicStoneCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		PlayerMagicStoneInventory magicStoneBag = Player.MagicStoneBag;
		ItemInfo itemAt = magicStoneBag.GetItemAt(slot);
		if (itemAt.goodsLock)
		{
			itemAt.goodsLock = false;
		}
		else
		{
			itemAt.goodsLock = true;
		}
		magicStoneBag.UpdateItem(itemAt);
		return true;
	}
}
