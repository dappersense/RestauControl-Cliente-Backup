using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


public class RegisterAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private TMP_Text texto�xitoRegistro;
    [SerializeField] private TMP_InputField inputTextoNombre;
    [SerializeField] private TMP_InputField inputTextoContrase�a;
    [SerializeField] private TMP_InputField inputTextoContrase�aRepetida;
    [SerializeField] private Button bot�nConfirmar;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulaci�n deseado


    private bool esperandoEnError = false;
    private bool esperandoEnExito = false;

    M�todosAPIController instanceM�todosAPIController;
    TrabajadorController instanceTrabajadorController;
    GestionarTrabajadoresController instanceGestionarTrabajadoresController;

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosAPIController = M�todosAPIController.InstanceM�todosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;
        instanceGestionarTrabajadoresController = GestionarTrabajadoresController.InstanceGestionarTrabajadoresController;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }

        if (bot�nConfirmar.IsInteractable())
        {
            // Detectar Enter principal
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmarRegistrarUsuario();
            }

            // Detectar Enter del teclado num�rico
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ConfirmarRegistrarUsuario();
            }
        }
    }

    
    public void DesactivarCanvasRegistrarUsuario()
    {
        canvasRegistroUsuario.SetActive(false);

        textoErrorRegistro.text = "";
        texto�xitoRegistro.text = "";

        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContrase�a.text = "";
        inputTextoContrase�aRepetida.text = "";
    }

    public void ConfirmarRegistrarUsuario()
    {
        textoErrorRegistro.text = "";
        StartCoroutine(DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string textoNombre = inputTextoNombre.text.Trim();
        string textoPassword = inputTextoContrase�a.text.Trim();
        string textoRepeatedPassword = inputTextoContrase�aRepetida.text.Trim();

        //Si se han rellenado todos los huecos (InputFields) pasa por aqu�
        if (!string.IsNullOrEmpty(textoNombre) && !string.IsNullOrEmpty(textoPassword) && !string.IsNullOrEmpty(textoRepeatedPassword))
        {
            // La contrase�a es igual en ambos lados. 
            if (textoPassword.CompareTo(textoRepeatedPassword) == 0)
            {
                // La contrase�a tiene m�s de 5 caracteres
                if (textoPassword.Length > 5)
                {
                    // El nombre tiene m�s de 2 caracteres
                    if (textoNombre.Length > 2)
                    {
                        // El nombre no tiene la �
                        //if (!textoNombre.Contains("�") && !textoNombre.Contains("�"))
                        //{
                            Debug.Log("Nombre sin �");

                            // El nombre no tiene espacios entre letras
                            if (!textoNombre.Contains(" "))
                            {
                               RegisterUserAsync(textoNombre, textoPassword);     
                            }
                            else
                            {
                                Debug.Log("El nombre no puede tener espacios entre letras");
                                if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                                {
                                    textoErrorRegistro.text = "El nombre no puede tener espacios entre letras.";
                                }
                                else
                                {
                                    textoErrorRegistro.text = "The name cannot have spaces between letters.";
                                }
                                
                            }
                        /*}
                        else
                        {
                            Debug.Log("Nombre con �");
                            textoErrorRegistro.text = "El nombre no debe tener �.";
                        }*/
                    }
                    else
                    {
                        Debug.Log("Nombre con menos de 3 caracteres");
                        if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                        {
                            textoErrorRegistro.text = "Nombre con menos de 3 caracteres.";
                        }
                        else
                        {
                            textoErrorRegistro.text = "Name with less than 3 characters.";
                        }
                    }
                }
                else
                {
                    Debug.Log("Contrase�a con menos de 6 caracteres");
                    if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                    {
                        textoErrorRegistro.text = "Contrase�a con menos de 6 caracteres.";
                    }
                    else
                    {
                        textoErrorRegistro.text = "Password with less than 6 characters.";
                    }
                }
            }
            else
            {
                Debug.Log("Contrase�a diferente en ambos lados");
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
                {
                    textoErrorRegistro.text = "Contrase�a diferente en ambos lados.";
                }
                else
                {
                    textoErrorRegistro.text = "Different password on both sides.";
                }
            }
        }
        else
        {
            Debug.Log("El usuario no se puede registrar.");
            if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
            {
                textoErrorRegistro.text = "Rellene todos los campos por favor.";
            }
            else
            {
                textoErrorRegistro.text = "Please fill in all fields.";
            }
        }
        Espero1SegundoYQuitoElTextoDe�xitoOErrorAsync();
    }

    private IEnumerator DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {
        bot�nConfirmar.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(1.5f);

        bot�nConfirmar.interactable = true;
    }


    // M�todo para registrar al usuario
    public async void RegisterUserAsync(string username, string password)
    {
        // Creo la solicitud de registro
        Debug.Log("El usuario trata de registrarse");
        Trabajador t = new Trabajador(username, password, 0, 0);
        string cad = await instanceM�todosAPIController.PostDataAsync("trabajador/registrarUser", t);

        // Deserializo la respuesta
        JObject jsonObject = JObject.Parse(cad);
        int resultValue = jsonObject["result"].Value<int>();

        switch (resultValue)
        {
            case 0:
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0 || Usuario.Idioma == null)
                {
                    textoErrorRegistro.text = "Error inesperado";
                }
                else
                {
                    textoErrorRegistro.text = "Unexpected error";
                }
                break;
            case 1:
                textoErrorRegistro.text = "";

                if (Usuario.Idioma.CompareTo("Espa�ol") == 0 || Usuario.Idioma == null)
                {
                    texto�xitoRegistro.text = "Trabajador registrado y a�adido correctamente al restaurante";
                    instanceGestionarTrabajadoresController.A�adirTrabajadorARestaurante(username);
                }
                else
                {
                    texto�xitoRegistro.text = "Worker registered and successfully added to the restaurant";
                    instanceGestionarTrabajadoresController.A�adirTrabajadorARestaurante(username);
                }
                DejarVac�osLosCampos();
                break;
            case 2:
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0 || Usuario.Idioma == null)
                {
                    textoErrorRegistro.text = "El usuario " + username + " ya existe";
                }
                else
                {
                    textoErrorRegistro.text = "The user " + username + " already exists";
                }
                break;            
        }
        Espero1SegundoYQuitoElTextoDe�xitoOErrorAsync();
    }

    
    private void DejarVac�osLosCampos()
    {
        //texto�xitoRegistro.text = "";
        textoErrorRegistro.text = "";
        
        // Dejo vac�os los campos
        inputTextoNombre.text = "";
        inputTextoContrase�a.text = "";
        inputTextoContrase�aRepetida.text = "";
    }

    private async void Espero1SegundoYQuitoElTextoDe�xitoOErrorAsync()
    {
        if (textoErrorRegistro.text.Trim().Length > 0)
        {
            if (!esperandoEnError)
            {
                esperandoEnError = true;
                await Task.Delay(1500); // Espero 1 segundo sin bloquear

                textoErrorRegistro.text = "";

                esperandoEnError = false;
                return;
            }
            
        }

        if (texto�xitoRegistro.text.Trim().Length > 0)
        {
            if (!esperandoEnExito)
            {
                esperandoEnExito = true;
                await Task.Delay(1500); // Espero 1 segundo sin bloquear

                texto�xitoRegistro.text = "";

                esperandoEnExito = false;
                return;
            }
        }
    }

    /// <summary>
    ///  M�todo para cambiar de componente con TAB en la interfaz gr�fica.
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
