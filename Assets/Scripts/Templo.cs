using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Templo : MonoBehaviour
{
    Transform trono;
    float tiempo;
    GameObject jugador;
    int distancia;

    void Start ()
    {
        jugador = GameObject.Find("Jugador");
        trono = GameObject.Find("Trono").transform;
        distancia = (int)Vector3.Distance(trono.position, transform.position);
        tiempo = Time.time;
    }

    void Update()
    {
        // Crecimiento de oro por segundo
        if (Time.time - tiempo >= 10f)
        {
            if (distancia <= 500)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 10;
            }
            else if (distancia <= 1000)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 9;
            }
            else if (distancia <= 1500)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 8;
            }
            else if (distancia <= 2000)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 7;
            }
            else if (distancia <= 2500)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 6;
            }
            else if (distancia <= 3000)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 5;
            }
            else if (distancia <= 3500)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 4;
            }
            else if (distancia <= 4000)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 3;
            }
            else if (distancia <= 4500)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 2;
            }
            else if (distancia <= 5000)
            {
                jugador.GetComponentInChildren<Control>().cambiarOro += 1;
            }
            tiempo = Time.time;
        }

        // if (transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 1f/3f)
        // {
        //     gameObject.GetComponentInChildren<Animator>().Play("En ruinas");
        // }
        // else if (transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 2f/3f)
        // {
        //     gameObject.GetComponentInChildren<Animator>().Play("Mal estado");
        // }
    }
}
