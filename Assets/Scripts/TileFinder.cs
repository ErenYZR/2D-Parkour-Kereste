using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFinder : MonoBehaviour
{
	public Tilemap tilemap;
	public Vector3Int position; // Tilemap grid pozisyonu

	void Start()
	{
		TileBase tile = tilemap.GetTile(position);
		if (tile != null)
		{
			Debug.Log("Bulunan Tile: " + tile.name);
		}
		else
		{
			Debug.Log("Bu pozisyonda bir Tile yok.");
		}
	}
}