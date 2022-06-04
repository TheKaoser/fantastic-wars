using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuego : MonoBehaviour
{
    Vector3 destino;
	GameObject jugador;
	string ignorar;
	Vector3 dirObjetivo;
	float momentoDisparo;
	string nombreFuego;
	
	public void Lanzar (Vector3 dest, string hechicero, string nFuego)
	{
		momentoDisparo = Time.time;
		jugador = GameObject.Find("Jugador");
		destino = dest;
		ignorar = hechicero;
		nombreFuego = nFuego;
	}

	void Update () 
	{
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destino, (Time.time - momentoDisparo) * 0.2f);
		gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.one * 0.1f, (Time.time - momentoDisparo) * 0.2f);
		if (Vector2.Distance(gameObject.transform.position, destino) < 100)
		{
			Destroy (gameObject, 0.2f);
			if (jugador.GetComponentInChildren<Control>().aliados[0].objeto.name == ignorar)
			{
				jugador.GetComponent<Servidor>().MatarAliadoServidor(nombreFuego);
			}
		}
		dirObjetivo = destino - transform.position;
		Vector3 nuevaDir = Vector3.RotateTowards(new Vector3(0f, 0f, 1f), dirObjetivo, 10, 0f);
		Debug.DrawRay(transform.position, nuevaDir, Color.red);
		transform.rotation = Quaternion.LookRotation(nuevaDir);
	}


	void OnTriggerEnter(Collider otro)
	{
		if (otro.CompareTag("Bloqueo") || (gameObject.CompareTag("Enemigo") && otro.gameObject.CompareTag("No Bloqueo") || otro.name == "Muralla"))
		{
            if (jugador.GetComponentInChildren<Control>().aliados[0].objeto.name == ignorar)
			{
				jugador.GetComponent<Servidor>().MatarAliadoServidor(nombreFuego);
			}
            Destroy(gameObject);
        }
		int indice = jugador.GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto == otro.gameObject);
		if (indice != -1 && otro.gameObject.name != ignorar && !otro.CompareTag("Agua"))
		{

			jugador.GetComponent<Servidor>().AsignarVidaAliadoServidor(int.Parse(jugador.GetComponentInChildren<Control>().aliados[indice].objeto.name.Split(':')[1]), jugador.GetComponentInChildren<Control>().aliados[indice].vida - 1);
		}
	}
}