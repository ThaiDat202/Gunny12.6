using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GameUtils;

public class PlayerRoulette
{
	private static ThreadSafeRandom rand = new ThreadSafeRandom();

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private bool m_saveToDb;

	public GamePlayer Player => m_player;

	public PlayerRoulette(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
	}

	public virtual void LoadFromDatabase()
	{
		if (m_saveToDb)
		{
			using (new PlayerBussiness())
			{
			}
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using (new PlayerBussiness())
		{
			lock (m_lock)
			{
			}
		}
	}

	public void SendLeftGunStart()
	{
		string[] array = GameProperties.RouletteRate.Split('|');
		string text = "0";
		int val = 1;
		int num = rand.Next(100);
		if (num < 15)
		{
			text = array[0];
		}
		else if (num < 25)
		{
			text = array[1];
		}
		else if (num < 35)
		{
			text = array[2];
		}
		else if (num < 45)
		{
			text = array[3];
		}
		else if (num < 55)
		{
			text = array[4];
		}
		if (text != "0")
		{
			val = 0;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(163);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteString(text);
		Player.Out.SendTCP(gSPacketIn);
	}

	public void SendRoulette()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(137);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteBoolean(GameProperties.RouletteOpenOrClose);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteString("0");
		string[] array = GameProperties.RouletteRate.Split('|');
		foreach (string text in array)
		{
			string[] array2 = text.Split('.');
			gSPacketIn.WriteInt(int.Parse(array2[0]));
			gSPacketIn.WriteInt(-2);
			gSPacketIn.WriteInt(int.Parse(array2[1]));
			gSPacketIn.WriteInt(-1);
		}
		gSPacketIn.WriteDateTime(DateTime.Now);
		gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
		Player.Out.SendTCP(gSPacketIn);
	}
}
