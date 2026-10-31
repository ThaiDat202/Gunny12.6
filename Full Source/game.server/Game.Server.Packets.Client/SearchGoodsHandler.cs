using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(98, "客户端日记")]
public class SearchGoodsHandler : IPacketHandler
{
	private readonly int[] mapID = new int[3] { 1, 2, 3 };

	private static ThreadSafeRandom rand = new ThreadSafeRandom();

	public int maxTime
	{
		get
		{
			return 3500;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		new GSPacketIn(98);
		PlayerExtra extra = client.Player.Extra;
		if (client.Player.PlayerCharacter.Grade < 25)
		{
			client.Player.SendMessage($"Nivel insuficiente!");
			return 0;
		}
		switch (b)
		{
		case 0:
		{
			rand.Shuffer(mapID);
			extra.GetSearchGoodItemsDb();
			int num = rand.Next(mapID.Length);
			extra.MapId = mapID[num];
			client.Player.SendTCP(PlayerEnter(extra));
			break;
		}
		case 1:
		{
			bool isRemindRollBind = packet.ReadBoolean();
			extra.RollDiceCallBack(isRemindRollBind);
			break;
		}
		case 2:
		{
			if (extra.Info.starlevel >= AwardMgr.MaxStar())
			{
				break;
			}
			SearchGoodsTempInfo searchGoodsTempInfo = AwardMgr.GetSearchGoodsTempInfo(extra.Info.starlevel + 1);
			if (searchGoodsTempInfo == null)
			{
				client.Player.SendMessage("Treasure atingiu o mais alto nível!");
				break;
			}
			bool flag = false;
			if (client.Player.RemoveMoney(searchGoodsTempInfo.NeedMoney) <= 0)
			{
				client.Player.SendMessage("Cupons Insuficiêntes !");
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				extra.Info.starlevel++;
				client.Player.SendMessage("Operação bem sucedida!");
				extra.CreateSearchGoodItems();
				client.Player.SendTCP(PlayerEnter(extra));
			}
			break;
		}
		case 3:
		{
			bool useMoney = packet.ReadBoolean();
			extra.TakeCard(useMoney);
			break;
		}
		case 4:
			extra.Info.starlevel = 1;
			extra.Info.nowPosition = 0;
			extra.CreateSearchGoodItems();
			client.Player.SendTCP(PlayerEnter(extra));
			break;
		case 5:
			extra.ConvertSearchGoodItems();
			break;
		default:
		{
			SearchGoodsPackageType searchGoodsPackageType = (SearchGoodsPackageType)b;
			Console.WriteLine("SearchGoodsPackageType." + searchGoodsPackageType);
			break;
		}
		}
		return 0;
	}

	private GSPacketIn PlayerEnter(PlayerExtra Extra)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(98);
		gSPacketIn.WriteByte(16);
		gSPacketIn.WriteInt(Extra.MapId);
		gSPacketIn.WriteInt(Extra.Info.starlevel);
		gSPacketIn.WriteInt(Extra.Info.nowPosition);
		gSPacketIn.WriteInt(Extra.Info.FreeCount);
		gSPacketIn.WriteInt(Extra.SearchGoodItems.Count);
		foreach (EventAwardInfo searchGoodItem in Extra.SearchGoodItems)
		{
			gSPacketIn.WriteInt(searchGoodItem.Position);
			gSPacketIn.WriteInt(searchGoodItem.TemplateID);
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		return gSPacketIn;
	}
}
