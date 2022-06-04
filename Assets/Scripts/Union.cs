using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Union : MonoBehaviour
{
    public Vector3 posicionVariable;
    [SerializeField]
    float velocidad = 5f;
    [SerializeField]
    float frecuencia = 20f;
    [SerializeField]
    float magnitud = 0.5f;

    public GameObject hechicero;
    public GameObject clon;

    float distanciaX;
    float distanciaZ;

    bool moviendoDerecha = true;

    Vector3 direccion;
    Vector3 centro;

    void Update()
    {
        // Mantiene el sistema de partículas entre el hechicero y su clon
        distanciaX = (hechicero.transform.position.x - clon.transform.position.x) / 2;
        distanciaZ = (hechicero.transform.position.z - clon.transform.position.z) / 2;

        centro = new Vector3(clon.transform.position.x + distanciaX, clon.transform.position.y, clon.transform.position.z + distanciaZ);

        if (gameObject.name.Contains("Enlace"))
        {
            if (transform.position != centro)
            {
                transform.GetChild(0).GetComponent<Union>().posicionVariable = Vector3.zero;
                transform.position = centro;
            }
        }
        else
        {
            if (clon.transform.position.x > hechicero.transform.position.x)
            {
                direccion = clon.transform.position - hechicero.transform.position;

                if (transform.position.x > clon.transform.position.x)
                {
                    moviendoDerecha = false;
                }
                else if (transform.position.x < hechicero.transform.position.x)
                {
                    moviendoDerecha = true;
                }
            }
            else
            {
                direccion = hechicero.transform.position - clon.transform.position;

                if (transform.position.x > hechicero.transform.position.x)
                {
                    moviendoDerecha = false;
                }
                else if (transform.position.x < clon.transform.position.x)
                {
                    moviendoDerecha = true;
                }
            }

            if (moviendoDerecha)
            {
                MoverDerecha();
            }
            else
            {
                MoverIzquierda();
            }


        }
    }

    void MoverDerecha()
    {
        posicionVariable += direccion.normalized * Time.deltaTime * velocidad;
        transform.localPosition = posicionVariable + transform.up * Mathf.Sin(Time.time * frecuencia) * magnitud;
    }

    void MoverIzquierda()
    {
        posicionVariable -= direccion.normalized * Time.deltaTime * velocidad;
        transform.localPosition = posicionVariable + transform.up * Mathf.Sin(Time.time * frecuencia) * magnitud;
    }
}
