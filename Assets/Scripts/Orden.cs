using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orden : MonoBehaviour {

	void Start () 
	{
		if (gameObject.CompareTag("Mapa"))
		{
			GetComponent<SpriteRenderer>().sortingOrder = -(int)(GetComponent<BoxCollider>().center.y + transform.localPosition.y - (GetComponent<BoxCollider>().size.y * transform.localScale.y * transform.parent.localScale.y)/2);
		}
	}
	
	void Update () 
	{
		if (!gameObject.CompareTag("Mapa") && !gameObject.CompareTag("Estela"))
		{
			if (transform.parent)
			{
				GetComponent<SpriteRenderer>().sortingOrder = -(int)(transform.position.z - (GetComponent<BoxCollider>().size.y * transform.localScale.y * transform.parent.localScale.y)/2f);
			}
			else
			{
				GetComponent<SpriteRenderer>().sortingOrder = -(int)(transform.position.z - (GetComponent<BoxCollider>().size.y * transform.localScale.y)/2f);
			}
		}
		else if (gameObject.CompareTag("Estela"))
		{
			GetComponent<ParticleSystemRenderer>().sortingOrder = GetComponentInParent<SpriteRenderer>().sortingOrder - 1;
		}
	}
}
