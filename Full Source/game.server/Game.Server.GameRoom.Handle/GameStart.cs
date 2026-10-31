using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(7)]
public class GameStart : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		BaseRoom currentRoom = Player.CurrentRoom;
		if (currentRoom != null && currentRoom.Host == Player)
		{
			if (currentRoom.RoomType == eRoomType.EntertainmentPK && !Player.MoneyDirect(100))
			{
				return false;
			}
			List<GamePlayer> players = currentRoom.GetPlayers();
			bool flag = false;
			if (currentRoom.RoomType == eRoomType.Dungeon && currentRoom.IsElementPVE(currentRoom.MapId))
			{
				currentRoom.RoomType = eRoomType.Element_PVE;
			}
			int ticketID = currentRoom.GetTicketID(currentRoom.MapId);
			if (ticketID != 0)
			{
				foreach (GamePlayer item in players)
				{
					if (item.PropBag.GetItemCount(ticketID) <= 0)
					{
						Player.CurrentRoom.IsPlaying = false;
						Player.CurrentRoom.SendCancelPickUp();
						item.SendMessage("玩家: [" + item.PlayerCharacter.NickName + "] 无此副本门票,无法开启副本.");
						return false;
					}
				}
				foreach (GamePlayer item2 in players)
				{
					if (item2.RemoveTemplate(ticketID, 1))
					{
						item2.SendMessage("扣除成功,剩余门票:" + item2.PropBag.GetItemCount(ticketID) + "张.");
					}
				}
			}
			if (currentRoom.RoomType == eRoomType.TEAM_ROOM)
			{
				DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;
				if (dayOfWeek != 0 && dayOfWeek != DayOfWeek.Saturday)
				{
					using List<GamePlayer>.Enumerator enumerator3 = players.GetEnumerator();
					if (enumerator3.MoveNext())
					{
						GamePlayer current3 = enumerator3.Current;
						Player.CurrentRoom.IsPlaying = false;
						Player.CurrentRoom.SendCancelPickUp();
						current3.SendMessage("战队赛只有周末才开放哦~~敬请期待...");
						return false;
					}
				}
				int num = 0;
				foreach (GamePlayer item3 in players)
				{
					if (num != 0)
					{
						if (item3.PlayerCharacter.Team.teamID != num)
						{
							Player.CurrentRoom.IsPlaying = false;
							Player.CurrentRoom.SendCancelPickUp();
							item3.SendMessage("不是相同战队,无法开始比赛.");
							return false;
						}
					}
					else
					{
						num = item3.PlayerCharacter.Team.teamID;
					}
				}
			}
			if (Player.MainWeapon == null)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				flag = true;
			}
			else if (currentRoom.RoomType == eRoomType.FarmBoss && (players.Count == 1 || players.Count > 2))
			{
				Player.SendMessage(LanguageMgr.GetTranslation("GameStart.Msg2"));
				flag = true;
			}
			else
			{
				if (currentRoom.RoomType == eRoomType.SingleBattle && currentRoom.PlayerCount > 2)
				{
					flag = true;
				}
				if (currentRoom.RoomType == eRoomType.SpecialActivityDungeon || currentRoom.RoomType == eRoomType.SpecialDungeon3 || currentRoom.RoomType == eRoomType.SpecialDungeon2 || currentRoom.RoomType == eRoomType.SpecialDungeon1)
				{
					string format = "{0}" + LanguageMgr.GetTranslation("GameRoomRoomSetupChange.Msg5");
					int dungeonTicketId = currentRoom.GetDungeonTicketId(currentRoom.MapId);
					int num2 = 0;
					string text = "";
					foreach (GamePlayer item4 in players)
					{
						if (item4.PropBag.GetItemCount(dungeonTicketId) <= 0)
						{
							text = ((item4.PlayerCharacter.ID != Player.PlayerCharacter.ID) ? (text + item4.PlayerCharacter.NickName) : (text + LanguageMgr.GetTranslation("GameRoomRoomSetupChange.Msg4")));
							text += ",";
							num2++;
						}
					}
					if (num2 > 0)
					{
						text = text.Substring(0, text.Length - 1);
						Player.SendMessage(string.Format(format, text));
						flag = true;
					}
				}
				if (!flag)
				{
					foreach (GamePlayer item5 in players)
					{
						if (item5 == null || item5.PlayerCharacter.isViewer)
						{
							continue;
						}
						switch (currentRoom.RoomType)
						{
						case eRoomType.ActivityDungeon:
							if (item5.Actives.Info.activityTanabataNum == 0)
							{
								item5.Actives.Info.activityTanabataNum++;
							}
							break;
						case eRoomType.SpecialActivityDungeon:
						case eRoomType.SpecialDungeon1:
						case eRoomType.SpecialDungeon2:
						case eRoomType.SpecialDungeon3:
							item5.PropBag.RemoveTemplate(currentRoom.GetDungeonTicketId(currentRoom.MapId, currentRoom.HardLevel), 1);
							break;
						}
						if (currentRoom.RoomType == eRoomType.Lanbyrinth || currentRoom.RoomType == eRoomType.Dungeon)
						{
							item5.Extra.KingBlessStrengthEnchance(isUp: true);
						}
						else
						{
							item5.Extra.KingBlessStrengthEnchance(isUp: false);
						}
						item5.PetBag.ReduceHunger();
						if (currentRoom.RoomType == eRoomType.Entertainment || currentRoom.RoomType == eRoomType.EntertainmentPK)
						{
							item5.ClearFightBag();
							BufferList.CreatePayBuffer(400, 20000, 1)?.Start(item5);
						}
						item5.PlayerCharacter.ringFlag = false;
						if (CanAddLoveBuff(item5, players))
						{
							ConsortiaBufferTempInfo consortiaBufferTempInfo = ConsortiaExtraMgr.FindConsortiaBuffInfo(item5.LovelSKillID);
							if (consortiaBufferTempInfo != null)
							{
								item5.PlayerCharacter.ringFlag = true;
								item5.PlayerCharacter.loveBuffLevel = consortiaBufferTempInfo.level;
								item5.PlayerCharacter.loveBuffValue = consortiaBufferTempInfo.value;
							}
						}
					}
					RoomMgr.StartGame(Player.CurrentRoom);
				}
			}
			if (flag)
			{
				Player.CurrentRoom.IsPlaying = false;
				Player.CurrentRoom.SendCancelPickUp();
			}
		}
		return true;
	}

	private bool CanAddLoveBuff(GamePlayer seft, List<GamePlayer> list)
	{
		foreach (GamePlayer item in list)
		{
			if (item != null && seft != null)
			{
			}
		}
		return false;
	}
}
