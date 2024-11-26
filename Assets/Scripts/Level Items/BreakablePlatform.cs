using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
	[SerializeField] private float delay;
	PlayerMovement playerMovement;
	[SerializeField] GameObject player;
	private bool breaking;
	private Animator anim;


	private void Awake()
	{
		playerMovement = player.GetComponent<PlayerMovement>();
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		if(playerMovement.dead == true)
		{
			transform.localScale = Vector3.one;
		}

		anim.SetBool("breaking", breaking);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.transform.tag == "Player")
		{
			StartCoroutine(Breaking());
		}
	}

	private IEnumerator Breaking()
	{
		breaking = true;
		yield return new WaitForSeconds(delay);
		transform.localScale = Vector3.zero;
		breaking = false;
		yield return new WaitForSeconds(3);
		transform.localScale = Vector3.one;
	}
}
