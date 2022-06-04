using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour {

	int limitePantalla = 100;
	float sensibilidad = 250;
	float velocidadX;
	float velocidadY;
	float velocidadZ;
	int pantallaAlto, pantallaAncho;
	float horizontal, vertical;
	Vector3 posicion;
	float zoom;
	Vector2 valoresX = new Vector2(-3000, 6000);
	Vector2 valoresY = new Vector2();
	Vector2 valoresZ = new Vector2(-3185, 3300);
    bool ladoX;
	bool ladoZ;
	float yObjetivo;
	GameObject aspirante;
	bool camaraFija = true;

	void Start () {
		pantallaAncho = Screen.width;
		pantallaAlto = Screen.height;
		valoresY.x = transform.position.y - 1300;
		valoresY.y = transform.position.y + 300;
		yObjetivo = -10415.69f;
		aspirante = GameObject.Find("Jugador").transform.GetChild(0).GetChild(0).gameObject;
	}

	void Update () {

		posicion = transform.position;

		if (Input.touchCount >= 1)
		{

		}

		if (Input.mousePosition.x < pantallaAncho - limitePantalla && Input.mousePosition.x > 0 + limitePantalla)
		{
			ladoX = false;
			velocidadX = 0;
		}

		if (Input.mousePosition.y < pantallaAlto - limitePantalla && Input.mousePosition.y > 0 + limitePantalla)
		{
			ladoZ = false;
			velocidadZ = 0;
		}

		if (Input.mousePosition.x > pantallaAncho - limitePantalla && posicion.x < valoresX.y)
		{
			if (!ladoX)
			{
				horizontal = Input.GetAxis("Mouse X") * sensibilidad;
				ladoX = true;
			}

			if (horizontal >= 0)
			{
				posicion.x = Mathf.SmoothDamp(posicion.x, posicion.x + horizontal, ref velocidadX, 0.5f);
				posicion.x = Mathf.Clamp(posicion.x, valoresX.x, valoresX.y);
			}
		}

		if (Input.mousePosition.x < 0 + limitePantalla && posicion.x > valoresX.x)
		{
			if (!ladoX)
			{
				horizontal = Input.GetAxis("Mouse X") * sensibilidad;
				ladoX = true;
			}

			if (horizontal <= 0)
			{
				posicion.x = Mathf.SmoothDamp(posicion.x, posicion.x + horizontal, ref velocidadX, 0.5f);
				posicion.x = Mathf.Clamp(posicion.x, valoresX.x, valoresX.y);
			}
		}

		if (Input.mousePosition.y > pantallaAlto - limitePantalla && posicion.z < valoresZ.y)
		{
			if (!ladoZ)
			{
				vertical = Input.GetAxis("Mouse Y") * sensibilidad;
				ladoZ = true;
			}

			if (vertical >= 0)
			{
				posicion.z = Mathf.SmoothDamp(posicion.z, posicion.z + vertical, ref velocidadZ, 0.5f);
				posicion.z = Mathf.Clamp(posicion.z, valoresZ.x, valoresZ.y);
			}
		}

		if (Input.mousePosition.y < 0 + limitePantalla && posicion.z > valoresZ.x)
		{
			if (!ladoZ)
			{
				vertical = Input.GetAxis("Mouse Y") * sensibilidad;
				ladoZ = true;
			}

			if (vertical <= 0)
			{
				posicion.z = Mathf.SmoothDamp(posicion.z, posicion.z + vertical, ref velocidadZ, 0.5f);
				posicion.z = Mathf.Clamp(posicion.z, valoresZ.x, valoresZ.y);
			}
		}

		// Rueda
		zoom = Input.GetAxis("Mouse ScrollWheel") * 5000;
		if(zoom!=0)
		{
			yObjetivo = posicion.y - zoom;
		}
		posicion.y = Mathf.SmoothDamp(posicion.y, yObjetivo, ref velocidadY, 0.3f);
		posicion.y = Mathf.Clamp(posicion.y, valoresY.x, valoresY.y);

		if (Input.GetKey(KeyCode.Space) || camaraFija)
		{
			if (aspirante)
			{
				posicion = new Vector3 (aspirante.transform.position.x, posicion.y, aspirante.transform.position.z);
			}
		}

		if (Input.GetKeyDown(KeyCode.F))
		{
			camaraFija = !camaraFija;
		}

		transform.position = posicion;
	}
}