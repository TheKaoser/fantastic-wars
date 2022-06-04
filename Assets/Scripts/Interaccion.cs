using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaccion : MonoBehaviour {
	
	bool hacerGrande;
	bool hacerPequeño;
	float tamañoNuevoTexto;
	float tamañoAntiguoTexto;
	Vector3 tamañoNuevoImagen;
	Vector3 tamañoAntiguoImagen;
	AudioSource [] audios;

	void Start()
	{
		if (GetComponent<TextMesh>())
		{
			tamañoAntiguoTexto = GetComponent<TextMesh>().characterSize;
		}
		else if (GetComponent<Image>())
		{
            GameObject.Find("Interfaz").GetComponent<Interfaz>().RedimensionarInterfaz(PlayerPrefs.GetFloat("Interfaz"));
			tamañoAntiguoImagen = GetComponent<RectTransform>().localScale;
		}
		audios = GetComponents<AudioSource>();
	}

	// Actualiza las variables para hacer el texto más grande y reproduce el sonido al entrar el ratón
    void OnMouseEnter ()
    {
		hacerGrande = true;
		hacerPequeño = false;
		if (audios.Length==2)
		{
			audios[1].Play();
		}
    }

	// Actualiza las variables para hacer el texto más grande y reproduce el sonido al entrar el ratón
    void OnMouseDown ()
    {
		if (audios.Length>=1)
		{
			audios[0].Play();
		}
    }

    // Actualiza las variables para hacer el texto más pequeño al salir el ratón
    void OnMouseExit ()
    {
		hacerPequeño = true;
		hacerGrande = false;
	}

	void Update ()
	{
		if (GetComponent<TextMesh>())
		{
			tamañoNuevoTexto = GetComponent<TextMesh>().characterSize;
			if (hacerGrande && tamañoNuevoTexto < tamañoAntiguoTexto + tamañoAntiguoTexto/10)
			{
				GetComponent<TextMesh>().characterSize += tamañoAntiguoTexto/25;
			}
			else if (hacerPequeño && tamañoNuevoTexto > tamañoAntiguoTexto)
			{
				GetComponent<TextMesh>().characterSize -= tamañoAntiguoTexto/25;
			}
		}
		else if (GetComponent<Image>())
		{
			tamañoNuevoImagen = GetComponent<RectTransform>().localScale;
			if (hacerGrande && Vector3.Distance(tamañoNuevoImagen, tamañoAntiguoImagen + tamañoAntiguoImagen/7) > 0.1f)
			{
				GetComponent<RectTransform>().localScale += tamañoAntiguoImagen/12;
			}
			else if (hacerPequeño && Vector3.Distance(tamañoNuevoImagen, tamañoAntiguoImagen) > 0.1f)
			{
				GetComponent<RectTransform>().localScale -= tamañoAntiguoImagen/12;
			}
			else 
			{
				hacerPequeño = false;
			}
		}
	}

	public IEnumerator AumentarYDisminuir ()
	{
		if (!hacerGrande && !hacerPequeño)
		{
			tamañoAntiguoImagen = GetComponent<RectTransform>().localScale;
		}
		hacerGrande = true;
		yield return new WaitForSeconds(0.2f);
		hacerGrande = false;
		hacerPequeño = true;
	}
}
