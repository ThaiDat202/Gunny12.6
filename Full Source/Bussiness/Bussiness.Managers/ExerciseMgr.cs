using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ExerciseMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ExerciseInfo> _exercises;

	private static Random rand;

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool Init()
	{
		try
		{
			_exercises = new Dictionary<int, ExerciseInfo>();
			rand = new Random();
			return LoadExercise(_exercises);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ExercisesMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ExerciseInfo> dictionary = new Dictionary<int, ExerciseInfo>();
			if (LoadExercise(dictionary))
			{
				try
				{
					_exercises = dictionary;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ExerciseMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadExercise(Dictionary<int, ExerciseInfo> exercise)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ExerciseInfo[] allExercise = produceBussiness.GetAllExercise();
			ExerciseInfo[] array = allExercise;
			foreach (ExerciseInfo exerciseInfo in array)
			{
				if (!exercise.ContainsKey(exerciseInfo.Grage))
				{
					exercise.Add(exerciseInfo.Grage, exerciseInfo);
				}
			}
		}
		return true;
	}

	public static ExerciseInfo FindExercise(int grage)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (_exercises.ContainsKey(grage))
			{
				return _exercises[grage];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static int GetMaxLevel()
	{
		return _exercises.Values.Count;
	}

	public static ExerciseInfo GetExercise(int gp)
	{
		if (gp >= FindExercise(GetMaxLevel()).GP)
		{
			return FindExercise(GetMaxLevel());
		}
		for (int num = GetMaxLevel(); num >= 1; num--)
		{
			if (gp >= FindExercise(num).GP)
			{
				return FindExercise(num);
			}
		}
		return new ExerciseInfo
		{
			Grage = 1,
			ExerciseA = 1,
			ExerciseD = 1,
			ExerciseAG = 1,
			ExerciseL = 1,
			ExerciseH = 1
		};
	}
}
