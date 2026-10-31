using SqlDataProvider.Data;

namespace Game.Server.BuffHorseRace;

public class CopySpeedBuffer : AbstractHorseRaceBuffer
{
	private int speedCopy = 0;

	public CopySpeedBuffer(BuffHorseRaceInfo buffer, int speed)
		: base(buffer)
	{
		speedCopy = speed;
	}

	public override void Start(UserHorseRaceInfo player, UserHorseRaceInfo ower)
	{
		CopySpeedBuffer copySpeedBuffer = player.BuffList.GetOfType(typeof(CopySpeedBuffer)) as CopySpeedBuffer;
		if (copySpeedBuffer == null)
		{
			base.Start(player, ower);
			player.Speed = speedCopy;
		}
	}

	public override void Stop()
	{
		m_player.Speed = m_player.DefaultSpeed;
		base.Stop();
	}
}
