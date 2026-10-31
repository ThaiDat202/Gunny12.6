using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd;

[GameCommand(114, "付费翻牌")]
public class PaymentTakeCardCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		if (player.HasPaymentTakeCard)
		{
			return;
		}
		int num = packet.ReadByte();
		if (packet.ReadBoolean())
		{
			if (player.PlayerDetail.RemoveGiftToken(500) > 0)
			{
				player.CanTakeOut++;
				player.FinishTakeCard = false;
				player.HasPaymentTakeCard = true;
				if (num < 0 || num > game.Cards.Length)
				{
					game.TakeCard(player);
				}
				else
				{
					game.TakeCard(player, num, isAuto: false);
				}
			}
			else
			{
				player.PlayerDetail.SendInsufficientMoney(1);
			}
		}
		else if (player.PlayerDetail.RemoveMoney(500) > 0)
		{
			player.CanTakeOut++;
			player.FinishTakeCard = false;
			player.HasPaymentTakeCard = true;
			if (num < 0 || num > game.Cards.Length)
			{
				game.TakeCard(player);
			}
			else
			{
				game.TakeCard(player, num, isAuto: false);
			}
		}
		else
		{
			player.PlayerDetail.SendInsufficientMoney(1);
		}
	}
}
