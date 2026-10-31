using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveEpicThirdBoss : ABrain
{
	private int m_attackTurn = 0;

	protected Player m_targer;

	private List<PhysicalObj> m_objects = new List<PhysicalObj>();

	private PhysicalObj m_effectTop;

	private SimpleNpc m_npc1;

	private SimpleNpc m_npc2;

	private SimpleNpc m_npc3;

	private int m_npcId1 = 5422;

	private int m_npcId2 = 5423;

	private int m_npcId3 = 5424;

	private int m_npcEnemyId = 5404;

	private int m_countForzenCanKill = 3;

	private List<Point> m_pointCreateNpc2 = new List<Point>
	{
		new Point(1518, 699),
		new Point(500, 699)
	};

	private string[] PersonAttackSay = new string[3] { "Phạt.. phạt...", "Xem lưỡi hái của ta này.", "Đủ trình để tiếp chiêu?" };

	private string[] GlobalAttackSay = new string[3] { "Cú này đau lắm đấy!", "Xem tuyệt chiêu của ta.", "Tra tấn là sở thích của ta." };

	private string[] CallNpc1Say = new string[3] { "Ta chuẩn bị cho các ngươi món quà khá hot đấy.", "Món quà nhỏ dành cho các ngươi này.", "Mỏ hàn đâu ra dí ass bọn nó.." };

	private string[] CallNpc3Say = new string[3] { "<span class=\"red\">{0}</span> hãy xem cái này nhé.", "<span class=\"red\">{0}</span> thử cái này xem.", "Hãy xem <span class=\"red\">{0}</span> bị xử kìa." };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		foreach (PhysicalObj @object in m_objects)
		{
			base.Game.RemovePhysicalObj(@object, sendToClient: true);
		}
		m_objects = new List<PhysicalObj>();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.MaxBeatDis = 200;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_attackTurn++;
		switch (m_attackTurn)
		{
		case 1:
			PersonAttack();
			break;
		case 2:
			if (m_npc2 != null && m_npc2.IsLiving)
			{
				PersonAttack();
			}
			else
			{
				CallNpc2Attack();
			}
			break;
		case 3:
			CallGlobalAttack();
			break;
		case 4:
			if (m_npc3 != null && m_npc3.IsLiving)
			{
				PersonAttack();
			}
			else
			{
				CallNpc3Attack();
			}
			break;
		case 5:
			if (m_npc1 != null && m_npc1.IsLiving)
			{
				CallGlobalAttack();
			}
			else
			{
				CallNpc1Attack();
			}
			m_attackTurn = 0;
			break;
		}
	}

	private void CallGlobalAttack()
	{
		base.Body.ChangeDirection(-1, 500);
		base.Body.MoveTo(1000, 641, "fly", 1000, 10, RunGlobalAttack);
	}

	private void RunGlobalAttack()
	{
		base.Body.CurrentDamagePlus = 2f;
		((SimpleBoss)base.Body).RandomSay(GlobalAttackSay, 0, 1000, 0);
		base.Body.PlayMovie("beatA", 1200, 0);
		base.Body.CallFuction(CreateGlobalAttackEffect, 2500);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 4500, directDamage: true);
		base.Body.PlayMovie("beatC", 4000, 2000);
	}

	private void CreateGlobalAttackEffect()
	{
		m_effectTop = ((PVEGame)base.Game).CreateLayerTop(500, 300, "", "asset.game.4.heip", "", 1, 1);
		base.Body.CallFuction(RemoveGlobalAttackEffect, 2400);
	}

	private void RemoveGlobalAttackEffect()
	{
		if (m_effectTop != null)
		{
			base.Game.RemovePhysicalObj(m_effectTop, sendToClient: true);
		}
	}

	private void PersonAttack()
	{
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			((SimpleBoss)base.Body).RandomSay(PersonAttackSay, 0, 1000, 0);
			base.Body.MoveTo(m_targer.X, m_targer.Y - 100, "fly", 2000, 10, BeatE);
		}
	}

	private void CallNpc1Attack()
	{
		((SimpleBoss)base.Body).RandomSay(CallNpc1Say, 0, 1000, 0);
		base.Body.PlayMovie("beatD", 1200, 0);
		base.Body.CallFuction(CreateNpc1, 3800);
	}

	private void CreateNpc1()
	{
		int x = base.Game.Random.Next(440, 1570);
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.DamageForzen = true;
		livingConfig.CanTakeDamage = false;
		livingConfig.IsFly = true;
		m_npc1 = ((SimpleBoss)base.Body).CreateChild(m_npcId1, x, 580, 1, showBlood: false, livingConfig);
		m_npc1.Properties2 = m_countForzenCanKill;
	}

	private void CallNpc3Attack()
	{
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			base.Body.ChangeDirection(m_targer, 500);
			int num = base.Game.Random.Next(CallNpc3Say.Length);
			base.Body.Say(string.Format(CallNpc3Say[num], m_targer.PlayerDetail.PlayerCharacter.NickName), 0, 1000);
			base.Body.PlayMovie("beatD", 3000, 0);
			base.Body.CallFuction(CreateNpc3, 5000);
		}
	}

	private void CreateNpc3()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.IsFly = true;
		m_npc3 = ((SimpleBoss)base.Body).CreateChild(m_npcId3, 114, 453, 1, showBlood: false, livingConfig);
		m_npc3.Properties1 = m_targer.Id;
		m_npc3.Properties2 = new Point(m_targer.X, m_targer.Y);
		int highDelayTurn = base.Game.GetHighDelayTurn();
		((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 1500, 0);
		base.Body.CallFuction(CreateEffectMovePlayer, 2500);
		base.Body.CallFuction(BlockAndHidePlayer, 3500);
		m_targer.BoltMove(m_npc3.X, m_npc3.Y, 3900);
		((PVEGame)base.Game).SendObjectFocus(m_npc3, 1, 4000, 0);
		m_npc3.PlayMovie("in", 5000, 4000);
		((PVEGame)base.Game).PveGameDelay = highDelayTurn + 1;
	}

	private void CallNpc2Attack()
	{
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			base.Body.ChangeDirection(m_targer, 500);
			base.Body.Say("Bắt lại.. Ta cần bắt <span class=\"red\">" + m_targer.PlayerDetail.PlayerCharacter.NickName + "</span> nhốt lại.", 0, 1000);
			base.Body.PlayMovie("beatD", 3000, 0);
			base.Body.CallFuction(CreateNpc2, 5000);
		}
	}

	private void CreateNpc2()
	{
		int index = base.Game.Random.Next(m_pointCreateNpc2.Count);
		m_npc2 = ((SimpleBoss)base.Body).CreateChild(m_npcId2, m_pointCreateNpc2[index].X, m_pointCreateNpc2[index].Y, showBlood: true, ((PVEGame)base.Game).BaseConfig());
		m_npc2.Properties1 = m_targer.Id;
		int highDelayTurn = base.Game.GetHighDelayTurn();
		((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 4000, 0);
		base.Body.CallFuction(CreateEffectMovePlayer, 5000);
		base.Body.CallFuction(BlockAndHidePlayer, 6000);
		m_targer.BoltMove(m_npc2.X, m_npc2.Y, 6100);
		((PVEGame)base.Game).SendObjectFocus(m_npc2, 1, 6800, 0);
		m_npc2.PlayMovie("AtoB", 7500, 0);
		m_npc2.PlayMovie("beatA", 10000, 2000);
		base.Body.BeatDirect(m_targer, "", 11000, 1, 1);
		((PVEGame)base.Game).PveGameDelay = highDelayTurn + 1;
	}

	private void CreateEffectMovePlayer()
	{
		m_objects.Add(((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.4.lanhuo", "", 1, 1));
	}

	private void BlockAndHidePlayer()
	{
		m_targer.SetVisible(state: false);
		m_targer.BlockTurn = true;
	}

	private void BeatE()
	{
		base.Body.Beat(m_targer, "beatE", 100, 1, 500, 1, 1);
		base.Body.CallFuction(BackToDefaultPoint, 3500);
	}

	private void BackToDefaultPoint()
	{
		int x = base.Game.Random.Next(376, 1643);
		int y = base.Game.Random.Next(112, 593);
		base.Body.MoveTo(x, y, "fly", 500, 10);
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnAfterTakedBomb()
	{
		if (base.Body.IsLiving)
		{
		}
	}
}
