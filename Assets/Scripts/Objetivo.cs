using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objetivo : MonoBehaviour {

	public Color temporal;
	public int desvaneciendo = 0;
	public float velocidad;

	void OnEnable()
	{
		desvaneciendo = 0;
		if (gameObject.transform.parent && gameObject.transform.parent.tag == "Enemigo")
		{
			GetComponent<SpriteRenderer>().enabled = true;
			GetComponent<SpriteRenderer>().color = new Color32(255,80,80,255);
		}
		else if (gameObject.transform.parent && gameObject.transform.parent.tag == "Aliado")
		{
			GetComponent<Objetivo>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (desvaneciendo == 0)
		{
			temporal = GetComponent<SpriteRenderer>().color;
			temporal.a = Mathf.SmoothDamp(GetComponent<SpriteRenderer>().color.a, 1, ref velocidad, 0.1f);
			GetComponent<SpriteRenderer>().color = temporal;
			if (temporal.a > 0.8)
			{
				desvaneciendo = 1;
				velocidad = 0;
			}
		}
		else if (desvaneciendo == 1)
		{
			Invoke ("Desvanecer", 0.2f);
		}
		else if (desvaneciendo == 2)
		{
			temporal = GetComponent<SpriteRenderer>().color;
			temporal.a = Mathf.SmoothDamp(GetComponent<SpriteRenderer>().color.a, 0, ref velocidad, 0.1f);
			GetComponent<SpriteRenderer>().color = temporal;
			if (temporal.a < 0.01)
			{
				desvaneciendo = 3;
			}
		}
		else if (gameObject.name == "Objetivo(Clone)")
		{
			Destroy(gameObject, 1);
		}
		else
		{
			GetComponent<Objetivo>().enabled = false;
		}
	}

	void Desvanecer ()
	{
		desvaneciendo = 2;
	}
}
