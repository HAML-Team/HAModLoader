using System;

public class looted_chest
{
	public DateTime respawns_at;

	public int container_id;

	public looted_chest(int container_id, bool auto_generate)
	{
		this.container_id = container_id;
		if (auto_generate)
		{
			respawns_at = DateTime.Now.AddHours(10.0);
		}
	}
}
