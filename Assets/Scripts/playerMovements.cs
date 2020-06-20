using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovements : StereoConfiguration
{
	public float maxXMovements = 10.0f;
	public float maxXSpeed     = 0.5f;
	private float xMove;
	GameObject player;

	// Start is called before the first frame update
	void Start()
    {
	    player = GameObject.Find("Player");
    }

	// Update is called once per frame
	void Update()
	{
		if (xMove < maxXMovements)
		{ 
			xMove += maxXSpeed;
	    }
		else
		{
			xMove = 0.0f;
		}
        player.transform.position = new Vector3( xMove, 1.0f, 6.0f);
	}
}
