using SqlDataProvider.Data;

namespace Game.Server.BuffHorseRace;

public class HourglassBuffer : AbstractHorseRaceBuffer
{
	public HourglassBuffer(BuffHorseRaceInfo buffer)
		: base(buffer)
	{
	}

	public override void Start(UserHorseRaceInfo player, UserHorseRaceInfo ower)
	{
		HourglassBuffer hourglassBuffer = player.BuffList.GetOfType(typeof(HourglassBuffer)) as HourglassBuffer;
		if (hourglassBuffer == null)
		{
			base.Start(player, ower);
			if (player.UserID != ower.UserID)
			{
				player.Speed = 0;
			}
		}
	}

	public override void Stop()
	{
		m_player.Speed = m_player.DefaultSpeed;
		base.Stop();
	}
}
