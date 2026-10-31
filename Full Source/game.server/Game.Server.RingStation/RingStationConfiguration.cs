using System.Threading;

namespace Game.Server.RingStation;

public class RingStationConfiguration
{
	public static int ServerID;

	public static string ServerName;

	private static int int_0;

	private static int int_1;

	public static string[] RandomName;

	static RingStationConfiguration()
	{
		RandomName = new string[20]
		{
			"Cô Đơn", "Yêu Tớ Không?", "Faker Nè", "Yasuo", "Irelia", "Jax", "Lux", "Jinx", "Yone", "T1 Faker",
			"Bot Không Ngu", "Bot Dễ Thương", "Bot not Bot", "Trùm Bot", "Jinx Lép Thật", "Jinx Không Lép", "1 2 3 Dô", "Jinx Vú Bự", "Irelia King", "Yasuo King"
		};
		ServerID = 103;
		ServerName = "AutoBot";
		int_0 = 1000;
		int_1 = 1000;
	}

	public static int NextPlayerID()
	{
		return Interlocked.Increment(ref int_1);
	}

	public static int NextRoomId()
	{
		return Interlocked.Increment(ref int_0);
	}
}
