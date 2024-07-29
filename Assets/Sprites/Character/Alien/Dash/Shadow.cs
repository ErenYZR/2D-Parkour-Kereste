using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelaySeconds;
    public GameObject ghost;
    // Start is called before the first frame update
    void Start()
    {
        ghostDelaySeconds = ghostDelay;  
    }

    // Update is called once per frame
    void Update()
    {
        if (ghostDelaySeconds > 0)
        { ghostDelaySeconds -= Time.deltaTime; }
        else
        {
            GameObject currentGhost = Instantiate(ghost, transform.position,transform.rotation );
            ghostDelaySeconds = ghostDelay;
        }    
    }
}
