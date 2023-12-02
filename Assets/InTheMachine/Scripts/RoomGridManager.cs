using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGridManager : MonoBehaviour
{
	[SerializeField] protected Grid roomGrid;
	#region Singleton + Awake
	private static RoomGridManager _singleton;
	public static RoomGridManager main
	{
		get => _singleton;
		private set
		{
			if (_singleton == null)
			{
				_singleton = value;
			}
			else if (_singleton != value)
			{
				Debug.LogWarning("RoomGridManager instance already exists, destroy duplicate!");
				Destroy(value);
			}
		}
	}

	private void Awake()
	{
		main = this;
	}
	#endregion

	public Vector2 GetCurrentRoom(Transform transform)
	{
		Vector3Int cell = roomGrid.WorldToCell(transform.position);
		return roomGrid.CellToWorld(cell);

    }
}
