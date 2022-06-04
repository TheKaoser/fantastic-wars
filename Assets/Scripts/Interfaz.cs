using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Interfaz : MonoBehaviour 
{
    GameObject habilidadComun1;
    GameObject habilidadComun2;
    GameObject habilidadRara;
    GameObject habilidadLegendaria;
    GameObject accion;

    GameObject barraVida;

    GameObject destello;
    public Text cantidad;
    GameObject elegirIzquierda;
    GameObject elegirDerecha;

    GameObject aspirante;

    Vector2 velocidad1 = Vector2.zero;
    Vector2 velocidad2 = Vector2.zero;
    Vector2 velocidad3 = Vector2.zero;
    Vector2 velocidad4 = Vector2.zero;
    Vector2 velocidad5 = Vector2.zero;
    Vector2 velocidad6 = Vector2.zero;
    Vector2 velocidad7 = Vector2.zero;
    Vector2 velocidad8 = Vector2.zero;
    float velocidad9;

    Ray rayo3D;
    RaycastHit colisionado3D;

    public GameObject [] aspirantes;
    public GameObject particulasSpawn;
    public Sprite [] habilidades;
    int indiceAnimador;
    string tipo;
    int vida;

    Color temporal;

    bool colocandoInterfaz = false;
    bool interfazColocada;

    public Canvas contenedorHab1;
    public Canvas contenedorHab2;
    public Canvas contenedorHab3;
    public Canvas contenedorHab4;
    public Canvas contenedorAccion;

    // Use this for initialization
    void OnEnable ()
    {
        habilidadComun1 = GameObject.Find("Habilidad comun 1");
        habilidadComun2 = GameObject.Find("Habilidad comun 2");
        habilidadRara = GameObject.Find("Habilidad rara");
        habilidadLegendaria = GameObject.Find("Habilidad legendaria");
        accion = GameObject.Find("Accion");
        destello = GameObject.Find("Destello");
        elegirDerecha = GameObject.Find("Elegir derecha");
        elegirIzquierda = GameObject.Find("Elegir izquierda");
        cantidad = GameObject.Find("Cantidad").GetComponent<Text>();
        ActivarFlechas();
        barraVida = GameObject.Find("Barra de vida");
        foreach (Renderer r in GameObject.Find("Menu opciones 2").GetComponentsInChildren<Renderer>())
        {
            r.sortingOrder = 11000;
        }
        indiceAnimador = 0;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (colocandoInterfaz)
        {
            // Aparición de las habilidades
            habilidadComun1.GetComponent<RectTransform>().anchorMin = Vector2.SmoothDamp(new Vector2(habilidadComun1.GetComponent<RectTransform>().anchorMin.x, habilidadComun1.GetComponent<RectTransform>().anchorMin.y), new Vector2(0.019f, 0.1671322f), ref velocidad1, 0.1f);
            habilidadComun1.GetComponent<RectTransform>().anchorMax = Vector2.SmoothDamp(new Vector2(habilidadComun1.GetComponent<RectTransform>().anchorMax.x, habilidadComun1.GetComponent<RectTransform>().anchorMax.y), new Vector2(0.09483721f, 0.3001322f), ref velocidad2, 0.1f);
            
            habilidadComun2.GetComponent<RectTransform>().anchorMin = Vector2.SmoothDamp(new Vector2(habilidadComun2.GetComponent<RectTransform>().anchorMin.x, habilidadComun2.GetComponent<RectTransform>().anchorMin.y), new Vector2(0.09483721f, 0.3001322f), ref velocidad3, 0.1f);
            habilidadComun2.GetComponent<RectTransform>().anchorMax = Vector2.SmoothDamp(new Vector2(habilidadComun2.GetComponent<RectTransform>().anchorMax.x, habilidadComun2.GetComponent<RectTransform>().anchorMax.y), new Vector2(0.1705116f, 0.434f), ref velocidad4, 0.1f);
            
            habilidadRara.GetComponent<RectTransform>().anchorMin = Vector2.SmoothDamp(new Vector2(habilidadRara.GetComponent<RectTransform>().anchorMin.x, habilidadRara.GetComponent<RectTransform>().anchorMin.y), new Vector2(0.1705116f, 0.1671322f), ref velocidad5, 0.1f);
            habilidadRara.GetComponent<RectTransform>().anchorMax = Vector2.SmoothDamp(new Vector2(habilidadRara.GetComponent<RectTransform>().anchorMax.x, habilidadRara.GetComponent<RectTransform>().anchorMax.y), new Vector2(0.2451628f, 0.3001322f), ref velocidad6, 0.1f);
            
            habilidadLegendaria.GetComponent<RectTransform>().anchorMin = Vector2.SmoothDamp(new Vector2(habilidadLegendaria.GetComponent<RectTransform>().anchorMin.x, habilidadLegendaria.GetComponent<RectTransform>().anchorMin.y), new Vector2(0.09483721f, 0.033f), ref velocidad7, 0.1f);
            habilidadLegendaria.GetComponent<RectTransform>().anchorMax = Vector2.SmoothDamp(new Vector2(habilidadLegendaria.GetComponent<RectTransform>().anchorMax.x, habilidadLegendaria.GetComponent<RectTransform>().anchorMax.y), new Vector2(0.1705116f, 0.1671322f), ref velocidad8, 0.1f);
           
            // Aparición del destello
            temporal = destello.GetComponent<Image>().color;
            temporal.a = Mathf.SmoothDamp(destello.GetComponent<Image>().color.a, 1, ref velocidad9, 1);
            destello.GetComponent<Image>().color = temporal;

            if (temporal.a > 0.7f)
            {
                aspirante = transform.parent.GetChild(0).gameObject;
                aspirante.transform.GetChild(0).GetComponent<Control>().enabled = true;
                aspirante.GetComponent<NavMeshAgent>().enabled = true;
                aspirante.GetComponent<Movimiento>().enabled = true;
                Camera.main.GetComponent<Camara>().enabled = true;
                accion.GetComponent<Image>().enabled = true;
                accion.GetComponentInChildren<Canvas>().enabled = true;
            }

            // Aparición de la cantidad de oro
            if (temporal.a > 0.9f)
            {
                cantidad.enabled = true;
                colocandoInterfaz = false;
                
                switch (indiceAnimador)
                {
                    case 0:
                        tipo = "Rey Trasgo";
                    break;
                    case 1:
                        tipo = "Hechicero Elemental";
                    break;
                    case 2:
                        tipo = "Diosa Divina";
                    break;
                }

                // Crear aspirante en servidor
                GetComponentInParent<Servidor>().ObservarJugadores();
                GetComponentInParent<Servidor>().AsignarTipoAspiranteServidor(tipo);
                interfazColocada = true;
            }
        }

        // Dar a las flechas para cambiar el aspirante
        rayo3D = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(rayo3D, out colisionado3D);
        if (Input.GetMouseButtonDown(0))
        {
            if (colisionado3D.collider)
            {
                if (colisionado3D.collider.gameObject == elegirDerecha)
                {
                    indiceAnimador += 1;
                    if (indiceAnimador > 2)
                    {
                        indiceAnimador = 0;
                    }
                }
                else if (colisionado3D.collider.gameObject == elegirIzquierda)
                {
                    indiceAnimador -= 1 ;
                    if (indiceAnimador < 0)
                    {
                        indiceAnimador = 2;
                    }
                }

                if (colisionado3D.collider.gameObject == elegirDerecha || colisionado3D.collider.gameObject == elegirIzquierda)
                {
                    GameObject.Destroy(transform.parent.GetChild(0).gameObject);
                    DesactivarFlechas();
                    StartCoroutine (InstanciarSiguienteAspirante());
                }
            } 
        }

        // Indicar estado de las habilidades
        if (interfazColocada)
        {
            habilidadComun1.GetComponent<Image>().fillAmount = (float)int.Parse(cantidad.text) / (float)transform.parent.GetComponentInChildren<Atributos>().habilidadComun1;
            habilidadComun2.GetComponent<Image>().fillAmount = (float)int.Parse(cantidad.text) / (float)transform.parent.GetComponentInChildren<Atributos>().habilidadComun2;
            habilidadRara.GetComponent<Image>().fillAmount = (float)int.Parse(cantidad.text) / (float)transform.parent.GetComponentInChildren<Atributos>().habilidadRara;
            habilidadLegendaria.GetComponent<Image>().fillAmount = (float)int.Parse(cantidad.text) / (float)transform.parent.GetComponentInChildren<Atributos>().habilidadLegendaria;
        }
    }

    IEnumerator InstanciarSiguienteAspirante()
    {
        GameObject.Instantiate(particulasSpawn, GameObject.Find("Menu").GetComponent<Menu>().posicionSpawn, Quaternion.Euler(90, 0, 0));
        yield return new WaitForSeconds (0.3f);
        // Instanciar el aspirante donde está mirando la cámara
        aspirante = GameObject.Instantiate(aspirantes[indiceAnimador], GameObject.Find("Menu").GetComponent<Menu>().posicionSpawn, Quaternion.identity);
        if (!colocandoInterfaz)
        {
            ActivarFlechas();
        }
        aspirante.transform.parent = transform.parent;
        aspirante.transform.SetSiblingIndex(0);
        aspirante.transform.GetChild(0).name = transform.parent.GetComponent<Servidor>().nombresJugadores[0] + ":0";
        // Cambiar habilidades y color de vida
        habilidadComun1.GetComponent<Image>().sprite = habilidades[indiceAnimador * 4];
        habilidadComun2.GetComponent<Image>().sprite = habilidades[indiceAnimador * 4 + 1];
        habilidadRara.GetComponent<Image>().sprite = habilidades[indiceAnimador * 4 + 2];
        habilidadLegendaria.GetComponent<Image>().sprite = habilidades[indiceAnimador * 4 + 3];
        foreach(Image imagen in barraVida.GetComponentsInChildren<Image>())
        {
            switch (indiceAnimador)
            {
                case 0:
                    imagen.color = Color.red;
                    barraVida.GetComponentInChildren<Text>().text = "4";
                break;
                case 1:
                    imagen.color = Color.magenta;
                    barraVida.GetComponentInChildren<Text>().text = "7";
                break;
                case 2:
                    imagen.color = Color.yellow;
                    barraVida.GetComponentInChildren<Text>().text = "1";
                break;
            }
        }
    }

    public void ActivarFlechas()
    {
        elegirIzquierda.GetComponent<MeshRenderer>().sortingOrder = 4000;
        elegirDerecha.GetComponent<MeshRenderer>().sortingOrder = 4000;
        elegirIzquierda.GetComponent<MeshRenderer>().enabled = true;
        elegirIzquierda.GetComponent<Collider>().enabled = true;
        elegirDerecha.GetComponent<MeshRenderer>().enabled = true;
        elegirDerecha.GetComponent<Collider>().enabled = true;
    }

    public void DesactivarFlechas()
    {
        elegirIzquierda.GetComponent<MeshRenderer>().enabled = false;
        elegirIzquierda.GetComponent<Collider>().enabled = false;
        elegirDerecha.GetComponent<MeshRenderer>().enabled = false;
        elegirDerecha.GetComponent<Collider>().enabled = false;
    }
    
    public void ColocarInterfaz()
    {
        DesactivarFlechas();
        habilidadComun1.GetComponentInChildren<Canvas>().enabled = true;
        habilidadComun2.GetComponentInChildren<Canvas>().enabled = true;
        habilidadRara.GetComponentInChildren<Canvas>().enabled = true;
        habilidadLegendaria.GetComponentInChildren<Canvas>().enabled = true;
        colocandoInterfaz = true;
    }

    public void OcultarInterfaz ()
    {
        DesactivarFlechas();
        gameObject.GetComponent<Canvas>().enabled = false;
        contenedorAccion.enabled = false;
        contenedorHab1.enabled = false;
        contenedorHab2.enabled = false;
        contenedorHab3.enabled = false;
        contenedorHab4.enabled = false;
    }

    public void RedimensionarInterfaz (float escala)
    {
        escala /= 2.857142857142857f;
        escala += 0.75f;

        foreach (RectTransform elemento in gameObject.transform.GetComponentsInChildren<RectTransform>())
        {
            if (elemento.tag != "No Alterar")
            {
                elemento.localScale = Vector3.one * escala;
            }
        }
        elegirIzquierda.GetComponent<MeshRenderer>().enabled = false;
        elegirIzquierda.GetComponent<Collider>().enabled = false;
        elegirDerecha.GetComponent<MeshRenderer>().enabled = false;
        elegirDerecha.GetComponent<Collider>().enabled = false;
    }

    public IEnumerator CuentaAtras ()
    {
        transform.GetChild(10).GetComponent<Image>().enabled = true;
        for (int i = 5; i > 0; i--)
        {
            transform.GetChild(9).GetComponent<Text>().fontSize = 45;
            yield return new WaitForSeconds (0.1f);
            transform.GetChild(9).GetComponent<Text>().fontSize = 50;
            transform.GetChild(9).GetComponent<Text>().color = Color.white; //new Color32((byte)Random.Range(0,256), (byte)Random.Range(0,64), (byte)Random.Range(0,64), 255);
            transform.GetChild(9).GetComponent<AudioSource>().Play();
            transform.GetChild(9).GetComponent<Text>().text = i + "";
            yield return new WaitForSeconds (0.1f);
            transform.GetChild(9).GetComponent<Text>().fontSize = 45;
            yield return new WaitForSeconds (0.1f);
            transform.GetChild(9).GetComponent<Text>().color = Color.white;
            transform.GetChild(9).GetComponent<Text>().fontSize = 40;
            yield return new WaitForSeconds (0.7f);
        }
        transform.GetChild(9).GetComponent<Text>().text = "";
        transform.GetChild(10).GetComponent<Image>().enabled = false;
        ColocarInterfaz();
    }

    public void ResetearInterfaz()
    {
        interfazColocada = false;
        OcultarInterfaz();

        // Aparición de las habilidades
        habilidadComun1.GetComponent<RectTransform>().anchorMin = new Vector2(0.351f, 0.2357265f);
        habilidadComun1.GetComponent<RectTransform>().anchorMax = new Vector2(0.423f, 0.3658633f);

        habilidadComun2.GetComponent<RectTransform>().anchorMin = new Vector2(0.423f, 0.2357265f);
        habilidadComun2.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.3658633f);

        habilidadRara.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.2357265f);
        habilidadRara.GetComponent<RectTransform>().anchorMax = new Vector2(0.5754039f, 0.3658633f);

        habilidadLegendaria.GetComponent<RectTransform>().anchorMin = new Vector2(0.5754039f, 0.2357265f);
        habilidadLegendaria.GetComponent<RectTransform>().anchorMax = new Vector2(0.6486058f, 0.3658633f);

        // Aparición del destello
        temporal = destello.GetComponent<Image>().color;
        temporal.a = 0;
        destello.GetComponent<Image>().color = temporal;

        cantidad.text = 50 + "";
        cantidad.enabled = false;
    }
}