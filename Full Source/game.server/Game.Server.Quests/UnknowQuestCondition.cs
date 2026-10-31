using System;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class UnknowQuestCondition : BaseCondition
{
	public UnknowQuestCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override bool IsCompleted(GamePlayer player)
	{
		Console.WriteLine("UnknowQuestCondition, questID {0}.", base.Info.QuestID);
		if (base.Value > 0)
		{
			base.Value = 0;
			return true;
		}
		base.Value = base.Info.Para2;
		return true;
	}
}
