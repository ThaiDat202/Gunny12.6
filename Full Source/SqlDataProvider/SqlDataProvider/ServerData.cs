using System.Data.Entity;

namespace SqlDataProvider;

public class ServerData : DbContext
{
	public virtual DbSet<VipStoreList> VipStoreList { get; set; }

	public ServerData()
		: base("name=ServerData")
	{
	}

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
	}
}
