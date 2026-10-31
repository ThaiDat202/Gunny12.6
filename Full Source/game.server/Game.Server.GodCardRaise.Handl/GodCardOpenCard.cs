using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle;

[GodCardRaiseHandleAttbute(21)]
public class GodCardOpenCard : IGodCardRaiseCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		packet.ReadBoolean();
		GodCardInfo[] array = GodCardMgr.GetAllGodCard().ToArray();
		List<GodCardUser> value = JsonConvert.DeserializeObject<List<GodCardUser>>(player.Actives.Info.cardListCard);
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(21);
		if (num > 1)
		{
			if (!player.MoneyDirect(GameProperties.GodCardOpenFiveTimeMoney) || array == null || array.Length == 0)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();
				int num2 = threadSafeRandom.Next(10);
				player.Actives.Info.cardScore += 100 * num;
				gSPacketIn.WriteInt(num);
				for (int i = 0; i < num; i++)
				{
					int num3 = 0;
					if (num2 >= 3)
					{
						while (num3 < array.Length / 2)
						{
							num3 = array[threadSafeRandom.Next(array.Length)].ID;
						}
					}
					else
					{
						num3 = array[threadSafeRandom.Next(array.Length)].ID;
					}
					gSPacketIn.WriteInt(num3);
					player.Actives.SaveListCard(num3, 1);
				}
			}
		}
		else if (player.Actives.Info.cardFreeCount < 3)
		{
			if (array == null || array.Length == 0)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				player.Actives.Info.cardFreeCount++;
				ThreadSafeRandom threadSafeRandom2 = new ThreadSafeRandom();
				int num4 = threadSafeRandom2.Next(10);
				player.Actives.Info.cardScore += 100 * num;
				gSPacketIn.WriteInt(num);
				for (int j = 0; j < num; j++)
				{
					int num5 = 0;
					if (num4 >= 3)
					{
						while (num5 < array.Length / 2)
						{
							num5 = array[threadSafeRandom2.Next(array.Length)].ID;
						}
					}
					else
					{
						num5 = array[threadSafeRandom2.Next(array.Length)].ID;
					}
					gSPacketIn.WriteInt(num5);
					player.Actives.SaveListCard(num5, 1);
				}
			}
		}
		else if (!player.MoneyDirect(GameProperties.GodCardOpenOneTimeMoney) || array == null || array.Length == 0)
		{
			gSPacketIn.WriteInt(0);
		}
		else
		{
			ThreadSafeRandom threadSafeRandom3 = new ThreadSafeRandom();
			int num6 = threadSafeRandom3.Next(10);
			player.Actives.Info.cardScore += 100 * num;
			gSPacketIn.WriteInt(num);
			for (int k = 0; k < num; k++)
			{
				int num7 = 0;
				if (num6 >= 3)
				{
					while (num7 < array.Length / 2)
					{
						num7 = array[threadSafeRandom3.Next(array.Length)].ID;
					}
				}
				else
				{
					num7 = array[threadSafeRandom3.Next(array.Length)].ID;
				}
				gSPacketIn.WriteInt(num7);
				player.Actives.SaveListCard(num7, 1);
			}
		}
		gSPacketIn.WriteInt(player.Actives.Info.cardScore);
		gSPacketIn.WriteInt(player.Actives.Info.cardFreeCount);
		player.Actives.Info.cardListCard = JsonConvert.SerializeObject(value);
		player.Out.SendTCP(gSPacketIn);
		return 1;
	}
}
