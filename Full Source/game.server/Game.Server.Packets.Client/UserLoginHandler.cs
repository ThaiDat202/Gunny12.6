using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.CenterService;
using Bussiness.Interface;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(1, "User Login handler")]
public class UserLoginHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		try
		{
			if (client.Player == null)
			{
				bool flag = false;
				if (GameProperties.VERSION >= 8100)
				{
					flag = packet.ReadBoolean();
				}
				int version = packet.ReadInt();
				int num = packet.ReadInt();
				if (num != 69)
				{
					byte[] array = new byte[8];
					byte[] rgb = packet.ReadBytes();
					try
					{
						rgb = WorldMgr.RsaCryptor.Decrypt(rgb, fOAEP: false);
					}
					catch (Exception exception)
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError"));
						client.Disconnect();
						GameServer.log.Error("RsaCryptor", exception);
						return 0;
					}
					for (int i = 0; i < 8; i++)
					{
						array[i] = rgb[i + 7];
					}
					client.SetKey(array);
					string[] array2 = Encoding.UTF8.GetString(rgb, 15, rgb.Length - 15).Split(',');
					if (array2.Length == 2)
					{
						string text = array2[0];
						string text2 = array2[1];
						if (!LoginMgr.ContainsUser(text))
						{
							bool isFirst = false;
							BaseInterface baseInterface = BaseInterface.CreateInterface();
							PlayerInfo playerInfo = baseInterface.LoginGame(text, text2, GameServer.Instance.Configuration.AreaId, ref isFirst);
							if (playerInfo != null && playerInfo.ID != 0)
							{
								if (playerInfo.ID == -2)
								{
									client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid"));
									client.Disconnect();
									Console.WriteLine("{0} Forbid!", text);
									return 0;
								}
								if (!isFirst)
								{
									client.Player = new GamePlayer(playerInfo.ID, text, client, playerInfo);
									LoginMgr.Add(playerInfo.ID, client);
									client.Server.LoginServer.SendAllowUserLogin(playerInfo.ID);
									client.Version = version;
									Console.WriteLine("{0} login to server!", text);
								}
								else
								{
									client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Register"));
									client.Disconnect();
								}
							}
							else
							{
								Console.WriteLine("{0} Login with {1} OverTime....", text, text2);
								client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.OverTime"));
								client.Disconnect();
							}
						}
						else
						{
							client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LoginError"));
							client.Disconnect();
						}
					}
					else
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LengthError"));
						client.Disconnect();
					}
				}
				else
				{
					switch (packet.ReadByte())
					{
					case 1:
					{
						string text3 = packet.ReadString();
						string text4 = packet.ReadString();
						if (text3 == "" || text4 == "")
						{
							client.Out.SendKitoff("Preencha todos os campos");
							return 0;
						}
						using (AccountBussiness accountBussiness = new AccountBussiness())
						{
							bool flag2 = false;
							List<PlayerInfo> list = accountBussiness.LoginMobile(text3, text4, ref flag2);
							if (flag2)
							{
								client.LoginMobileAceito = true;
								client.usernameLoginMobile = text3;
								Console.WriteLine("Allow Login Account Mobile: " + text3 + " : " + text4 + "  .....");
								GSPacketIn gSPacketIn = new GSPacketIn(1);
								gSPacketIn.WriteByte(1);
								gSPacketIn.WriteInt(list.Count);
								foreach (PlayerInfo item in list)
								{
									gSPacketIn.WriteInt(item.ID);
									gSPacketIn.WriteInt(item.Grade);
									gSPacketIn.WriteString(item.NickName);
								}
								client.Out.SendTCP(gSPacketIn);
							}
							else
							{
								client.Out.SendKitoff("Nome de Usuario ou Senha invalidos");
							}
						}
						break;
					}
					case 2:
						if (!client.LoginMobileAceito)
						{
							break;
						}
						using (new PlayerBussiness())
						{
							PlayerInfo playerInfo2 = new PlayerInfo
							{
								ID = packet.ReadInt(),
								UserName = client.usernameLoginMobile
							};
							using (CenterServiceClient centerServiceClient = new CenterServiceClient())
							{
								centerServiceClient.CreatePlayer(playerInfo2.ID, playerInfo2.NickName, client.usernameLoginMobile + playerInfo2.NickName, GameServer.Instance.Configuration.AreaId, isFirst: false);
							}
							if (playerInfo2.UserName == client.usernameLoginMobile)
							{
								client.Player = new GamePlayer(playerInfo2.ID, playerInfo2.UserName, client, playerInfo2);
								LoginMgr.Add(playerInfo2.ID, client);
								client.Server.LoginServer.SendAllowUserLogin(playerInfo2.ID);
								client.Version = version;
								Console.WriteLine("Allow Login Player Mobile {0}:{1}  ....", playerInfo2.UserName, playerInfo2.NickName);
							}
							else
							{
								client.Out.SendKitoff("Tentativa de Hacker detectada");
							}
						}
						break;
					}
				}
			}
		}
		catch (Exception exception2)
		{
			client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.ServerError"));
			client.Disconnect();
			GameServer.log.Error(LanguageMgr.GetTranslation("UserLoginHandler.ServerError"), exception2);
		}
		return 1;
	}
}
