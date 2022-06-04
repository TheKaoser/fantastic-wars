using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.AI;

// Estructura de un personaje
[System.Serializable]
public struct Personaje
{
    public string tipo;
    public bool seleccionado;
    public GameObject objeto;
    public Vector3 destino;
    public GameObject objetivo;
    public List<GameObject> objetivos;
    public int vida;
    public int daño;
    public float velocidadAtaque;
    public int rango;
    public int numeroGolpes;
}

public class Control : MonoBehaviour
{
    // Personajes
    public List<Personaje> aliados = new List<Personaje>();
    public List<Personaje> enemigos = new List<Personaje>();
    public List<string> nombresJugadoresaux = new List<string> ();
    public Personaje personaje = new Personaje();
    Dictionary<string, object> p = new Dictionary<string, object>();
    Dictionary<string, Dictionary<string, object>> listaPersonajes = new Dictionary<string, Dictionary<string, object>>();

    // Costes
    public int costeHabilidadComun1;
    public int costeHabilidadComun2;
    public int costeHabilidadRara;
    public int costeHabilidadLegendaria;
    bool enCoolDown;
    float coolDown;

    // Objetos
    GameObject camara;
    GameObject mapa;

    // Colisiones ratón
    int mascaraCapas;
    Ray rayo;
    RaycastHit colisionado;

    // Punteros
    [SerializeField]
    Texture2D puntero;
    [SerializeField]
    Texture2D punteroAzul;
    [SerializeField]
    Texture2D punteroRojo;
    int punteroActual;

    // Caja del GUI
    bool estasArrastrando = false;
    Vector2 posicionInicialRatonGUI;
    Vector2 posicionActualRatonGUI;
    Rect cajaSeleccion;
    [SerializeField]
    GUIStyle estilo;
    
    // Caja real de selección
    Vector3 posicionInicialRatonMundo;
    Vector3 posicionActualRatonMundo;
    Vector3 centro;
    [SerializeField]
    Collider[] colisionados;

    //Colocacion de personajes en area
    List<Vector3> posiciones = new List<Vector3>();
    bool algunAliadoSeleccionado;
    bool algunEnemigoSeleccionado;
    int numAliados = 0;
    int numEnemigos = 0;
    int i;
    int columnasSeleccion, filasSeleccion;
    int capacidadSeleccion;
    int nColumnas, nFilas;
    int aliadosSinDistribuir;
    int aliadoMinimo;
    int objetivoMinimo;
    List<int> indicesUsadosAliados;
    List<int> indicesUsadosObjetivos;
    float maxX, maxY;
    float seleccionX, seleccionY;
    float distanciaPosicionesColumna, distanciaPosicionesFila;
    float desplazamientoCentroFila, desplazamientoCentroColumna;
    float distanciaMin, distanciaActual;
    float seleccionInicialX, seleccionInicialY;
    Vector3 posicion;
    Vector3 direccionAnimacion;

    // Interfaz
    float tiempo;
    public int cambiarOro;
    public bool muerto;
    bool menuActivo;
    public List<Personaje> aspirantesMuertos = new List<Personaje>();

    // Cierre mapa
    GameObject obstaculoCierre;
    public bool cierreMapa;
    public float momentoCeroCierre;
    ParticleSystem sistemaDeParticulas;

    // Enlace
    float enlaceX;
    float enlaceZ;
    Vector3 enlaceCentro;
    GameObject enlaces;

    // Prefabricados
    [SerializeField]
    GameObject objetivo;
    [SerializeField]
    GameObject sangre;
    [SerializeField]
    GameObject humo;
    [SerializeField]
    GameObject particulasMuerte;
    [SerializeField]
    GameObject particulasSpawn;

    [SerializeField]
    GameObject[] aspiranteRival;
    [SerializeField]
    GameObject espadachin;
    [SerializeField]
    GameObject arquero;
    [SerializeField]
    GameObject trol;
    [SerializeField]
    GameObject templo;
    [SerializeField]
    GameObject murallaHorizontal;
    [SerializeField]
    GameObject murallaVertical;
    [SerializeField]
    GameObject murallaInclinadaAbajo;
    [SerializeField]
    GameObject murallaInclinadaArriba;
    [SerializeField]
    GameObject cuartel;
    [SerializeField]
    GameObject angel;
    [SerializeField]
    GameObject maravilla;

    [SerializeField]
    GameObject flecha;
    [SerializeField]
    GameObject lanza;
    [SerializeField]
    GameObject fuego;
    [SerializeField]
    GameObject tierra;
    [SerializeField]
    GameObject tierraDoble;
    [SerializeField]
    GameObject agua;
    [SerializeField]
    GameObject enlace;

    [SerializeField]
    Sprite mano;
    [SerializeField]
    Sprite bandera;

    [SerializeField]
    AudioClip [] vocesEleccionHechicero;

    int habilidadApuntada;
    int habilidadSeleccionada;
    public bool accion;
    public bool accionPuedeCambiar = true;
    bool noCambiarEsteFrame;
    public Sprite habSeleccionada;
    public Sprite habSinSeleccionar;

    void OnEnable()
    {
        // Se cogen todas menos la capa 5 (UI)
        mascaraCapas = 1 << 2;
        mascaraCapas = ~mascaraCapas;

        // Se guarda el aspirante en la lista
        personaje = new Personaje();
        personaje.objeto = gameObject;
        personaje.seleccionado = true;
        personaje.objetivos = new List<GameObject>();
        aliados.Add(personaje);
        // Se le ajusta el volumen al que debería
        GameObject.Find("Slider Efectos 2").GetComponent<Volumen>().CambiarVolumen(PlayerPrefs.GetFloat("Efectos"));
        gameObject.GetComponents<AudioSource>()[3].Play();

        // Se obtienen los costes
        costeHabilidadComun1 = GetComponentInParent<Atributos>().habilidadComun1;
        costeHabilidadComun2 = GetComponentInParent<Atributos>().habilidadComun2;
        costeHabilidadRara = GetComponentInParent<Atributos>().habilidadRara;
        costeHabilidadLegendaria = GetComponentInParent<Atributos>().habilidadLegendaria;

        camara = GameObject.Find("Camara");
        mapa = GameObject.Find("Mapa");
        obstaculoCierre = GameObject.Find("Cierre mapa");
        obstaculoCierre.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5000;

        tiempo = Time.time;
        cambiarOro = 0;
        obstaculoCierre.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().enabled = true;
    }

    void Update()
    {
        // Cerrar mapa
        if (cierreMapa)
        {
            obstaculoCierre.transform.GetChild(0).localScale = Vector3.Lerp(new Vector3(12000, 12000, 1), new Vector3(1000, 1000, 1), (Time.time - momentoCeroCierre) / 300f);
            obstaculoCierre.transform.GetChild(2).localScale = Vector3.Lerp(new Vector3(21000, 21000, 1), new Vector3(1750, 1750, 1), (Time.time - momentoCeroCierre) / 300f);
            sistemaDeParticulas = obstaculoCierre.transform.GetChild(1).GetComponent<ParticleSystem>();
            ParticleSystem.ShapeModule shape = sistemaDeParticulas.shape;
            shape.radius = Mathf.Lerp(10500, 875, (Time.time - momentoCeroCierre) / 300f);
        }

        noCambiarEsteFrame = false;

        // Fijar camara
        if (Input.GetKeyDown(KeyCode.C))
        {
            camara.GetComponent<Camara>().enabled = !camara.GetComponent<Camara>().enabled;
        }

        // Capacidad de poder echar habilidad
        if (enCoolDown)
        {  
            if (Time.time - coolDown >= 0.5f)
            {
                enCoolDown = false;
            }
        }

        for (int i = 0; i < aliados.Count; i++)
        {
            // Animaciones de caminar para los aliados incluyendo que si llegan a su destino o a lo más cerca que puede estar de él se ponen en reposo
            if (aliados[i].objeto.GetComponentInParent<Movimiento>() && aliados[i].objeto.GetComponentInParent<Movimiento>().agente)
            {
                if (!aliados[i].objeto.transform.hasChanged && !aliados[i].objetivo)
                {
                    // Si ya hay una animación en curso, no hacer nada
                    if (!aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad abajo") && !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad arriba") &&
                    !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad derecha") && !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad izquierda") &&
                    !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento abajo") && !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento arriba") &&
                    !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento derecha") && !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento izquierda") &&
                    !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Mal estado") && !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("En ruinas") &&
                    !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Generacion reposo") && !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Generacion mal estado") &&
                    !aliados[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Generacion en ruinas"))
                    {
                        aliados[i].objeto.GetComponent<Animator>().Play("Reposo");
                    }
                }
                else if (!aliados[i].objetivo)
                {
                    IniciarAnimacionMovimiento(aliados[i]);
                    aliados[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = false;
                    aliados[i].objeto.transform.hasChanged = false;
                }
            }

            if (aliados[i].objetivo)
            {
                // Atacar enemigo si está lo suficientemente cerca
                if ((aliados[i].tipo == "Espadachin" || aliados[i].tipo == "Angel") && aliados[i].objeto.transform.parent.GetComponent<Movimiento>().colisionEnemigo && aliados[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.Find(x => x && x.name == aliados[i].objetivo.name))
                {
                    // Dejar quieto
                    aliados[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = true;

                    if (aliados[i].velocidadAtaque == aliados[i].objeto.transform.parent.GetComponent<Atributos>().velocidadAtaque)
                    {
                        personaje = aliados[i];
                        personaje.numeroGolpes++;
                        aliados[i] = personaje;
                        GetComponentInParent<Servidor>().AsignarAtaqueAliadosServidor(int.Parse(aliados[i].objeto.name.Split(':')[1]), aliados[i].numeroGolpes, aliados[i].daño);
                    }

                    personaje = aliados[i];
                    personaje.velocidadAtaque = personaje.velocidadAtaque - Time.deltaTime;
                    if (personaje.velocidadAtaque <= 0)
                    {
                        personaje.velocidadAtaque = aliados[i].objeto.transform.parent.GetComponent<Atributos>().velocidadAtaque;
                    }
                    aliados[i] = personaje;
                }
                else if (aliados[i].tipo == "Arquero" && aliados[i].rango >= Vector3.Distance(enemigos.Find(x => x.objeto.name == aliados[i].objetivo.name).objeto.transform.position, aliados[i].objeto.transform.position))
                {
                    // Dejar quieto
                    aliados[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = true;
                    aliados[i].objeto.GetComponentInParent<Movimiento>().ActualizarPosicion(aliados[i].objeto.transform.position);

                    if (aliados[i].velocidadAtaque == aliados[i].objeto.transform.parent.GetComponent<Atributos>().velocidadAtaque)
                    {
                        personaje = aliados[i];
                        personaje.numeroGolpes++;
                        aliados[i] = personaje;
                        GetComponentInParent<Servidor>().AsignarAtaqueAliadosServidor(int.Parse(aliados[i].objeto.name.Split(':')[1]), aliados[i].numeroGolpes, aliados[i].daño);
                    }

                    personaje = aliados[i];
                    personaje.velocidadAtaque = personaje.velocidadAtaque - Time.deltaTime;
                    if (personaje.velocidadAtaque <= 0)
                    {
                        personaje.velocidadAtaque = aliados[i].objeto.transform.parent.GetComponent<Atributos>().velocidadAtaque;
                    }
                    aliados[i] = personaje;
                }
                else
                {
                    // Ir hacia el enemigo fijado
                    IniciarAnimacionMovimiento(aliados[i]);
                    aliados[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = false;
                    aliados[i].objeto.GetComponentInParent<Movimiento>().ActualizarPosicion(aliados[i].objetivo.transform.position);
                }
            }
        }

        for (int i = 0; i < enemigos.Count; i++)
        {
            // Animaciones de caminar para los enemigos incluyendo que si llegan a su destino o a lo más cerca que puede estar de él se ponen en reposo
            if (enemigos[i].objeto.GetComponentInParent<Movimiento>() && enemigos[i].objeto.GetComponentInParent<Movimiento>().agente)
            {
                if (!enemigos[i].objeto.transform.hasChanged && !enemigos[i].objetivo)
                {
                    // Si ya hay una animación en curso, no hacer nada
                    if (!enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad abajo") && !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad arriba") &&
                    !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad derecha") && !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad izquierda") &&
                    !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento abajo") && !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento arriba") &&
                    !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento derecha") && !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento izquierda") &&
                    !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Mal estado") && !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("En ruinas") &&
                    !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Generacion reposo") && !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Generacion mal estado") &&
                    !enemigos[i].objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Generacion en ruinas"))
                    {
                        enemigos[i].objeto.GetComponent<Animator>().Play("Reposo");
                    }
                }
                else if (!enemigos[i].objetivo)
                {
                    IniciarAnimacionMovimiento(enemigos[i]);
                    enemigos[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = false;
                    enemigos[i].objeto.transform.hasChanged = false;
                }
            }

            if (enemigos[i].objetivo)
            {
                // Atacar enemigo si está lo suficientemente cerca
                if ((enemigos[i].tipo == "Espadachin" || enemigos[i].tipo == "Angel") && enemigos[i].objeto.transform.parent.GetComponent<Movimiento>().colisionEnemigo && enemigos[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.Find(x => x.name == enemigos[i].objetivo.name))
                {
                    // Dejar quieto
                    enemigos[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = true;
                }
                else if (enemigos[i].tipo == "Arquero")
                {
                    if (enemigos.FindIndex(x => x.objeto.name == enemigos[i].objetivo.name) != -1 && enemigos[i].rango >= Vector3.Distance(enemigos.Find(x => x.objeto.name == enemigos[i].objetivo.name).objeto.transform.position, enemigos[i].objeto.transform.position))
                    {
                        // Dejar quieto si está en rango y atacando a un enemigo
                        enemigos[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = true;
                        enemigos[i].objeto.GetComponentInParent<Movimiento>().ActualizarPosicion(enemigos[i].objeto.transform.position);
                    }
                    else if (aliados.FindIndex(x => x.objeto.name == enemigos[i].objetivo.name) != -1 && enemigos[i].rango >= Vector3.Distance(aliados.Find(x => x.objeto.name == enemigos[i].objetivo.name).objeto.transform.position, enemigos[i].objeto.transform.position))
                    {
                        // Dejar quieto si está en rango y atacando a un aliado
                        enemigos[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = true;
                        enemigos[i].objeto.GetComponentInParent<Movimiento>().ActualizarPosicion(enemigos[i].objeto.transform.position);
                    }
                }
                else
                {
                    // Ir hacia el enemigo fijado
                    IniciarAnimacionMovimiento(enemigos[i]);
                    enemigos[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = false;
                    enemigos[i].objeto.GetComponentInParent<Movimiento>().ActualizarPosicion(enemigos[i].objetivo.transform.position);
                }
            }
        }

        // Abrir menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuActivo = GameObject.Find("Menu").GetComponent<Menu>().OpcionesPartida();
        }

        // Si estás muerto, no sigas
        if (muerto)
        {
            return;
        }

        // Crecimiento de oro por segundo
        if (Time.time - tiempo >= 1f)
        {
            cambiarOro++;
            tiempo = Time.time;
        }
        // Bajar oro usado
        if (cambiarOro > 0)
        {
            cambiarOro--;
            gameObject.transform.parent.parent.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = int.Parse(gameObject.transform.parent.parent.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text) + 1 + "";
        }
        else if (cambiarOro < 0)
        {
            cambiarOro++;
            gameObject.transform.parent.parent.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = int.Parse(gameObject.transform.parent.parent.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text) - 1 + "";
        }

        // Diferencia vida
        foreach (Personaje aliado in aliados)
        {
            if (aliado.objeto != gameObject && aliado.tipo != "Agua")
            {
                aliado.objeto.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(aliado.objeto.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount, aliado.objeto.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount, 10 * Time.deltaTime);
            }
            else
            {
                gameObject.transform.parent.parent.GetChild(1).GetChild(8).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(gameObject.transform.parent.parent.GetChild(1).GetChild(8).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount, gameObject.transform.parent.parent.GetChild(1).GetChild(8).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount, 10 * Time.deltaTime);
            }
        }
        foreach (Personaje enemigo in enemigos)
        {
            if (enemigo.tipo != "Agua")
            {
                enemigo.objeto.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = Mathf.Lerp(enemigo.objeto.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount, enemigo.objeto.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount, 10 * Time.deltaTime);
            }
        }

        // Cambiar cursores de color
        rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(rayo, out colisionado, 3000, mascaraCapas);
        if (colisionado.collider && !menuActivo)
        {
            if (colisionado.collider.gameObject.tag == "Habilidad comun 1" && aliados[0].seleccionado)
            {
                habilidadApuntada = 1;
                if (habilidadSeleccionada != 1)
                {
                    accionPuedeCambiar = true;
                }
                else
                {
                    accionPuedeCambiar = false;
                }
            }
            else if (colisionado.collider.gameObject.tag == "Habilidad comun 2" && aliados[0].seleccionado)
            {
                habilidadApuntada = 2;
                if (habilidadSeleccionada != 2)
                {
                    accionPuedeCambiar = true;
                }
                else
                {
                    accionPuedeCambiar = false;
                }
            }
            else if (colisionado.collider.gameObject.tag == "Habilidad rara" && aliados[0].seleccionado)
            {
                habilidadApuntada = 3;
                if (habilidadSeleccionada != 3)
                {
                    accionPuedeCambiar = true;
                }
                else
                {
                    accionPuedeCambiar = false;
                }
            }
            else if (colisionado.collider.gameObject.tag == "Habilidad legendaria" && aliados[0].seleccionado)
            {
                habilidadApuntada = 4;
                if (habilidadSeleccionada != 4)
                {
                    accionPuedeCambiar = true;
                }
                else
                {
                    accionPuedeCambiar = false;
                }
            }
            else if (colisionado.collider.gameObject.tag == "Accion")
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (!accion)
                    {
                        colisionado.collider.gameObject.GetComponent<Image>().sprite = bandera;
                        habilidadSeleccionada = 0;
                        transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                        transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                        transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                        transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    }
                    else
                    {
                        colisionado.collider.gameObject.GetComponent<Image>().sprite = mano;
                        habilidadSeleccionada = 0;
                        transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                        transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                        transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                        transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    }
                    accion = !accion;
                    noCambiarEsteFrame = true;
                }
            }
            else
            {
                if (habilidadSeleccionada == 0)
                {
                    accionPuedeCambiar = true;
                }
                else
                {
                    accionPuedeCambiar = false;
                }
                habilidadApuntada = 0;
            }
            if (colisionado.collider.gameObject.tag == "Aliado")
            {
                if (punteroActual != 0)
                {
                    punteroActual = 0;
                    Cursor.SetCursor(punteroAzul, new Vector2(14, 14), CursorMode.Auto);
                }
            }
            else if (colisionado.collider.gameObject.tag == "Enemigo")
            {
                if (punteroActual != 1)
                {
                    punteroActual = 1;
                    Cursor.SetCursor(punteroRojo, new Vector2(14, 14), CursorMode.Auto);
                }
            }
            else
            {
                if (punteroActual != 2)
                {
                    punteroActual = 2;
                    Cursor.SetCursor(puntero, new Vector2(14, 14), CursorMode.Auto);
                }
            }
        }
        else if (menuActivo)
        {
            if (punteroActual != 2)
            {
                punteroActual = 2;
                Cursor.SetCursor(puntero, new Vector2(14, 14), CursorMode.Auto);
            }
        }

        // Mostrar aliados seleccionados
        foreach (Personaje aliado in aliados)
        {
            if (aliado.seleccionado)
            {
                aliado.objeto.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (aliado.tipo != "Agua")
            {
                aliado.objeto.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        // Seleccionar aspirante con el tabulador
        if (Input.GetKeyDown(KeyCode.Tab) && !menuActivo)
        {
            for (int i = 0; i < aliados.Count; i++)
            {
                if (aliados[i].tipo == "Rey Trasgo" || aliados[i].tipo == "Hechicero Elemental" || aliados[i].tipo == "Diosa Divina")
                {
                    personaje = aliados[i];
                    personaje.seleccionado = true;
                    aliados[i] = personaje;
                }
                else
                {
                    personaje = aliados[i];
                    personaje.seleccionado = false;
                    aliados[i] = personaje;
                }
            }
        }

        // Clic izquierdo
        if (Input.GetMouseButtonDown(0) && !menuActivo && !accion && accionPuedeCambiar)
        {
            posicionInicialRatonMundo = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y - 1)));
        }
        if (Input.GetMouseButtonUp(0) && !menuActivo && !accion)
        {
            if (accionPuedeCambiar)
            {
                if (habilidadApuntada == 1 && aliados[0].seleccionado)
                {
                    habilidadSeleccionada = 1;
                    noCambiarEsteFrame = true;
                    transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSeleccionada;
                    transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                }
                else if (habilidadApuntada == 2 && aliados[0].seleccionado)
                {
                    habilidadSeleccionada = 2;
                    noCambiarEsteFrame = true;
                    transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSeleccionada;
                    transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                }
                else if (habilidadApuntada == 3 && aliados[0].seleccionado)
                {
                    habilidadSeleccionada = 3;
                    noCambiarEsteFrame = true;
                    transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSeleccionada;
                    transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                }
                else if (habilidadApuntada == 4 && aliados[0].seleccionado)
                {
                    habilidadSeleccionada = 4;
                    noCambiarEsteFrame = true;
                    transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSeleccionada;
                    transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                }
                else
                {   
                    habilidadSeleccionada = 0;
                    transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                    transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                }
            }
            else if (habilidadApuntada == habilidadSeleccionada)
            {   
                habilidadSeleccionada = 0;
                transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
                transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
            }
        }
        if (Input.GetMouseButtonUp(0) && !menuActivo && habilidadSeleccionada == 0 && !accion && accionPuedeCambiar && habilidadSeleccionada == 0 && !noCambiarEsteFrame)
        {
            posicionActualRatonMundo = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y - 1)));
            centro = Vector3.Lerp(posicionInicialRatonMundo, posicionActualRatonMundo, 0.5f);
            colisionados = Physics.OverlapBox(centro, new Vector3(Mathf.Abs(posicionActualRatonMundo.x - centro.x), 1, Mathf.Abs(posicionActualRatonMundo.z - centro.z)));
            if (colisionados.Length == 0)
            {
                for (int i = 0; i < aliados.Count; i++)
                {
                    personaje = aliados[i];
                    personaje.seleccionado = false;
                    aliados[i] = personaje;
                }
            }
            else
            {
                for (int i = 0; i < aliados.Count; i++)
                {
                    personaje = aliados[i];
                    personaje.seleccionado = false;
                    aliados[i] = personaje;
                }
                // Seleccionar
                foreach (Collider colisionado in colisionados)
                {
                    if (colisionado.gameObject.tag == "Aliado")
                    {
                        for (int i = 0; i < aliados.Count; i++)
                        {
                            if (aliados[i].objeto.name == colisionado.gameObject.name && (aliados[i].tipo != "Agua" && aliados[i].tipo != "Templo" && aliados[i].tipo != "Muralla" && aliados[i].tipo != "Cuartel" && aliados[i].tipo != "Maravilla")) 
                            {
                                personaje = aliados[i];
                                personaje.seleccionado = true;
                                aliados[i] = personaje;
                                if (aliados[i].tipo == "Hechicero Elemental")
                                {
                                aliados[i].objeto.GetComponents<AudioSource>()[0].clip = vocesEleccionHechicero[UnityEngine.Random.Range(0, 4)];
                                }
                                aliados[i].objeto.GetComponents<AudioSource>()[0].Play();
                            }
                        }
                        algunAliadoSeleccionado = true;
                    }
                }

                if (algunAliadoSeleccionado == false)
                {
                    for (int i = 0; i < aliados.Count; i++)
                    {
                        if (aliados[i].tipo != "Agua")
                        {
                            aliados[i].objeto.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                        }
                        personaje = aliados[i];
                        personaje.seleccionado = false;
                        aliados[i] = personaje;
                    }
                }
                algunAliadoSeleccionado = false;
            }
        }

        // Habilidades
        // Q
        if ((Input.GetKeyDown(KeyCode.Q) || (Input.GetMouseButtonUp(0) && habilidadSeleccionada == 1)) && aliados[0].seleccionado && !menuActivo && !enCoolDown && !noCambiarEsteFrame)
        {
            // Si hay un personaje ya ahí
            if (aliados[0].tipo != "Hechicero Elemental" && (colisionado.collider.CompareTag("Aliado") || colisionado.collider.CompareTag("Enemigo")) || (int.Parse(transform.parent.parent.GetComponentInChildren<Interfaz>().cantidad.text) + cambiarOro < costeHabilidadComun1))
            {
                // Sonido error
                gameObject.GetComponents<AudioSource>()[4].Play();
            }
            else
            {
                listaPersonajes = new Dictionary<string, Dictionary<string, object>>();
                p = new Dictionary<string, object>();
                p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                if (aliados[0].tipo == "Rey Trasgo" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    p.Add("t", "Espadachin");
                    listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + 1 + "", p);
                }
                else if (aliados[0].tipo == "Hechicero Elemental")
                {
                    p.Add("t", "Fuego");
                    listaPersonajes.Add(Time.frameCount + "", p);

                    if (aliados.Count == 2)
                    {
                        p = new Dictionary<string, object>();
                        p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                        p.Add("t", "Fuego");
                        listaPersonajes.Add("Clon-" + Time.frameCount, p);
                    }
                }
                else if (aliados[0].tipo == "Diosa Divina" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 1500)
                {
                    p.Add("t", "Templo");
                    listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + 1 + "", p);
                }
                // Si no estas en rango
                else
                {
                    // Sonido error
                    gameObject.GetComponents<AudioSource>()[4].Play();
                    return;
                }

                cambiarOro -= costeHabilidadComun1;
                StartCoroutine(gameObject.transform.parent.parent.GetChild(1).GetChild(1).GetComponent<Interaccion>().AumentarYDisminuir());
                // Se pone en cooldown para no disparar demasiado rápido habilidades
                enCoolDown = true;
                coolDown = Time.time;
                GetComponentInParent<Servidor>().CrearNuevoPersonaje(listaPersonajes);
            }
            habilidadSeleccionada = 0;
            transform.parent.parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
        }

        // W
        if ((Input.GetKeyDown(KeyCode.W) || (Input.GetMouseButtonUp(0) && habilidadSeleccionada == 2)) && aliados[0].seleccionado && !menuActivo && !enCoolDown && !noCambiarEsteFrame)
        {
            // Si hay un personaje ya ahí
            if (aliados[0].tipo != "Hechicero Elemental" && (colisionado.collider.CompareTag("Aliado") || colisionado.collider.CompareTag("Enemigo")) || int.Parse(transform.parent.parent.GetComponentInChildren<Interfaz>().cantidad.text) + cambiarOro < costeHabilidadComun2)
            {
                // Sonido error
                gameObject.GetComponents<AudioSource>()[4].Play();
            }
            else
            {
                listaPersonajes = new Dictionary<string, Dictionary<string, object>>();
                p = new Dictionary<string, object>();
                if (aliados[0].tipo == "Rey Trasgo" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                    p.Add("t", "Arquero");
                    listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + 1 + "", p);
                }
                else if (aliados[0].tipo == "Hechicero Elemental")
                {
                    p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                    p.Add("t", "Aire");
                    listaPersonajes.Add(Time.frameCount + "", p);
                }
                else if (aliados[0].tipo == "Diosa Divina" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    // Calcular rotación para que sea perpendicular a tu personaje
                    Vector3 direccionMuralla;
                    direccionMuralla = aliados[0].objeto.GetComponentInParent<Movimiento>().agente.steeringTarget - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y)));
                    int rotacionMuralla = 0;
                    // Primer cuadrante
                    if (direccionMuralla.x >= 0 && direccionMuralla.z > 0)
                    {
                        // if (direccionMuralla.x / direccionMuralla.z <= 2f/3f)
                        if (direccionMuralla.x/direccionMuralla.z > 3f/2f)
                        {
                            rotacionMuralla = 0;
                        }
                        // else if (direccionMuralla.x / direccionMuralla.z > 4f/3f)
                        else if (direccionMuralla.x/direccionMuralla.z < 3f/4f)
                        {
                            rotacionMuralla = 90;
                        }
                        else
                        {
                            rotacionMuralla = 45;
                        }
                    }
                    // Segundo cuadrante
                    else if (direccionMuralla.x < 0 && direccionMuralla.z >= 0)
                    {
                        if (-direccionMuralla.x/direccionMuralla.z < 3f/4f)
                        {
                            rotacionMuralla = 90;
                        }
                        else if (-direccionMuralla.x/direccionMuralla.z > 3f/2f)
                        {
                            rotacionMuralla = 180;
                        }
                        else
                        {
                            rotacionMuralla = 135;
                        }
                    }
                    // Tercer cuadrante
                    else if (direccionMuralla.x <= 0 && direccionMuralla.z < 0)
                    {
                        if (direccionMuralla.x/direccionMuralla.z > 3f/2f)
                        {
                            rotacionMuralla = 180;
                        }
                        else if (direccionMuralla.x/direccionMuralla.z < 3f/4f)
                        {
                            rotacionMuralla = 270;
                        }
                        else
                        {
                            rotacionMuralla = 225;
                        }
                    }
                    // Cuarto cuadrante
                    else if (direccionMuralla.x > 0 && direccionMuralla.z <= 0)
                    {
                        if (direccionMuralla.x/-direccionMuralla.z > 3f/2f)
                        {
                            rotacionMuralla = 0;
                        }
                        else if (direccionMuralla.x/-direccionMuralla.z < 3f/4f)
                        {
                            rotacionMuralla = 270;
                        }
                        else
                        {
                            rotacionMuralla = 315;
                        }
                    }
                    p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                    p.Add("t", "Muralla:" + rotacionMuralla);
                    listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + 1 + "", p);
                }
                // Si no estas en rango
                else
                {
                    // Sonido error
                    gameObject.GetComponents<AudioSource>()[4].Play();
                    return;
                }

                cambiarOro -= costeHabilidadComun2;
                StartCoroutine(gameObject.transform.parent.parent.GetChild(1).GetChild(2).GetComponent<Interaccion>().AumentarYDisminuir());
                // Se pone en cooldown para no disparar demasiado rápido habilidades
                enCoolDown = true;
                coolDown = Time.time;
                GetComponentInParent<Servidor>().CrearNuevoPersonaje(listaPersonajes);
            }
            habilidadSeleccionada = 0;
            transform.parent.parent.GetChild(1).GetChild(2).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
        }

        // E
        if ((Input.GetKeyDown(KeyCode.E) || (Input.GetMouseButtonUp(0) && habilidadSeleccionada == 3)) && aliados[0].seleccionado && !menuActivo && !enCoolDown && !noCambiarEsteFrame)
        {
            // Si hay un personaje ya ahí
            if (aliados[0].tipo != "Hechicero Elemental" && (colisionado.collider.CompareTag("Aliado") || colisionado.collider.CompareTag("Enemigo")) || int.Parse(transform.parent.parent.GetComponentInChildren<Interfaz>().cantidad.text) + cambiarOro < costeHabilidadRara)
            {
                // Sonido error
                gameObject.GetComponents<AudioSource>()[4].Play();
            }
            else
            {
                listaPersonajes = new Dictionary<string, Dictionary<string, object>>();
                p = new Dictionary<string, object>();
                p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                if (aliados[0].tipo == "Rey Trasgo" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    p.Add("t", "Trol");
                    listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + 1 + "", p);
                }
                else if (aliados[0].tipo == "Hechicero Elemental" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    p.Add("t", "Tierra");
                    listaPersonajes.Add(Time.frameCount + "", p);
                }
                else if (aliados[0].tipo == "Diosa Divina" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    p.Add("t", "Cuartel");
                    listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + 1 + "", p);
                }
                // Si no estas en rango
                else
                {
                    // Sonido error
                    gameObject.GetComponents<AudioSource>()[4].Play();
                    return;
                }

                cambiarOro -= costeHabilidadRara;
                StartCoroutine(gameObject.transform.parent.parent.GetChild(1).GetChild(3).GetComponent<Interaccion>().AumentarYDisminuir());
                // Se pone en cooldown para no disparar demasiado rápido habilidades
                enCoolDown = true;
                coolDown = Time.time;
                GetComponentInParent<Servidor>().CrearNuevoPersonaje(listaPersonajes);
            }
            habilidadSeleccionada = 0;
            transform.parent.parent.GetChild(1).GetChild(3).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
        }

        // R
        if ((Input.GetKeyDown(KeyCode.R) || (Input.GetMouseButtonUp(0) && habilidadSeleccionada == 4)) && aliados[0].seleccionado && !menuActivo && !enCoolDown && !noCambiarEsteFrame)
        {
            // Si hay un personaje ya ahí
            if (colisionado.collider.CompareTag("Aliado") || colisionado.collider.CompareTag("Enemigo") || int.Parse(transform.parent.parent.GetComponentInChildren<Interfaz>().cantidad.text) + cambiarOro < costeHabilidadLegendaria)
            {
                // Sonido error
                gameObject.GetComponents<AudioSource>()[4].Play();
            }
            else
            {
                listaPersonajes = new Dictionary<string, Dictionary<string, object>>();
                p = new Dictionary<string, object>();
                if (aliados[0].tipo == "Rey Trasgo" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    ClonarUnidades();
                }
                else if (aliados[0].tipo == "Hechicero Elemental" && aliados.Count == 1 && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                    p.Add("t", "Agua");
                    listaPersonajes.Add(Time.frameCount + "", p);
                }
                else if (aliados[0].tipo == "Diosa Divina" && Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))), transform.position) < 800)
                {
                    p.Add("d", Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y))) + "");
                    p.Add("t", "Maravilla");
                    listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + 1 + "", p);
                }
                // Si no estas en rango
                else
                {
                    // Sonido error
                    gameObject.GetComponents<AudioSource>()[4].Play();
                    return;
                }

                cambiarOro -= costeHabilidadLegendaria;
                StartCoroutine(gameObject.transform.parent.parent.GetChild(1).GetChild(4).GetComponent<Interaccion>().AumentarYDisminuir());
                // Se pone en cooldown para no disparar demasiado rápido habilidades
                enCoolDown = true;
                coolDown = Time.time;
                GetComponentInParent<Servidor>().CrearNuevoPersonaje(listaPersonajes);
            }
            habilidadSeleccionada = 0;
            transform.parent.parent.GetChild(1).GetChild(4).GetChild(0).GetComponent<Image>().sprite = habSinSeleccionar;
        }

        // Clic derecho
        if (Input.GetMouseButtonDown(0) && !menuActivo && accion && !noCambiarEsteFrame)
        {
            posicionInicialRatonMundo = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y - 1)));
        }
        if (Input.GetMouseButtonUp(0) && !menuActivo && accion && !noCambiarEsteFrame)
        {
            numAliados = 0;
            foreach (Personaje aliado in aliados)
            {
                if (aliado.seleccionado)
                {
                    numAliados++;
                }
            }
            
            if (numAliados != 0)
            {
                posicionActualRatonMundo = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y - 1)));
                centro = Vector3.Lerp(posicionInicialRatonMundo, posicionActualRatonMundo, 0.5f);
                colisionados = Physics.OverlapBox(centro, new Vector3(Mathf.Abs(posicionActualRatonMundo.x - centro.x), 1, Mathf.Abs(posicionActualRatonMundo.z - centro.z)));

                numEnemigos = 0;
                foreach (Collider colisionado in colisionados)
                {
                    if (colisionado.transform.childCount != 0 && colisionado.gameObject.tag == "Enemigo")
                    {
                        numEnemigos++;
                    }
                }

                // Quitar los objetivos
                for (int i = 1; i < aliados.Count; i++)
                {
                    if (aliados[i].seleccionado)
                    {
                        personaje = aliados[i];
                        if (numEnemigos == 0) {
                            personaje.velocidadAtaque = aliados[i].objeto.transform.parent.GetComponent<Atributos>().velocidadAtaque;
                        }
                        personaje.objetivo = null;
                        personaje.objetivos.Clear();
                        aliados[i] = personaje;
                        Dictionary <string, object> d = new Dictionary<string, object>();
                        d [aliados[i].objeto.name.Split(':')[1] + "/o"] = "";
                        GetComponentInParent<Servidor>().AsignarObjetivosAliadosServidor(d);
                        aliados[i].objeto.GetComponentInParent<Movimiento>().agente.isStopped = false;
                    }
                }

                // Moverse
                if (numEnemigos == 0)
                {
                    GenerarDestinos();
                    AsignarPosiciones();
                }

                // Atacar
                else
                {
                    Atacar();
                }
            }
        }

    }

    public IEnumerator Golpe (Personaje pj, int daño)
    {
        if (pj.objetivo)
        {
            // Ataque de espadachin
            if (pj.tipo == "Espadachin" && Vector3.Distance(pj.objeto.transform.position, pj.objetivo.transform.GetComponent<Collider>().ClosestPoint(pj.objeto.transform.position)) < pj.rango) // - pj.objetivo.transform.GetComponent<Collider>(). // pj.objetivo.transform.parent.GetComponent<NavMeshAgent>().radius * 20 < pj.rango)
            {
                int i = aliados.FindIndex(x => x.objeto.name == pj.objetivo.name);
                if (i != -1)
                {
                    GetComponentInParent<Servidor>().AsignarVidaAliadoServidor(int.Parse(aliados[i].objeto.name.Split(':')[1]), Mathf.Clamp(aliados[i].vida - daño, 0, aliados[i].objeto.GetComponentInParent<Atributos>().vida));
                }
            }
            // Ataque de arquero
            else if (pj.tipo == "Arquero")
            {
                GameObject f = GameObject.Instantiate(flecha, pj.objeto.transform.position, Quaternion.identity);
                if (aliados.FindIndex(x => x.objeto.name == pj.objeto.name) != -1)
                {
                    f.tag = "Aliado";
                }
                f.GetComponent<Flecha>().Lanzar(pj.objetivo.transform.position, pj.objeto.name);
            }
            // Ataque de angel
            else if (pj.tipo == "Angel")
            {
                GameObject l = GameObject.Instantiate(lanza, pj.objeto.transform.position, Quaternion.identity);
                if (aliados.FindIndex(x => x.objeto.name == pj.objeto.name) != -1)
                {
                    l.tag = "Aliado";
                }
                l.GetComponentInChildren<Lanza>().Golpear(pj.objetivo.transform.position, pj.objeto.name);
            }

            // Subir el oro pertinente si un aliado mata a un enemigo
            if (aliados.FindIndex(x => x.objeto.name == pj.objeto.name) != -1 && enemigos.Find(x => x.objeto.name == pj.objetivo.name).vida - daño <= 0)
            {
                cambiarOro += Mathf.CeilToInt(((float)pj.objetivo.GetComponentInParent<Atributos>().vida + (float)pj.objetivo.GetComponentInParent<Atributos>().daño / (float)pj.objetivo.GetComponentInParent<Atributos>().velocidadAtaque) / 2f);

                if (pj.objetivo.name.Split(':')[1] == "0")
                {
                    foreach (Personaje enemigo in enemigos)
                    {
                        if (enemigo.objeto.name.Split(':')[0] == pj.objetivo.name.Split(':')[0])
                        {
                            cambiarOro += Mathf.CeilToInt(((float)enemigo.objeto.GetComponentInParent<Atributos>().vida + (float)enemigo.objeto.GetComponentInParent<Atributos>().daño / (float)enemigo.objeto.GetComponentInParent<Atributos>().velocidadAtaque) / 2f);
                        }
                    }
                }
            }

            // Hacer animación de golpe
            IniciarAnimacionAtaque(pj, pj.objetivo.transform.position);
            yield return new WaitUntil(() => pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.9f);
            // Sonido de golpe
            pj.objeto.GetComponents<AudioSource>()[3].Play();
        }
    }

    public void LanzarHabilidad (string hechicero, Vector3 destino, string nombreHabilidad, string tipo)
    {
        switch (tipo)
        {
            case "Fuego":
                if (aliados[0].objeto.name == hechicero)
                {
                    if (nombreHabilidad.Split('-')[0] == "Clon")
                    {
                        IniciarAnimacionHabilidad (aliados[1], destino, "Habilidad");
                        GameObject.Instantiate(fuego, aliados[1].objeto.transform.position, Quaternion.identity).GetComponent<Fuego>().Lanzar(aliados[1].objeto.transform.position + (destino - aliados[1].objeto.transform.position) * 1500f / Vector3.Magnitude(destino - aliados[1].objeto.transform.position), hechicero, nombreHabilidad);
                    }
                    else
                    {
                        IniciarAnimacionHabilidad (aliados[0], destino, "Habilidad");
                        GameObject.Instantiate(fuego, aliados[0].objeto.transform.position, Quaternion.identity).GetComponent<Fuego>().Lanzar(aliados[0].objeto.transform.position + (destino - aliados[0].objeto.transform.position) * 1500f / Vector3.Magnitude(destino - aliados[0].objeto.transform.position), hechicero, nombreHabilidad);
                    }
                }
                else
                {
                    GameObject fuegoInstacia;
                    if (nombreHabilidad.Split('-')[0] == "Clon")
                    {
                        personaje = enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0])[1];
                        IniciarAnimacionHabilidad (personaje, destino, "Habilidad");
                        fuegoInstacia = GameObject.Instantiate(fuego, personaje.objeto.transform.position, Quaternion.identity);
                        fuegoInstacia.tag = "Enemigo";
                        fuegoInstacia.GetComponent<Fuego>().Lanzar(personaje.objeto.transform.position + (destino - personaje.objeto.transform.position) * 1500f / Vector3.Magnitude(destino - personaje.objeto.transform.position), hechicero, nombreHabilidad);
                    }
                    else
                    {
                        personaje = enemigos.Find(x => x.objeto.name == hechicero);
                        IniciarAnimacionHabilidad (personaje, destino, "Habilidad");
                        fuegoInstacia = GameObject.Instantiate(fuego, personaje.objeto.transform.position, Quaternion.identity);
                        fuegoInstacia.tag = "Enemigo";
                        fuegoInstacia.GetComponent<Fuego>().Lanzar(personaje.objeto.transform.position + (destino - personaje.objeto.transform.position) * 1500f / Vector3.Magnitude(destino - personaje.objeto.transform.position), hechicero, nombreHabilidad);
                    }
                }
                break;

            case "Aire":
                Vector3 destinoFinal;
                if (aliados[0].objeto.name == hechicero)
                {
                    if (aliados.Count == 2)
                    {
                        IniciarAnimacionHabilidad (aliados[1], aliados[0].objeto.transform.position, "Desplazamiento");
                        aliados[1].objeto.GetComponents<AudioSource>()[1].Play();
                        aliados[1].objeto.GetComponentInParent<Movimiento>().DesplazamientoHechicero(aliados[0].objeto.transform.position, nombreHabilidad);
                        aliados[1].objeto.GetComponentInChildren<ParticleSystem>().Play(true);
                    }
                    IniciarAnimacionHabilidad (aliados[0], destino, "Desplazamiento");
                    aliados[0].objeto.GetComponents<AudioSource>()[5].Play();
                    if (Vector3.Distance(aliados[0].objeto.transform.position, destino) < 500)
                    {
                        destinoFinal = destino;
                    }
                    else
                    {
                        destinoFinal = aliados[0].objeto.transform.position + (destino - aliados[0].objeto.transform.position) * 500f / Vector3.Magnitude(destino - aliados[0].objeto.transform.position);
                    }
                    aliados[0].objeto.GetComponentInParent<Movimiento>().DesplazamientoHechicero(destinoFinal, nombreHabilidad);
                    aliados[0].objeto.GetComponentInChildren<ParticleSystem>().Play(true);
                }
                else
                {
                    if (enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0]).Count == 2)
                    {
                        personaje = enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0])[1];
                        IniciarAnimacionHabilidad (personaje, enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0])[0].objeto.transform.position, "Desplazamiento");
                        personaje.objeto.GetComponents<AudioSource>()[1].Play();
                        personaje.objeto.GetComponentInParent<Movimiento>().DesplazamientoHechicero(enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0])[0].objeto.transform.position, nombreHabilidad);
                        personaje.objeto.GetComponentInChildren<ParticleSystem>().Play(true);
                    }
                    personaje = enemigos.Find(x => x.objeto.name == hechicero);
                    IniciarAnimacionHabilidad (personaje, destino, "Desplazamiento");
                    personaje.objeto.GetComponents<AudioSource>()[5].Play();
                    if (Vector3.Distance(personaje.objeto.transform.position, destino) < 500)
                    {
                        destinoFinal = destino;
                    }
                    else
                    {
                        destinoFinal = personaje.objeto.transform.position + (destino - personaje.objeto.transform.position) * 500f / Vector3.Magnitude(destino - personaje.objeto.transform.position);
                    }
                    personaje.objeto.GetComponentInParent<Movimiento>().DesplazamientoHechicero(destinoFinal, nombreHabilidad);
                    personaje.objeto.GetComponentInChildren<ParticleSystem>().Play(true);
                }
                break;

            case "Tierra":
                if (aliados[0].objeto.name == hechicero)
                {
                    if (aliados.Count == 2)
                    {
                        IniciarAnimacionHabilidad (aliados[0], destino, "Habilidad");
                        IniciarAnimacionHabilidad (aliados[1], destino, "Habilidad");
                        GameObject.Instantiate(tierraDoble, destino, Quaternion.Euler(new Vector3 (90, 0, int.Parse(nombreHabilidad)))).name = hechicero.Split(':')[0] + ':' + nombreHabilidad;
                    }
                    else
                    {
                        IniciarAnimacionHabilidad (aliados[0], destino, "Habilidad");
                        GameObject.Instantiate(tierra, destino, Quaternion.Euler(new Vector3 (90, 0, int.Parse(nombreHabilidad)))).name = hechicero.Split(':')[0] + ':' + nombreHabilidad;
                    }
                }
                else
                {
                    if (enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0]).Count == 2)
                    {
                        IniciarAnimacionHabilidad (enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0])[0], destino, "Habilidad");
                        IniciarAnimacionHabilidad (enemigos.FindAll(x => x.objeto.name.Split(':')[0] == hechicero.Split(':')[0])[1], destino, "Habilidad");
                        GameObject tierraInstancia;
                        tierraInstancia = GameObject.Instantiate(tierraDoble, destino, Quaternion.Euler(new Vector3 (90, 0, int.Parse(nombreHabilidad))));
                        tierraInstancia.tag = "Bloqueo";
                        tierraInstancia.transform.GetChild(0).tag = "Bloqueo";
                        tierraInstancia.transform.GetChild(1).tag = "Bloqueo";
                        tierraInstancia.transform.GetChild(2).tag = "Bloqueo";
                        tierraInstancia.transform.GetChild(3).tag = "Bloqueo";
                    }
                    else
                    {
                        IniciarAnimacionHabilidad (enemigos.Find(x => x.objeto.name == hechicero), destino, "Habilidad");
                        GameObject tierraInstancia;
                        tierraInstancia = GameObject.Instantiate(tierra, destino, Quaternion.Euler(new Vector3 (90, 0, int.Parse(nombreHabilidad))));
                        tierraInstancia.tag = "Bloqueo";
                        tierraInstancia.transform.GetChild(0).tag = "Bloqueo";
                        tierraInstancia.transform.GetChild(1).tag = "Bloqueo";
                    }
                }
                break;

            case "Agua":

                personaje = new Personaje();
                personaje.objeto = GameObject.Instantiate(agua, destino, Quaternion.identity).transform.GetChild(0).gameObject;
                personaje.objeto.name = hechicero.Split(':')[0] + ':' + nombreHabilidad;
                personaje.objetivos = new List<GameObject>();
                personaje.tipo = "Agua";
                if (aliados[0].objeto.name == hechicero)
                {
                    enlaceX = (aliados[0].objeto.transform.position.x - personaje.objeto.transform.position.x) / 2;
                    enlaceZ = (aliados[0].objeto.transform.position.z - personaje.objeto.transform.position.z) / 2;
                    centro = new Vector3(personaje.objeto.transform.position.x + enlaceX, personaje.objeto.transform.position.y, personaje.objeto.transform.position.z + enlaceZ);
                    enlaces = GameObject.Instantiate(enlace, centro, Quaternion.identity);
                    foreach (Union u in enlaces.GetComponentsInChildren<Union>())
                    {
                        u.clon = personaje.objeto;
                        u.hechicero = aliados[0].objeto;
                    }

                    IniciarAnimacionHabilidad (aliados[0], destino, "Habilidad");
                    aliados.Add(personaje);
                }
                else
                {
                    enlaceX = (enemigos.Find(x => x.objeto.name == hechicero).objeto.transform.position.x - personaje.objeto.transform.position.x) / 2;
                    enlaceZ = (enemigos.Find(x => x.objeto.name == hechicero).objeto.transform.position.z - personaje.objeto.transform.position.z) / 2;
                    centro = new Vector3(personaje.objeto.transform.position.x + enlaceX, 0, personaje.objeto.transform.position.z + enlaceZ);
                    enlaces = GameObject.Instantiate(enlace, centro, Quaternion.identity);
                    foreach (Union u in enlaces.GetComponentsInChildren<Union>())
                    {
                        u.clon = personaje.objeto;
                        u.hechicero = enemigos.Find(x => x.objeto.name == hechicero).objeto;
                    }

                    IniciarAnimacionHabilidad (enemigos.Find(x => x.objeto.name == hechicero), destino, "Habilidad");
                    enemigos.Add(personaje);
                }
                break;
        }
    }

    void Atacar()
    {
        // Variables
        float distancia = 0;
        bool bandera = false;
        List<KeyValuePair<Collider, float>> enemigosAux = new List<KeyValuePair<Collider, float>>();
        List<GameObject> enemigosObjetivo = new List<GameObject>();
        List<Collider> enemigosOrdenados = new List<Collider>();
        float distanciaMin = Mathf.Infinity;
        int aliadosConEsteEnemigo = 0;
        float distanciaTotal = 0;
        float distanciaMax = -1;
        int aliadoMaximo = 0;
        List<KeyValuePair<Personaje, float>> distancias = new List<KeyValuePair<Personaje, float>>();
        int indiceActual = 0;
        int indiceAux = 0;
        List<Vector3> posicionesAux = new List<Vector3>();

        // Si alguien va a atacar se continua
        for (int i = 1; i < aliados.Count; i++)
        {
            if (aliados[i].seleccionado && aliados[i].daño != 0)
            {
                bandera = true;
                break;
            }
        }
        if (!bandera)
        {
            // Sonido
            gameObject.GetComponents<AudioSource>()[4].Play();
            return;
        }

        // Los enemigos se marcan como seleccionados y se guardan en una lista
        foreach (Collider colisionado in colisionados)
        {
            if (colisionado.gameObject.CompareTag("Enemigo"))
            {
                for (int i = 0; i < enemigos.Count; i++)
                {
                    if (enemigos[i].objeto.name == colisionado.gameObject.name)
                    {
                        enemigos[i].objeto.transform.GetChild(0).GetComponent<Objetivo>().enabled = true;
                        distancia = 0;
                        foreach (Personaje aliado in aliados)
                        {
                            distancia = Vector3.Distance(aliado.objeto.transform.position, colisionado.gameObject.transform.position);
                        }
                        enemigosAux.Add(new KeyValuePair<Collider, float>(colisionado, distancia));
                    }
                }
            }
        }

        enemigosAux.Sort((x, y) => x.Value.CompareTo(y.Value));
        enemigosOrdenados.AddRange((from enemigo in enemigosAux select enemigo.Key).ToList());
        enemigosObjetivo.AddRange((from enemigo in enemigosAux select enemigo.Key.gameObject).ToList());

        // Se asigna a cada aliado seleccionado la lista de enemigos seleccionados. También se crea una lista auxiliar de aliados seleccionados. También se reproduce sonido
        List<Personaje> aliadosSeleccionados = new List<Personaje>();
        for (int i = 1; i < aliados.Count; i++)
        {
            if (aliados[i].seleccionado && aliados[i].daño != 0)
            {
                personaje = aliados[i];
                personaje.objetivos = new List<GameObject>();
                personaje.objetivos.AddRange(enemigosObjetivo);
                aliados[i] = personaje;
                aliadosSeleccionados.Add(aliados[i]);
                aliados[i].objeto.GetComponents<AudioSource>()[1].Play();
            }
        }

        // Asigna en el primer elemento de la lista de cada aliado el enemigo correspondiente dependiendo de su cercanía
        // Caso en el que hay mas aliados que enemigos
        // if (aliadosSeleccionados.Count > enemigosObjetivo.Count)
        // {
        //     foreach (Collider colisionado in enemigosOrdenados)
        //     {
        //         if (colisionado.gameObject.CompareTag("Enemigo"))
        //         {
        //             aliadosConEsteEnemigo = Mathf.CeilToInt((float)aliadosSeleccionados.Count / (float)enemigosObjetivo.Count);

        //             while (aliadosConEsteEnemigo > 0)
        //             {
        //                 for (int i = 0; i < aliadosSeleccionados.Count; i++)
        //                 {
        //                     distanciaTotal = 0;
        //                     distanciaMax = -1;
        //                     for (int j = 0; j < enemigosObjetivo.Count; j++)
        //                     {
        //                         distanciaTotal = distanciaTotal + Vector3.Distance(aliadosSeleccionados[i].objeto.transform.position, enemigosObjetivo[j].transform.position);
        //                     }
        //                     if (distanciaTotal > distanciaMax)
        //                     {
        //                         distanciaMax = distanciaTotal;
        //                         aliadoMaximo = i;
        //                     }
        //                 }

        //                 aliados[aliados.FindIndex(x => x.objeto.name == aliadosSeleccionados[aliadoMaximo].objeto.name)].objetivos.Remove(colisionado.gameObject);
        //                 aliados[aliados.FindIndex(x => x.objeto.name == aliadosSeleccionados[aliadoMaximo].objeto.name)].objetivos.Insert(0, colisionado.gameObject);
        //                 aliadosSeleccionados.Remove(aliadosSeleccionados[aliadoMaximo]);
        //                 aliadosConEsteEnemigo--;
        //             }
        //             enemigosObjetivo.Remove(colisionado.gameObject);
        //         }
        //     }
        // }
        // // Caso en el que hay menos o igual numero de aliados que de enemigos
        // else
        // {
        //     foreach (GameObject enemigo in enemigosObjetivo)
        //     {
        //         posicionesAux.Add(enemigo.transform.position);
        //     }

        //     // Guarda cada aliado seleccionado con la suma de las distancias a cada uno de los enemigos
        //     for (int i = 0; i < aliadosSeleccionados.Count; i++)
        //     {
        //             distancia = 0;
        //             foreach (Vector3 posicion in posicionesAux)
        //             {
        //                 distancia = distancia + Vector3.Distance(aliadosSeleccionados[i].objeto.transform.position, posicion);
        //             }
        //             personaje = aliadosSeleccionados[i];
        //             distancias.Add(new KeyValuePair<Personaje, float>(personaje, distancia));
        //     }

        //     // Ordena la lista de mayor a menor distancia
        //     distancias.Sort((x, y) => y.Value.CompareTo(x.Value));

        //     // A cada aliado (empezando por el que esta mas lejos de todas las posiciones) se le deja elegir posición y elige la más cercana
        //     while (0 < distancias.Count)
        //     {
        //         distancia = 0;
        //         distanciaMin = Mathf.Infinity;
        //         for (int j = 0; j < posicionesAux.Count; j++)
        //         {
        //             distancia = Vector3.Distance(distancias[0].Key.objeto.transform.position, posicionesAux[j]);
        //             if (distancia < distanciaMin)
        //             {
        //                 distanciaMin = distancia;
        //                 indiceActual = j;
        //             }
        //         }

        //         personaje = distancias[0].Key;
        //         personaje.objetivos.Remove(enemigosObjetivo[enemigosObjetivo.FindIndex(x => Equals(x.transform.position, posicionesAux[indiceActual]))]);
        //         personaje.objetivos.Insert(0, enemigosObjetivo[enemigosObjetivo.FindIndex(x => Equals(x.transform.position, posicionesAux[indiceActual]))]);
        //         aliados[aliados.FindIndex(x => Equals(x, distancias[0].Key))] = personaje;

        //         for (int k = 1; k < distancias.Count; k++)
        //         {
        //             personaje = distancias[k].Key;
        //             indiceAux = aliados.FindIndex(x => x.objeto.name == distancias[k].Key.objeto.name);
        //             aliados[indiceAux] = personaje;
        //             distancias[k] = new KeyValuePair<Personaje, float>(aliados[indiceAux], distancias[k].Value);
        //         }

        //         distancias.Remove(distancias[0]);
        //         distancias.Sort((x, y) => y.Value.CompareTo(x.Value));
        //         posicionesAux.Remove(posicionesAux[indiceActual]);
        //     }
        // }

        // Se envía al servidor el primer objetivo de cada aliado
        Dictionary <string, object> objetivosAliados = new Dictionary<string, object>();
        for (int i = 1; i < aliados.Count; i++)
        {
            if (aliados[i].seleccionado)
            {
                try
                {
                    objetivosAliados.Add(aliados[i].objeto.name.Split(':')[1] + "/o", aliados[i].objetivos[0].name);
                }
                catch
                {
                    continue;
                }
            }
        }

        GetComponentInParent<Servidor>().AsignarObjetivosAliadosServidor(objetivosAliados);
    }

    void IniciarAnimacionMovimiento(Personaje pj)
    {
        // Dependiendo del destino, se elige una de las cuatro direcciones de animación posibles
        if (!pj.objeto.GetComponentInParent<Movimiento>().agente)
        {
            return;
        }

        // Si ya hay una animación en curso, no hacer nada
        if (pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad abajo") || pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad arriba") ||
        pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad derecha") || pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Habilidad izquierda") ||
        pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento abajo") || pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento arriba") ||
        pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento derecha") || pj.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Desplazamiento izquierda"))
        {
            return;
        }
        direccionAnimacion = pj.objeto.GetComponentInParent<Movimiento>().agente.steeringTarget - pj.objeto.transform.position;
        if (direccionAnimacion.x >= 0 && direccionAnimacion.z > 0)
        {
            if (direccionAnimacion.x >= direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento derecha");
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento arriba");
            }
        }
        else if (direccionAnimacion.x > 0 && direccionAnimacion.z <= 0)
        {
            if (direccionAnimacion.x >= -direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento derecha");
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento abajo");
            }
        }
        else if (direccionAnimacion.x < 0 && direccionAnimacion.z >= 0)
        {
            if (-direccionAnimacion.x >= direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento izquierda");
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento arriba");
            }
        }
        else if (direccionAnimacion.x <= 0 && direccionAnimacion.z < 0)
        {
            if (-direccionAnimacion.x >= -direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento izquierda");
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Movimiento abajo");
            }
        }
    }

    void IniciarAnimacionAtaque (Personaje pj, Vector3 posicionEnemigo)
    {
        // Dependiendo de la posición del atacado se elige una de las cuatro direcciones de ataque posibles
        direccionAnimacion = posicionEnemigo - pj.objeto.transform.position;
        if (direccionAnimacion.x >= 0 && direccionAnimacion.z > 0)
        {
            if (direccionAnimacion.x >= direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque derecha", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque arriba", -1, 0);
            }
        }
        else if (direccionAnimacion.x > 0 && direccionAnimacion.z <= 0)
        {
            if (direccionAnimacion.x >= -direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque derecha", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque abajo", -1, 0);
            }
        }
        else if (direccionAnimacion.x < 0 && direccionAnimacion.z >= 0)
        {
            if (-direccionAnimacion.x >= direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque izquierda", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque arriba", -1, 0);
            }
        }
        else if (direccionAnimacion.x <= 0 && direccionAnimacion.z < 0)
        {
            if (-direccionAnimacion.x >= -direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque izquierda", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play("Ataque abajo", -1, 0);
            }
        }
    }

    void IniciarAnimacionHabilidad (Personaje pj, Vector3 destino, string habilidad)
    {
        // Dependiendo de la posición del atacado se elige una de las cuatro direcciones de ataque posibles
        direccionAnimacion = destino - pj.objeto.transform.position;
        if (direccionAnimacion.x >= 0 && direccionAnimacion.z > 0)
        {
            if (direccionAnimacion.x >= direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " derecha", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " arriba", -1, 0);
            }
        }
        else if (direccionAnimacion.x > 0 && direccionAnimacion.z <= 0)
        {
            if (direccionAnimacion.x >= -direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " derecha", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " abajo", -1, 0);
            }
        }
        else if (direccionAnimacion.x < 0 && direccionAnimacion.z >= 0)
        {
            if (-direccionAnimacion.x >= direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " izquierda", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " arriba", -1, 0);
            }
        }
        else if (direccionAnimacion.x <= 0 && direccionAnimacion.z < 0)
        {
            if (-direccionAnimacion.x >= -direccionAnimacion.z)
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " izquierda", -1, 0);
            }
            else
            {
                pj.objeto.GetComponent<Animator>().Play(habilidad + " abajo", -1, 0);
            }
        }
    }

    public Personaje ActualizarDestinoPersonaje (Personaje pj, Vector3 destino, bool aliado)
    {
        personaje = pj;
        if (personaje.objeto.transform.position.x != destino.x || personaje.objeto.transform.position.z != destino.z)
        {
            // Actualizar destino en la lista
            personaje.destino = destino;
            
            // Limpiar objetivos
            if (aliado && pj.objeto.name.Split(':')[1] != "0")
            {
                personaje.objetivo = null;
                personaje.objetivos.Clear();
                Dictionary <string, object> d = new Dictionary<string, object>();
                d [pj.objeto.name.Split(':')[1] + "/o"] = "";
                GetComponentInParent<Servidor>().AsignarObjetivosAliadosServidor(d);
            }

            // Actualizar movimiento del agente
            personaje.objeto.GetComponentInParent<Movimiento>().agente.isStopped = false;
            personaje.objeto.GetComponentInParent<Movimiento>().ActualizarPosicion(destino);
        }
        return personaje;
    }

    public Personaje ActualizarObjetivoPersonaje (Personaje pj, string objetivo)
    {
        // Si el objetivo es un enemigo
        if (objetivo != "")
        {
            Personaje obj = new Personaje();
            if (aliados.FindIndex(x => x.objeto.name == objetivo) != -1)
            {
                obj = aliados.Find(x => x.objeto.name == objetivo);
                pj.objetivo = obj.objeto;
            }
            else if (enemigos.FindIndex(x => x.objeto.name == objetivo) != -1)
            {
                obj = enemigos.Find(x => x.objeto.name == objetivo);
                pj.objetivo = obj.objeto;
            }
        }
        // Si se está limpiando el objetivo anterior con un desplazamiento
        else
        {
            pj.objetivo = null;
        }
        return pj;
    }

    public void ActualizarVidaPersonaje (Personaje pj, int vida, bool aliado)
    {
        // Si el trol ha otorgado vida
        if (vida > pj.vida && (pj.tipo == "Trol" || pj.tipo == "Espadachin" || pj.tipo == "Arquero" || pj.tipo == "Rey Trasgo"))
        {
            pj.objeto.GetComponentInParent<Atributos>().AsignarAtributos();
            pj.objeto.GetComponentInParent<Atributos>().vida++;
        }

        pj.vida = Mathf.Clamp(vida, 0, pj.objeto.GetComponentInParent<Atributos>().vida);

        // Si es tu aspirante, bajar la vida en la interfaz
        if (pj.objeto.name == aliados[0].objeto.name)
        {
            gameObject.transform.parent.parent.GetChild(1).GetChild(8).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = (float)pj.vida / (float)pj.objeto.GetComponentInParent<Atributos>().vida;
            gameObject.transform.parent.parent.GetChild(1).GetChild(8).GetChild(1).GetComponent<Text>().text = pj.vida + "";
        }
        // O si no bajarla en el personaje correspondiente
        else
        {
            pj.objeto.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = (float)vida / (float)pj.objeto.GetComponentInParent<Atributos>().vida;
        }

        Personaje atacante;
        atacante = new Personaje();

        // Se comprueba si existe el personaje y su atacante
        bool iParticulas = false;
        int indice;
        // Si el atacado es un aliado se busca en la lista de enemigos
        if (aliado)
        {
            indice = aliados.FindIndex(x => x.objeto.name == pj.objeto.name);
            foreach (Personaje e in enemigos)
            {
                if (e.objetivo)
                {
                    if (e.objetivo.name == pj.objeto.name)
                    {
                        atacante = e;
                        break;
                    }
                }
            }
            if (atacante.objeto)
            {
                // Si existe se instanciará sangre
                iParticulas = true;
            }
        }
        // Si es un enemigo, el atacante se busca primero en la lista de aliados y después en la de enemigos
        else
        {
            indice = enemigos.FindIndex(x => x.objeto.name == pj.objeto.name);
            foreach (Personaje a in aliados)
            {
                if (a.objetivo && a.objetivo.name == pj.objeto.name)
                {
                    atacante = a;

                    // Si matas al atacado, miras si tenías más objetivos en la lista para seguir atacando
                    if (vida <= 0 && atacante.objetivos.Count > 1)
                    {
                        try
                        {
                            Dictionary <string, object> objetivoAliado = new Dictionary<string, object>();
                            objetivoAliado.Add(atacante.objeto.name.Split(':')[1] + "/o", atacante.objetivos[1].name);
                            GetComponentInParent<Servidor>().AsignarObjetivosAliadosServidor(objetivoAliado);
                        }
                        catch (System.NullReferenceException)
                        {
                            continue;
                        }
                    }
                }
            }
            if (!atacante.objeto)
            {
                foreach (Personaje e in enemigos)
                {
                    if (e.objetivo && e.objetivo.name == pj.objeto.name)
                    {
                        atacante = e;
                        break;
                    }
                }
            }
            if (atacante.objeto)
            {
                // Si existe atacante, se instanciará sangre
                iParticulas = true;
            }
        }

        // Sangre o humo!
        if (iParticulas)
        {
            if (pj.objeto.transform.parent.CompareTag("Edificio"))
            {
                GameObject.Instantiate(humo, pj.objeto.GetComponent<BoxCollider>().ClosestPoint(atacante.objeto.transform.position), Quaternion.identity);
            }
            else
            {
                GameObject.Instantiate(sangre, pj.objeto.GetComponent<BoxCollider>().ClosestPoint(atacante.objeto.transform.position), Quaternion.identity);
            }
        }

        // Si el atacado es un aliado y se queda con cero, se mata a ese aliado en el servidor 
        if (vida <= 0 && aliado)
        {
            GetComponentInParent<Servidor>().MatarAliadoServidor(aliados[indice].objeto.name.Split(':')[1]);
            return;
        }
        
        if (aliado)
        {
            aliados[indice] = pj;
        }
        else
        {
            enemigos[indice] = pj;
        }
    }

    public void MatarUnidad (GameObject unidad)
    {
        unidad.GetComponent<BoxCollider>().center = new Vector3(0, 100000, 0);
        // Se elimina de las listas de enemigos y aliados
        if (aliados.FindIndex(x => x.objeto.name == unidad.name) != -1)
        {
            aliados.RemoveAt(aliados.FindIndex(x => x.objeto.name == unidad.name));
        }
        else if (enemigos.FindIndex(x => x.objeto.name == unidad.name) != -1)
        {
            enemigos.RemoveAt(enemigos.FindIndex(x => x.objeto.name == unidad.name));
        }

        // Se eliminan sus atacantes
        for (int i = 0; i < aliados.Count; i++)
        {
            if (aliados[i].objetivo && aliados[i].objetivo.name == unidad.name)
            {
                personaje = aliados[i];
                personaje.objetivo = null;
                aliados[i] = personaje;
            }
            if (aliados[i].objetivos.FindIndex(x => x.name == unidad.name) != -1)
            {
                aliados[i].objetivos.RemoveAt(aliados[i].objetivos.FindIndex(x => x.name == unidad.name));
            }
            if (aliados[i].objeto.transform.parent.GetComponent<Movimiento>() && aliados[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.Find(x => x == unidad))
            {
                aliados[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.RemoveAt(aliados[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.FindIndex(x => x && x.name == unidad.name));
            }
        }
        for (int i = 0; i < enemigos.Count; i++)
        {
            if (enemigos[i].objetivo && enemigos[i].objetivo.name == unidad.name)
            {
                personaje = enemigos[i];
                personaje.objetivo = null;
                enemigos[i] = personaje;
            }
            if (enemigos[i].objetivos.FindIndex(x => x.name == unidad.name) != -1)
            {
                enemigos[i].objetivos.RemoveAt(enemigos[i].objetivos.FindIndex(x => x.name == unidad.name));
            }
            if (enemigos[i].objeto.transform.parent.GetComponent<Movimiento>() && enemigos[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.Find(x => x == unidad))
            {
                enemigos[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.RemoveAt(enemigos[i].objeto.transform.parent.GetComponent<Movimiento>().personajesColisionados.FindIndex(x => x && x.name == unidad.name));
            }
        }

        // Sonido
        unidad.GetComponents<AudioSource>()[2].Play();
        if (unidad.transform.parent.GetComponent<Movimiento>())
        {
            unidad.transform.parent.GetComponent<Movimiento>().enabled = false;
        }

        // Si es un aspirante
        if (unidad.name.Split(':')[1] == "0")
        {
            // Animación
            unidad.GetComponent<Animator>().Play("Muerte");
            // No clicable y transitable
            unidad.transform.parent.GetComponent<NavMeshAgent>().enabled = false;
            unidad.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            if (unidad.CompareTag("Enemigo"))
            {
                unidad.transform.GetChild(1).GetComponent<Canvas>().enabled = false;
            }
        }
        else
        {
            if (unidad.CompareTag("Agua"))
            {
                // Animación
                unidad.GetComponent<Animator>().Play("Muerte");
                Destroy(enlaces);
            }
            else
            {
                // Partículas
                GameObject.Instantiate(particulasMuerte, unidad.transform.parent.position, Quaternion.Euler(90, 0, 0));
            }
            StartCoroutine(DestruirUnidad(unidad.transform.parent.gameObject));
        }
    }

    IEnumerator DestruirUnidad (GameObject unidad)
    {
        // Se destruye la unidad tras dejar escuchar el sonido
        foreach (Renderer r in unidad.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        foreach (Canvas c in unidad.GetComponentsInChildren<Canvas>())
        {
            c.enabled = false;
        }

        if (unidad.CompareTag("Trol"))
        {
            unidad.transform.GetChild(0).GetChild(2).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 1600);
        }

        yield return new WaitForSeconds(1f);
        unidad.SetActive(false);
        Destroy(unidad);
    }

    public IEnumerator ColocarPersonaje (string nombre, Vector3 destino, string tipo)
    {
        // Particulas
        GameObject.Instantiate(particulasSpawn, destino, Quaternion.Euler(90, 0, 0));
        yield return new WaitForSeconds(0.3f);

        // Se instancia el personaje correspondiente
        GameObject personajeObjeto = null;
        switch (tipo)
        {
            case "Rey Trasgo":
                personajeObjeto = GameObject.Instantiate(aspiranteRival[0], destino, Quaternion.identity);
                break;
            case "Hechicero Elemental":
                personajeObjeto = GameObject.Instantiate(aspiranteRival[1], destino, Quaternion.identity);
                break;
            case "Diosa Divina":
                personajeObjeto = GameObject.Instantiate(aspiranteRival[2], destino, Quaternion.identity);
                break;
            case "Espadachin":
                personajeObjeto = GameObject.Instantiate(espadachin, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                break;
            case "Arquero":
                personajeObjeto = GameObject.Instantiate(arquero, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                break;
            case "Trol":
                personajeObjeto = GameObject.Instantiate(trol, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                break;
            case "Templo":
                personajeObjeto = GameObject.Instantiate(templo, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                break;
            case "Muralla:0":
                personajeObjeto = GameObject.Instantiate(murallaVertical, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Muralla:45":
                personajeObjeto = GameObject.Instantiate(murallaInclinadaAbajo, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Muralla:90":
                personajeObjeto = GameObject.Instantiate(murallaHorizontal, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Muralla:135":
                personajeObjeto = GameObject.Instantiate(murallaInclinadaArriba, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Muralla:180":
                personajeObjeto = GameObject.Instantiate(murallaVertical, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Muralla:225":
                personajeObjeto = GameObject.Instantiate(murallaInclinadaAbajo, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Muralla:270":
                personajeObjeto = GameObject.Instantiate(murallaHorizontal, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Muralla:315":
                personajeObjeto = GameObject.Instantiate(murallaInclinadaArriba, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                tipo = "Muralla";
                break;
            case "Cuartel":
                personajeObjeto = GameObject.Instantiate(cuartel, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                break;
            case "Angel":
                personajeObjeto = GameObject.Instantiate(angel, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                foreach (Personaje aliado in aliados)
                {
                    if (aliado.tipo == "Cuartel" && aliado.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    {
                        if (aliado.objeto.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 1f/3f)
                        {
                            aliado.objeto.GetComponent<Animator>().Play("Generacion en ruinas");
                        }
                        else if (aliado.objeto.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 2f/3f)
                        {
                            aliado.objeto.GetComponent<Animator>().Play("Generacion mal estado");
                        }
                        else
                        {
                            aliado.objeto.GetComponent<Animator>().Play("Generacion reposo");
                        }

                        aliado.objeto.GetComponentInParent<Cuartel>().creado = false;
                    }
                }
                foreach (Personaje enemigo in enemigos)
                {
                    if (enemigo.tipo == "Cuartel" && enemigo.objeto.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                    {
                        if (enemigo.objeto.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 1f/3f)
                        {
                            enemigo.objeto.GetComponent<Animator>().Play("Generacion en ruinas");
                        }
                        else if (enemigo.objeto.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>().fillAmount <= 2f/3f)
                        {
                            enemigo.objeto.GetComponent<Animator>().Play("Generacion mal estado");
                        }
                        else
                        {
                            enemigo.objeto.GetComponent<Animator>().Play("Generacion reposo");
                        }

                        enemigo.objeto.GetComponentInParent<Cuartel>().creado = false;
                    }
                }
                break;
            case "Maravilla":
                personajeObjeto = GameObject.Instantiate(maravilla, destino, Quaternion.identity);
                // Sonido
                personajeObjeto.transform.GetChild(0).gameObject.GetComponents<AudioSource>()[0].Play();
                break;
        }

        // Se le ajusta el volumen al que debería
        GameObject.Find("Slider Efectos 2").GetComponent<Volumen>().CambiarVolumen(PlayerPrefs.GetFloat("Efectos"));

        // Se renombra
        personaje = new Personaje();
        personajeObjeto.transform.GetChild(0).name = nombre;
        // Se le añade el gameobject y se crea la lista de objetivos
        personaje.objeto = personajeObjeto.transform.GetChild(0).gameObject;
        personaje.objetivos = new List<GameObject>();

        // Se le ponen los atributos correspondientes a su tipo
        personaje = EstablecerTipo(personaje, tipo);
        
        // Se añade a la lista de aliados o de enemigos, poniendole el tag correspondiente
        if (GetComponentInParent<Servidor>().nombresJugadores[0] == nombre.Split(':')[0])
        {
            personajeObjeto.transform.GetChild(0).tag = "Aliado";
            aliados.Add(personaje);
        }
        else
        {
            personajeObjeto.transform.GetChild(0).tag = "Enemigo";
            enemigos.Add(personaje);
        }
    }

    public Personaje EstablecerTipo(Personaje personaje, string tipo)
    {
        // Se guarda el tipo
        personaje.objeto.transform.parent.name = tipo;
        personaje.tipo = tipo;

        // Se le ponen los atributos en el personaje
        personaje.vida = personaje.objeto.GetComponentInParent<Atributos>().vida;
        personaje.daño = personaje.objeto.GetComponentInParent<Atributos>().daño;
        personaje.velocidadAtaque = personaje.objeto.GetComponentInParent<Atributos>().velocidadAtaque;
        personaje.rango = personaje.objeto.GetComponentInParent<Atributos>().rango;

        // Se le pone el color en la barra de vida
        if (personaje.objeto != gameObject)
        {
            foreach (Image imagen in personaje.objeto.transform.GetComponentsInChildren<Image>())
            {
                if (personaje.objeto.name.Split(':')[0] == nombresJugadoresaux[0])
                {
                    imagen.color = Color.cyan;
                }
                else if (personaje.objeto.name.Split(':')[0] == nombresJugadoresaux[1])
                {
                    imagen.color = Color.red;
                }
                else if (personaje.objeto.name.Split(':')[0] == nombresJugadoresaux[2])
                {
                    imagen.color = Color.yellow;
                }
            }
        }
        return personaje;
    }

    void OnGUI()
    {
        if (menuActivo)
        {
            return;
        }
        
        Event evento = Event.current;
        if (evento.type == EventType.MouseDrag)
        {
            if (!estasArrastrando)
            {
                estasArrastrando = true;
                posicionInicialRatonGUI = evento.mousePosition;
            }
        }

        if (evento.type == EventType.MouseUp)
        {
            estasArrastrando = false;
        }

        // Se dibuja la caja de selección
        if (estasArrastrando)
        {
            posicionActualRatonGUI = evento.mousePosition;
            cajaSeleccion = new Rect(posicionInicialRatonGUI.x, posicionInicialRatonGUI.y, posicionActualRatonGUI.x - posicionInicialRatonGUI.x, posicionActualRatonGUI.y - posicionInicialRatonGUI.y);
            GUI.Box(cajaSeleccion, GUIContent.none, estilo);
        }
    }

    void AsignarPosiciones ()
    {
        List <KeyValuePair<Personaje, float>> distancias = new List <KeyValuePair<Personaje, float>>();
        float distancia = 0;
        int indiceActual = 0;
        float distanciaMin = 0;
        int indiceAux = 0;
        List<Vector3> posicionesAux = new List<Vector3>();

        foreach (Vector3 posicion in posiciones)
        {
            GameObject.Instantiate(objetivo, posicion, Quaternion.Euler(90, 0, 0)).GetComponent<SpriteRenderer>().color = new Color32(154, 181, 231, 0);
            posicionesAux.Add(posicion);
        }

        // Guarda cada aliado seleccionado con la suma de las distancias a cada uno de los destinos
        for (int i = 0; i < aliados.Count; i++)
        {
            if (aliados[i].seleccionado)
            {
                distancia = 0;
                foreach (Vector3 posicion in posiciones)
                {
                    distancia = distancia + Vector3.Distance(aliados[i].objeto.transform.position, posicion);
                }
                personaje = aliados[i];
                distancias.Add(new KeyValuePair<Personaje, float> (personaje, distancia));
                aliados[i].objeto.GetComponents<AudioSource>()[1].Play();
            }
        }

        // Ordena la lista de mayor a menor distancia
        distancias.Sort((x, y) => y.Value.CompareTo(x.Value));

        // A cada aliado (empezando por el que esta mas lejos de todas las posiciones) se le deja elegir posición y elige la más cercana
        while (0 < distancias.Count)
        {
            distancia = 0;
            distanciaMin = Mathf.Infinity;
            for (int j = 0; j < posicionesAux.Count; j++)
            {
                distancia = Vector3.Distance(distancias[0].Key.objeto.transform.position, posicionesAux[j]);
                if (distancia < distanciaMin){
                    distanciaMin = distancia;
                    indiceActual = j;
                }
            }

            personaje = distancias[0].Key;
            personaje.destino = posicionesAux[indiceActual];
            aliados[aliados.FindIndex(x => x.objeto.name == distancias[0].Key.objeto.name)] = personaje;

            for (int k = 1; k < distancias.Count; k++)
            {
                personaje = distancias[k].Key;
                personaje.velocidadAtaque = personaje.velocidadAtaque - Vector3.Distance(personaje.objeto.transform.position, posicionesAux[indiceActual]);
                indiceAux = aliados.FindIndex(x => x.objeto.name == distancias[k].Key.objeto.name);
                aliados[indiceAux] = personaje;
                distancias[k] = new KeyValuePair<Personaje, float>(aliados[indiceAux], distancias[k].Value);
            }

            distancias.Remove(distancias[0]);
            distancias.Sort((x, y) => y.Value.CompareTo(x.Value));
            posicionesAux.Remove(posicionesAux[indiceActual]);

            Dictionary <string, object> destinosAliados = new Dictionary<string, object>();
            for (int i = 0; i < aliados.Count; i++)
            {
                if (aliados[i].seleccionado)
                {
                    destinosAliados.Add(aliados[i].objeto.name.Split(':')[1] + "/d", aliados[i].destino + "");
                }
            }

            GetComponentInParent<Servidor>().AsignarDestinosAliadosServidor(destinosAliados);
        }


        // Creamos una lista auxiliar de aliados seleccionados y en una matriz a cada aliado (fila) guardamos su distancia a cada posición (columna)
        /*distancias = new List<float> [numAliados];
        List<Personaje> aliadosSeleccionados = new List<Personaje>();
        int k = 0;
        for (int i = 0; i< aliados.Count; i++)
        {
            if (aliados[i].seleccionado)
            {
                distancias[k] = new List<float>();
                aliadosSeleccionados.Add(aliados[i]);
                for (int j = 0; j < posiciones.Count; j++)
                {
                    distancias[k].Add(Vector3.Distance(aliados[i].objeto.transform.position, posiciones[j]));
                }
                k++;
            }
        }

        // Hacer las diagonales directas de la matriz sumando por cada aliado (filas) su distacia a cada posicion (columnas) y guardando el índice de la posicion correspondiente al primer aliado así como un flag que indica como recuperar las posiciones del resto de aliados para la menor suma de distancias.
        distanciaMin = Mathf.Infinity;
        bool diagonalPositiva = true;
        for (int j = 0; j < posiciones.Count; j++)
        {
            distanciaActual = 0;
            for (int i = 0; i < aliadosSeleccionados.Count; i++)
            {
                distanciaActual = distanciaActual + distancias[i][(j + i) % aliadosSeleccionados.Count];   
            }
            if (distanciaActual < distanciaMin)
            {
                distanciaMin = distanciaActual;
                objetivoMinimo = j;
            }
        }

        // Hacer las diagonales inversas de la matriz sumando por cada aliado (filas) su distacia a cada posicion (columnas) y guardando el índice de la posicion correspondiente al primer aliado así como un flag que indica como recuperar las posiciones del resto de aliados para la menor suma de distancias.
        for (int j = posiciones.Count-1; j >= 0; j--)
        {
            distanciaActual = 0;
            for (int i = 0; i < aliadosSeleccionados.Count; i++)
            {
                distanciaActual = distanciaActual + distancias[i][(objetivoMinimo - i + aliadosSeleccionados.Count) % aliadosSeleccionados.Count];
            }
            if (distanciaActual < distanciaMin)
            {
                distanciaMin = distanciaActual;
                objetivoMinimo = j;
                diagonalPositiva = false;
            }
        }

        // Se asignan las posiciones a cada aliado recuperándolas a partir del índice inicial y el flag que indica si se ha obtenido de una diagonal directa o inversa
        for (int i = 0; i < aliadosSeleccionados.Count; i++)
        {
            personaje = aliados[aliados.FindIndex(x => x.Equals(aliadosSeleccionados[i]))];
            if (diagonalPositiva)
            {
                personaje.destino = posiciones[(objetivoMinimo + i) % aliadosSeleccionados.Count];
            }
            else
            {
                personaje.destino = posiciones[(objetivoMinimo - i + aliadosSeleccionados.Count) % aliadosSeleccionados.Count];
            }
            aliados[aliados.FindIndex(x => x.Equals(aliadosSeleccionados[i]))] = personaje;
        }*/
    }

    void GenerarDestinos ()
    {
        // Repartir posiciones
        posiciones.Clear();

        //Obtiene el máximo ancho y alto actualizandolo en cada iteración
        for (int i = 0; i < aliados.Count; i++)
        {
            if (aliados[i].seleccionado)
            {
                maxX = Mathf.Max((aliados[i].objeto.GetComponent<BoxCollider>().size.x * aliados[i].objeto.transform.localScale.x * 10), maxX);
                maxY = Mathf.Max((aliados[i].objeto.GetComponent<BoxCollider>().size.y * aliados[i].objeto.transform.localScale.z * 10), maxY);
            }
        }

        //Obtiene el tamaño en X y en Y de la selección rectangular con el ratón
        seleccionX = posicionActualRatonMundo.x - posicionInicialRatonMundo.x;
        seleccionY = posicionActualRatonMundo.z - posicionInicialRatonMundo.z;

        //Obtiene en número de filas, de columnas, y la capacidad total de la selección
        columnasSeleccion = Mathf.Abs((int)(seleccionX / maxX));
        filasSeleccion = Mathf.Abs((int)(seleccionY / maxY));
        capacidadSeleccion = filasSeleccion * columnasSeleccion;

        // Si no caben hacer que quepan
        if (capacidadSeleccion < numAliados)
        {
            // Si se ha hecho clic se crea un cuadrado
            if (Vector2.Distance(posicionInicialRatonMundo, posicionActualRatonMundo) < 1)
            {
                seleccionX = 1;
                seleccionY = 1;
                seleccionInicialX = 0;
                seleccionInicialY = 0;
            }
            else
            {
                seleccionInicialX = seleccionX;
                seleccionInicialY = seleccionY;
            }

            // En cualquier caso se extrapola la selección
            while (capacidadSeleccion < numAliados)
            {
                if (seleccionX == 0)
                {
                    seleccionX = 0.01f;
                }
                else if (seleccionY == 0)
                {
                    seleccionY = 0.01f;
                }
                seleccionX = seleccionX * 1.1f;
                seleccionY = seleccionY * 1.1f;

                //Obtiene en número de filas, de columnas, y la capacidad total de la selección
                columnasSeleccion = Mathf.Abs((int)(seleccionX / maxX));
                filasSeleccion = Mathf.Abs((int)(seleccionY / maxY));
                capacidadSeleccion = filasSeleccion * columnasSeleccion;
            }

            posicionInicialRatonMundo = new Vector3(posicionInicialRatonMundo.x - seleccionX / 2 + seleccionInicialX / 2, mapa.transform.position.y + 1, posicionInicialRatonMundo.z - seleccionY / 2 + seleccionInicialY / 2);
        }

        // Obtiene el número máximo de columnas que tendrá cada fila
        nFilas = Mathf.CeilToInt((float)numAliados / (float)columnasSeleccion);
        // Obtiene el número máximo de columnas que tendrá cada fila
        nColumnas = Mathf.CeilToInt((float)numAliados / (float)nFilas);

        aliadosSinDistribuir = numAliados;
        // Comprueba si cuantas columnas hay en cada fila 

        for (int i = 0; i < nFilas; i++)
        {
            aliadosSinDistribuir = aliadosSinDistribuir - nColumnas;

            if (aliadosSinDistribuir < 0)
            {
                nColumnas += aliadosSinDistribuir;
            }

            // Obtiene la distancia a la que van a estar separadas cada una de sus posiciones
            distanciaPosicionesColumna = seleccionX / nColumnas;
            distanciaPosicionesFila = seleccionY / nFilas;

            // Obtiene el desplazamiento a sumar para obtener el centro de las posiciones
            desplazamientoCentroColumna = distanciaPosicionesColumna / 2;
            desplazamientoCentroFila = distanciaPosicionesFila / 2;

            for (int j = 0; j < nColumnas; j++)
            {
                posiciones.Add(new Vector3(posicionInicialRatonMundo.x + (distanciaPosicionesColumna * j) + desplazamientoCentroColumna, mapa.transform.position.y + 1, posicionInicialRatonMundo.z + (distanciaPosicionesFila * i) + desplazamientoCentroFila));
            }
        }
    }

    void ClonarUnidades()
    {
        // Clona los personajes (habilidad legendaria de rey trasgo)
        posiciones.Clear();
        numAliados = 0;
        int a = 1;
        posicionInicialRatonMundo = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(camara.transform.position.y - mapa.transform.position.y - 1)));
        maxX = 0;
        maxY = 0;

        if (aliados.Count <= 1)
        {
            return;
        }

        for (int i = 1; i < aliados.Count; i++)
        {
            maxX = Mathf.Max((aliados[i].objeto.GetComponent<BoxCollider>().size.x * aliados[i].objeto.transform.localScale.x * 10), maxX);
            maxY = Mathf.Max((aliados[i].objeto.GetComponent<BoxCollider>().size.y * aliados[i].objeto.transform.localScale.z * 10), maxY);
            numAliados++;
        }

        capacidadSeleccion = 0;
        seleccionX = 100;
        seleccionY = 100;

        while (capacidadSeleccion < numAliados)
        {
            seleccionX = seleccionX * 1.1f;
            seleccionY = seleccionY * 1.1f;

            // Obtiene en número de filas, de columnas, y la capacidad total de la selección
            columnasSeleccion = Mathf.Abs((int)(seleccionX / maxX));
            filasSeleccion = Mathf.Abs((int)(seleccionY / maxY));
            capacidadSeleccion = filasSeleccion * columnasSeleccion;
        }

        posicionInicialRatonMundo = new Vector3(posicionInicialRatonMundo.x - seleccionX / 2, mapa.transform.position.y + 1, posicionInicialRatonMundo.z - seleccionY / 2);

        // Obtiene el número máximo de columnas que tendrá cada fila
        nFilas = Mathf.CeilToInt((float)numAliados / (float)columnasSeleccion);
        // Obtiene el número máximo de columnas que tendrá cada fila
        nColumnas = Mathf.CeilToInt((float)numAliados / (float)nFilas);

        aliadosSinDistribuir = numAliados;
        // Comprueba si cuantas columnas hay en cada fila 

        //listaPersonajes = new Dictionary<string, Dictionary<string, object>>();

        for (int i = 0; i < nFilas; i++)
        {
            aliadosSinDistribuir = aliadosSinDistribuir - nColumnas;

            if (aliadosSinDistribuir < 0)
            {
                nColumnas += aliadosSinDistribuir;
            }

            // Obtiene la distancia a la que van a estar separadas cada una de sus posiciones
            distanciaPosicionesColumna = seleccionX / nColumnas;
            distanciaPosicionesFila = seleccionY / nFilas;

            // Obtiene el desplazamiento a sumar para obtener el centro de las posiciones
            desplazamientoCentroColumna = distanciaPosicionesColumna / 2;
            desplazamientoCentroFila = distanciaPosicionesFila / 2;

            for (int j = 0; j < nColumnas; j++)
            {
                p = new Dictionary<string, object>();
                posicion = new Vector3(posicionInicialRatonMundo.x + (distanciaPosicionesColumna * j) + desplazamientoCentroColumna, mapa.transform.position.y + 1, posicionInicialRatonMundo.z + (distanciaPosicionesFila * i) + desplazamientoCentroFila);
                p.Add("d", posicion + "");
                p.Add("t", aliados[a].tipo);
                listaPersonajes.Add(int.Parse(aliados[aliados.Count-1].objeto.name.Split(':')[1]) + a + "", p);
                a++;
            }
        }
        //GetComponentInParent<Servidor>().CrearNuevoPersonaje(listaPersonajes);
    }

    public void ComenzarCierreMapa ()
    {
        // Se empieza a cerrar el mapa
        cierreMapa = true;
        momentoCeroCierre = Time.time;
    }
}