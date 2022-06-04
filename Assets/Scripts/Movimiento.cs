using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movimiento : MonoBehaviour {

	public NavMeshAgent agente;
	public bool colisionEnemigo;
	GameObject jugador;
	public List<GameObject> personajesColisionados;
    Vector3 destinoDesplazamiento;
    bool activarDesplazamiento;
    string nombreViento;
    float velocidadAgente;
    int restauraciones;

	void Start()
	{
		personajesColisionados = new List<GameObject>();
		agente = GetComponent<NavMeshAgent> ();
		agente.destination = gameObject.transform.GetChild(0).position;
		jugador = GameObject.Find("Jugador");
        velocidadAgente = agente.speed;
	}

	public void ActualizarPosicion (Vector3 destino)
	{
        if (activarDesplazamiento)
        {
            activarDesplazamiento = false;
            agente.speed = velocidadAgente;
            jugador.GetComponent<Servidor>().MatarAliadoServidor(nombreViento);
            gameObject.GetComponentInChildren<ParticleSystem>().Stop(true);
        }
		agente.destination = destino;
    }

    void Update()
    {
        // Mira si hay algun aliado o enemigo colisionado con el mismo destino que tu y en caso afirmativo se corrige el destino a la posicion actual
        for (int i = 0; i < jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().enemigos.Count; i++)
        {
            if (personajesColisionados.Contains(jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().enemigos[i].objeto) && jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().enemigos[i].objeto.transform.parent.GetComponent<Movimiento>() && Vector3.Distance(jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().enemigos[i].objeto.transform.parent.GetComponent<Movimiento>().agente.destination, agente.destination) < 100)
            {
                agente.destination = gameObject.transform.position;
            }
        }
        for (int i = 0; i < jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().aliados.Count; i++)
        {
            if (personajesColisionados.Contains(jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().aliados[i].objeto) && jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().aliados[i].objeto.transform.parent.GetComponent<Movimiento>() && Vector3.Distance(jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().aliados[i].objeto.transform.parent.GetComponent<Movimiento>().agente.destination, agente.destination) < 100)
            {
                agente.destination = gameObject.transform.position;
            }
        }

        // Mira si se está colisionando a algún atacado para activar el flag
        bool encontrado = false;
        for (int i = 1; i < jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().enemigos.Count; i++)
        {
            if (jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().enemigos[i].objetivos.Count == 0)
            {
                continue;
            }
            else if (personajesColisionados.Find(x => x.Equals(jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().enemigos[i].objetivo)))
            {
                colisionEnemigo = true;
                encontrado = true;
                break;
            }
        }
        if (!encontrado)
        {
            for (int i = 1; i < jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().aliados.Count; i++)
            {
                if (jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().aliados[i].objetivos.Count == 0)
                {
                    continue;
                }
                else if (personajesColisionados.Find(x => x.Equals(jugador.transform.GetChild(0).GetChild(0).GetComponent<Control>().aliados[i].objetivo)))
                {
                    colisionEnemigo = true;
                    encontrado = true;
                    break;
                }
            }
        }
        if (!encontrado)
        {
            colisionEnemigo = false;
        }

        if (activarDesplazamiento)
        {
            if (Vector3.Distance(transform.position, destinoDesplazamiento) < 10 || transform.hasChanged == false)
            {
                activarDesplazamiento = false;
                agente.speed = velocidadAgente;
                jugador.GetComponent<Servidor>().MatarAliadoServidor(nombreViento);
                gameObject.GetComponentInChildren<ParticleSystem>().Stop(true);
            }
            else
            {
                transform.hasChanged = false;
            }
        }
    }

    public void DesplazamientoHechicero (Vector3 destino, string nViento)
    {
        agente.speed = 1000;
        agente.destination = destino;
        activarDesplazamiento = true;
        destinoDesplazamiento = destino;
        nombreViento = nViento;
    }

    public void BajarVelocidadEnTierra (bool invocador)
    {
        if (!invocador || !activarDesplazamiento)
        {
            agente.speed /= 5f;
            restauraciones++;
        }
    }
    
    public void RestaurarVelocidad ()
    {
        if (restauraciones > 0)
        {
            agente.speed *= 5;
            restauraciones--;
        }
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.GetComponent<Collider>().gameObject.name.Split(':')[0] != gameObject.name.Split(':')[0])
        {
            personajesColisionados.Add(otro.GetComponent<Collider>().gameObject);
        }
    }

    void OnTriggerExit (Collider otro)
	{
        if (otro.gameObject)
        {
            if (otro.GetComponent<Collider>().gameObject.name.Split(':')[0] != gameObject.name.Split(':')[0])
            {
                for (int i = personajesColisionados.Count - 1; i >= 0; i--)
                {
                    if (!personajesColisionados[i])
                    {
                        personajesColisionados.RemoveAt(i);
                    }
                }
                if (personajesColisionados.FindIndex(x => x.name == otro.gameObject.transform.name) >= 0)
                {
                    personajesColisionados.RemoveAt(personajesColisionados.FindIndex(x => x.name == otro.gameObject.transform.name));
                }
            }
        }
    }
}