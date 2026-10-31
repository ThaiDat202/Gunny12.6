using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Conditions;

public abstract class Condition
{
	public readonly GamePlayer Player;

	public readonly Consortia Consortia;

	protected Condition(GamePlayer player, Consortia consortia)
	{
		Player = player;
		Consortia = consortia;
	}

	public virtual void AddTrigger()
	{
	}

	public virtual void RemoveTrigger()
	{
	}

	public void Update(ConsortiaTaskConditionInfo condition)
	{
		foreach (GamePlayer player in Consortia.Players)
		{
			ConsortiaTaskMgr.Out.SendTaskUpdate(player, condition);
		}
	}
}
