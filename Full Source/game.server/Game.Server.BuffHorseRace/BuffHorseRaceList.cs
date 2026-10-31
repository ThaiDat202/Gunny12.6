using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.BuffHorseRace;

public class BuffHorseRaceList
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private object m_lock;

	protected List<AbstractHorseRaceBuffer> m_buffers;

	private UserHorseRaceInfo m_player;

	public BuffHorseRaceList(UserHorseRaceInfo player)
	{
		m_player = player;
		m_lock = new object();
		m_buffers = new List<AbstractHorseRaceBuffer>();
	}

	public bool AddBuffer(AbstractHorseRaceBuffer buffer)
	{
		lock (m_buffers)
		{
			m_buffers.Add(buffer);
		}
		return true;
	}

	public bool RemoveBuffer(AbstractHorseRaceBuffer buffer)
	{
		lock (m_buffers)
		{
			if (m_buffers.Remove(buffer))
			{
				return true;
			}
		}
		return false;
	}

	public virtual AbstractHorseRaceBuffer GetOfType(Type bufferType)
	{
		lock (m_buffers)
		{
			foreach (AbstractHorseRaceBuffer buffer in m_buffers)
			{
				if (buffer.GetType().Equals(bufferType))
				{
					return buffer;
				}
			}
		}
		return null;
	}

	public List<AbstractHorseRaceBuffer> GetAllBuffers()
	{
		List<AbstractHorseRaceBuffer> list = new List<AbstractHorseRaceBuffer>();
		lock (m_lock)
		{
			foreach (AbstractHorseRaceBuffer buffer in m_buffers)
			{
				list.Add(buffer);
			}
		}
		return list;
	}

	public void Update()
	{
		List<AbstractHorseRaceBuffer> allBuffers = GetAllBuffers();
		foreach (AbstractHorseRaceBuffer item in allBuffers)
		{
			try
			{
				if (!item.Check())
				{
					item.Stop();
				}
			}
			catch (Exception message)
			{
				log.Error(message);
			}
		}
	}

	public void StopAll()
	{
		List<AbstractHorseRaceBuffer> allBuffers = GetAllBuffers();
		foreach (AbstractHorseRaceBuffer item in allBuffers)
		{
			try
			{
				item.Stop();
			}
			catch (Exception message)
			{
				log.Error(message);
			}
		}
	}

	public AbstractHorseRaceBuffer CreateBuffer(BuffHorseRaceInfo info, int value)
	{
		AbstractHorseRaceBuffer result = null;
		switch (info.Type)
		{
		case 1:
			result = new BananaBuffer(info);
			break;
		case 5:
			result = new CopySpeedBuffer(info, value);
			break;
		case 7:
			result = new HourglassBuffer(info);
			break;
		case 3:
			result = new MagnetBuffer(info);
			break;
		case 2:
			result = new NailBuffer(info);
			break;
		case 6:
			result = new ShoesSpeedBuffer(info);
			break;
		}
		return result;
	}
}
