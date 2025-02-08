using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	[SerializeField] GameObject player;
    [SerializeField] Transform teleportPoint;
	[SerializeField] Animator levelTransitionAnim;
	[SerializeField] TextMeshProUGUI levelNumberText;

	static int number;

	private void Start()
	{
		number = 1;
	}

	private void Update()
	{
		levelNumberText.text ="Level:" +number + "/6";
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
        {
			StartCoroutine(LevelTeleport());
			number++;
        }
	}
	

	public IEnumerator LevelTeleport()
	{
		levelTransitionAnim.SetBool("Change", true);
		yield return new WaitForSeconds(0.7f);
		player.transform.position = teleportPoint.position;
		yield return new WaitForSeconds(1.7f);
		levelTransitionAnim.SetBool("Change", false);
	}
}
