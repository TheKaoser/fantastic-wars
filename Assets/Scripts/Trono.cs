using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trono : MonoBehaviour {

	// public List <Dictionary <bool, double>> tiemposEnTrono = new List<Dictionary<bool, double>>();
	public List <bool> tiemposEnTrono = new List<bool>();
	GameObject jugador;
	float relleno;
	Color azul;
	Color rojo;
	Color amarillo;

	void Start () 
	{
		jugador = GameObject.Find("Jugador");
		tiemposEnTrono.Add(false);
		tiemposEnTrono.Add(false);
		tiemposEnTrono.Add(false);

		azul = Color.blue;
		azul.a = 0.25f;

		rojo = Color.red;
		rojo.a = 0.25f;

		amarillo = Color.yellow;
		amarillo.a = 0.25f;
	}
	
	void Update ()
	{
		if (tiemposEnTrono.FindIndex(x => x == true) == -1)
		{
			if (relleno > 0)
			{
				relleno -= Time.deltaTime / 30;
			}
		}
		else if (tiemposEnTrono.FindAll(x => x == true).Count == 1)
		{
			if (tiemposEnTrono.FindIndex(x => x == true) == 0)
			{
				if (GetComponent<Image>().color == azul)
				{
					relleno += Time.deltaTime / 15;
				}
				else if (relleno > 0)
				{
					relleno -= Time.deltaTime / 15;
				}
				else
				{
					GetComponent<Image>().color = azul;
					relleno += Time.deltaTime / 15;
				}
			}
			else if (tiemposEnTrono.FindIndex(x => x == true) == 1)
			{
				if (GetComponent<Image>().color == rojo)
				{
					relleno += Time.deltaTime / 15;
				}
				else if (relleno > 0)
				{
					relleno -= Time.deltaTime / 15;
				}
				else
				{
					
					GetComponent<Image>().color = rojo;
					relleno += Time.deltaTime / 15;
				}
			}
			else
			{
				if (GetComponent<Image>().color == amarillo)
				{
					relleno += Time.deltaTime / 15;
				}
				else if (relleno > 0)
				{
					relleno -= Time.deltaTime / 15;
				}
				else
				{
					GetComponent<Image>().color = amarillo;
					relleno += Time.deltaTime / 15;
				}
			}
		}

		GetComponent<Image>().fillAmount = relleno;

		if (jugador.GetComponent<Servidor>().partidaEmpezada && relleno >= 1 && jugador.GetComponentInChildren<Control>().nombresJugadoresaux[tiemposEnTrono.FindIndex(x => x == true)] == jugador.GetComponent<Servidor>().nombresJugadores[0])
		{
			jugador.GetComponent<Servidor>().partidaEmpezada = false;
			jugador.GetComponent<Servidor>().GanarServidor();
		}
	}

	void OnTriggerEnter(Collider otro)
	{
		if (otro.gameObject == jugador.GetComponentInChildren<Control>().aliados[0].objeto)
		{
			jugador.GetComponent<Servidor>().AsignarEntradaTronoAliadoServidor(1);
		}
	}

	void OnTriggerExit(Collider otro)
	{
		if (otro.gameObject.name == jugador.GetComponent<Servidor>().nombresJugadores[0] + ":0")
		{
			jugador.GetComponent<Servidor>().AsignarEntradaTronoAliadoServidor(0);
		}
	}
}
