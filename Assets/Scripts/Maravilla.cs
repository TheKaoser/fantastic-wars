using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maravilla : MonoBehaviour
{
    Dictionary <string, float> subirVida = new Dictionary<string, float>();
    Dictionary <string, float> bajarVida = new Dictionary<string, float>();
    GameObject jugador;
    int indice;

    void Start()
    {
        jugador = GameObject.Find("Jugador");
        if (transform.parent.GetChild(0).CompareTag("Aliado"))
        {
            ParticleSystem.MinMaxGradient colores = new ParticleSystem.MinMaxGradient();
            colores.mode = ParticleSystemGradientMode.TwoColors;
            colores.colorMax = new Color32(50, 200, 70, 5);
            colores.colorMin = new Color32(30, 170, 30, 5);
            ParticleSystem sp = GetComponent<ParticleSystem>();
            var particulas = sp.main;
            particulas.startColor = colores;
        }
        GetComponent<ParticleSystem>().Play();
    }

    void Update()
    {
        List<string> sPersonajes = new List<string> (subirVida.Keys);
        foreach (string personaje in sPersonajes)
        {
            if (Time.time - subirVida[personaje] >= 1f)
            {
                subirVida[personaje] = Time.time;
                indice = jugador.GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == personaje);
                if (indice != -1)
                {
                    jugador.GetComponent<Servidor>().AsignarVidaAliadoServidor(int.Parse(jugador.GetComponentInChildren<Control>().aliados[indice].objeto.name.Split(':')[1]), jugador.GetComponentInChildren<Control>().aliados[indice].vida + 1);
                }
                else
                {
                    subirVida.Remove(personaje);
                }
            }
        }

        List<string> bPersonajes = new List<string> (bajarVida.Keys);
        foreach (string personaje in bPersonajes)
        {
            if (Time.time - bajarVida[personaje] >= 1f)
            {
                bajarVida[personaje] = Time.time;
                indice = jugador.GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == personaje);
                if (indice != -1)
                {
                    jugador.GetComponent<Servidor>().AsignarVidaAliadoServidor(int.Parse(jugador.GetComponentInChildren<Control>().aliados[indice].objeto.name.Split(':')[1]), jugador.GetComponentInChildren<Control>().aliados[indice].vida - 1);
                }
                else
                {
                    bajarVida.Remove(personaje);
                }
            }
        }
    }

    void OnTriggerEnter (Collider otro)
    {
        if (transform.parent.GetChild(0).CompareTag("Aliado") && otro.CompareTag("Aliado"))
        {
            subirVida.Add(otro.name, Time.time);
        }
        else if (transform.parent.GetChild(0).CompareTag("Enemigo") && otro.CompareTag("Aliado"))
        {
            bajarVida.Add(otro.name, Time.time);
        }
    }

    void OnTriggerExit (Collider otro)
    {
        if (transform.parent.GetChild(0).CompareTag("Aliado") && otro.CompareTag("Aliado"))
        {
            subirVida.Remove(otro.name);
        }
        else if (transform.parent.GetChild(0).CompareTag("Enemigo") && otro.CompareTag("Aliado"))
        {
            bajarVida.Remove(otro.name);
        }
    }
}
