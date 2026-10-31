using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(1)]
public class PyramidEnter : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.Actives.LoadPyramid())
		{
			GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
			PyramidInfo pyramid = Player.Actives.Pyramid;
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(pyramid.currentLayer);
			gSPacketIn.WriteInt(pyramid.maxLayer);
			gSPacketIn.WriteInt(pyramid.totalPoint);
			gSPacketIn.WriteInt(pyramid.turnPoint);
			gSPacketIn.WriteInt(pyramid.pointRatio);
			gSPacketIn.WriteInt(pyramid.currentFreeCount);
			gSPacketIn.WriteInt(pyramid.currentReviveCount);
			gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
			if (pyramid.isPyramidStart)
			{
				string[] lists = pyramid.LayerItems.Split('|');
				int[] array = new int[7] { 8, 7, 6, 5, 4, 3, 2 };
				gSPacketIn.WriteInt(array.Length);
				for (int i = 1; i <= array.Length; i++)
				{
					string[] layerItems = GetLayerItems(lists, i);
					gSPacketIn.WriteInt(i);
					gSPacketIn.WriteInt(layerItems.Length);
					for (int j = 0; j < layerItems.Length; j++)
					{
						int val = int.Parse(layerItems[j].Split('-')[1]);
						int val2 = int.Parse(layerItems[j].Split('-')[2]);
						gSPacketIn.WriteInt(val);
						gSPacketIn.WriteInt(val2);
					}
				}
			}
			Player.SendTCP(gSPacketIn);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg10"));
		}
		return true;
	}

	private string[] GetLayerItems(string[] lists, int layer)
	{
		List<string> list = new List<string>();
		foreach (string text in lists)
		{
			string text2 = text.Split('-')[0];
			if (text2 == layer.ToString())
			{
				list.Add(text);
			}
		}
		return list.ToArray();
	}
}
