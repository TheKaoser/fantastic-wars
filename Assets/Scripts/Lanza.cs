using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lanza : MonoBehaviour
{
    Vector3 destino;
	GameObject jugador;
	string ignorar;
	Vector3 dirObjetivo;
	
	void Update () 
	{
		dirObjetivo = destino - transform.position;
		Vector3 nuevaDir = Vector3.RotateTowards(new Vector3(0f, 0f, 1f), dirObjetivo, 10, 0f);
		transform.parent.rotation = Quaternion.LookRotation(nuevaDir);
	}

	public void Golpear (Vector3 dest, string angel)
	{
		destino = dest;
		ignorar = angel;
		Destroy(transform.parent.gameObject, 1f);
		jugador = GameObject.Find("Jugador");
	}

	void OnTriggerEnter(Collider otro)
	{
		int indice = jugador.GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == otro.gameObject.name);
		if (indice != -1 && otro.gameObject.name != ignorar && transform.parent.CompareTag("Enemigo"))
		{
			jugador.GetComponent<Servidor>().AsignarVidaAliadoServidor(int.Parse(jugador.GetComponentInChildren<Control>().aliados[indice].objeto.name.Split(':')[1]), jugador.GetComponentInChildren<Control>().aliados[indice].vida - 3);
		}
	}
}
