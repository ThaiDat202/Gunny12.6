using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.EquipAmulet.Handle;

[EquipAmuletHandleAttbute(4)]
public class EquipAmuletBuyStive : IEquipAmuletCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.PlayerCharacter.buyStiveNum < GameProperties.AmuletBuyDustMax)
		{
			string[] array = GameProperties.AmuletBuyDustCountAndNeedMoney.Split('|');
			int value = (player.PlayerCharacter.buyStiveNum - 1) * int.Parse(array[1]) + int.Parse(array[2]);
			int num = (player.PlayerCharacter.buyStiveNum - 1) * int.Parse(array[0]) + int.Parse(array[2]);
			bool flag = false;
			if (player.PlayerCharacter.buyStiveNum < 1)
			{
				player.PlayerCharacter.buyStiveNum++;
				player.PlayerCharacter.stive += 50;
				flag = true;
			}
			else if (player.MoneyDirect(value))
			{
				player.PlayerCharacter.buyStiveNum++;
				player.PlayerCharacter.stive += num;
				flag = true;
			}
			if (flag)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(386);
				gSPacketIn.WriteByte(4);
				gSPacketIn.WriteInt(player.PlayerCharacter.buyStiveNum);
				gSPacketIn.WriteInt(player.PlayerCharacter.stive);
				player.SendTCP(gSPacketIn);
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("EquipAmuletBuyStive.buyStiveCount"));
		}
		return 1;
	}
}
