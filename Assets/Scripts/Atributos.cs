using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atributos : MonoBehaviour
{

    public int vida;
    public int daño;
    public float velocidadAtaque;
    public int rango;
    public int habilidadComun1;
    public int habilidadComun2;
    public int habilidadRara;
    public int habilidadLegendaria;

    void OnEnable()
    {
        velocidadAtaque = 1;
        AsignarAtributos();
    }

    public void AsignarAtributos()
    {
        if (gameObject.name.Contains("Rey Trasgo"))
        {
            vida = 5;
            habilidadComun1 = 10;
            habilidadComun2 = 15;
            habilidadRara = 30;
            habilidadLegendaria = 120;
        }
        else if (gameObject.name.Contains("Espadachin"))
        {
            vida = 2;
            daño = 1;
            rango = 150;
        }
        else if (gameObject.name.Contains("Arquero"))
        {
            vida = 1;
            daño = 1;
            velocidadAtaque = 2;
            rango = 1000;
        }
        else if (gameObject.name.Contains("Trol"))
        {
            vida = 8;
        }
        else if (gameObject.name.Contains("Hechicero Elemental"))
        {
            vida = 7;
            habilidadComun1 = 15;
            habilidadComun2 = 25;
            habilidadRara = 25;
            habilidadLegendaria = 75;
        }
        else if (gameObject.name.Contains("Diosa Divina"))
        {
            vida = 1;
            habilidadComun1 = 20;
            habilidadComun2 = 10;
            habilidadRara = 30;
            habilidadLegendaria = 80;
        }
        else if (gameObject.name.Contains("Angel"))
        {
            vida = 5;
            daño = 3;
            velocidadAtaque = 2.5f;
            rango = 300;
        }
        else if (gameObject.name.Contains("Templo"))
        {
            vida = 6;
        }
        else if (gameObject.name.Contains("Muralla"))
        {
            vida = 10;
        }
        else if (gameObject.name.Contains("Cuartel"))
        {
            vida = 8;
        }
        else if (gameObject.name.Contains("Maravilla"))
        {
            vida = 13;
            rango = 500;
        }
    }
}
