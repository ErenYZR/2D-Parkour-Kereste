using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform PosA, PosB;
    [SerializeField] int speed;
    Vector2 targetPos;


    // Start is called before the first frame update
    void Start()
    {
        targetPos = PosB.position;   
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position,PosA.position) < 0.1f) targetPos=PosB.position;

		if (Vector2.Distance(transform.position, PosB.position) < 0.1f) targetPos = PosA.position;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
	}
}
