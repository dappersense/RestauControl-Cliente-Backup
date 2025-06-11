using System.Collections;
using UnityEngine;

using System.Threading.Tasks;
using System;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;


public class MainController : MonoBehaviour
{
    [SerializeField] private GameObject canvasLogInUsuario;
    [SerializeField] private GameObject canvasIdiomasLogInYRegistro;
    [SerializeField] private GameObject medioYFinTelon;
    [SerializeField] private TMP_Text textUserNombre;
    [SerializeField] private TMP_Text textUserRol;
    [SerializeField] private TMP_Text textUserRestaurante;
    [SerializeField] private GameObject canvasCrearRestaurante;
    [SerializeField] private Button bot�nCerrarSesi�n;
    [SerializeField] private Button bot�nComprarServicio;
    [SerializeField] private Button bot�nEditarRestaurante;
    [SerializeField] private Button bot�nGestionarMesas;
    [SerializeField] private Button bot�nGestionarTrabajadores;
    [SerializeField] private Button bot�nGestionarArticulos;
    [SerializeField] private RectTransform rtBot�nIdiomaSpanish;
    [SerializeField] private RectTransform rtBot�nIdiomaEnglish;
    [SerializeField] private RectTransform rtTextoIniciarSesi�n;
    [SerializeField] private GameObject canvasV�deoTutorialApp;
    [SerializeField] private VideoClip videoClipEmpleado;
    [SerializeField] private VideoClip videoClipGerente;
    [SerializeField] private VideoClip videoClipGerenteUnaVezCreadoElRestaurante;
    [SerializeField] private Image imgBtnOnYPausa;
    [SerializeField] private Sprite imgPausa;
    [SerializeField] private Sprite imgPlay;
    [SerializeField] private VideoPlayer videoPlayer;

    private bool tel�nMovi�ndose = false;
    private bool tel�nAbajo = false;

    M�todosAPIController instanceM�todosAPIController;
    TrabajadorController instanceTrabajadorController;
    FicheroController instanceFicheroController;

    public static MainController InstanceMainController { get; private set; }

    void Awake()
    {
        if (InstanceMainController == null)
        {
            InstanceMainController = this;
        }

        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);

    }

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosAPIController = M�todosAPIController.InstanceM�todosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;
        instanceFicheroController = FicheroController.InstanceFicheroController;

        instanceFicheroController.GestionarFicheros();

        TrabajadorController.ComprobandoDatosTrabajador = false;
        instanceTrabajadorController.PonerDatosEnPerfilTrabajador(textUserNombre, textUserRol, textUserRestaurante);


        Gesti�nInicioDelProgramaAsync();

        // M�todo para prevenir
        QuitarYPonerBotonesSeg�nElTrabajador();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitarYPonerBotonesSeg�nElTrabajador()
    {
        QuitarBot�nComprarServicio();

        PonerBotonesFuncionalidadesProgramaSegunElRol();
    }


    // Si el usuario tiene un restaurante_ID superior a 0 (el trabajador ya est� asignado a un restaurante), se desactiva el bot�n de comprar el servicio
    private void QuitarBot�nComprarServicio()
    {
        if (!GeneralController.NoHayConexion)
        {
            if (Usuario.Restaurante_ID > 0)
            {
                bot�nComprarServicio.gameObject.SetActive(false);
            }
            else // El trabajador no est� en ning�n restaurante a�n y le sale el bot�n de comprar el servicio
            {// Quiz�s no es necesario este else
                bot�nComprarServicio.gameObject.SetActive(true);
            }
        }
    }

    private void PonerBotonesFuncionalidadesProgramaSegunElRol()
    {
        RectTransform rt = bot�nGestionarMesas.gameObject.GetComponent<RectTransform>();

        // Si el trabajador est� en un restaurante, se comprueba el rol
        if (Usuario.Restaurante_ID > 0)
        {
            switch (Usuario.Rol_ID)
            {
                case 1:
                    bot�nEditarRestaurante.gameObject.SetActive(false);
                    rt.anchoredPosition = new Vector2(0, 0);
                    bot�nGestionarMesas.gameObject.SetActive(true);
                    bot�nGestionarTrabajadores.gameObject.SetActive(false);
                    bot�nGestionarArticulos.gameObject.SetActive(false);
                    break;
                case 2: // Si el trabajador tiene el rol de "Gerente", se muestran los botones espec�ficos para editar el restaurante y para gestionar los trabajadores
                    bot�nEditarRestaurante.gameObject.SetActive(true);
                    rt.anchoredPosition = new Vector2(0, 262);
                    bot�nGestionarMesas.gameObject.SetActive(true);
                    bot�nGestionarTrabajadores.gameObject.SetActive(true);
                    bot�nGestionarArticulos.gameObject.SetActive(true);
                    break;
            }
        }
        else // El trabajador no est� en ning�n restaurante
        {
            bot�nEditarRestaurante.gameObject.SetActive(false);
            bot�nGestionarMesas.gameObject.SetActive(false);
            bot�nGestionarTrabajadores.gameObject.SetActive(false);
            bot�nGestionarArticulos.gameObject.SetActive(false);
        }
                  
    }

    private async void Gesti�nInicioDelProgramaAsync()
    {
        //PlayerPrefs.SetInt("UsuarioRegistrado", 0); // - - - Quitar esta l�nea cuando deje de hacer pruebas con el registro e inicio de sesi�n

        int usuarioRegistrado = PlayerPrefs.GetInt("UsuarioRegistrado", 0); // 1 es s�, 0 es no
        //Si el usuario no se ha registrado, le aparece el canvas de iniciar sesi�n
        if (usuarioRegistrado.Equals(0))
        {
            canvasLogInUsuario.SetActive(true);
            canvasIdiomasLogInYRegistro.SetActive(true);
        }
        else // Si el usuario ya est� registrado, compruebo si sigue en la BDD por si lo han eliminado y obtengo su rol_ID actualizado, por si el gerente se lo ha cambiado.
        {
           await ComprueboSiUserExisteAsync();
        }

        Debug.Log("ID Usuario: " + Usuario.ID + ", Nombre Usuario: " + Usuario.Nombre + ", Rol_ID Usuario: " + Usuario.Rol_ID + ", Restaurante_ID Usuario: " + Usuario.Restaurante_ID);
    }

    private async Task ComprueboSiUserExisteAsync()
    {
        string cad = await instanceM�todosAPIController.GetDataAsync("trabajador/existe/" + Usuario.ID);
        try
        {
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            // El trabajador no existe. Ha sido eliminado de la BDD y tiene que volver a registrarse.
            if (resultado.Result.Equals(0))
            {
                PlayerPrefs.SetInt("UsuarioRegistrado", 0);
                PlayerPrefs.Save();
                canvasLogInUsuario.SetActive(true);
                canvasIdiomasLogInYRegistro.SetActive(true);
            }
            else // El trabajdor existe y obtengo sus datos por si ha tenido cambios. Ejemplo: le han puesto un rol distinto o le han agregado a un restaurante.
            {
                instanceTrabajadorController.ObtenerDatosTrabajadorPorIdAsync(Usuario.ID);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }
    }


    public void PresionarBot�nPerfil()
    {
        RectTransform rt = medioYFinTelon.GetComponent<RectTransform>();

        // Si el tel�n no se mueve...
        if (!tel�nMovi�ndose)
        {
            tel�nMovi�ndose = true;
            bot�nCerrarSesi�n.interactable = false;
            // Y el tel�n no est� abajo, va para abajo
            if (!tel�nAbajo)
            {
                StartCoroutine(MoverTel�nHaciaAbajo(rt));
            }
            else // Y el tel�n est� abajo, va para arriba
            {
                StartCoroutine(MoverTel�nHaciaArriba(rt));
            }
        }
        
    }

    private IEnumerator MoverTel�nHaciaAbajo(RectTransform rt)
    {
        float velocidad = 1200f;
        while (rt.anchoredPosition.y > 193)
        {
            //Actualizo
            float yTelon = rt.anchoredPosition.y - velocidad * Time.deltaTime;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, yTelon);

            // Espero al siguiente frame antes de continuar. M�s fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
        
        tel�nMovi�ndose = false;
        tel�nAbajo = true;
        bot�nCerrarSesi�n.interactable = true;
    }

    private IEnumerator MoverTel�nHaciaArriba(RectTransform rt)
    {
        float velocidad = 1200f;
        while (rt.anchoredPosition.y < 1143)
        {
            //Actualizo
            float y = rt.anchoredPosition.y + velocidad * Time.deltaTime;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero
            yield return null;
        }

        tel�nMovi�ndose = false;
        tel�nAbajo = false;
        bot�nCerrarSesi�n.interactable = true;
    }

    public void IrAlCanvasCrearRestaurante()
    {
        canvasCrearRestaurante.SetActive(true);
        //Si el tel�n est� abajo
        if (tel�nAbajo)
            PresionarBot�nPerfil();
    }

    public void IrALaEscenaEditarRestaurante()
    {
        SceneManager.LoadScene("Editar Restaurante");
    }

    public void IrALaEscenaGestionarMesas()
    {
        SceneManager.LoadScene("Gestionar Mesas");
    }

    public void IrALaEscenaGestionarTrabajadores()
    {
        SceneManager.LoadScene("Gestionar Trabajadores");
    }
    public void IrALaEscenaGestionarArticulos()
    {
        SceneManager.LoadScene("Gestionar Articulos");
    }
    public void CerrarSesi�n()
    {
        PlayerPrefs.SetInt("UsuarioRegistrado", 0);
        
        // Pongo los botones de idiomas con una "Y" espec�fica para el canvas de iniciar sesi�n
        rtBot�nIdiomaSpanish.anchoredPosition = new Vector2(rtBot�nIdiomaSpanish.anchoredPosition.x, rtTextoIniciarSesi�n.anchoredPosition.y);
        rtBot�nIdiomaEnglish.anchoredPosition = new Vector2(rtBot�nIdiomaEnglish.anchoredPosition.x, rtTextoIniciarSesi�n.anchoredPosition.y);

        canvasLogInUsuario.SetActive(true);
        canvasIdiomasLogInYRegistro.SetActive(true);
        PresionarBot�nPerfil();
    }

    public void SalirDeLaApp()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        Application.wantsToQuit += OnWantsToQuitAsync;
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= OnWantsToQuitAsync;
    }

    // Intercepta el intento de cerrar la aplicaci�n (por ejemplo, con Alt + F4 o clic en el bot�n de cierre de la ventana).
    private bool OnWantsToQuitAsync() // Hacer que no funcione este m�todo hasta que una parte del programa cargue que es la generaci�n del lobby (creo, no estoy seguro)
    {
        Debug.Log("Interceptando Alt + F4 o cierre manual.");
        
        return true; // Unity cierra la aplicaci�n autom�ticamente.
    }

    public void ActivarCanvasV�deoTutorial()
    {
        VideoPlayer vp = canvasV�deoTutorialApp.GetComponent<VideoPlayer>();


        if (textUserRol.text.Trim().Length > 0)
        {
            if (textUserRol.text.Contains("Empleado") || textUserRol.text.Contains("Employee"))
            {
                vp.clip = videoClipEmpleado;
            }
            else
            {
                if (bot�nGestionarMesas.IsActive())
                {
                    vp.clip = videoClipGerenteUnaVezCreadoElRestaurante;
                }
                else
                {
                    vp.clip = videoClipGerente;
                }                    
            }
        }
        
        canvasV�deoTutorialApp.SetActive(true);
        
        vp.Play();
    }

    public void DesactivarCanvasTutorialV�deo()
    {
        VideoPlayer vp = canvasV�deoTutorialApp.GetComponent<VideoPlayer>();
        vp.Stop();
        canvasV�deoTutorialApp.SetActive(false);

        imgBtnOnYPausa.sprite = imgPausa;
    }

    public void PausarODespausarV�deo()
    {
        VideoPlayer vp =  canvasV�deoTutorialApp.GetComponent<VideoPlayer>();
        if (imgBtnOnYPausa.sprite == imgPausa)
        {
            vp.Pause();
            imgBtnOnYPausa.sprite = imgPlay;
            return;
        }

        if (imgBtnOnYPausa.sprite == imgPlay)
        {
            vp.Play();
            imgBtnOnYPausa.sprite = imgPausa;
            return;
        }

    }

    public TMP_Text getTextPerfilUserNombre()
    {
        return textUserNombre;
    }

    public TMP_Text getTextPerfilUserRol()
    {
        return textUserRol;
    }

    public TMP_Text getTextPerfilUserRestaurante()
    {
        return textUserRestaurante;
    }

}
