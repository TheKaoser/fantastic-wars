using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volumen : MonoBehaviour {

	// AudioSource [] audiosCamara;
    // [SerializeField]
    // GameObject sangre;
    // [SerializeField]
    // GameObject fuego;

	// void Awake()
	// {
	// 	audiosCamara = GameObject.Find("Camara").GetComponentsInChildren<AudioSource>();
	// }

    // // Se llama cuando subes o bajas el volumen de los efectos
    public void CambiarVolumen(float nuevoVolumen)
    {
    //     if (gameObject.name == "Slider Musica 1" || gameObject.name == "Slider Musica 2")
    //     {
    //         foreach (AudioSource audio in audiosCamara)
    //         {
    //             audio.volume = nuevoVolumen;
    //         }
    //     }
    //     else if (gameObject.name == "Slider Efectos 1" || gameObject.name == "Slider Efectos 2")
    //     {
    //         foreach (GameObject g in GameObject.FindGameObjectsWithTag("Aliado"))
    //         {
    //             foreach (AudioSource audio in g.GetComponentsInChildren<AudioSource>())
    //             {
    //                 audio.volume = nuevoVolumen;
    //             }
    //         }

    //         foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemigo"))
    //         {
    //             foreach (AudioSource audio in g.GetComponentsInChildren<AudioSource>())
    //             {
    //                 audio.volume = nuevoVolumen;
    //             }
    //         }

    //         foreach (AudioSource audio in GameObject.Find("Menu").GetComponentsInChildren<AudioSource>())
    //         {
    //             audio.volume = nuevoVolumen;
    //         }

    //         foreach (AudioSource audio in sangre.GetComponentsInChildren<AudioSource>())
    //         {
    //             audio.volume = nuevoVolumen;
    //         }

    //         foreach (AudioSource audio in fuego.GetComponentsInChildren<AudioSource>())
    //         {
    //             audio.volume = nuevoVolumen;
    //         }
    //     }
    }
}
