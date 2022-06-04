using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trol : MonoBehaviour {

	GameObject jugador;
	Color azul;
	Color rojo;
	Color amarillo;

	void Start () 
	{
		jugador = GameObject.Find("Jugador");
		if (gameObject.transform.parent.name.Split(':')[0] == jugador.GetComponentInChildren<Control>().nombresJugadoresaux[0])
		{
			azul = Color.blue;
			azul.a = 0.25f;
			GetComponent<Image>().color = azul;
		}
		else if (gameObject.transform.parent.name.Split(':')[0] == jugador.GetComponentInChildren<Control>().nombresJugadoresaux[1])
		{
			rojo = Color.red;
			rojo.a = 0.25f;
			GetComponent<Image>().color = rojo;
		}
		else if (gameObject.transform.parent.name.Split(':')[0] == jugador.GetComponentInChildren<Control>().nombresJugadoresaux[2])
		{
			amarillo = Color.yellow;
			amarillo.a = 0.25f;
			GetComponent<Image>().color = amarillo;
		}
	}

	void OnTriggerEnter(Collider otro)
	{
		int indice = jugador.GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == otro.gameObject.name);
		if (indice != -1 && jugador.GetComponent<Servidor>().nombresJugadores[0] == transform.parent.gameObject.name.Split(':')[0])
		{
			jugador.GetComponent<Servidor>().AsignarVidaAliadoServidor(int.Parse(jugador.GetComponentInChildren<Control>().aliados[indice].objeto.name.Split(':')[1]), jugador.GetComponentInChildren<Control>().aliados[indice].vida + 2);
		}
	}

	void OnTriggerExit(Collider otro)
	{
		int indice = jugador.GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == otro.gameObject.name);
		if (indice != -1 && jugador.GetComponent<Servidor>().nombresJugadores[0] == transform.parent.gameObject.name.Split(':')[0])
		{
			jugador.GetComponent<Servidor>().AsignarVidaAliadoServidor(int.Parse(jugador.GetComponentInChildren<Control>().aliados[indice].objeto.name.Split(':')[1]), jugador.GetComponentInChildren<Control>().aliados[indice].vida - 2);
		}
	}
}
