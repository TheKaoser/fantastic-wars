using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suicidio : MonoBehaviour {

	GameObject jugador;
    bool invocadoDentro;

	void Start () 
	{
		jugador = GameObject.Find("Jugador");
	}

    void Update()
    {
        Invoke("MatarPersonaje", 0.1f);
    }

    void OnTriggerEnter(Collider otro)
	{
		if (otro.gameObject.CompareTag("Fuera"))
		{
			if (gameObject.CompareTag("Aliado"))
			{
				gameObject.tag = "Muerto";
				jugador.GetComponent<Servidor>().MatarAliadoServidor(gameObject.name.Split(':')[1]);
			}
		}

        if (otro.gameObject.CompareTag("Dentro") )
        {
            invocadoDentro = true;
        }
    }

	void OnTriggerExit (Collider otro)
	{
		if (otro.gameObject.CompareTag("Dentro"))
		{
			if (gameObject.CompareTag("Aliado"))
			{
				gameObject.tag = "Muerto";
				jugador.GetComponent<Servidor>().MatarAliadoServidor(gameObject.name.Split(':')[1]);
			}
		}
    }

    void MatarPersonaje()
    {
        if (!invocadoDentro)
        {
            gameObject.tag = "Muerto";
            jugador.GetComponent<Servidor>().MatarAliadoServidor(gameObject.name.Split(':')[1]);
            invocadoDentro = false;
        }
    }
}
