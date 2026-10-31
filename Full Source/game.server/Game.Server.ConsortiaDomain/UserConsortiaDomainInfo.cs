using System.Drawing;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaDomain;

public class UserConsortiaDomainInfo
{
	public int UserID { get; set; }

	public int ConsortiaID { get; set; }

	public GamePlayer player { get; set; }

	public Point Postion { get; set; }

	public bool IsFighting { get; set; }

	public int repairBuildId { get; set; }
}
