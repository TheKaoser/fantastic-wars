using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cuartel : MonoBehaviour
{
    Vector3 posicion;
    GameObject jugador;
    Dictionary<string, object> p;
    Dictionary<string, Dictionary<string, object>> listaPersonajes;
    public bool creado;

    void Start()
    {
        jugador = GameObject.Find("Jugador");
    }

    void Update()
    {
        // Invocación de ángel cada 45 segundos
        if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && transform.GetChild(0).CompareTag("Aliado") && !creado)
        {
            // Posición aleatoria en el perímetro del cuartel
            do
            {
                posicion = new Vector3(gameObject.transform.position.x + Random.Range(-100000, 100000), gameObject.transform.position.y, gameObject.transform.position.z + Random.Range(-100000, 100000));
            } 
            while (Vector3.Distance(posicion, transform.position) < 5000);

            posicion = gameObject.transform.GetChild(0).GetComponent<BoxCollider>().ClosestPointOnBounds(posicion);

            listaPersonajes = new Dictionary<string, Dictionary<string, object>>();
            p = new Dictionary<string, object>();

            p.Add("d", posicion + "");
            p.Add("t", "Angel");

            listaPersonajes.Add(int.Parse(jugador.GetComponentInChildren<Control>().aliados[jugador.GetComponentInChildren<Control>().aliados.Count - 1].objeto.name.Split(':')[1]) + 1 + "", p);
            jugador.GetComponent<Servidor>().CrearNuevoPersonaje(listaPersonajes);

            creado = true;
        }
    }
}
