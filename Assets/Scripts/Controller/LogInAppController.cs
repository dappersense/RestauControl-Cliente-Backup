using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;



public class LogInAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private GameObject canvasInicioSesi�nUsuario;
    [SerializeField] private TMP_Text textoErrorLogin;
    [SerializeField] private TMP_Text textoExitoLogin;
    [SerializeField] private TMP_InputField inputFieldNombreLogin;
    [SerializeField] private TMP_InputField inputFieldPasswordLogin;
    [SerializeField] private Button bot�nRegistrarse;
    [SerializeField] private Button bot�nAcceder;
    [SerializeField] private GameObject canvasIdiomasLogInYRegistro;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulaci�n deseado
    [SerializeField] private RectTransform rtBot�nIdiomaSpanish;
    [SerializeField] private RectTransform rtBot�nIdiomaEnglish;
    [SerializeField] private RectTransform rtTextoRegistrarse;
    [SerializeField] private GameObject canvasInformaci�nApp;

    //private bool mensajeAPIPlayFabDevuelto = false;

    M�todosAPIController instanceM�todosAPIController;
    TrabajadorController instanceTrabajadorController;


    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosAPIController = M�todosAPIController.InstanceM�todosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;
                
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }

        if (bot�nAcceder.IsInteractable())
        {
            // Detectar Enter principal
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Se ha pulsado Enter");
                ConfirmarIniciarSesi�n();
            }

            // Detectar Enter del teclado num�rico
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("Se ha pulsado Enter del teclado num�rico");
                ConfirmarIniciarSesi�n();
            }
        }
        
    }


    // M�todo para ser llamado por el bot�n de inicio de sesi�n
    public void ConfirmarIniciarSesi�n()
    {
        textoErrorLogin.text = "";
        StartCoroutine(DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string username = inputFieldNombreLogin.text.Trim();
        string password = inputFieldPasswordLogin.text.Trim();

        // Validaciones b�sicas antes de intentar iniciar sesi�n
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            textoErrorLogin.text = "Por favor, ingresa tu nombre de usuario y contrase�a.";
            return;
        }
        Debug.Log("El usuario trata de iniciar sesi�n");

        LoginUserAsync(username, password);
    }

    private IEnumerator DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {
        //mensajeAPIPlayFabDevuelto = false;

        bot�nRegistrarse.interactable = false;
        bot�nAcceder.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(2f);

        bot�nRegistrarse.interactable = true;
        bot�nAcceder.interactable = true;
    }

    // M�todo para iniciar sesi�n al usuario
    public async void LoginUserAsync(string nombre, string password)
    {
        // Creo la solicitud de inicio de sesi�n
        Trabajador t = new Trabajador(nombre, password, 0, 0);
        string cad = await instanceM�todosAPIController.PostDataAsync("trabajador/logIn/", t);

        // Deserializo la respuesta
        JObject jsonObject = JObject.Parse(cad);
        int resultValue = jsonObject["result"].Value<int>();
        

        //Resultado data = JsonConvert.DeserializeObject<Resultado>(cad);
        switch (resultValue)
        {
            case 1:
                string tokenValue = jsonObject["token"].Value<string>();
                Usuario.Token = tokenValue;
                Debug.Log("Token: " + Usuario.Token);
                FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Restaurante_ID, Usuario.Idioma, tokenValue); // Guardo el token en el fichero

                if (Usuario.Idioma.CompareTo("Espa�ol") == 0 || Usuario.Idioma == null)
                {
                    textoExitoLogin.text = "Inicio de sesi�n correcto";
                }
                else
                {
                    textoExitoLogin.text = "Successful login";
                }
                GestionarLogInExitoso();
                instanceTrabajadorController.ObtenerDatosTrabajadorPorNombreAsync(new Trabajador(nombre, "", 0, 0));
                break;
            case 0:
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0 || Usuario.Idioma == null)
                {
                    textoErrorLogin.text = "Contrase�a incorrecta";
                }
                else
                {
                    textoErrorLogin.text = "Incorrect password";
                }
                break;
            case -1:
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0 || Usuario.Idioma == null)
                {
                    textoErrorLogin.text = "El trabajador " + nombre + " no existe";
                }
                else
                {
                    textoErrorLogin.text = "The worker " + nombre + " does not exist";
                }
                break;
        }
    }

    private void GestionarLogInExitoso()
    {
        StartCoroutine(FinInicioSesion());

        string nombreUsuario = inputFieldNombreLogin.text.Trim();

        Usuario.Nombre = nombreUsuario;
    }

    // Coroutine para finalizar el proceso de inicio de sesi�n
    private IEnumerator FinInicioSesion()
    {
        yield return new WaitForSeconds(1f); // Espera 1 segundo

        textoExitoLogin.text = "";
        textoErrorLogin.text = "";
        
        canvasIdiomasLogInYRegistro.SetActive(false);
        canvasInicioSesi�nUsuario.SetActive(false);

        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
        PlayerPrefs.Save();
    }

    

    public void IrAlCanvasInformationApp()
    {
        canvasInicioSesi�nUsuario.SetActive(false);
        canvasIdiomasLogInYRegistro.SetActive(false);
        canvasInformaci�nApp.SetActive(true);

        textoErrorLogin.text = "";

        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";
    }

    public void ActivarCanvasIniciarSesi�n()
    {
        canvasInformaci�nApp.SetActive(false);
        canvasInicioSesi�nUsuario.SetActive(true);
        canvasIdiomasLogInYRegistro.SetActive(true);
    }



    /// <summary>
    /// M�todo para cambiar de componente con TAB en la interfaz gr�fica.
    /// </summary>
    private void SelectNextInputField()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].gameObject == current)
            {
                int nextIndex = (i + 1) % inputFields.Length;
                inputFields[nextIndex].Select();
                break;
            }
        }
    }

}
