using System.Text;

namespace Game.Server.GameServerScript.AI.Game;

public class ExplorationGame5 : ExplorationGame
{
	public override void OnCreated()
	{
		base.Game.SetupMissions(GetMissionIds());
		base.Game.TotalMissionCount = 5;
		base.OnCreated();
	}

	public override void OnPrepated()
	{
		base.OnPrepated();
	}

	private string GetMissionIds()
	{
		int[] array = new int[2] { 1005, 1006 };
		int num = base.Game.Random.Next(0, array.Length);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 5; i++)
		{
			stringBuilder.Append(array[num] + ",");
		}
		return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
	}
}
