using System;
using System.Reflection;
using System.Threading;
using Game.Base.Events;
using Game.Server;
using Game.Server.Packets;
using Game.Server.Packets.Client;
using log4net;

namespace Game.Base.Packets;

public class PacketProcessor
{
	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected IPacketHandler ActivePacketHandler;

	protected int HandlerThreadId = 0;

	protected GameClient Client;

	protected static readonly IPacketHandler[] PacketHandlers = new IPacketHandler[32767];

	public PacketProcessor(GameClient client)
	{
		Client = client;
	}

	public void HandlePacket(GSPacketIn packet)
	{
		int code = packet.Code;
		Statistics.BytesIn += packet.Length;
		Statistics.PacketsIn++;
		IPacketHandler packetHandler = null;
		if (code < PacketHandlers.Length)
		{
			packetHandler = PacketHandlers[code];
			if (packetHandler == null)
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine(string.Concat("PacketProcessor Received ePackageType.", (ePackageType)code, " <", $"0x{code:x}", ">"));
				Console.WriteLine("_________________________________");
				return;
			}
		}
		else if (Log.IsErrorEnabled)
		{
			Log.ErrorFormat("Received packet code is outside of m_packetHandlers array bounds! " + Client);
			Log.Error(Marshal.ToHexDump(string.Format("===> <{2}> Packet 0x{0:X2} (0x{1:X2}) length: {3} (ThreadId={4})", code, code ^ 0xA8, Client.TcpEndpoint, packet.Length, Thread.CurrentThread.ManagedThreadId), packet.Buffer));
		}
		if (packetHandler == null)
		{
			return;
		}
		long num = Environment.TickCount;
		try
		{
			if (Client != null && Client.TcpEndpoint != "not connected")
			{
				if (Client.Server.Configuration.DebugMode)
				{
					int num2 = code;
					int num3 = num2;
					if (num3 != 176 && num3 != 179 && num3 != 181)
					{
						Console.WriteLine(packetHandler);
					}
				}
				packetHandler.HandlePacket(Client, packet);
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				if (Client != null)
				{
					string tcpEndpoint = Client.TcpEndpoint;
					Log.Error("Error while processing packet (handler=" + packetHandler.GetType().FullName + "  client: " + tcpEndpoint + ")", exception);
				}
				Log.Error(Marshal.ToHexDump("Package Buffer:", packet.Buffer, 0, packet.Length));
			}
		}
		long num4 = Environment.TickCount - num;
		ActivePacketHandler = null;
		if (Log.IsDebugEnabled)
		{
			Log.Debug("Package process Time:" + num4 + "ms!");
		}
		if (num4 > 1500 && Client != null)
		{
			string tcpEndpoint2 = Client.TcpEndpoint;
			if (Log.IsWarnEnabled)
			{
				Log.Warn("(" + tcpEndpoint2 + ") Handle packet Thread " + Thread.CurrentThread.ManagedThreadId + " " + packetHandler?.ToString() + " took " + num4 + "ms!");
			}
		}
	}

	[ScriptLoadedEvent]
	public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
	{
		Array.Clear(PacketHandlers, 0, PacketHandlers.Length);
		int num = SearchPacketHandlers("v168", Assembly.GetAssembly(typeof(GameServer)));
		if (Log.IsInfoEnabled)
		{
			Log.Info("PacketProcessor: Loaded " + num + " handlers from GameServer Assembly!");
		}
	}

	public static void RegisterPacketHandler(int packetCode, IPacketHandler handler)
	{
		PacketHandlers[packetCode] = handler;
	}

	protected static int SearchPacketHandlers(string version, Assembly assembly)
	{
		int num = 0;
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsClass && !(type.GetInterface("Game.Server.Packets.Client.IPacketHandler") == null))
			{
				PacketHandlerAttribute[] array = (PacketHandlerAttribute[])type.GetCustomAttributes(typeof(PacketHandlerAttribute), inherit: true);
				if (array.Length != 0)
				{
					num++;
					RegisterPacketHandler(array[0].Code, (IPacketHandler)Activator.CreateInstance(type));
				}
			}
		}
		return num;
	}
}
