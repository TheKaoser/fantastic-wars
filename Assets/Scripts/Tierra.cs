using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tierra : MonoBehaviour
{
    GameObject jugador;

    void Start()
    {
        if (gameObject.name != "Tentaculo")
        {
            jugador = GameObject.Find("Jugador");
            Invoke ("Destruir", 25);
        }
    }

    void Destruir ()
    {
        GetComponent<AudioSource>().Play();
        GetComponent<Animator>().Play("Tierra abajo");
        Destroy (gameObject, 2f);
        transform.position = new Vector3(transform.position.x, -12640, transform.position.z);

        if (gameObject.name.Split(':')[0] == jugador.GetComponent<Servidor>().nombresJugadores[0])
        {
            jugador.GetComponent<Servidor>().MatarAliadoServidor(gameObject.name.Split(':')[1]);
        }
    }

    void OnTriggerEnter (Collider otro)
    {
        // Si tiene script de movimiento es un personaje
        if (otro.gameObject.GetComponentInParent<Movimiento>()) 
        {
            otro.gameObject.GetComponentInParent<Movimiento>().BajarVelocidadEnTierra(otro.gameObject.name.Split(':')[0] == gameObject.transform.parent.name.Split(':')[0]);
        }
    }

    void OnTriggerExit(Collider otro)
    {
        // Si tiene script de movimiento es un personaje
        if (otro.gameObject.GetComponentInParent<Movimiento>()) 
        {
            otro.gameObject.GetComponentInParent<Movimiento>().RestaurarVelocidad();

        }
    }
}
