using System.Collections.Generic;
using Bussiness;
using EntityDatabase.ServerModels;

namespace Game.Server.ConsortiaTask;

public class ConsortiaTaskData
{
	private readonly List<Consortia_Task_Conditions>[,] _tasks = new List<Consortia_Task_Conditions>[5, 10];

	public void AddTask(Consortia_Task_Conditions taskConditionData)
	{
		if (_tasks[taskConditionData.CondictionType - 1, taskConditionData.Level - 1] == null)
		{
			_tasks[taskConditionData.CondictionType - 1, taskConditionData.Level - 1] = new List<Consortia_Task_Conditions>();
		}
		_tasks[taskConditionData.CondictionType - 1, taskConditionData.Level - 1].Add(taskConditionData);
	}

	public Consortia_Task_Conditions GetTaskConditionDataInfo(int type, int level)
	{
		_tasks[type - 1, level - 1].Shuffle();
		return _tasks[type - 1, level - 1].Random();
	}
}
