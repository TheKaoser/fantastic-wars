using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    Ray rayo2D;
    Ray rayo3D;
    RaycastHit2D colisionado2D;
    RaycastHit colisionado3D;

    GameObject jugador;
    GameObject interfaz;
    GameObject aspirante;
    public GameObject aspirantePrefab;

    GameObject menuPrincipal;
    GameObject jugar;
    GameObject aspirantes;
    GameObject opciones;
    GameObject salir;
    Renderer[] renderPrincipal;
    AudioSource[] reproductoresPrincipal;
    BoxCollider2D[] colisionadoresPrincipal;

    GameObject menuJugar;
    TextMesh buscandoPartida;
    GameObject cancelarJugar;
    Renderer[] renderJugar;
    AudioSource[] reproductoresJugar;
    BoxCollider2D[] colisionadoresJugar;

    GameObject menuAspirantes;
    GameObject volverAspirantes;
    GameObject aspiranteIzquierda;
    GameObject aspiranteDerecha;
    GameObject aspiranteNombre;
    GameObject aspiranteImagen;
    GameObject descripcionAspirante;
    GameObject habilidadIzquierda;
    GameObject habilidadDerecha;
    GameObject nombreHabilidad;
    GameObject iconoHabilidad;
    GameObject descripcionHabilidad;
    Renderer[] renderAspirantes;
    AudioSource[] reproductoresAspirantes;
    BoxCollider2D[] colisionadoresAspirantes;

    GameObject menuOpciones1;
    GameObject volverOpciones;
    GameObject ventana1;
    GameObject pantallaCompleta1;
    GameObject ventana2;
    GameObject pantallaCompleta2;
    GameObject controles;
    GameObject detalleControles;
    GameObject volverImagen;
    Renderer[] renderOpciones;
    AudioSource[] reproductoresOpciones;
    BoxCollider2D[] colisionadoresOpciones;
    Canvas[] canvasOpciones;
    Renderer[] renderImagen;
    AudioSource[] reproductoresImagen;
    BoxCollider2D[] colisionadoresImagen;
    GameObject conceder;

    GameObject menuOpciones2;
    GameObject camara;
    bool camaraYendo;
    bool camaraMoviendose;
    Vector3 direccion;
    Vector3 velocidad = Vector3.zero;
    bool activarSiguienteMenu;
    string[] nombresAspirantes = new string[3] { "Rey Trasgo", "Hechicero Elemental", "Diosa Divina" };
    string[] descripcionesAspirantes = new string[3] { "Soberano cruel que se transporta arrastrado por súbditos.\nSu poder reside en invocar criaturas y dirigirlas para abrirse paso\ny hacerse con el control del reino a través de batallas encarnizadas", "Capaz de controlar los elementos a su antojo.\nEl hechicero es completamente independiente y se sirve únicamente de su magia\npara dominar la partida. Además goza de una gran resistencia en el combate", "Vulnerable ante daños profanos e invulnerable en zonas sagradas.\nLa Diosa canaliza su energía para alzar diversos edificios y poblar así el territorio.\nDesde este podrá optar por ampliar sus dominios o defenderlos de sus enemigos" };
    string[,] nombresHabilidades = new string[3, 4] { { "Convocar Espadachín", "Convocar Arquero", "Convocar Trol", "Convocar Escuadrón" },
                                                      { "Invocar al Fuego", "Invocar al Aire", "Invocar a la Tierra", "Invocar al Agua" },
                                                      { "Alzar Templo", "Alzar Muralla", "Alzar Cuartel", "Alzar Maravilla" } };

    [SerializeField]
    RuntimeAnimatorController[] imagenenesAspirantes = new RuntimeAnimatorController[3];
    [SerializeField]
    Sprite[] iconosHabilidadesTrasgo = new Sprite[4];
    [SerializeField]
    Sprite[] iconosHabilidadesHechicero = new Sprite[4];
    [SerializeField]
    Sprite[] iconosHabilidadesDiosa = new Sprite[4];
    string[,] descripcionesHabilidades = new string[3, 4] { {   "Genera un Espadachín", "Genera un Arquero capaz de alcanzar unidades aéreas", "Genera un Trol incapaz de atacar.\nLas unidades aliadas circundantes obtienen un aumento de vida", "Duplica tus unidades actuales" },
                                                            {   "Lanza una llamarada en la dirección indicada\ncausando daños a toda unidad que atraviese.\nSi controlas un clon, este lanzará una llamarada adicional", "Aparece una ráfaga de viento que desplaza al hechicero rápidamente.\nSi controlas un clon, ocupará la posición del hechicero", "Provoca una elevación del terreno con forma de “x” que ralentiza el paso.\nSi controlas un clon, la elevación será de mayor tamaño", "El agua conforma un clon invulnerable del hechicero durante breves\ninstantes que modifica el resto de sus habilidades" },
                                                            {   "Construye una estructura que produce oro de manera continua.\nCuanto menor sea su distancia al trono, mayores son las ganancias", "Construye un bloque defensivo entre dos regiones\nque bloquea el paso y los ataques entre ambas", "Construye una estructura que genera ángeles periódicamente.\nEstos tienen la capacidad de sobrevolar los accidentes del terreno,\natacan en área y no pueden ser objetivo de unidades terrestres", "Construye una estructura que cura a los aliados\nal mismo tiempo que daña a los enemigos sobre un área" } };

    [SerializeField]
    int indiceAspirantes = 0;
    [SerializeField]
    int indiceHabilidades = 0;

    public GameObject particulasSpawn;
    public Vector3 posicionSpawn;

    Color temporal;
    float velocidad2;
    bool volviendoAlMenu;
    bool llamarFuncion;
    Coroutine buscarPartida;

    // Busca los menus, sus componentes y sus atributos que se modificarán
    void Start()
    {
        StartCoroutine(Cargando());

        // General
        camara = GameObject.Find("Camara");

        // Menu principal
        menuPrincipal = GameObject.Find("Menu principal");
        jugar = GameObject.Find("Jugar");
        aspirantes = GameObject.Find("Aspirantes");
        opciones = GameObject.Find("Opciones");
        salir = GameObject.Find("Salir");
        renderPrincipal = menuPrincipal.GetComponentsInChildren<Renderer>();
        reproductoresPrincipal = menuPrincipal.GetComponentsInChildren<AudioSource>();
        colisionadoresPrincipal = menuPrincipal.GetComponentsInChildren<BoxCollider2D>();

        //Jugar
        menuJugar = GameObject.Find("Menu jugar");
        buscandoPartida = GameObject.Find("Buscando partida").GetComponent<TextMesh>();
        cancelarJugar = GameObject.Find("Cancelar");
        renderJugar = menuJugar.GetComponentsInChildren<Renderer>();
        reproductoresJugar = menuJugar.GetComponentsInChildren<AudioSource>();
        colisionadoresJugar = menuJugar.GetComponentsInChildren<BoxCollider2D>();

        //Aspirantes
        menuAspirantes = GameObject.Find("Menu aspirantes");
        volverAspirantes = GameObject.Find("Volver aspirantes");
        aspiranteIzquierda = GameObject.Find("Aspirante izquierda");
        aspiranteDerecha = GameObject.Find("Aspirante derecha");
        habilidadIzquierda = GameObject.Find("Habilidad izquierda");
        habilidadDerecha = GameObject.Find("Habilidad derecha");
        aspiranteNombre = GameObject.Find("Aspirante nombre");
        aspiranteImagen = GameObject.Find("Aspirante imagen");
        descripcionAspirante = GameObject.Find("Descripcion");
        nombreHabilidad = GameObject.Find("Nombre habilidad");
        iconoHabilidad = GameObject.Find("Icono habilidad");
        descripcionHabilidad = GameObject.Find("Descripción habilidad");
        renderAspirantes = menuAspirantes.GetComponentsInChildren<Renderer>();
        reproductoresAspirantes = menuAspirantes.GetComponentsInChildren<AudioSource>();
        colisionadoresAspirantes = menuAspirantes.GetComponentsInChildren<BoxCollider2D>();

        // Opciones
        menuOpciones1 = GameObject.Find("Menu opciones 1");
        menuOpciones2 = GameObject.Find("Menu opciones 2");
        volverOpciones = GameObject.Find("Volver opciones");
        ventana1 = GameObject.Find("Ventana 1");
        pantallaCompleta1 = GameObject.Find("Pantalla completa 1");
        ventana2 = GameObject.Find("Ventana 2");
        pantallaCompleta2 = GameObject.Find("Pantalla completa 2");
        controles = GameObject.Find("Controles");
        detalleControles = GameObject.Find("Controles detalle");
        volverImagen = GameObject.Find("Volver imagen");
        renderOpciones = menuOpciones1.GetComponentsInChildren<Renderer>();
        reproductoresOpciones = menuOpciones1.GetComponentsInChildren<AudioSource>();
        colisionadoresOpciones = menuOpciones1.GetComponentsInChildren<BoxCollider2D>();
        canvasOpciones = menuOpciones1.GetComponentsInChildren<Canvas>();
        renderImagen = detalleControles.GetComponentsInChildren<Renderer>();
        reproductoresImagen = detalleControles.GetComponentsInChildren<AudioSource>();
        colisionadoresImagen = detalleControles.GetComponentsInChildren<BoxCollider2D>();
        conceder = GameObject.Find("Conceder");

        GameObject.Find("Cielo").GetComponent<Renderer>().sortingOrder = -11000;

        jugador = GameObject.Find("Jugador");
        interfaz = GameObject.Find("Interfaz");

        // Opciones iniciales
        ActualizarOpciones();
    }

    IEnumerator Cargando ()
    {
        yield return new WaitUntil(() => GetComponentInChildren<VideoPlayer>().isPrepared);
        Material material = new Material(Shader.Find("Standard"));
        material.color = Color.white;
        transform.GetChild(0).GetComponent<Renderer>().material = material;
        GameObject.Find("Engranajes").GetComponent<SpriteRenderer>().enabled = false;
        foreach (BoxCollider2D c in menuPrincipal.GetComponentsInChildren<BoxCollider2D>())
        {
            c.enabled = true;
        }
        foreach (Renderer r in menuPrincipal.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
    }

    // Hace el texto grande y pequeño y desplaza la cámara para viajar a otros menús
    void Update()
    {
        // Intersectar colisionadores 2D y 3D
        rayo2D = Camera.main.ScreenPointToRay(Input.mousePosition);
        colisionado2D = Physics2D.GetRayIntersection(rayo2D);

        rayo3D = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(rayo3D, out colisionado3D);

        // Camara hacia destino
        if (camaraYendo)
        {
            camara.transform.position = Vector3.SmoothDamp(camara.transform.position, direccion, ref velocidad, 1.5f);
            if (Vector2.Distance(camara.transform.position, direccion) < 100)
            {
                velocidad = Vector3.zero;
                camaraYendo = false;
                activarSiguienteMenu = true;
            }
            else if (Vector2.Distance(camara.transform.position, direccion) < 1000)
            {
                camaraMoviendose = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (colisionado2D.collider)
            {
                // colisionado2D.collider.gameObject.GetComponents<AudioSource>()[0].Play();
                if (colisionado2D.collider.gameObject.gameObject == jugar)
                {
                    direccion = new Vector3(2400, -2600, -6600);
                    PasarAlSiguienteMenu(renderPrincipal, reproductoresPrincipal, colisionadoresPrincipal, new Canvas[] { }, renderJugar, reproductoresJugar, colisionadoresJugar, new Canvas[] { });
                    buscarPartida = StartCoroutine(BuscandoPartida());
                }
                else if (colisionado2D.collider.gameObject == aspirantes)
                {
                    direccion = new Vector3(1500, 1900, -7000);
                    PasarAlSiguienteMenu(renderPrincipal, reproductoresPrincipal, colisionadoresPrincipal, new Canvas[] { }, renderAspirantes, reproductoresAspirantes, colisionadoresAspirantes, new Canvas[] { });
                }
                else if (colisionado2D.collider.gameObject == opciones)
                {
                    direccion = new Vector3(-4650, -1600, -7400);
                    PasarAlSiguienteMenu(renderPrincipal, reproductoresPrincipal, colisionadoresPrincipal, new Canvas[] { }, renderOpciones, reproductoresOpciones, colisionadoresOpciones, canvasOpciones);
                }
                else if (colisionado2D.collider.gameObject == salir)
                {
                    Application.Quit();
                }
                else if (colisionado2D.collider.gameObject == cancelarJugar)
                {
                    direccion = new Vector3(-130, -150, -13800);
                    StopCoroutine(buscarPartida);
                    // jugador.GetComponent<Jugador>().CerrarConexion();
                    jugador.GetComponent<Servidor>().DejarDeBuscarPartida();
                    PasarAlSiguienteMenu(renderJugar, reproductoresJugar, colisionadoresJugar, new Canvas[] { }, renderPrincipal, reproductoresPrincipal, colisionadoresPrincipal, new Canvas[] { });
                }
                else if (colisionado2D.collider.gameObject == volverAspirantes)
                {
                    direccion = new Vector3(-130, -150, -13800);
                    PasarAlSiguienteMenu(renderAspirantes, reproductoresAspirantes, colisionadoresAspirantes, new Canvas[] { }, renderPrincipal, reproductoresPrincipal, colisionadoresPrincipal, new Canvas[] { });
                }
                else if (colisionado2D.collider.gameObject == volverOpciones)
                {
                    GuardarCambiosOpciones();
                    direccion = new Vector3(-130, -150, -13800);
                    PasarAlSiguienteMenu(renderOpciones, reproductoresOpciones, colisionadoresOpciones, canvasOpciones, renderPrincipal, reproductoresPrincipal, colisionadoresPrincipal, new Canvas[] { });
                }
                else if (colisionado2D.collider.gameObject == aspiranteIzquierda)
                {
                    indiceAspirantes--;
                    if (indiceAspirantes < 0)
                    {
                        indiceAspirantes = nombresAspirantes.Length - 1;
                    }
                    aspiranteNombre.GetComponent<TextMesh>().text = nombresAspirantes[indiceAspirantes];
                    descripcionAspirante.GetComponent<TextMesh>().text = descripcionesAspirantes[indiceAspirantes];
                    nombreHabilidad.GetComponent<TextMesh>().text = nombresHabilidades[indiceAspirantes, 0];
                    descripcionHabilidad.GetComponent<TextMesh>().text = descripcionesHabilidades[indiceAspirantes, 0];

                    // Se muestra la animación de cada aspirante y su primer icono
                    if (indiceAspirantes == 0)
                    {
                        aspiranteImagen.GetComponent<Animator>().runtimeAnimatorController = imagenenesAspirantes[0];
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesTrasgo[0];
                    }
                    else if (indiceAspirantes == 1)
                    {
                        aspiranteImagen.GetComponent<Animator>().runtimeAnimatorController = imagenenesAspirantes[1];
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesHechicero[0];
                    }
                    else if (indiceAspirantes == 2)
                    {
                        aspiranteImagen.GetComponent<Animator>().runtimeAnimatorController = imagenenesAspirantes[2];
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesDiosa[0];
                    }
                }
                else if (colisionado2D.collider.gameObject == aspiranteDerecha)
                {
                    indiceAspirantes++;
                    if (indiceAspirantes > nombresAspirantes.Length - 1)
                    {
                        indiceAspirantes = 0;
                    }
                    aspiranteNombre.GetComponent<TextMesh>().text = nombresAspirantes[indiceAspirantes];
                    descripcionAspirante.GetComponent<TextMesh>().text = descripcionesAspirantes[indiceAspirantes];
                    nombreHabilidad.GetComponent<TextMesh>().text = nombresHabilidades[indiceAspirantes, 0];
                    descripcionHabilidad.GetComponent<TextMesh>().text = descripcionesHabilidades[indiceAspirantes, 0];

                    // Se muestra la animación de cada aspirante y su primer icono
                    if (indiceAspirantes == 0)
                    {
                        aspiranteImagen.GetComponent<Animator>().runtimeAnimatorController = imagenenesAspirantes[0];
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesTrasgo[0];
                    }
                    else if (indiceAspirantes == 1)
                    {
                        aspiranteImagen.GetComponent<Animator>().runtimeAnimatorController = imagenenesAspirantes[1];
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesHechicero[0];
                    }
                    else if (indiceAspirantes == 2)
                    {
                        aspiranteImagen.GetComponent<Animator>().runtimeAnimatorController = imagenenesAspirantes[2];
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesDiosa[0];
                    }
                }
                else if (colisionado2D.collider.gameObject == habilidadIzquierda)
                {
                    indiceHabilidades--;
                    if (indiceHabilidades < 0)
                    {
                        indiceHabilidades = 3;
                    }
                    nombreHabilidad.GetComponent<TextMesh>().text = nombresHabilidades[indiceAspirantes, indiceHabilidades];
                    descripcionHabilidad.GetComponent<TextMesh>().text = descripcionesHabilidades[indiceAspirantes, indiceHabilidades];

                    // Se muestra el icono de la habilidad correspondiente según el aspirante
                    if (indiceAspirantes == 0)
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesTrasgo[indiceHabilidades];
                    else if (indiceAspirantes == 1)
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesHechicero[indiceHabilidades];
                    else if (indiceAspirantes == 2)
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesDiosa[indiceHabilidades];
                }
                else if (colisionado2D.collider.gameObject == habilidadDerecha)
                {
                    indiceHabilidades++;
                    if (indiceHabilidades > 3)
                    {
                        indiceHabilidades = 0;
                    }
                    nombreHabilidad.GetComponent<TextMesh>().text = nombresHabilidades[indiceAspirantes, indiceHabilidades];
                    descripcionHabilidad.GetComponent<TextMesh>().text = descripcionesHabilidades[indiceAspirantes, indiceHabilidades];

                    // Se muestra el icono de la habilidad correspondiente según el aspirante
                    if (indiceAspirantes == 0)
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesTrasgo[indiceHabilidades];
                    else if (indiceAspirantes == 1)
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesHechicero[indiceHabilidades];
                    else if (indiceAspirantes == 2)
                        iconoHabilidad.GetComponent<SpriteRenderer>().sprite = iconosHabilidadesDiosa[indiceHabilidades];
                }
                else if (colisionado2D.collider.gameObject == ventana1)
                {
                    Screen.fullScreen = false;
                    colisionado2D.collider.gameObject.GetComponent<TextMesh>().color = new Color32(245, 240, 150, 255);
                    pantallaCompleta1.GetComponent<TextMesh>().color = Color.white;
                }
                else if (colisionado2D.collider.gameObject == pantallaCompleta1)
                {
                    Screen.fullScreen = true;
                    colisionado2D.collider.gameObject.GetComponent<TextMesh>().color = new Color32(245, 240, 150, 255);
                    ventana1.GetComponent<TextMesh>().color = Color.white;
                }
                else if (colisionado2D.collider.gameObject == controles)
                {
                    direccion = new Vector3(-4554, -1569, -7534);
                    camaraMoviendose = true;
                    PasarAlSiguienteMenu(renderOpciones, reproductoresOpciones, colisionadoresOpciones, canvasOpciones, renderImagen, reproductoresImagen, colisionadoresImagen, new Canvas[] { });
                }
                else if (colisionado2D.collider.gameObject == volverImagen)
                {
                    direccion = new Vector3(-4554, -1569, -7534);
                    camaraMoviendose = true;
                    PasarAlSiguienteMenu(renderImagen, reproductoresImagen, colisionadoresImagen, new Canvas[] { }, renderOpciones, reproductoresOpciones, colisionadoresOpciones, canvasOpciones);
                }
            }
            else if (colisionado3D.collider)
            {
                if (colisionado3D.collider.gameObject.name == ventana2.name)
                {
                    Screen.fullScreen = false;
                    colisionado3D.collider.gameObject.GetComponent<TextMesh>().color = new Color32(245, 240, 150, 255);
                    pantallaCompleta2.GetComponent<TextMesh>().color = Color.white;
                }
                else if (colisionado3D.collider.gameObject.name == pantallaCompleta2.name)
                {
                    Screen.fullScreen = true;
                    colisionado3D.collider.gameObject.GetComponent<TextMesh>().color = new Color32(245, 240, 150, 255);
                    ventana2.GetComponent<TextMesh>().color = Color.white;
                }
                else if (colisionado3D.collider.gameObject.name == conceder.name)
                {
                    jugador.GetComponent<Servidor>().victoria = -1;
                    OpcionesPartida();
                    jugador.GetComponent<Servidor>().MatarAliadoServidor("0");
                    StartCoroutine(VolverAlMenu());
                }
            }
        }

        if (volviendoAlMenu)
        {
            temporal = camara.GetComponentsInChildren<SpriteRenderer>()[0].color;
            temporal.a = Mathf.SmoothDamp(camara.GetComponentsInChildren<SpriteRenderer>()[0].color.a, 1, ref velocidad2, 1);
            camara.GetComponentsInChildren<SpriteRenderer>()[0].color = temporal;
        }
    }

    public bool OpcionesPartida()
    {
        bool activado = false;
        foreach (Renderer r in menuOpciones2.GetComponentsInChildren<Renderer>())
        {
            if (r.enabled)
            {
                activado = true;
            }
            r.enabled = !r.enabled;
        }

        foreach (BoxCollider c in menuOpciones2.GetComponentsInChildren<BoxCollider>())
        {
            c.enabled = !c.enabled;
        }

        foreach (Canvas c in menuOpciones2.GetComponentsInChildren<Canvas>())
        {
            c.enabled = !c.enabled;
        }

        foreach (AudioSource a in menuOpciones2.GetComponentsInChildren<AudioSource>())
        {
            a.enabled = !a.enabled;
        }

        if (activado)
        {
            GuardarCambiosOpciones();
        }

        return !activado;
    }

    void GuardarCambiosOpciones()
    {
        float musica;
        float efectos;
        bool PantallaCompleta;
        float tamanioInterfaz;

        if (!jugador.GetComponent<Servidor>().partidaEmpezada)
        {
            musica = GameObject.Find("Musica deslizador 1").GetComponentInChildren<Slider>().value;
            efectos = GameObject.Find("Efectos deslizador 1").GetComponentInChildren<Slider>().value;
            PantallaCompleta = pantallaCompleta1.GetComponent<TextMesh>().color != Color.white;
            tamanioInterfaz = GameObject.Find("Interfaz deslizador 1").GetComponentInChildren<Slider>().value;
        }
        else
        {
            musica = GameObject.Find("Musica deslizador 2").GetComponentInChildren<Slider>().value;
            efectos = GameObject.Find("Efectos deslizador 2").GetComponentInChildren<Slider>().value;
            PantallaCompleta = pantallaCompleta2.GetComponent<TextMesh>().color != Color.white;
            tamanioInterfaz = GameObject.Find("Interfaz deslizador 2").GetComponentInChildren<Slider>().value;
        }

        PlayerPrefs.SetFloat("Musica", musica);
        PlayerPrefs.SetFloat("Efectos", efectos);
        if (PantallaCompleta)
        {
            PlayerPrefs.SetString("PantallaCompleta", "Si");
        }
        else
        {
            PlayerPrefs.SetString("PantallaCompleta", "No");
        }
        PlayerPrefs.SetFloat("Interfaz", tamanioInterfaz);

        ActualizarOpciones();
    }

    void ActualizarOpciones()
    {
        if (PlayerPrefs.HasKey("Musica"))
        {
            GameObject.Find("Musica deslizador 1").GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Musica");
            GameObject.Find("Musica deslizador 2").GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Musica");
        }
        else
        {
            PlayerPrefs.SetFloat("Musica", 0.75f);
        }

        if (PlayerPrefs.HasKey("Efectos"))
        {
            GameObject.Find("Efectos deslizador 1").GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Efectos");
            GameObject.Find("Efectos deslizador 2").GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Efectos");
        }
        else
        {
            PlayerPrefs.SetFloat("Efectos", 0.75f);
        }

        if (PlayerPrefs.HasKey("PantallaCompleta"))
        {
            if (PlayerPrefs.GetString("PantallaCompleta") == "No")
            {
                ventana1.GetComponent<TextMesh>().color = new Color32(245, 240, 150, 255);
                pantallaCompleta1.GetComponent<TextMesh>().color = Color.white;
                ventana2.GetComponent<TextMesh>().color = new Color32(245, 240, 150, 255);
                pantallaCompleta2.GetComponent<TextMesh>().color = Color.white;
            }
        }
        else
        {
            PlayerPrefs.SetString("PantallaCompleta", "Si");
        }

        if (PlayerPrefs.HasKey("Interfaz"))
        {
            GameObject.Find("Interfaz deslizador 1").GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Interfaz");
            GameObject.Find("Interfaz deslizador 2").GetComponentInChildren<Slider>().value = PlayerPrefs.GetFloat("Interfaz");
        }
        else
        {
            PlayerPrefs.SetFloat("Interfaz", 0.7125f);
        }
    }

    // Borra un menú y llama a la función que pinta el siguiente
    void PasarAlSiguienteMenu(Renderer[] rendersBorrar, AudioSource[] reproductoresBorrar, BoxCollider2D[] colisionadoresBorrar, Canvas[] canvasBorrar, Renderer[] rendersPintar, AudioSource[] reproductoresPintar, BoxCollider2D[] colisionadoresPintar, Canvas[] canvasPintar)
    {
        foreach (Renderer r in rendersBorrar)
            r.enabled = false;

        foreach (Canvas c in canvasBorrar)
            c.enabled = false;

        foreach (BoxCollider2D c in colisionadoresBorrar)
            c.enabled = false;

        camaraYendo = true;

        StartCoroutine(PintarMenu(reproductoresBorrar, rendersPintar, reproductoresPintar, colisionadoresPintar, canvasPintar));
    }

    // Hace aparecer un menú
    IEnumerator PintarMenu(AudioSource[] reproductoresBorrar, Renderer[] renders, AudioSource[] reproductores, BoxCollider2D[] colisionadores, Canvas[] canvas)
    {
        yield return new WaitForSeconds(1.2f);
        foreach (AudioSource a in reproductoresBorrar)
        {
            a.enabled = false;
        }

        yield return new WaitUntil(() => camaraMoviendose);
        foreach (Renderer r in renders)
        {
            r.enabled = true;
        }

        foreach (Canvas c in canvas)
        {
            c.enabled = true;
        }

        yield return new WaitUntil(() => activarSiguienteMenu);

        foreach (AudioSource a in reproductores)
        {
            a.enabled = true;
        }

        foreach (BoxCollider2D c in colisionadores)
        {
            c.enabled = true;
        }

        camaraMoviendose = false;
        activarSiguienteMenu = false;
    }

    // Modifica el texto de buscar la partida periódicamente
    IEnumerator BuscandoPartida()
    {
        yield return new WaitForSeconds(1.5f);
        llamarFuncion = true;
        while (!jugador.GetComponent<Servidor>().partidaEmpezada)
        {
            yield return new WaitForSeconds(1.1f);
            if (buscandoPartida.text == "Buscando partida")
            {
                buscandoPartida.text = "Buscando partida .";
            }
            else if (buscandoPartida.text == "Buscando partida .")
            {
                buscandoPartida.text = "Buscando partida ..";
            }
            else if (buscandoPartida.text == "Buscando partida ..")
            {
                buscandoPartida.text = "Buscando partida ...";
            }
            else
            {
                buscandoPartida.text = "Buscando partida";
                // jugador.GetComponent<Jugador>().enabled = true;
                if (llamarFuncion)
                {
                    jugador.GetComponent<Servidor>().BuscarPartida();
                    llamarFuncion = false;
                }
            }
        }

        camara.GetComponents<AudioSource>()[0].enabled = false;
        GetComponent<AudioSource>().Play();
        buscandoPartida.GetComponent<MeshRenderer>().enabled = false;
        cancelarJugar.GetComponent<MeshRenderer>().enabled = false;
        cancelarJugar.GetComponent<BoxCollider2D>().enabled = false;

        // Movimiento trono
        direccion = new Vector3(1400, -2600, 800);
        camaraYendo = true;
        camara.GetComponents<AudioSource>()[1].PlayDelayed(2.5f);
        yield return new WaitUntil(() => activarSiguienteMenu);
        activarSiguienteMenu = false;

        // Movimiento mapa
        camara.transform.rotation = Quaternion.Euler(90, 0, 0);
        GameObject.Find("Video Menu").GetComponent<MeshRenderer>().enabled = false;
        direccion = new Vector3(1400, -3500, 0);
        camaraYendo = true;
        yield return new WaitForSeconds(2f);

        // Movimiento spawn
        direccion = jugador.GetComponent<Servidor>().miSpawn;
        camaraYendo = true;
        yield return new WaitForSeconds(4.1f);
        interfaz.GetComponent<Canvas>().enabled = true;
        interfaz.GetComponent<Interfaz>().ActivarFlechas();

        posicionSpawn = new Vector3(camara.transform.position.x, GameObject.Find("Mapa").transform.position.y + 1, camara.transform.position.z);
        GameObject.Instantiate(particulasSpawn, posicionSpawn, Quaternion.Euler(90, 0, 0));
        yield return new WaitForSeconds(0.3f);
        // Instanciar el aspirante donde está mirando la cámara
        aspirante = GameObject.Instantiate(aspirantePrefab, posicionSpawn, Quaternion.identity);
        aspirante.transform.parent = jugador.transform;
        aspirante.transform.SetSiblingIndex(0);
        aspirante.transform.GetChild(0).name = jugador.GetComponent<Servidor>().nombresJugadores[0] + ":0";
        camaraYendo = false;
        activarSiguienteMenu = false;
        camaraMoviendose = false;
        velocidad = Vector3.zero;
        GameObject.Find("Slider Efectos 2").GetComponent<Volumen>().CambiarVolumen(PlayerPrefs.GetFloat("Efectos"));
    }

    public IEnumerator VolverAlMenu()
    {
        jugador.GetComponent<Servidor>().ReiniciarServidor();
        camara.GetComponents<AudioSource>()[1].enabled = false;
        volviendoAlMenu = true;
        camara.GetComponentInChildren<ParticleSystem>().Play(true);

        yield return new WaitForSeconds(1f); 
        // Derrota
        if(jugador.GetComponent<Servidor>().victoria == -1)
        {
            camara.GetComponentInChildren<Animator>().Play("Derrota Entrada");
        }
        // Victoria
        else if (jugador.GetComponent<Servidor>().victoria == 1)
        {
            camara.GetComponentInChildren<Animator>().Play("Victoria Entrada");
        }
        camara.transform.GetChild(1).GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(3f); 

        // Derrota
        if(jugador.GetComponent<Servidor>().victoria == -1)
        {
            camara.GetComponentInChildren<Animator>().Play("Derrota Salida");
        }
        // Victoria
        else if (jugador.GetComponent<Servidor>().victoria == 1)
        {
            camara.GetComponentInChildren<Animator>().Play("Victoria Salida");
        }
        
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(0);
    }
}