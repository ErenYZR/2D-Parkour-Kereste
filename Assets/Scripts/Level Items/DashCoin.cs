using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCoin : MonoBehaviour
{
	[SerializeField] private float delay;
	PlayerMovement playerMovement;
	[SerializeField] GameObject player;

	private void Awake()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
	}

    void Update()
    {
		if (playerMovement.dead == true)
		{
			transform.localScale = Vector3.one;
		}
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.transform.tag == "Player")
		{
			StartCoroutine(TurningBack());
		}
	}

	private IEnumerator TurningBack()
	{
		transform.localScale = Vector3.zero;
		playerMovement.canDashCondition = true;
		playerMovement.dashingCooldown = 1f;
		Time.timeScale = 0.75f;
		yield return new WaitForSeconds(0.06f);
		Time.timeScale = 1f;
		yield return new WaitForSeconds(delay-0.1f);
		transform.localScale = Vector3.one;

	}
}
