using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Server.GameServerScript.AI.Messions;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourHardBlowArmsNpc : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_moive = null;

	private SimpleNpc npcDamage = null;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		Point point = default(Point);
		switch (m_attackTurn)
		{
		case 0:
			point = new Point(672, 746);
			break;
		case 1:
			point = new Point(1059, 749);
			break;
		case 2:
			point = new Point(1412, 751);
			break;
		}
		int num = int.MaxValue;
		SimpleNpc[] array = base.Game.FindAllNpcLiving();
		foreach (SimpleNpc simpleNpc in array)
		{
			if (simpleNpc.IsLiving && simpleNpc.X >= base.Body.X && simpleNpc.X <= base.Body.X + point.X)
			{
				int num2 = (int)base.Body.Distance(simpleNpc.X, simpleNpc.Y);
				if (num2 < num)
				{
					npcDamage = simpleNpc;
					num = num2;
				}
			}
		}
		if (npcDamage != null)
		{
			MoveToPlace(npcDamage.X - 20, npcDamage.Y, BeatNpc);
		}
		else if (m_attackTurn < 2)
		{
			MoveToPlace(point.X, point.Y, null);
		}
		else
		{
			MoveToPlace(point.X, point.Y, StartBomber);
			m_attackTurn = 0;
		}
		m_attackTurn++;
	}

	private void MoveToPlace(int x, int y, LivingCallBack callBack)
	{
		base.Body.MoveTo(x, y, "walk", 1000, 5, callBack);
	}

	public void BeatNpc()
	{
		base.Body.Beat(npcDamage, "die", 5000, 5000, 800);
		base.Body.Die(3000);
	}

	private void StartBomber()
	{
		base.Body.PlayMovie("beatA", 2000, 6000);
		base.Body.CallFuction(CreateCrashGate, 4500);
	}

	private void CreateCrashGate()
	{
		switch (((PVEGame)base.Game).MissionAI.UpdateUIData())
		{
		case 0:
			if (m_moive == null)
			{
				m_moive = ((PVEGame)base.Game).Createlayer(1590, 750, "", "game.asset.Gate", "cryA", 1, 0);
			}
			else
			{
				m_moive.PlayMovie("cryA", 0, 0);
			}
			break;
		case 1:
			if (m_moive == null)
			{
				m_moive = ((PVEGame)base.Game).Createlayer(1590, 750, "", "game.asset.Gate", "cryB", 1, 0);
			}
			else
			{
				m_moive.PlayMovie("cryB", 0, 0);
			}
			break;
		case 2:
			if (m_moive == null)
			{
				m_moive = ((PVEGame)base.Game).Createlayer(1590, 750, "", "game.asset.Gate", "cryC", 1, 0);
			}
			else
			{
				m_moive.PlayMovie("cryC", 0, 0);
			}
			break;
		}
		(((PVEGame)base.Game).MissionAI as DCH4201).CountKill++;
		base.Body.Die();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
