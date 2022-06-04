using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agua : MonoBehaviour
{
    GameObject jugador;

    void Start()
    {
        jugador = GameObject.Find("Jugador");
        Invoke ("Destruir", 25);    
    }

    void Destruir ()
    {
        // GetComponentsInChildren<AudioSource>()[0].Play();
        // Particulas muriendo
        // Destroy (gameObject, 2f);

        if (transform.GetChild(0).name.Split(':')[0] == jugador.GetComponent<Servidor>().nombresJugadores[0])
        {
            jugador.GetComponent<Servidor>().MatarAliadoServidor(transform.GetChild(0).name.Split(':')[1]);
        }
    }
}
