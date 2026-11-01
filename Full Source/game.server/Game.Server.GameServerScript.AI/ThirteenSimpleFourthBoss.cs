using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenSimpleFourthBoss : ABrain
{
	private int m_attackTurn = 0;

	private int m_lastBloodFriend = 0;

	private SimpleBoss friendBoss;

	protected Player m_targer;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private PhysicalObj phyLineDead;

	private int m_friendBoss = 13009;

	private string[] Say_OpenLineDead = new string[3] { "Bọn mày nhớ bố không?", "Tụi mi còn nhớ ta không?", "Lũ cờ hó này nhớ ta là ai không?" };

	private string[] Say_DamageSingle = new string[5] { "Liệu có đỡ nổi không đây?", "Ta sẽ giết từng đứa một", "Trước kia bọn ngươi còn củ hành bọn ta. Giờ thì đừng hòng.", "Ngươi sẽ là mục tiêu ta giết đầu tiên", "Liệu ngươi đỡ được không đây?" };

	private string[] Say_DamageGlobal = new string[5] { "Lũ tép diu các ngươi phải chết hết", "Đừng mơ thắng được ta", "Ngươi nghĩ ngươi đánh bại được AD đẹp zai à?", "Ad éo cho ngươi qua đâu. Ahahaa...", "Đậu xanh ta giết tất cả" };

	private string[] Say_Angry = new string[5] { "Ta chịu hết nổi rồi", "Mọe chứ chơi lại chiêu cũ. Chết chùm luôn", "Ta chết các ngươi cũng phải chết.", "Lũ cờ hó ta chịu mếu nổi rồi", "Đùa đủ rồi. Chết hết. Giết hết" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (friendBoss == null)
		{
			SimpleBoss[] array = ((PVEGame)base.Game).FindLivingTurnBossWithID(m_friendBoss);
			if (array.Length != 0)
			{
				friendBoss = array[0];
			}
		}
		if (friendBoss.IsLiving || m_lastBloodFriend == 0)
		{
			m_lastBloodFriend = friendBoss.Blood;
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (phyObjects == null || phyObjects.Count <= 0)
		{
			return;
		}
		foreach (PhysicalObj phyObject in phyObjects)
		{
			base.Game.RemovePhysicalObj(phyObject, sendToClient: true);
		}
		phyObjects.Clear();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		if (friendBoss != null && !friendBoss.IsLiving)
		{
			EffectFriendDead();
			return;
		}
		switch (m_attackTurn)
		{
		case 0:
			CreateLineDead();
			break;
		case 1:
			DamageDeadLine();
			break;
		case 2:
			GlobalAttack();
			break;
		case 3:
			SingleAttack();
			break;
		case 4:
			ChangeFireStatus();
			break;
		case 5:
			if ((int)base.Body.Properties1 == 0 && (int)friendBoss.Properties1 == 0)
			{
				CreateLineDead();
			}
			else
			{
				WishDead();
			}
			m_attackTurn = 0;
			break;
		}
		m_attackTurn++;
	}

	private void EffectFriendDead()
	{
		int num = m_lastBloodFriend + m_lastBloodFriend / 100 * 10;
		if (num >= base.Body.Blood)
		{
			base.Body.Say("Ta sẽ không tha thứ cho các ngươi đâu", 0, 1000);
			base.Body.PlayMovie("die", 2000, 0);
			base.Body.Die(5000);
		}
		else
		{
			base.Body.AddBlood(-num, 1);
			base.Body.Say("Các ngươi nghĩ như vậy là đủ hạ gục ta à?", 0, 1000);
			base.Body.PlayMovie("angry", 1000, 0);
			base.Body.PlayMovie("beatC", 3000, 5000);
			base.Body.CallFuction(DeadAllPlayers, 6000);
		}
	}

	private void WishDead()
	{
		if ((int)base.Body.Properties1 == 1)
		{
			base.Body.Say("Hãy vĩnh biệt cuộc đời này đi lũ cờ hó.", 0, 1000);
			base.Body.PlayMovie("beatC", 1000, 5000);
			base.Body.CallFuction(DeadAllPlayers, 4500);
		}
		else
		{
			(base.Game as PVEGame).SendObjectFocus(friendBoss, 1, 1000, 0);
			friendBoss.Say("Hãy vĩnh biệt cuộc đời này đi lũ cờ hó.", 0, 2000);
			friendBoss.PlayMovie("beatC", 2000, 5000);
			friendBoss.CallFuction(DeadAllPlayers, 5500);
		}
	}

	private void DeadAllPlayers()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			allLivingPlayer.SyncAtTime = true;
			allLivingPlayer.Die();
		}
	}

	private void ChangeFireStatus()
	{
		base.Body.Properties1 = 1;
		friendBoss.Properties1 = 1;
		int num = base.Game.Random.Next(Say_Angry.Length);
		base.Body.Say(Say_Angry[num], 0, 1000);
		base.Body.PlayMovie("angry", 1000, 0);
		base.Body.Config.CanTakeDamage = false;
		(base.Game as PVEGame).SendObjectFocus(friendBoss, 1, 4000, 0);
		friendBoss.Say(Say_Angry[num], 0, 4500);
		friendBoss.PlayMovie("angry", 4500, 4000);
		friendBoss.Config.CanTakeDamage = false;
		base.Body.SetRectBomb((base.Body as SimpleBoss).NpcInfo.X, (base.Body as SimpleBoss).NpcInfo.Y, (base.Body as SimpleBoss).NpcInfo.Width, (base.Body as SimpleBoss).NpcInfo.Height);
		base.Body.SetRelateDemagemRect(-20, -210, 40, 20);
		friendBoss.SetRectBomb(friendBoss.NpcInfo.X, friendBoss.NpcInfo.Y, friendBoss.NpcInfo.Width, friendBoss.NpcInfo.Height);
		friendBoss.SetRelateDemagemRect(-20, -210, 40, 20);
	}

	private void SingleAttack()
	{
		int num = base.Game.Random.Next(Say_DamageSingle.Length);
		base.Body.Say(Say_DamageSingle[num], 0, 1000);
		base.Body.PlayMovie("cast", 1000, 0);
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 2000, 0);
			base.Body.CallFuction(EffectSingle, 2200);
			base.Body.BeatDirect(m_targer, "", 3000, 1, 1);
		}
	}

	private void EffectSingle()
	{
		phyObjects.Add((base.Game as PVEGame).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.ten.danbao", "", 1, 0));
	}

	private void EffectGlobal()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			phyObjects.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.qunbao", "", 1, 0));
		}
	}

	private void GlobalAttack()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		int num = base.Game.Random.Next(Say_DamageGlobal.Length);
		base.Body.Say(Say_DamageGlobal[num], 0, 1000);
		base.Body.PlayMovie("cast", 1000, 0);
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 2000, 0);
			base.Body.CallFuction(EffectGlobal, 2200);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 3000, directDamage: false);
		}
	}

	private void DamageDeadLine()
	{
		base.Body.CurrentDamagePlus = 10f;
		base.Body.PlayMovie("beatD", 1000, 2000);
		base.Body.CallFuction(RemoveDeadLine, 1800);
		base.Body.RangeAttacking(0, 1427, "cry", 2500, directDamage: true);
	}

	private void RemoveDeadLine()
	{
		base.Game.RemovePhysicalObj(phyLineDead, sendToClient: true);
		phyLineDead = null;
	}

	private void CreateLineDead()
	{
		int num = base.Game.Random.Next(Say_OpenLineDead.Length);
		base.Body.Say(Say_OpenLineDead[num], 0, 1000);
		base.Body.PlayMovie("beatA", 1100, 4000);
		base.Body.CallFuction(CreateLineDeadObject, 2500);
	}

	private void CreateLineDeadObject()
	{
		phyLineDead = (base.Game as PVEGame).Createlayer(783, 1020, "", "asset.game.ten.tedabiaoji", "", 1, 0);
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnAfterTakedFrozen()
	{
		base.OnAfterTakedFrozen();
		if ((int)base.Body.Properties1 == 1)
		{
			base.Body.Properties1 = 0;
			base.Body.PlayMovie("stand", 100, 2000);
			base.Body.SetRelateDemagemRect((base.Body as SimpleBoss).NpcInfo.X, (base.Body as SimpleBoss).NpcInfo.Y, (base.Body as SimpleBoss).NpcInfo.Width, (base.Body as SimpleBoss).NpcInfo.Height);
			base.Body.Config.CanTakeDamage = true;
		}
	}
}
