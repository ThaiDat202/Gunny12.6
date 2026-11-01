using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdHardBloodNpc : ABrain
{
	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		((PVEGame)base.Game).SendFreeFocus(base.Body.X, base.Body.Y, 0, 150, 1000);
		base.Body.CallFuction(AddBlood, 2000);
	}

	public void AddBlood()
	{
		List<Living> livedLivings = base.Game.GetLivedLivings();
		foreach (Living item in livedLivings)
		{
			if (item.IsLiving)
			{
				item.AddBlood(700);
			}
		}
		List<TurnedLiving> turnQueue = base.Game.TurnQueue;
		foreach (TurnedLiving item2 in turnQueue)
		{
			if (item2.IsLiving && !(item2 is Player))
			{
				item2.AddBlood(700);
			}
		}
		base.Body.PlayMovie("renew", 0, 0);
	}
}
