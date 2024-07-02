using UnityEngine;

public class Door : MonoBehaviour
{
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject cam;
	[SerializeField] private GameObject entry;
	[SerializeField] private GameObject roomcampoint;
	private Animator anim;




	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			player.transform.position = entry.transform.position;
			cam.transform.position = roomcampoint.transform.position;
		}
	}
}
