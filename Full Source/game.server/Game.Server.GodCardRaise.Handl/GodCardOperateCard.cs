using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle;

[GodCardRaiseHandleAttbute(22)]
public class GodCardOperateCard : IGodCardRaiseCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int id = packet.ReadInt();
		int num2 = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(22);
		GodCardInfo godCardInfo = GodCardMgr.FindGodCard(id);
		if (godCardInfo == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GodCardOperateCard.Fail"));
		}
		else
		{
			int num3 = player.Actives.FindCardCount(godCardInfo.ID);
			gSPacketIn.WriteInt(godCardInfo.ID);
			switch (num)
			{
			case 2:
			{
				int num4 = godCardInfo.Composition * num2;
				if (player.Actives.Info.cardChipCount < num4)
				{
					gSPacketIn.WriteInt(num3);
					break;
				}
				player.Actives.Info.cardChipCount -= num4;
				player.SendMessage(LanguageMgr.GetTranslation("GodCardOperateCard.Success2", godCardInfo.Name, num2));
				gSPacketIn.WriteInt(player.Actives.SaveListCard(godCardInfo.ID, num2));
				break;
			}
			case 1:
				if (num3 < num2)
				{
					gSPacketIn.WriteInt(num3);
					break;
				}
				gSPacketIn.WriteInt(player.Actives.RemoveListCard(godCardInfo.ID, num2));
				player.Actives.Info.cardChipCount += godCardInfo.Decompose * num2;
				player.SendMessage(LanguageMgr.GetTranslation("GodCardOperateCard.Success", godCardInfo.Decompose * num2));
				break;
			}
			gSPacketIn.WriteInt(player.Actives.Info.cardChipCount);
			player.Out.SendTCP(gSPacketIn);
		}
		return 1;
	}
}
