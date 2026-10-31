using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NormalKingSecond : ABrain
{
	private int int_0;

	private int int_1;

	private PhysicalObj physicalObj_0;

	private PhysicalObj physicalObj_1;

	private int int_2;

	private int int_3;

	private static string[] string_0;

	private static string[] string_1;

	private static string[] string_2;

	private static string[] string_3;

	private static string[] string_4;

	private static string[] string_5;

	private static string[] QpThbgkXgwu;

	private static string[] string_6;

	static NormalKingSecond()
	{
		string_0 = new string[3] { "要地震喽！！<br/>各位请扶好哦", "把你武器震下来！", "看你们能还经得起几下！！" };
		string_1 = new string[3] { "让你知道什么叫百发百中！", "送你一个球~你可要接好啦", "你们这群无知的低等庶民" };
		string_2 = new string[2] { "哎呀~~你们为什么要攻击我？<br/>我在干什么？", "噢~~好痛!我为什么要战斗？<br/>我必须战斗…" };
		string_3 = new string[3] { "马迪亚斯不要再控制我！", "这就是挑战我的下场！", "不！！这不是我的意愿… " };
		string_4 = new string[3] { "扭啊扭~<br/>扭啊扭~~", "哈利路亚~<br/>路亚路亚~~", "呀呀呀，<br/>好舒服啊！" };
		string_5 = new string[1] { "君临天下！！" };
		QpThbgkXgwu = new string[3] { "来尝尝这个吧", "让你冷静一下", "你们激怒了我" };
		string_6 = new string[2] { "神啊，赐予我力量吧！", "绝望吧，看我的水晶防护墙！" };
	}

	public NormalKingSecond()
	{
		int_3 = 1110;
	}

	public void CreateChild()
	{
		base.Body.PlayMovie("renew", 100, 2000);
		((SimpleBoss)base.Body).CreateChild(int_3, 520, 530, 400, 6, 1);
	}

	private void method_0()
	{
		base.Game.GetFrostPlayerRadom();
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		List<Player> list = new List<Player>();
		foreach (Player item in allFightPlayers)
		{
			if (!item.IsFrost)
			{
				list.Add(item);
			}
		}
		((SimpleBoss)base.Body).CurrentDamagePlus = 30f;
		if (list.Count == allFightPlayers.Count)
		{
			base.Body.Say("小的们给我上，好好教训敌人！", 1, 3300);
			base.Body.PlayMovie("renew", 3500, 0);
			base.Body.CallFuction(CreateChild, 6000);
		}
		else if (list.Count != 0)
		{
			base.Body.PlayMovie("beat1", 0, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "beat1", 1500, list);
		}
		else
		{
			base.Body.PlayMovie("beat1", 0, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "beat1", 1500, null);
		}
	}

	private void method_1()
	{
		int x = base.Game.Random.Next(850, 920);
		base.Body.MoveTo(x, base.Body.Y, "walk", 0, "", ((SimpleBoss)base.Body).NpcInfo.speed, method_5);
	}

	private void method_2()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		if (int_1 != 0)
		{
			int num = base.Game.Random.Next(0, string_0.Length);
			base.Body.Say(string_0[num], 1, 0);
			base.Body.PlayMovie("beat1", 1000, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
		}
		else
		{
			int num2 = base.Game.Random.Next(0, string_0.Length);
			base.Body.Say(string_0[num2], 1, 13000);
			base.Body.PlayMovie("beat1", 15000, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 17000, null);
			int_1++;
		}
	}

	private void method_3(int int_4, int int_5)
	{
		int num = base.Game.Random.Next(0, string_5.Length);
		if (int_1 != 0)
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say(string_5[num], 1, 0);
			base.Body.PlayMovie("beat1", 2000, 0);
			base.Body.RangeAttacking(int_4, int_5, "cry", 4000, null);
		}
		else
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say(string_5[num], 1, 13000);
			base.Body.PlayMovie("beat1", 15000, 0);
			base.Body.RangeAttacking(int_4, int_5, "cry", 17000, null);
			int_1++;
		}
	}

	private void method_4()
	{
		if (int_2 == 0)
		{
			physicalObj_0 = ((PVEGame)base.Game).CreatePhysicalObj(base.Body.X - 65, 620, "wallLeft", "com.mapobject.asset.WaveAsset_01_left", "1", 1, 0);
			physicalObj_1 = ((PVEGame)base.Game).CreatePhysicalObj(base.Body.X + 65, 620, "wallLeft", "com.mapobject.asset.WaveAsset_01_right", "1", 1, 0);
			physicalObj_0.SetRect(-165, -169, 43, 330);
			physicalObj_1.SetRect(128, -165, 41, 330);
			int_2 = 1;
		}
		int num = base.Game.Random.Next(0, string_6.Length);
		base.Body.Say(string_6[num], 1, 0);
	}

	private void method_5()
	{
		int num = base.Game.Random.Next(1, 2);
		for (int i = 0; i < num; i++)
		{
			Player player = base.Game.FindRandomPlayer();
			if (player == null)
			{
				break;
			}
			int num2 = base.Game.Random.Next(0, string_1.Length);
			base.Body.Say(string_1[num2], 1, 0);
			if (player.X <= base.Body.X)
			{
				base.Body.ChangeDirection(-1, 500);
			}
			else
			{
				base.Body.ChangeDirection(1, 500);
			}
			if (player != null && !player.IsFrost && base.Body.ShootPoint(player.X, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 1.5f, 2000))
			{
				base.Body.PlayMovie("beat2", 1500, 0);
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 390 && allFightPlayer.X < 1110)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (flag)
		{
			method_3(390, 1110);
			return;
		}
		if (int_0 != 0)
		{
			if (int_0 == 1)
			{
				method_1();
				int_0++;
			}
			else if (int_0 != 2)
			{
				method_0();
				int_0 = 0;
			}
			else
			{
				method_4();
				int_0++;
			}
			return;
		}
		method_2();
		if (int_2 == 1)
		{
			physicalObj_0.CanPenetrate = true;
			physicalObj_1.CanPenetrate = true;
			base.Game.RemovePhysicalObj(physicalObj_0, sendToClient: true);
			base.Game.RemovePhysicalObj(physicalObj_1, sendToClient: true);
			int_2 = 0;
		}
		int_0++;
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
