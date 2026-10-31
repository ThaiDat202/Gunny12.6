using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(36)]
public class CatchbeastGetAward : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		int num = packet.ReadInt();
		string[] array = GameProperties.YearMonsterBoxInfo.Split('|');
		bool flag = CanGetGift(Player.Actives.Info.DamageNum, num, array);
		if (flag)
		{
			int dateId = int.Parse(array[num].Split(',')[0]);
			gSPacketIn.WriteByte(36);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteInt(num);
			Player.Out.SendTCP(gSPacketIn);
			Player.Actives.SetYearMonterBoxState(num);
			List<ItemInfo> list = new List<ItemInfo>();
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			ItemBoxMgr.CreateItemBox(dateId, list, ref specialValue);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ItemInfo item in list)
			{
				stringBuilder.Append("恭喜你获得了 " + item.Template.Name + " x" + item.Count + ", ");
			}
			Player.Out.SendMessage(eMessageType.Normal, stringBuilder.ToString());
			Player.AddTemplate(list);
		}
		return true;
	}

	private bool CanGetGift(int damageNum, int id, string[] YearMonsterBoxInfo)
	{
		if (id > 4 || id < 0)
		{
			return false;
		}
		int num = int.Parse(YearMonsterBoxInfo[id].Split(',')[1]) * 10000;
		return num <= damageNum;
	}
}
