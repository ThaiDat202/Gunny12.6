using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using log4net;

namespace Game.Base
{
	public class BaseConnector : BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly int RECONNECT_INTERVAL = 10000;

		private SocketAsyncEventArgs e;

		private IPEndPoint _remoteEP;

		private bool _autoReconnect;

		private Timer timer;

		private bool flag;

		public IPEndPoint RemoteEP => _remoteEP;

		public bool Connect()
		{
			try
			{
				m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				m_readBufEnd = 0;
				m_sock.Connect(_remoteEP);
				if (!flag)
				{
					log.InfoFormat("Connected to {0}", _remoteEP);
				}
			}
			catch
			{
				if (!flag)
				{
					log.ErrorFormat("Connect {0} failed!", _remoteEP);
				}
				m_sock = null;
				return false;
			}
			OnConnect();
			ReceiveAsync();
			return true;
		}

		private void TryReconnect()
		{
			if (Connect())
			{
				if (timer != null)
				{
					timer.Dispose();
					timer = null;
				}
				ReceiveAsync();
				return;
			}
			if (!flag)
			{
				log.ErrorFormat("Reconnect {0} failed:", _remoteEP);
				log.ErrorFormat("Retry after {0} ms!", RECONNECT_INTERVAL);
			}
			if (timer == null)
			{
				timer = new Timer(RetryTimerCallBack, this, -1, -1);
			}
			timer.Change(RECONNECT_INTERVAL, -1);
		}

		private static void RetryTimerCallBack(object target)
		{
			if (target is BaseConnector baseConnector)
			{
				baseConnector.TryReconnect();
			}
			else
			{
				log.Error("BaseConnector retryconnect timer return NULL!");
			}
		}

		public BaseConnector(string ip, int port, bool autoReconnect, byte[] readBuffer, byte[] sendBuffer)
			: base(readBuffer, sendBuffer)
		{
			_remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
			_autoReconnect = autoReconnect;
			e = new SocketAsyncEventArgs();
		}

		public BaseConnector(string ip, int port, bool autoReconnect, byte[] readBuffer, byte[] sendBuffer, bool flag)
			: this(ip, port, autoReconnect, readBuffer, sendBuffer)
		{
			this.flag = flag;
		}
	}
}
