using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestionarTrabajadoresController : MonoBehaviour
{
    [SerializeField] private RectTransform rtScrollViewContent;
    [SerializeField] private TMP_InputField inputFieldBuscarTrabajador;
    [SerializeField] private GameObject scrollViewNombresTrabajadores;
    [SerializeField] private RectTransform rtScrollViewContentNombresTrabajadores;
    [SerializeField] private Button buttonA�adir;
    [SerializeField] private GameObject prefabBot�nConInputFieldYDropdown;
    [SerializeField] private Button buttonGuardar;
    [SerializeField] private Button buttonEliminar;
    [SerializeField] private TMP_Text textoError;
    [SerializeField] private GameObject contenedorAdvertenciaCambiarGerente;
    [SerializeField] private TMP_Text textoContenedorAdvertenciaCambiarDeGerente;
    [SerializeField] private GameObject contenedorAdvertenciaEliminarTrabajador;
    [SerializeField] private TMP_Text textoAdvertenciaEliminarTrabajador;
    [SerializeField] private TMP_Text textoEntrePar�ntesisAlQuererEliminarTrabajador;
    [SerializeField] private GameObject contenedorErrorAlGuardarTrabajadores;
    [SerializeField] private TMP_Text textoErrorAlGuardarTrabajadores;
    [SerializeField] private Sprite imgRectangleMarroncitoParaMostrar;
    [SerializeField] private GameObject canvasRegistrarUsuario;


    private List<Trabajador> TrabajadoresEnRestaurante = new List<Trabajador>();
    private List<Trabajador> TrabajadoresSinRestaurante = new List<Trabajador>();

    private bool BuscandoTrabajadoresSinRest = false;
    private string TextoInputFieldAntes = "";
    private Button bot�nPulsadoParaEliminar;
    private Trabajador TrabajadorPosibleACambiarDeRol;


    M�todosAPIController instanceM�todosApiController;

    public static GestionarTrabajadoresController InstanceGestionarTrabajadoresController { get; private set; }

    private void Awake()
    {
        if (InstanceGestionarTrabajadoresController == null)
        {
            InstanceGestionarTrabajadoresController = this;
        }

        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosApiController = M�todosAPIController.InstanceM�todosAPIController;

        TrabajadorController.ComprobandoDatosTrabajador = false;

        InvokeRepeating(nameof(ObtenerTrabajadoresSinRestauranteAsync), 0f, 1f); // Llama a ObtenerTrabajadoresSinRestaurante() cada 1 segundo

        ObtenerTrabajadoresDeUnRestauranteYCrearBotonesAsync();
    }

    // Update is called once per frame
    void Update()
    {
        //GestionarBuscarTrabajador();

        GestionarCambiosEnTrabajadores();
    }

    private void GestionarCambiosEnTrabajadores()
    {
        List<Trabajador> trabajadoresEnScrollViewAhora = ObtenerDatosTrabajadoresEnScrollView();

        if (!HayUnInputFieldVac�oEnNombresTrabajadores() && (Alg�nNombreCambiado(trabajadoresEnScrollViewAhora) || Alg�nRolCambiado(trabajadoresEnScrollViewAhora)) )
        {
            if (Alg�nRolCambiado(trabajadoresEnScrollViewAhora) )//&& Hay2OM�sTrabajadoresConRolGerente(trabajadoresEnScrollViewAhora))
            {
                
                // Si hay m�s de un gerente, no se puede guardar
                buttonGuardar.interactable = false;

                if (CantGerentesEnScrollMenorQueEnLaBDD(trabajadoresEnScrollViewAhora))
                {
                    if (CantGerentesEnScrollIgualACero(trabajadoresEnScrollViewAhora))
                    {
                        if (!contenedorAdvertenciaCambiarGerente.activeSelf)
                        {
                            TrabajadorPosibleACambiarDeRol = ObtenerTrabajadorQueHaSidoPuestoEmpleadoSiendoGerente(trabajadoresEnScrollViewAhora);

                            // Mostrar contenedor advertencia
                            if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                            {
                                textoErrorAlGuardarTrabajadores.text = "El restaurante no puede estar sin un gerente";
                            }
                            else
                            {
                                textoErrorAlGuardarTrabajadores.text = "The restaurant cannot be without a manager";
                            }

                            contenedorErrorAlGuardarTrabajadores.SetActive(true);

                            CancelarAsignarNuevoRol();
                        }
                    }
                    else
                    {
                        if (!contenedorAdvertenciaCambiarGerente.activeSelf)
                        {
                            TrabajadorPosibleACambiarDeRol = ObtenerTrabajadorQueHaSidoPuestoEmpleadoSiendoGerente(trabajadoresEnScrollViewAhora);

                            // Mostrar contenedor advertencia
                            if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                            {
                                textoContenedorAdvertenciaCambiarDeGerente.text = "�Est� dispuest@ a otorgar el rol de empleado al usuario " + TrabajadorPosibleACambiarDeRol.Nombre + "?";
                            }
                            else
                            {
                                textoContenedorAdvertenciaCambiarDeGerente.text = "Are you willing to hand over your manager role to user " + TrabajadorPosibleACambiarDeRol.Nombre + "?";
                            }

                            contenedorAdvertenciaCambiarGerente.SetActive(true);
                        }
                    }
                }
                else
                {
                    // Si el contenedor no est� activo, se activa/muestra
                    if (!contenedorAdvertenciaCambiarGerente.activeSelf)
                    {
                        TrabajadorPosibleACambiarDeRol = ObtenerTrabajadorQueHaSidoPuestoGerenteExistiendoUno(trabajadoresEnScrollViewAhora);

                        // Mostrar contenedor advertencia
                        if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                        {
                            textoContenedorAdvertenciaCambiarDeGerente.text = "�Est� dispuest@ a otorgar el rol de gerente al usuario " + TrabajadorPosibleACambiarDeRol.Nombre + "?";
                        }
                        else
                        {
                            textoContenedorAdvertenciaCambiarDeGerente.text = "Are you willing to hand over your manager role to user " + TrabajadorPosibleACambiarDeRol.Nombre + "?";
                        }

                        contenedorAdvertenciaCambiarGerente.SetActive(true);
                    }
                }
                //Debug.Log("+: S�lo puede haber un gerente");                
            }
            else
            {
                if (Alg�nNombreCambiado(trabajadoresEnScrollViewAhora)) //Hay2OM�sTrabajadoresConRolGerente(trabajadoresEnScrollViewAhora))//if (HayUn�nicoGerente(trabajadoresEnScrollViewAhora))
                {
                    buttonGuardar.interactable = true;
                }
                else
                {
                    buttonGuardar.interactable = false;
                }
            }
        }
        else
        {
            buttonGuardar.interactable = false;
        }
    }

    private bool CantGerentesEnScrollIgualACero(List<Trabajador> trabajadoresEnScrollViewAhora)
    {
        int contGerentesEnScroll = 0;
        foreach (Trabajador trabajadorEnScroll in trabajadoresEnScrollViewAhora)
        {
            if (trabajadorEnScroll.Rol_ID.Equals(2))
            {
                contGerentesEnScroll++;
            }
        }

        return contGerentesEnScroll.Equals(0);
    }

    private Trabajador ObtenerTrabajadorQueHaSidoPuestoEmpleadoSiendoGerente(List<Trabajador> trabajadoresEnScrollViewAhora)
    {
        foreach (Trabajador trabajadorRest in TrabajadoresEnRestaurante)
        {
            foreach (Trabajador trabajadorEnScroll in trabajadoresEnScrollViewAhora)
            {
                if (trabajadorRest.Id.Equals(trabajadorEnScroll.Id))
                {
                    if (trabajadorRest.Rol_ID.Equals(2) && trabajadorEnScroll.Rol_ID.Equals(1))
                    {
                        return trabajadorEnScroll;
                    }
                }
            }
        }
        return null;
    }

    private bool CantGerentesEnScrollMenorQueEnLaBDD(List<Trabajador> trabajadoresEnScrollViewAhora)
    {
        int contGerentesEnRest = 0;
        foreach (Trabajador trabajadorRest in TrabajadoresEnRestaurante)
        {
            if (trabajadorRest.Rol_ID.Equals(2))
            {
                contGerentesEnRest++;
            }
        }

        int contGerentesEnScroll = 0;
        foreach (Trabajador trabajadorEnScroll in trabajadoresEnScrollViewAhora)
        {
            if (trabajadorEnScroll.Rol_ID.Equals(2))
            {
                contGerentesEnScroll++;
            }
        }

        return contGerentesEnScroll < contGerentesEnRest;
    }

    private bool HayUn�nicoGerente(List<Trabajador> trabajadoresEnScrollViewAhora)
    {
        int cont = 0;
        foreach (Trabajador trabajador in trabajadoresEnScrollViewAhora)
        {
            if (trabajador.Rol_ID.Equals(2))
            {
                cont++;
            }
        }
        Debug.Log("*Count: " + cont);
        return cont.Equals(1); // Si s�lo hay un gerente, devuelve true, sino false
    }

    private bool Hay2OM�sTrabajadoresConRolGerente(List<Trabajador> trabajadoresEnScrollViewAhora)
    {
        int cont = 0;
        foreach (Trabajador trabajador in trabajadoresEnScrollViewAhora)
        {
            if (trabajador.Rol_ID.Equals(2))
            {
                Debug.Log("+: " + trabajador.Rol_ID);
                cont++;
            }
        }

        return cont > 1;
    }

    private Trabajador ObtenerTrabajadorQueHaSidoPuestoGerenteExistiendoUno(List<Trabajador> trabajadoresEnScrollViewAhora)
    {
        //Trabajador t = ObtenerTrabajadorGerenteEnRestaurante();
        List<Trabajador> trabajadoresGerente = ObtenerTrabajadoresGerenteEnRestaurante();

        foreach (Trabajador trabajadorRest in TrabajadoresEnRestaurante)
        {
            foreach (Trabajador trabajadorEnScroll in trabajadoresEnScrollViewAhora)
            {
                if (trabajadorRest.Id.Equals(trabajadorEnScroll.Id))
                {
                    if (trabajadorRest.Rol_ID.Equals(1) && trabajadorEnScroll.Rol_ID.Equals(2))
                    {
                        return trabajadorEnScroll;
                    }
                }                
            }
        }
        return null;
        /*foreach (Trabajador trabajador in trabajadoresEnScrollViewAhora)
        {
            if (trabajador.Rol_ID.Equals(2) && trabajador.Nombre.CompareTo(t.Nombre) != 0)
            {
                return trabajador;
            }
        }
        return null;*/
    }

    private List<Trabajador> ObtenerTrabajadoresGerenteEnRestaurante()
    {
        List<Trabajador> trabajadores = new List<Trabajador>(); // Nuevo
        foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
        {
            if (trabajador.Rol_ID.Equals(2))
            {
                //return trabajador;
                trabajadores.Add(trabajador);
            }
        }
        //return null;
        return trabajadores;
    }

    private bool HayUnInputFieldVac�oEnNombresTrabajadores()
    {
        foreach (Transform hijo in rtScrollViewContent)
        {
            string nombre = hijo.GetComponentInChildren<TMP_InputField>().text.Trim();
            if (nombre.Length.Equals(0))
            {
                //buttonGuardar.interactable = false;
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                {
                    textoError.text = "Error: nombre vac�o";
                }
                else
                {
                    textoError.text = "Error: empty name";
                }
                return true;
            }

            if (nombre.Length < 3){
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                {
                    textoError.text = "Error: nombre con menos de 3 caracteres";
                }
                else
                {
                    textoError.text = "Error: name with less than 3 characters";
                }
                return true;
            }
        }
        textoError.text = "";
        return false;
    }

    private bool Alg�nNombreCambiado(List<Trabajador> trabajadoresEnScrollViewAhora)
    {        
        foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
        {
            int cont = 0;
            foreach (Trabajador trabajadorScrollViewAhora in trabajadoresEnScrollViewAhora)
            {
                if (trabajador.Id.Equals(trabajadorScrollViewAhora.Id))
                {
                    // Recorro todos los nombres que hay ahora en el scrollview y compruebo si cada nombre de los TrabajadoresEnRestaurante est�n id�nticos, sino, hay cambios.
                    if (trabajador.Nombre.CompareTo(trabajadorScrollViewAhora.Nombre) != 0)
                    {
                        cont++;
                    }
                }
            }
            // Un nombre ha sido cambiado
            if (cont > 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool Alg�nRolCambiado(List<Trabajador> trabajadoresEnScrollViewAhora)
    {
        foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
        {
            foreach (Trabajador tScrollViewAhora in trabajadoresEnScrollViewAhora)
            {
                if (trabajador.Id.Equals(tScrollViewAhora.Id) && !trabajador.Rol_ID.Equals(tScrollViewAhora.Rol_ID))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private List<Trabajador> ObtenerDatosTrabajadoresEnScrollView()
    {
        List<Trabajador> trabajadores = new List<Trabajador>();
        foreach (Transform hijo in rtScrollViewContent)
        {
            int id = ObtenerIdTrabajadorDeBot�n(hijo);
            string nombre = hijo.GetComponentInChildren<TMP_InputField>().text.Trim();
            TMP_Dropdown dropdown = hijo.GetComponentInChildren<TMP_Dropdown>();
            string rol = dropdown.options[dropdown.value].text;

            switch (rol)
            {
                case "Empleado":
                    trabajadores.Add(new Trabajador(id, nombre, "", 1, Usuario.Restaurante_ID));
                    break;
                case "Gerente":
                    trabajadores.Add(new Trabajador(id, nombre, "", 2, Usuario.Restaurante_ID));
                    break;
            }
        }
        return trabajadores;
    }

    private int ObtenerIdTrabajadorDeBot�n(Transform hijo)
    {
        string[] array = hijo.name.Split("-");
        return int.Parse(array[1]);
    }

    private async void ObtenerTrabajadoresSinRestauranteAsync()
    {
        if (!BuscandoTrabajadoresSinRest)
        {
            BuscandoTrabajadoresSinRest = true;
            string cad = await instanceM�todosApiController.GetDataAsync("trabajador/getTrabajadoresSinRestaurante");

            // Deserializo la respuesta
            TrabajadoresSinRestaurante = JsonConvert.DeserializeObject<List<Trabajador>>(cad);
            
            BuscandoTrabajadoresSinRest = false;
        }
    }

    private void GestionarBuscarTrabajador()
    {
        string textoInputField = inputFieldBuscarTrabajador.text.Trim();

        if (textoInputField.Length > 0)
        {
            if (textoInputField.ToLower().CompareTo(TextoInputFieldAntes.ToLower()) != 0)
            {
                List<string> nombres_Trabajadores_Sin_Restaurante = new List<string>();

                // Recorro la lista de todos los trabajadores sin un restaurante_ID
                foreach (Trabajador trabajador in TrabajadoresSinRestaurante)
                {
                    if (trabajador.Nombre.ToLower().Contains(textoInputField.ToLower()))
                    {
                        nombres_Trabajadores_Sin_Restaurante.Add(trabajador.Nombre);
                    }
                }
                TextoInputFieldAntes = textoInputField;
                // Se encuentran trabajadores sin restaurante con un nombre parecido al que se pone en el inputField
                if (nombres_Trabajadores_Sin_Restaurante.Count > 0)
                {
                    scrollViewNombresTrabajadores.SetActive(true);

                    // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (trabajadores actualizados)
                    EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);

                    // Muestro los trabajadores sin restaurante que coinciden con el contenido del inputField en un scrollview
                    foreach (string nombre in nombres_Trabajadores_Sin_Restaurante)
                    {
                        CrearBot�nNombreTrabajador(nombre);
                    }
                    
                }
                else
                {
                    // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (trabajadores actualizados)
                    EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);

                    scrollViewNombresTrabajadores.SetActive(false);
                }
            }
            
        }
        else
        {
            // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (trabajadores actualizados)
            EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);
            TextoInputFieldAntes = "";
            scrollViewNombresTrabajadores.SetActive(false);
        }


        if (ElNombreExiste())
        {
            buttonA�adir.interactable = true;

            // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (trabajadores actualizados)
            EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);
            scrollViewNombresTrabajadores.SetActive(false);
        }
        else
        {
            buttonA�adir.interactable = false;
        }
    }

    private bool ElNombreExiste()
    {
        foreach (Trabajador trabajador in TrabajadoresSinRestaurante)
        {
            if (trabajador.Nombre.CompareTo(inputFieldBuscarTrabajador.text.Trim()) == 0)
            {
                return true;
            }
        }
        return false;
    }

    // Elimino todos los hijos de Content
    private void EliminarObjetosHijoDeScrollView(RectTransform rectTransformContent)
    {
        foreach (Transform hijo in rectTransformContent)
        {
            Destroy(hijo.gameObject);
        }
    }

    private async void ObtenerTrabajadoresDeUnRestauranteYCrearBotonesAsync()
    {
        Debug.Log("Obtener datos trabajadores");
        string cad = await instanceM�todosApiController.GetDataAsync("trabajador/getTrabajadoresDeRestaurante/" + Usuario.Restaurante_ID);

        try
        {
            // Deserializo la respuesta
            TrabajadoresEnRestaurante = JsonConvert.DeserializeObject<List<Trabajador>>(cad);
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex);
        }

        CrearBotonesTrabajadores();
    }

    private void CrearBotonesTrabajadores()
    {
        if (TrabajadoresEnRestaurante.Count > 0)
        {
            foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
            {
                CrearBot�nTrabajador(trabajador);
            }
        }
    }

    private void CrearBot�nTrabajador(Trabajador trabajador)
    {
        Debug.Log("*OK");
        GameObject botonGO = Instantiate(prefabBot�nConInputFieldYDropdown, rtScrollViewContent, false);

        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        botonGO.name = "Button-" + trabajador.Id;

        // Referencias de componentes
        Image imgButton = botonGO.GetComponent<Image>();
        Button button = botonGO.GetComponent<Button>();
        TMP_InputField inputField = botonGO.GetComponentInChildren<TMP_InputField>();
        TMP_Dropdown dropdown = botonGO.GetComponentInChildren<TMP_Dropdown>();

        // Pongo una imagen espec�fica al bot�n
        imgButton.sprite = imgRectangleMarroncitoParaMostrar;

        // Configuro el inputField
        // Reduzco el alpha del inputField del bot�n
        Image imgInputField = inputField.gameObject.GetComponent<Image>();
        Color colorInputField = imgInputField.color;
        colorInputField.a = 47f / 255f;
        imgInputField.color = colorInputField;

        // Pongo el nombre del trabajador en el inputField
        inputField.text = trabajador.Nombre;

        // Limito el n�mero de caracteres que puede tener el inputField
        inputField.characterLimit = 17;

        List<string> opciones = new List<string> { "Empleado", "Gerente"};
        AgregarOpcionesADropdown(dropdown, opciones);

        switch (trabajador.Rol_ID)
        {
            case 1:
                BuscoElIndiceYLoPongoSiLoEncuentro(dropdown, "Empleado");
                break;
            case 2:
                BuscoElIndiceYLoPongoSiLoEncuentro(dropdown, "Gerente");
                //dropdown.interactable = false;
                break;
        }

        //Pongo listener al bot�n
        button.onClick.AddListener(() => GestionarActivarBot�nEliminarTrabajador(button));
        
        inputField.onSelect.AddListener((_) =>
        {
            buttonEliminar.interactable = false;
        });

        dropdown.onValueChanged.AddListener((_) =>
        {
            buttonEliminar.interactable = false;
        });

        //PonerListenerADropdown(dropdown);
        /*
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        GameObject botonGO = new GameObject("Button-" + trabajador.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(rtScrollViewContent, false);

        // Agregar el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�adimos expl�citamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        Image imagen = botonGO.AddComponent<Image>();

        // Agrego un componente Button para que sea interactivo
        botonGO.AddComponent<Button>();

        // Creo un nuevo GameObject hijo, el texto del bot�n
        CrearTextoDelButton(rt, trabajador);
        */


    }

    private void GestionarActivarBot�nEliminarTrabajador(Button button)
    {
        bot�nPulsadoParaEliminar = button;

        buttonEliminar.interactable = true;
    }

    private void AgregarOpcionesADropdown(TMP_Dropdown dropdown, List<string> opciones)
    {
        dropdown.ClearOptions();  // Limpia opciones anteriores
        dropdown.AddOptions(opciones);
    }

    private void BuscoElIndiceYLoPongoSiLoEncuentro(TMP_Dropdown dropdown, string valor)
    {
        // Busco el �ndice de ese valor en el Dropdown
        int index = dropdown.options.FindIndex(option => option.text == valor);

        // Si lo encuentra, establecerlo como el seleccionado
        if (index != -1)
        {
            dropdown.value = index;
            dropdown.RefreshShownValue(); // Refresca la UI para mostrar el valor seleccionado.
        }
        else
        {
            Debug.Log("Valor no encontrado");
        }
    }

    private void CrearTextoDelButton(RectTransform rt, Trabajador trabajador)
    {
        // Creo un GameObject para el bot�n y le asigno un nombre �nico.
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�ado expl�citamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // M�rgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando est� estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBot�n = textGO.AddComponent<TextMeshProUGUI>();
        textoBot�n.fontStyle = FontStyles.Bold;
        textoBot�n.fontSize = 46;
        textoBot�n.alignment = TextAlignmentOptions.Left;

        switch (trabajador.Rol_ID)
        {
            case 1:
                textoBot�n.text = "                     " + trabajador.Nombre + "                                " + "Empleado";
                break;
            case 2:
                textoBot�n.text = "                     " + trabajador.Nombre + "                                " + "Gerente";
                break;
        }
    }

    private void CrearBot�nNombreTrabajador(string nombreTrabajador)
    {
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        GameObject botonGO = new GameObject("Button-" + nombreTrabajador);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(rtScrollViewContentNombresTrabajadores, false);

        // Agregar el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�adimos expl�citamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        Image imagen = botonGO.AddComponent<Image>();

        // Agrego un componente Button para que sea interactivo
        Button button = botonGO.AddComponent<Button>();

        //Pongo listener al bot�n
        button.onClick.AddListener(() => PonerEnInputFieldElTextoDelBot�nSeleccionado(button));

        // Creo un nuevo GameObject hijo, el texto del bot�n
        CrearTextoDelButtonNombreTrabajador(rt, nombreTrabajador);
    }

    private void PonerEnInputFieldElTextoDelBot�nSeleccionado(Button button)
    {
        Debug.Log("+Pasa por listener bot�n");
        inputFieldBuscarTrabajador.text = button.transform.Find("TMP_Text").GetComponent<TMP_Text>().text;
    }

    private void CrearTextoDelButtonNombreTrabajador(RectTransform rt, string nombreTrabajador)
    {
        // Creo un GameObject para el bot�n y le asigno un nombre �nico.
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�ado expl�citamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // M�rgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando est� estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBot�n = textGO.AddComponent<TextMeshProUGUI>();
        textoBot�n.fontStyle = FontStyles.Bold;
        textoBot�n.fontSize = 46;
        textoBot�n.alignment = TextAlignmentOptions.Left;

        textoBot�n.text = " "+nombreTrabajador;
    }

    public async void A�adirTrabajadorARestaurante(string nombreTrabajador)
    {
        Debug.Log("+++++A"); // 
        string cad = await instanceM�todosApiController.PostDataAsync("trabajador/obtenerTrabajadorPorNombre", new Trabajador(nombreTrabajador, "", 0, 0));
        
        // Deserializo la respuesta
        Trabajador trabajador = JsonConvert.DeserializeObject<Trabajador>(cad);

        int id_Trabajador = trabajador.Id;

        Trabajador trabajador2 = new Trabajador(id_Trabajador, "", "", 0, Usuario.Restaurante_ID);

        string cad2 = await instanceM�todosApiController.PutDataAsync("trabajador/actualizarRestauranteIDTrabajador", trabajador2);

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad2);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("+++++B");
            inputFieldBuscarTrabajador.text = "";
            EliminarObjetosHijoDeScrollView(rtScrollViewContent);
            ObtenerTrabajadoresDeUnRestauranteYCrearBotonesAsync();
        }
        else
        {
            Debug.Log("+++++C");
            inputFieldBuscarTrabajador.text = "Error";
        }

        
    }

    private int ObtenerIdTrabajadorPorNombre(string nombreTrabajador)
    {
        foreach (Trabajador trabajador in TrabajadoresSinRestaurante)
        {
            if (trabajador.Nombre.CompareTo(nombreTrabajador) == 0)
            {
                return trabajador.Id;
            }
        }
        return 0;
    }

    public void CancelarAsignarNuevoRol()
    {
        TMP_Dropdown dropdown = rtScrollViewContent.transform.Find("Button-" + TrabajadorPosibleACambiarDeRol.Id).GetComponentInChildren<TMP_Dropdown>();

        //Pongo el rol que ten�a el trabajador al cancelar el ponerle gerente
        int rol_ID = ObtenerRol_IDTrabajadorEnRestaurante(TrabajadorPosibleACambiarDeRol.Id);

        switch (rol_ID)
        {
            case 1:
                BuscoElIndiceYLoPongoSiLoEncuentro(dropdown, "Empleado");
                break;
            case 2:
                BuscoElIndiceYLoPongoSiLoEncuentro(dropdown, "Gerente");
                break;
        }

        contenedorAdvertenciaCambiarGerente.SetActive(false);
    }

    private int ObtenerRol_IDTrabajadorEnRestaurante(int id)
    {
        foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
        {
            if (trabajador.Id.Equals(id))
            {
                return trabajador.Rol_ID;
            }
        }
        return 0;
    }

    public async void ConfirmarAsignarNuevoRol()
    {
        // Obtengo y actualizo el trabajador que va a ser a partir de ahora un nuevo gerente del restaurante
        Trabajador trabajador = ObtenerTrabajadorPorID(TrabajadorPosibleACambiarDeRol.Id);

        switch (trabajador.Rol_ID)
        {
            case 1:
                trabajador.Rol_ID = 2;
                break;
            case 2:
                trabajador.Rol_ID = 1;
                break;
        }
        
        trabajador.Restaurante_ID = Usuario.Restaurante_ID;
        string cad = await instanceM�todosApiController.PutDataAsync("trabajador/actualizarTrabajadorPorGerente/", trabajador);

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        // Actualizaci�n exitosa del nuevo rol
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Actualizaci�n nuevo rol exitosa");

            contenedorAdvertenciaCambiarGerente.SetActive(false);

            // Si el mismo usuario que era gerente se ha puesto as� mismo empleado, sale al men� principal
            if (Usuario.Nombre.CompareTo(trabajador.Nombre) == 0)
            {
                SceneManager.LoadScene("Main");
            }
            else
            {
                ObtenerTrabajadoresDeUnRestauranteAsync();
            }

            /*
            // Ahora actualizo al que antes era gerente poni�ndolo empleado
            Trabajador t = new Trabajador(Usuario.ID, Usuario.Nombre, "", 1, Usuario.Restaurante_ID);
            string cad2 = await instanceM�todosApiController.PutDataAsync("trabajador/actualizarTrabajadorPorGerente/", t);

            // Deserializo la respuesta
            Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);
            
            if (resultado2.Result.Equals(1))
            {
                SceneManager.LoadScene("Main");
            }*/
        }

        /*TMP_Dropdown dropdownPosibleFuturoGerente = rtScrollViewContent.transform.Find("Button-" + TrabajadorPosibleASerNuevoGerente.Id).GetComponentInChildren<TMP_Dropdown>();

        // Pongo el rol gerente al nuevo gerente
        BuscoElIndiceYLoPongoSiLoEncuentro(dropdownPosibleFuturoGerente, "Gerente");

        
        int id_TrabajadorGerenteAntesDelCambio = ObtenerIDTrabajadorGerenteAntesDelCambio(); // A�n no se ha guardado, por lo que obtengo el gerente que si est� en la BDD registrado

        // Busco el dropdown de ese trabajador
        TMP_Dropdown dropdownGerenteAntesDelCambio = rtScrollViewContent.transform.Find("Button-" + id_TrabajadorGerenteAntesDelCambio).GetComponentInChildren<TMP_Dropdown>();
        
        // Pongo el rol empleado al trabajador que antes era gerente
        BuscoElIndiceYLoPongoSiLoEncuentro(dropdownGerenteAntesDelCambio, "Empleado");

        Debug.Log("+Nuevo gerente hecho");

        contenedorAdvertenciaCambiarGerente.SetActive(false);*/
    }

    private async void ObtenerTrabajadoresDeUnRestauranteAsync()
    {
        Debug.Log("Obtener datos trabajadores");
        string cad = await instanceM�todosApiController.GetDataAsync("trabajador/getTrabajadoresDeRestaurante/" + Usuario.Restaurante_ID);

        // Deserializo la respuesta
        TrabajadoresEnRestaurante = JsonConvert.DeserializeObject<List<Trabajador>>(cad);
    }

    private Trabajador ObtenerTrabajadorPorID(int trabajador_ID)
    {
        foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
        {
            if (trabajador.Id.Equals(trabajador_ID))
            {
                return trabajador;
            }
        }
        return null;
    }

    public void EliminarTrabajador()
    {
        buttonEliminar.interactable = false;

        int id_trabajador = ObtenerElIdDelTrabajadorAEliminar();

        Trabajador t = ObtenerTrabajadorPorID(id_trabajador);

        RectTransform rtText = textoAdvertenciaEliminarTrabajador.gameObject.GetComponent<RectTransform>();
        textoEntrePar�ntesisAlQuererEliminarTrabajador.text = "";
        // Si el trabajador a eliminar es Gerente, muestro
        if (t.Rol_ID.Equals(2))
        {
            List<Trabajador> trabajadoresEnScrollViewAhora = ObtenerDatosTrabajadoresEnScrollView();

            if (HayUn�nicoGerente(trabajadoresEnScrollViewAhora))
            {
                rtText.anchoredPosition = new Vector2(rtText.anchoredPosition.x, 55);
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                {
                    textoEntrePar�ntesisAlQuererEliminarTrabajador.text = "(Tambi�n se eliminar�n el restaurante y sus trabajadores)";
                }
                else
                {
                    textoEntrePar�ntesisAlQuererEliminarTrabajador.text = "(The restaurant and its workers will also be eliminated.)";
                }
            }
            else
            {
                rtText.anchoredPosition = new Vector2(rtText.anchoredPosition.x, 0);
            }
        }
        else
        {
            rtText.anchoredPosition = new Vector2(rtText.anchoredPosition.x, 0);
        }

        string nombreTrabajador = bot�nPulsadoParaEliminar.gameObject.GetComponentInChildren<TMP_InputField>().text.Trim();

        if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
        {
            textoAdvertenciaEliminarTrabajador.text = "Confirmar eliminar trabajador/a " + nombreTrabajador;
        }
        else
        {
            textoAdvertenciaEliminarTrabajador.text = "Confirm delete worker " + nombreTrabajador;
        }
            

        // Mostrar contenedor con mensaje de confirmaci�n para eliminar el trabajador
        contenedorAdvertenciaEliminarTrabajador.SetActive(true);
    }

    public void CancelarEliminarTrabajador()
    {
        textoEntrePar�ntesisAlQuererEliminarTrabajador.text = "";
        contenedorAdvertenciaEliminarTrabajador.SetActive(false);
    }

    public async void ConfirmarEliminarTrabajador()
    {
        int id_trabajador = ObtenerElIdDelTrabajadorAEliminar();

        List<Trabajador> trabajadoresEnScrollViewAhora = ObtenerDatosTrabajadoresEnScrollView();

        // Si el trabajador a eliminar es Gerente y s�lo queda uno, elimino su restaurante. Al eliminar el restaurante, tambi�n se eliminan autom�ticamente todos sus trabajadores
        if (TrabajadorAEliminarEsGerente(id_trabajador) && HayUn�nicoGerente(trabajadoresEnScrollViewAhora))
        {
            // Elimino el restaurante del gerente, quien tiene gesti�n total del servicio
            string cad2 = await instanceM�todosApiController.DeleteDataAsync("restaurante/eliminarxid/" + Usuario.Restaurante_ID);

            // Deserializo la respuesta
            Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);

            if (resultado2.Result.Equals(1))
            {
                Debug.Log("Restaurante de gerente eliminado correctamente");
            }

            SceneManager.LoadScene("Main");
        }
        else
        {
            bool ElTrabajadorEliminadoEsElQueElimina = false;
            // Si el usuario eliminado es el mismo que elimina, se le env�a al men� principal
            if (Usuario.ID.Equals(id_trabajador))
            {
                ElTrabajadorEliminadoEsElQueElimina = true;
            }

            string cad = await instanceM�todosApiController.DeleteDataAsync("trabajador/eliminarxid/" + id_trabajador);

            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            if (resultado.Result.Equals(1))
            {
                Debug.Log("Trabajador eliminado con �xito");

                if (ElTrabajadorEliminadoEsElQueElimina)
                {
                    SceneManager.LoadScene("Main");
                    return;
                }

                // El trabajador eliminado no era gerente, no se han eliminado todos los trabajadores del restaurante en la BDD, y se actualiza el Scroll View
                EliminarObjetosHijoDeScrollView(rtScrollViewContent);
                ObtenerTrabajadoresDeUnRestauranteYCrearBotonesAsync();
                contenedorAdvertenciaEliminarTrabajador.SetActive(false);
            }
        }
        textoEntrePar�ntesisAlQuererEliminarTrabajador.text = "";
    }

    private bool TrabajadorAEliminarEsGerente(int id_trabajador)
    {
        int rol_ID = ObtenerRol_IDTrabajadorEnRestaurante(id_trabajador);

        Debug.Log("+ROl_ID: " + rol_ID);
        return rol_ID.Equals(2);
    }

    // Obtengo el id del trabajador a eliminar
    private int ObtenerElIdDelTrabajadorAEliminar()
    {
        string[] array = bot�nPulsadoParaEliminar.name.Split("-");
        return  int.Parse(array[1]);
    }

    public async void GuardarTrabajadores()
    {
        List<Trabajador> trabajadores = ObtenerDatosTrabajadoresEnScrollView();

        foreach (Trabajador trabajador in trabajadores)
        {
            string cad = await instanceM�todosApiController.PutDataAsync("trabajador/actualizarTrabajadorPorGerente/", trabajador);

            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            if (resultado.Result.Equals(0))
            {
                // Muestro mensaje: "El nombre X ya existe"
                
                Debug.Log("++Error: El nombre "+ trabajador.Nombre + " ya existe");
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                {
                    textoErrorAlGuardarTrabajadores.text = "El trabajador " + trabajador.Nombre + " ya existe";
                }
                else
                {
                    textoErrorAlGuardarTrabajadores.text = "The worker " + trabajador.Nombre + " already exists";
                }

                contenedorErrorAlGuardarTrabajadores.SetActive(true);
                break;
            }
            else
            {
                Debug.Log("++Okay");
            }
        }

        // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (trabajadores actualizados)
        EliminarObjetosHijoDeScrollView(rtScrollViewContent);
        ObtenerTrabajadoresDeUnRestauranteYCrearBotonesAsync();
    }

    public void PulsarOkayDelContenedorErrorAlGuardarTrabajadores()
    {
        contenedorErrorAlGuardarTrabajadores.SetActive(false);
    }

    public void ActivarCanvasRegistrarUsuario()
    {
        canvasRegistrarUsuario.SetActive(true);
    }

    public void IrALaEscenaMain()
    {
        SceneManager.LoadScene("Main");
    }

    

}
