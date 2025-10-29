using UnityEngine;

public class Chunk_f : MonoBehaviour
{
	public GameObject parent_obj;

	private int X;

	private int Z;

	private string zone;

	private string chunkStr;

	public Chunk_f(int X, int Z, string zone)
	{
		this.X = X;
		this.Z = Z;
		this.zone = zone;
		chunkStr = NewBiomeControl.Instance.GetChunkString_(zone, X, Z);
		if (this.X == -1 && this.Z == 0 && zone == "overworld")
		{
			GameController.Instance.scenic_elevator.SetActive(true);
			Shop_positioner.Instance.breeder_base.SetActive(true);
		}
	}

	public void Delete()
	{
		Object.Destroy(parent_obj);
		if (NewBiomeControl.Instance.chunks_with_specials.ContainsKey(chunkStr))
		{
			foreach (place_special item in NewBiomeControl.Instance.chunks_with_specials[chunkStr])
			{
				if (item.type == 8)
				{
					NewBiomeControl.biome_obj biome_obj = NewBiomeControl.Instance.biomes[item.biome_id].biome_scenic[item.obj_id];
					for (int i = 0; i < biome_obj.width; i++)
					{
						for (int j = 0; j < biome_obj.width; j++)
						{
							remove_1x1quest(item.innerX + i, item.innerZ + j);
						}
					}
				}
				else
				{
					remove_1x1quest(item.innerX, item.innerZ);
				}
			}
		}
		if (X == -1 && Z == 0 && zone == "overworld")
		{
			GameController.Instance.scenic_elevator.SetActive(false);
			Shop_positioner.Instance.breeder_base.SetActive(false);
		}
	}

	private void remove_1x1quest(int x, int z)
	{
		string item = (float)X * NewBiomeControl.chunk_width + (float)x + "," + ((float)Z * NewBiomeControl.chunk_width + (float)z) + "," + zone;
		NewBiomeControl.Instance.quest_fills.Remove(item);
	}
}
