using SqlDataProvider.Data;

namespace Game.Server.BuffHorseRace;

public class BananaBuffer : AbstractHorseRaceBuffer
{
	public BananaBuffer(BuffHorseRaceInfo buffer)
		: base(buffer)
	{
	}

	public override void Start(UserHorseRaceInfo player, UserHorseRaceInfo ower)
	{
		BananaBuffer bananaBuffer = player.BuffList.GetOfType(typeof(BananaBuffer)) as BananaBuffer;
		if (bananaBuffer == null)
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
