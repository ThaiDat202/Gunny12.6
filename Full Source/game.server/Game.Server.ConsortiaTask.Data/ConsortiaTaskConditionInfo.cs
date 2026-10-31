namespace Game.Server.ConsortiaTask.Data;

public class ConsortiaTaskConditionInfo
{
	public delegate void OnCompleted(Consortia consortia, ConsortiaTaskConditionInfo condition);

	public int ID { get; set; }

	public int TaskID { get; set; }

	public int CondictionType { get; set; }

	public string CondictionTitle { get; set; }

	public int Value { get; set; }

	public int TargetCount { get; set; }

	public int Finish { get; set; }

	public int TemplateID { get; set; }

	public bool MustWin { get; set; }

	public int MissionID { get; set; }

	public bool Completed => Value >= TargetCount;

	public event OnCompleted Complete;

	public void OnComplete(Consortia consortia, ConsortiaTaskConditionInfo condition)
	{
		this.Complete?.Invoke(consortia, condition);
	}
}
