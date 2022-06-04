using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Auth;
using System.Linq;
using System;


public class Servidor : MonoBehaviour
{
    DatabaseReference referencia;
    DatabaseReference referenciaJugador;
    DatabaseReference referenciaJugadorPartida;
    DatabaseReference referenciaPartida;
    // En la posición 0 estamos nosotros
    public string[] nombresJugadores = new string[3];
    public string idPartida;
    bool haEncontrado;
    bool esElUltimo;
    List<Vector3> spawns = new List<Vector3>();
    public Vector3 miSpawn;
    Vector3 spawn1;
    Vector3 spawn2;
    public bool partidaEmpezada;
    Personaje personaje = new Personaje();

    public int victoria;

    GameObject trono;

    bool flagPartidaEmpezada = false;
    bool flagCambioFase = false;

    void Start()
    {
        
        spawns.Add(new Vector3(-1250, -10500, -690));
        spawns.Add(new Vector3(1880, -10500, 3080));
        spawns.Add(new Vector3(3615, -10500, -1500));

        int valorAleatorio = UnityEngine.Random.Range(0, 3);
        miSpawn = spawns.ElementAt(valorAleatorio);
        spawns.RemoveAt(valorAleatorio);
        valorAleatorio = UnityEngine.Random.Range(0, 2);
        spawn1 = spawns.ElementAt(valorAleatorio);
        spawns.RemoveAt(valorAleatorio);
        spawn2 = spawns.ElementAt(0);
        spawns.RemoveAt(0);
        trono = GameObject.Find("Trono");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(tarea =>
        {
            if (tarea.Result == Firebase.DependencyStatus.Available)
            {
                InicializarFirebase();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", tarea.Result));
            }
        });
    }

    // Se conecta con la base de datos
    void InicializarFirebase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://fantastic-wars.firebaseio.com/");
        referencia = FirebaseDatabase.DefaultInstance.RootReference;

        // Creamos un nuevo jugador
        nombresJugadores[0] = referencia.Child("Jugadores").Push().Key;
        referencia.Child("Jugadores").Child(nombresJugadores[0]).SetValueAsync("");
        referenciaJugador = FirebaseDatabase.DefaultInstance.RootReference.Child("Jugadores").Child(nombresJugadores[0]);

        // Si no existe el padre de las partidas se crea
        referencia.GetValueAsync().ContinueWith(tarea =>
        {
            if (tarea.IsFaulted)
            {
                return;
            }
            else if (tarea.IsCompleted)
            {
                if (!tarea.Result.HasChild("Partidas"))
                {
                    referencia.Child("Partidas").SetValueAsync("");
                }
            }
        });        
    }

    // Observa cambios en los personajes de la partida y actualiza localmente los cambios
    void DetectarCambioAtributo(object emisor, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        string nombrepersonaje;
        nombrepersonaje = args.Snapshot.Reference.Parent.Parent.Key + ":" + args.Snapshot.Reference.Parent.Key;

        // print ((string)args.Snapshot.Key);
        // print (args.Snapshot.Value.ToString());

        // Actualizar destino en local
        if (args.Snapshot.Key == "d")
        {
            if (GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().aliados.Find(x => x.objeto.name == nombrepersonaje);
                personaje = GetComponentInChildren<Control>().ActualizarDestinoPersonaje(personaje, StringAVector3((string)args.Snapshot.Value),true);
                GetComponentInChildren<Control>().aliados[GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == nombrepersonaje)] = personaje;
            }
            else if (GetComponentInChildren<Control>().enemigos.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().enemigos.Find(x => x.objeto.name == nombrepersonaje);
                personaje = GetComponentInChildren<Control>().ActualizarDestinoPersonaje(personaje, StringAVector3((string)args.Snapshot.Value),false);
                GetComponentInChildren<Control>().enemigos[GetComponentInChildren<Control>().enemigos.FindIndex(x => x.objeto.name == nombrepersonaje)] = personaje;
            }
        }
        // Actualizar objetivo[0] en local
        else if (args.Snapshot.Key == "o")
        {
            if (GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().aliados.Find(x => x.objeto.name == nombrepersonaje);
                personaje = GetComponentInChildren<Control>().ActualizarObjetivoPersonaje(personaje, ((string)args.Snapshot.Value));
                GetComponentInChildren<Control>().aliados[GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == nombrepersonaje)] = personaje;
            }
            else if (GetComponentInChildren<Control>().enemigos.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().enemigos.Find(x => x.objeto.name == nombrepersonaje);
                personaje = GetComponentInChildren<Control>().ActualizarObjetivoPersonaje(personaje, ((string)args.Snapshot.Value));
                GetComponentInChildren<Control>().enemigos[GetComponentInChildren<Control>().enemigos.FindIndex(x => x.objeto.name == nombrepersonaje)] = personaje;
            }
        }
        // Actualizar vida en local
        else if (args.Snapshot.Key == "v")
        {
            if (GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().aliados.Find(x => x.objeto.name == nombrepersonaje);
                GetComponentInChildren<Control>().ActualizarVidaPersonaje(personaje, int.Parse(args.Snapshot.Value.ToString()), true);
            }
            else if (GetComponentInChildren<Control>().enemigos.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().enemigos.Find(x => x.objeto.name == nombrepersonaje);
                GetComponentInChildren<Control>().ActualizarVidaPersonaje(personaje, int.Parse(args.Snapshot.Value.ToString()), false);
            }
        }
        // Actualizar ataque en local
        else if (args.Snapshot.Key == "g")
        {
            if (GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().aliados.Find(x => x.objeto.name == nombrepersonaje);
                args.Snapshot.Reference.Parent.Child("a").GetValueAsync().ContinueWith(tarea =>
                {
                    if (tarea.IsFaulted)
                    {
                        return;
                    }
                    else if (tarea.IsCompleted)
                    {
                        StartCoroutine(GetComponentInChildren<Control>().Golpe(personaje, int.Parse(tarea.Result.Value.ToString())));
                    }
                });
            }
            else if (GetComponentInChildren<Control>().enemigos.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
            {
                personaje = GetComponentInChildren<Control>().enemigos.Find(x => x.objeto.name == nombrepersonaje);
                args.Snapshot.Reference.Parent.Child("a").GetValueAsync().ContinueWith(tarea =>
                {
                    if (tarea.IsFaulted)
                    {
                        return;
                    }
                    else if (tarea.IsCompleted)
                    {
                       StartCoroutine(GetComponentInChildren<Control>().Golpe(personaje, int.Parse(tarea.Result.Value.ToString())));
                    }
                });
            }
        }
        // Actualizar entrada en trono
        else if (args.Snapshot.Key == "p")
        {
            if (int.Parse(args.Snapshot.Value.ToString()) == 1)
            {
                trono.GetComponentInChildren<Trono>().tiemposEnTrono.RemoveAt(GetComponentInChildren<Control>().nombresJugadoresaux.FindIndex(x => x == args.Snapshot.Reference.Parent.Parent.Key));
                trono.GetComponentInChildren<Trono>().tiemposEnTrono.Insert(GetComponentInChildren<Control>().nombresJugadoresaux.FindIndex(x => x == args.Snapshot.Reference.Parent.Parent.Key), true);
            }
            else if (int.Parse(args.Snapshot.Value.ToString()) == 0)
            {
                trono.GetComponentInChildren<Trono>().tiemposEnTrono.RemoveAt(GetComponentInChildren<Control>().nombresJugadoresaux.FindIndex(x => x == args.Snapshot.Reference.Parent.Parent.Key));
                trono.GetComponentInChildren<Trono>().tiemposEnTrono.Insert(GetComponentInChildren<Control>().nombresJugadoresaux.FindIndex(x => x == args.Snapshot.Reference.Parent.Parent.Key), false);
            }
        }
    }

    // Observa cambios en los jugadores de la partida
    void DetectarNuevoPersonaje(object emisor, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // Suscribir una función al cambio del personaje
        referenciaPartida.Child("jugadores").Child(args.Snapshot.Reference.Parent.Key).Child(args.Snapshot.Key).ChildChanged += DetectarCambioAtributo;
        referenciaPartida.Child("jugadores").Child(args.Snapshot.Reference.Parent.Key).Child(args.Snapshot.Key).ChildAdded += DetectarCambioAtributo;


        personaje = new Personaje();
        Dictionary<string, object> d = args.Snapshot.Value as Dictionary<string, object>;
        if (args.Snapshot.Reference.Parent.Key != nombresJugadores[0] || args.Snapshot.Key != "0")
        {
            if (d.ContainsKey("t") && ((string)d["t"] == "Fuego" || (string)d["t"] == "Aire" || (string)d["t"] == "Tierra" || (string)d["t"] == "Agua"))
            {
                GetComponentInChildren<Control>().LanzarHabilidad (args.Snapshot.Reference.Parent.Key + ":0", StringAVector3((string)d["d"]), args.Snapshot.Key, (string)d["t"]);
            }
            else if (d.ContainsKey("t") && d.ContainsKey("d"))
            {
                StartCoroutine(GetComponentInChildren<Control>().ColocarPersonaje(args.Snapshot.Reference.Parent.Key + ":" + args.Snapshot.Key, new Vector3(StringAVector3((string)d["d"]).x, -12574.22f, StringAVector3((string)d["d"]).z),(string)d["t"]));
            }
        }
        else
        {
            GetComponentInChildren<Control>().aliados[0] = GetComponentInChildren<Control>().EstablecerTipo(GetComponentInChildren<Control>().aliados[0], (string)d["t"]);
        }
    }

    // Observa la desaparicion de un personaje enemigo
    void DetectarMuertePersonaje(object emisor, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        string nombrepersonaje = args.Snapshot.Reference.Parent.Key + ":" + args.Snapshot.Key;
        
        // Quitar agentes de escucha de las habilidades del hechicero
        if ((string)args.Snapshot.Child("t").Value == "Fuego" || (string)args.Snapshot.Child("t").Value == "Tierra")
        {
            args.Snapshot.Reference.ChildChanged -= DetectarCambioAtributo;
            args.Snapshot.Reference.ChildAdded -= DetectarCambioAtributo;
        }

        // Quitar agentes de escucha (de personaje y de atributos)
        if (GetComponentInChildren<Control>().aliados.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
        {
            personaje = GetComponentInChildren<Control>().aliados.Find(x => x.objeto.name == nombrepersonaje);
            // Si te han matado matar a tus unidades
            if ((string)args.Snapshot.Key == "0")
            {
                for (int i = GetComponentInChildren<Control>().aliados.Count -1; i >= 0; i--)
                {
                    referenciaJugadorPartida.Child(GetComponentInChildren<Control>().aliados[i].objeto.name.Split(':')[1]).ChildChanged -= DetectarCambioAtributo;
                    referenciaJugadorPartida.Child(GetComponentInChildren<Control>().aliados[i].objeto.name.Split(':')[1]).ChildAdded -= DetectarCambioAtributo;
                    GetComponentInChildren<Control>().MatarUnidad (GetComponentInChildren<Control>().aliados[i].objeto);
                }

                // Limpiamos todo el resto de personajes del servidor
                args.Snapshot.Reference.Parent.SetValueAsync("");
                        
                GetComponentInChildren<Control>().aliados.Clear();
                GetComponentInChildren<Control>().enemigos.Clear();
                // No poderte mover más y eliminar HUD, mostrar botón para salir y mensaje de que has perdido
                GetComponentInChildren<Control>().muerto = true;
                GetComponentInChildren<Interfaz>().OcultarInterfaz();

                victoria = -1;
                StartCoroutine(GameObject.Find("Menu").GetComponent<Menu>().VolverAlMenu());
            }
            else
            {
                args.Snapshot.Reference.ChildChanged -= DetectarCambioAtributo;
                args.Snapshot.Reference.ChildAdded -= DetectarCambioAtributo;
                GetComponentInChildren<Control>().MatarUnidad(personaje.objeto);
            }
        }
        else if (GetComponentInChildren<Control>().enemigos.FindIndex(x => x.objeto.name == nombrepersonaje) != -1)
        {
            personaje = GetComponentInChildren<Control>().enemigos.Find(x => x.objeto.name == nombrepersonaje);
            // Si es un aspirante matar a sus personajes
            if ((string)args.Snapshot.Reference.Key == "0")
            {
                for (int i = GetComponentInChildren<Control>().enemigos.Count -1; i >= 0; i--)
                {
                    if (GetComponentInChildren<Control>().enemigos[i].objeto.name.Split(':')[0] == personaje.objeto.name.Split(':')[0])
                    {
                        referenciaPartida.Child("jugadores").Child(args.Snapshot.Reference.Parent.Key).Child(GetComponentInChildren<Control>().enemigos[i].objeto.name.Split(':')[1]).ChildChanged -= DetectarCambioAtributo;
                        referenciaPartida.Child("jugadores").Child(args.Snapshot.Reference.Parent.Key).Child(GetComponentInChildren<Control>().enemigos[i].objeto.name.Split(':')[1]).ChildAdded -= DetectarCambioAtributo;
                        GetComponentInChildren<Control>().MatarUnidad (GetComponentInChildren<Control>().enemigos[i].objeto);
                    }
                }
                if (GetComponentInChildren<Control>().enemigos.Count == 0)
                {
                    partidaEmpezada = false;
			        GanarServidor();
                }
            }
            else
            {
                args.Snapshot.Reference.ChildChanged -= DetectarCambioAtributo;
                args.Snapshot.Reference.ChildAdded -= DetectarCambioAtributo;
                GetComponentInChildren<Control>().MatarUnidad(personaje.objeto);
            }
        }
    }

    // Observa cambios en los jugadores de la partida
    void DetectarCambioFase(object emisor, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        try
        {
            if (int.Parse(args.Snapshot.Value.ToString()) == 0)
            {
                return;
            }
            else if (int.Parse(args.Snapshot.Value.ToString()) == 1)
            {
                partidaEmpezada = true;
            }
            else if (int.Parse(args.Snapshot.Value.ToString()) == 2)
            {
                StartCoroutine(GetComponentInChildren<Interfaz>().CuentaAtras());
            }
            else if (int.Parse(args.Snapshot.Value.ToString()) == 3)
            {
                GetComponentInChildren<Control>().ComenzarCierreMapa();
            }
        }
        catch (System.FormatException)
        {
            Debug.Log(args.Snapshot.Value.ToString());
            if (args.Snapshot.Value.ToString() != nombresJugadores[0])
            {
                victoria = -1;
                StartCoroutine(GameObject.Find("Menu").GetComponent<Menu>().VolverAlMenu());
            }
            else if (args.Snapshot.Value.ToString() == nombresJugadores[0])
            {
                victoria = 1;
                StartCoroutine(GameObject.Find("Menu").GetComponent<Menu>().VolverAlMenu());
            }
        }
    }

    public IEnumerator CambiarFase(int fase)
    {
        yield return new WaitForSeconds(1);

        if (fase == 2)
        {
            yield return new WaitForSeconds(10);
        }

        // Cambiar la fase
        referenciaPartida.Child("fase").SetValueAsync(fase);

        if (fase == 2)
        {
            yield return new WaitForSeconds(100);
            StartCoroutine(CambiarFase(3));
        }
    }

    // Cambiar en el servidor el tipo de un aliado
    public void AsignarTipoAspiranteServidor(string tipo)
    {
        referenciaJugadorPartida.Child("0").Child("t").SetValueAsync(tipo);
    }

    // Cambiar en el servidor la entrada al trono
    public void AsignarEntradaTronoAliadoServidor(int entrada)
    {
        // p de en proceso de ganar
        referenciaJugadorPartida.Child("0").Child("p").SetValueAsync(entrada);
    }

    // Cambiar en el servidor la vida de un aliado
    public void AsignarVidaAliadoServidor(int indice, int vida)
    {
        referenciaJugadorPartida.Child(indice + "").Child("v").SetValueAsync(vida);
    }

    // Cambiar en el servidor los destinos de tus aliados
    public void AsignarDestinosAliadosServidor(Dictionary<string, object> destinosAliados)
    {
        referenciaJugadorPartida.UpdateChildrenAsync(destinosAliados);
    }

    // Cambiar en el servidor los objetivos de tus aliados
    public void AsignarObjetivosAliadosServidor(Dictionary<string, object> objetivosAliados)
    {
        referenciaJugadorPartida.UpdateChildrenAsync(objetivosAliados);
    }

    // Cambiar en el servidor el numero de golpes y el daño al objetivo
    public void AsignarAtaqueAliadosServidor(int indice, int numGolpe, int daño)
    {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d["a"] = daño;
        d["g"] = numGolpe;
        referenciaJugadorPartida.Child(indice + "").UpdateChildrenAsync(d);
    }

    // Cambiar en el servidor el numero de golpes y el daño al objetivo
    public void MatarAliadoServidor(string indice)
    {
        referenciaJugadorPartida.Child(indice).RemoveValueAsync();
    }

    // Subir al servidor que has ganado
    public void GanarServidor()
    {
        referenciaPartida.Child("fase").SetValueAsync(nombresJugadores[0]);
    }

    // Crea un nuevo personaje de un tipo y con un destino
    public void CrearNuevoPersonaje(Dictionary<string,Dictionary<string,object>> listaPersonajes)
    {
        referenciaJugadorPartida.GetValueAsync().ContinueWith(tarea =>
        {
            if (tarea.IsFaulted)
            {
                return;
            }
            else if (tarea.IsCompleted)
            {
                foreach (KeyValuePair<string, Dictionary<string, object>> p in listaPersonajes)
                {
                    referenciaJugadorPartida.Child(p.Key).SetValueAsync(p.Value);
                }
            }
        });
    }

    public void BuscarPartida()
    {
        haEncontrado = false;
        esElUltimo = false;
        // Conseguir lista de partidas disponibles
        referencia.Child("Partidas").GetValueAsync().ContinueWith(tarea =>
        {
            if (tarea.IsFaulted)
            {
                return;
            }
            else if (tarea.IsCompleted)
            {
                foreach (DataSnapshot partida in tarea.Result.Children)
                {
                    // Si hay un hueco en una partida
                    if (partida.Child("jugadores").ChildrenCount < 2)
                    {
                        if (int.Parse(partida.Child("fase").Value.ToString()) != 0)
                        {
                            continue;
                        }
                        if (partida.Child("jugadores").ChildrenCount > 0)
                        {
                            haEncontrado = true;
                        }

                        idPartida = partida.Key;
                        break;
                    }
                }

                if (!haEncontrado)
                {
                    // Si no hay hueco se crea una partida, o se recicla una ya creada que no tenga jugadores
                    if (idPartida.Length == 0)
                    {
                        idPartida = referencia.Child("Partidas").Push().Key;
                    }
                    referencia.Child("Partidas").Child(idPartida).Child("jugadores").Child(nombresJugadores[0]).SetValueAsync("");
                    referencia.Child("Partidas").Child(idPartida).Child("fase").SetValueAsync(0);
                    haEncontrado = true;
                    EsperarInicio();
                }
                else
                {
                    // Unirse a una partida ya existente con una transacción por si otro se une a la vez y deja de haber espacio
                    referencia.Child("Partidas").Child(idPartida).Child("jugadores").RunTransaction(datosMutables =>
                    {
                        // Se guardan los jugadores actuales en un diccionario
                        Dictionary<string, object> jugadores = datosMutables.Value as Dictionary<string, object>;
                        if (jugadores == null)
                        {
                            // Si está vacía se inicia el diccionario vacío
                            jugadores = new Dictionary<string, object>();
                        }
                        else if (datosMutables.ChildrenCount == 2)
                        {
                            // Si está llena se repite todo el proceso
                            haEncontrado = false;
                            BuscarPartida();
                            return TransactionResult.Abort();
                        }
                        else if (datosMutables.ChildrenCount == 1)
                        {
                            // Distribuir posiciones
                            Dictionary<string, object> d = new Dictionary<string, object>();
                            d["d"] = spawn1 + "";
                            List<object> l = new List<object>();
                            l.Add(d);
                            jugadores[jugadores.Keys.ElementAt(0)] = l;

                            // d = new Dictionary<string, object>();
                            // d["d"] = spawn2 + "";
                            // l = new List<object>();
                            // l.Add(d);
                            // jugadores[jugadores.Keys.ElementAt(1)] = l;

                            d = new Dictionary<string, object>();
                            d["d"] = miSpawn + "";
                            l = new List<object>();
                            l.Add(d);
                            jugadores[nombresJugadores[0]] = l;

                            esElUltimo = true;
                        }
                        else
                        {
                            // Se añade el nuevo jugador al diccionario
                            jugadores[nombresJugadores[0]] = "";
                        }

                        datosMutables.Value = jugadores;

                        // Se añade la partida al jugador
                        return TransactionResult.Success(datosMutables);

                    }).ContinueWith(tarea2 =>
                    {
                        if (tarea2.IsFaulted)
                        {
                            return;
                        }
                        else if (tarea2.IsCompleted)
                        {
                            EsperarInicio();
                        }
                    });
                }
            }
        });
    }

    void EsperarInicio()
    {
        if (haEncontrado)
        {
            referenciaJugadorPartida = FirebaseDatabase.DefaultInstance.RootReference.Child("Partidas").Child(idPartida).Child("jugadores").Child(nombresJugadores[0]);
            referenciaPartida = FirebaseDatabase.DefaultInstance.RootReference.Child("Partidas").Child(idPartida);
            Partida();
        }
    }

    void Update()
    {
        if (partidaEmpezada && !flagPartidaEmpezada)
        {
            AsignarTuAspirante();
            flagPartidaEmpezada = true;
        }
        if (esElUltimo && !flagCambioFase)
        {
            StartCoroutine(CambiarFase(1));
            flagCambioFase = true;
        }
    }

    void Partida()
    {
        // Suscribir una función al cambio del aspirante
        referenciaJugadorPartida.Child("0").ChildChanged += DetectarCambioAtributo;
        // Y otra al cambio de la fase de la partida
        referenciaPartida.Child("fase").ValueChanged += DetectarCambioFase;
    }

    // Empiezas a observar a los otros dos jugadores y a sus personajes
    public void ObservarJugadores()
    {
        referenciaPartida.Child("jugadores").GetValueAsync().ContinueWith(tarea =>
        {
            if (tarea.IsFaulted)
            {
                return;
            }
            else if (tarea.IsCompleted)
            {
                int i = 1;
                foreach (DataSnapshot jugador in tarea.Result.Children)
                {
                    if (jugador.Key != nombresJugadores[0])
                    {
                        nombresJugadores[i] = jugador.Key;
                        i++;
                    }

                    // Observas cuando el tipo de los aspirantes de los jugadores se establece
                    referenciaPartida.Child("jugadores").Child(jugador.Key).Child("0").ChildAdded += DetectarEleccionAspirante;
                }
            }
        });
        StartCoroutine(AñadirNombresJugadores());      
    }

    IEnumerator AñadirNombresJugadores ()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponentInChildren<Control>().nombresJugadoresaux.AddRange(nombresJugadores);
        GetComponentInChildren<Control>().nombresJugadoresaux.Sort();
    }

    void DetectarEleccionAspirante(object emisor, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        personaje = new Personaje();
        if (args.Snapshot.Key == "t")
        {
            // Se empiezan a observar los personajes de un jugador
            referenciaPartida.Child("jugadores").Child(args.Snapshot.Reference.Parent.Parent.Key).ChildAdded += DetectarNuevoPersonaje;
            referenciaPartida.Child("jugadores").Child(args.Snapshot.Reference.Parent.Parent.Key).ChildRemoved += DetectarMuertePersonaje;
            referenciaPartida.Child("jugadores").Child(args.Snapshot.Reference.Parent.Parent.Key).Child("0").ChildAdded -= DetectarEleccionAspirante;
        }
    }

    // Añadir en local los aspirantes
    void AsignarTuAspirante()
    {
        referenciaJugadorPartida.Child("0").Child("d").GetValueAsync().ContinueWith(tarea =>
        {
            if (tarea.IsFaulted)
            {
                return;
            }
            else if (tarea.IsCompleted)
            {
                // Inicia la partida y te da una posición
                miSpawn = StringAVector3((string)tarea.Result.Value);
            }
        });
        StartCoroutine(CambiarFase(2));
    }

    public void DejarDeBuscarPartida()
    {
        // Eliminar partida de jugador y jugador de partida
        referencia.Child("Jugadores").UpdateChildrenAsync(new Dictionary<string, object> { { "/" + nombresJugadores[0] + "/", "" } });
        referencia.Child("Partidas").Child(idPartida).Child("jugadores").Child(nombresJugadores[0]).RemoveValueAsync();
    }

    Vector3 StringAVector3(string sVector)
    {
        // Quitamos los paréntesis
        sVector = sVector.Trim('(').Trim(')');

        // Dividimos por las comas
        string[] sArray = sVector.Split(',');

        // Guardamos cada dimensión
        Vector3 resultado = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return resultado;
    }

    public void ReiniciarServidor()
    {
        referenciaPartida.Child("fase").ValueChanged -= DetectarCambioFase;
        foreach (string jugador in nombresJugadores)
        {
            referenciaPartida.Child("jugadores").Child(jugador).ChildAdded -= DetectarNuevoPersonaje;
            referenciaPartida.Child("jugadores").Child(jugador).ChildRemoved -= DetectarMuertePersonaje;
        }

        referenciaPartida.RemoveValueAsync();

        idPartida = "";
        haEncontrado = false;
        esElUltimo = false;
        partidaEmpezada = false;
    }

    void OnApplicationQuit()
    {
        if (!partidaEmpezada)
        {
            DejarDeBuscarPartida();
        }
        else if (GetComponentInChildren<Control>() && GetComponentInChildren<Control>().enabled)
        {
            MatarAliadoServidor("0");
        }
    }
}
