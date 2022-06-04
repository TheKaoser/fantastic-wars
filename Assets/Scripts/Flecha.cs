using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour {

	Vector3 destino;
	GameObject jugador;
	string ignorar;
	Vector3 dirObjetivo;
	
	void Update () 
	{
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destino, 0.1f);
		dirObjetivo = destino - transform.position;
		Vector3 nuevaDir = Vector3.RotateTowards(new Vector3(0f, 0f, 1f), dirObjetivo, 10, 0f);
		transform.rotation = Quaternion.LookRotation(nuevaDir);
		// Si no impacta con nadie se destruye al llegar al destino
		if (Vector3.Distance(gameObject.transform.position, destino) < 10)
		{
			Destroy(gameObject, 0.3f);
		}
	}

	public void Lanzar (Vector3 dest, string arquero)
	{
		jugador = GameObject.Find("Jugador");
		destino = dest;
		ignorar = arquero;
	}

	void OnTriggerEnter(Collider otro)
	{
		int indice = jugador.GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == otro.gameObject.name);
		if (indice != -1 && otro.gameObject.name != ignorar)
		{
			jugador.GetComponent<Servidor>().AsignarVidaAliadoServidor(int.Parse(jugador.GetComponentInChildren<Control>().aliados[indice].objeto.name.Split(':')[1]), jugador.GetComponentInChildren<Control>().aliados[indice].vida - 1);
            Destroy(gameObject, 0.1f);
        }
		else if (otro.CompareTag("Enemigo") && otro.gameObject.name != ignorar)
		{
			Destroy(gameObject, 0.1f);
		}
		else if (otro.CompareTag("Bloqueo") || (gameObject.CompareTag("Enemigo") && otro.gameObject.name != ignorar) || otro.name == "Muralla")
		{
			Destroy(gameObject, 0.1f);
		}
	}
}