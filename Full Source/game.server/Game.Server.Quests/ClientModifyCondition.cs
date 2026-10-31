using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class ClientModifyCondition : BaseCondition
{
	public ClientModifyCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void Reset(GamePlayer player)
	{
		int para = m_info.Para1;
		int num = para;
		if ((uint)num <= 1u)
		{
			base.Value = 1;
		}
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value <= 0;
	}
}
