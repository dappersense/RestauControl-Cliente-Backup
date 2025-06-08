using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuscarReservaController : MonoBehaviour
{
    [SerializeField] private GameObject canvasBuscarReserva;
    [SerializeField] private TMP_InputField inputFieldDniCliente;
    [SerializeField] private RectTransform rectTransformContent;
    [SerializeField] private Button bot�nCancelarReserva;
    [SerializeField] private GameObject bloqueConfirmarCancelarReserva;
    [SerializeField] private Button bot�nConfirmarCancelarReserva;
    [SerializeField] private Button bot�nBuscarReservas;
    [SerializeField] private TextMeshProUGUI titulo;

    private List<Button> botonesParaCancelar = new List<Button>();
    private Button bot�nMesaSeleccionado;
    private int id_ReservaACancelar;
    private string dniEnInputField;

    GestionarMesasController instanceGestionarMesasController;
    M�todosAPIController instanceM�todosAPIController;

    // Start is called before the first frame update
    void Start()
    {
        instanceGestionarMesasController = GestionarMesasController.InstanceGestionarMesasController;
        instanceM�todosAPIController = M�todosAPIController.InstanceM�todosAPIController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuscarReservasDeCliente()
    {
        if (inputFieldDniCliente.text.Trim().Length.Equals(9))
        {
            dniEnInputField = inputFieldDniCliente.text.Trim();

            BuscarReservasCliente(dniEnInputField);

            inputFieldDniCliente.text = "";
        }        
    }

    private void BuscarReservasCliente(string dniEnInputField)
    {
        List<Reserva> reservasCliente = ObtenerReservasCliente(dniEnInputField);

        // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
        EliminarObjetosHijoDeScrollView();

        if (reservasCliente.Count > 0)
        {
            // Una vez obtenidas las reservas del cliente, las coloco ordenadas como botones en un Scroll View
            CrearBotonesDeReservasDeCliente(reservasCliente);
        }
    }

    private void EliminarObjetosHijoDeScrollView()
    {
        foreach (Transform hijo in rectTransformContent)
        {
            Destroy(hijo.gameObject);
        }
    }

    private void CrearBotonesDeReservasDeCliente(List<Reserva> reservasCliente)
    {
        List<Reserva> reservasTerminadasOCanceladas = ObtenerReservasTerminadasOCanceladas(reservasCliente);
        List<Reserva> reservasConfirmadas = ObtenerReservasConfirmadas(reservasCliente);
        List<Reserva> reservasEnUso = ObtenerReservasEnUso(reservasCliente);

        botonesParaCancelar.Clear();

        // Existe una o varias reservas en uso  
        if (reservasEnUso.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasEnUso
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (m�s recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();


            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                CrearBot�nEnScrollView(r, 1);
            }            
        }

        // Existen reservas de la mesa para hoy confirmadas 
        if (reservasConfirmadas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasConfirmadas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (m�s recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();


            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                CrearBot�nEnScrollView(r, 2);
            }
        }

        // Existen reservas de la mesa para hoy terminadas 
        if (reservasTerminadasOCanceladas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasTerminadasOCanceladas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (m�s recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();

            foreach (Reserva reserv in reservasOrdenadasPorHora)
            {
                CrearBot�nEnScrollView(reserv, 3);
            }
        }

        // Poner listener a los botones que se pueden cancelar
        PonerListenerABotonesDeReservasQueSePuedenCancelar();
    }

    private void PonerListenerABotonesDeReservasQueSePuedenCancelar()
    {
        // Obtenemos todos los componentes Button que sean hijos del contenedor
        foreach (Button button in botonesParaCancelar)
        {
            // Es recomendable capturar la referencia del bot�n para evitar problemas con clausuras
            Button capturedButton = button;
            capturedButton.onClick.AddListener(() => ActivarBot�nCancelarReserva(capturedButton));
        }
    }

    private void ActivarBot�nCancelarReserva(Button capturedButton)
    {
        bot�nMesaSeleccionado = capturedButton; // Obtengo el bot�n mesa que he pulsado

        bot�nCancelarReserva.interactable = true;
    }

    private List<Reserva> ObtenerReservasTerminadasOCanceladas(List<Reserva> reservasCliente)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasCliente)
        {
            if (reserva.Estado.CompareTo(""+EstadoReserva.Terminada) == 0 || reserva.Estado.CompareTo(""+EstadoReserva.Cancelada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private List<Reserva> ObtenerReservasConfirmadas(List<Reserva> reservasCliente)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasCliente)
        {
            if (reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private List<Reserva> ObtenerReservasEnUso(List<Reserva> reservasCliente)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasCliente)
        {
            if (reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private void CrearBot�nEnScrollView(Reserva reserva, int num)
    {
        // Creo un GameObject para el bot�n y le asigno un nombre �nico.
        GameObject bot�nGO = new GameObject("Button-" + reserva.Id);

        // Establezco el padre para que se muestre en el UI.
        bot�nGO.transform.SetParent(rectTransformContent, false);

        // Agrego el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�ado expl�citamente).
        RectTransform rt = bot�nGO.AddComponent<RectTransform>();

        // Defino un tama�o por defecto para el bot�n.
        rt.sizeDelta = new Vector2(1530.9f, 138f);

        // Agrego CanvasRenderer para poder renderizar el UI.
        bot�nGO.AddComponent<CanvasRenderer>();

        // Agrego el componente Image para mostrar el sprite.
        Image imagen = bot�nGO.AddComponent<Image>();

        switch (num)
        {
            case 1:
                instanceGestionarMesasController.PonerColorCorrectoAImg(imagen, "#6DEC6F");
                //imagen.color = Color.green;
                break;
            case 2:
                instanceGestionarMesasController.PonerColorCorrectoAImg(imagen, "#FDF468");
                break;
            case 3:
                instanceGestionarMesasController.PonerColorCorrectoAImg(imagen, "#EC6C6C");
                break;
        }

        // Agrego un componente Button para que sea interactivo
        bot�nGO.AddComponent<Button>();

        // Creo un nuevo GameObject hijo, el texto del bot�n
        CrearTextoDelButton(rt, reserva);

        if (num.Equals(1) || num.Equals(2))
        {
            botonesParaCancelar.Add(bot�nGO.GetComponent<Button>());
        }
    }

    private void CrearTextoDelButton(RectTransform rt, Reserva reserva)
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

        // Obtengo el Id de la mesa en el mapa
        Button bot�nMesaSelected = instanceGestionarMesasController.padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id).GetComponent<Button>();
        int id_Mesa_En_Mapa = int.Parse(instanceGestionarMesasController.ObtenerIDMesaDelMapa(bot�nMesaSelected));

        PonerTextoBot�nDeFormaCorrecta(textoBot�n, reserva, id_Mesa_En_Mapa);
        
    }

    private void PonerTextoBot�nDeFormaCorrecta(TMP_Text textoBot�n, Reserva reserva, int id_Mesa_En_Mapa)
    {
        // El cliente de la reserva tiene un n�mero de tel�fono registrado
        if (reserva.Cliente.NumTelefono.Trim().Length > 0)
        {
            if (id_Mesa_En_Mapa.ToString().Length.Equals(2))
            {
                textoBot�n.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "      " + reserva.Cliente.Dni + "   " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
            }
            else
            {
                textoBot�n.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "        " + reserva.Cliente.Dni + "   " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
            }
        }
        else
        {
            if (id_Mesa_En_Mapa.ToString().Length.Equals(2))
            {
                textoBot�n.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "      " + reserva.Cliente.Dni + "                       " + reserva.Cliente.Nombre;
            }
            else
            {
                textoBot�n.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "        " + reserva.Cliente.Dni + "                       " + reserva.Cliente.Nombre;
            }
        }
    }

    private List<Reserva> ObtenerReservasCliente(string dni)
    {
        List<Mesa> mesasRestaurante = instanceGestionarMesasController.GetMesas();

        List<Reserva> reservasCliente = new List<Reserva>();
        foreach (Mesa mesa in mesasRestaurante)
        {
            foreach (Reserva reserva in mesa.Reservas)
            {
                if (reserva.Cliente.Dni.CompareTo(dni) == 0)
                {
                    reservasCliente.Add(reserva);
                }
            }
        }
        return reservasCliente;
    }

    public void CancelarReserva()
    {
        bot�nBuscarReservas.interactable = false;

        PonerInteractuablesONoLosBotonesHijoDelScrollView(false);
        bot�nCancelarReserva.interactable = false;

        Debug.Log("- -Paso por cancelar reserva");

        string[] nombreBot�nSeleccionadoArray = bot�nMesaSeleccionado.gameObject.name.Split("-");
        id_ReservaACancelar = int.Parse(nombreBot�nSeleccionadoArray[1]);

        Debug.Log("- -ID reserva: "+ id_ReservaACancelar);

        bloqueConfirmarCancelarReserva.SetActive(true);
    }

    private void PonerInteractuablesONoLosBotonesHijoDelScrollView(bool b)
    {
        foreach (Transform bot�n in rectTransformContent)
        {
            bot�n.GetComponent<Button>().interactable = b;
        }
    }

    public async void ConfirmarCancelarReserva()
    {
        bot�nConfirmarCancelarReserva.interactable = false;

        string cad = await instanceM�todosAPIController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(id_ReservaACancelar, "", "", "" + EstadoReserva.Cancelada, 0, 0, 0, new Cliente("", "", "")));

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("- -Reserva cancelada correctamente");

            // Espero 1.5 segundos a que el c�digo del cliente actualice sus mesas
            await Task.Delay(1500);

            BuscarReservasCliente(dniEnInputField);
            bloqueConfirmarCancelarReserva.SetActive(false);
            bot�nBuscarReservas.interactable = true;
            PonerInteractuablesONoLosBotonesHijoDelScrollView(true);
        }
        else
        {
            Debug.Log("Error al intentar terminar una reserva");
        }
        
        bot�nConfirmarCancelarReserva.interactable = true;
    }

    public void DesactivarBloqueConfirmarCancelarReserva()
    {
        bot�nCancelarReserva.interactable = false;
        bloqueConfirmarCancelarReserva.SetActive(false);
        bot�nBuscarReservas.interactable = true;
        PonerInteractuablesONoLosBotonesHijoDelScrollView(true);
    }

    public void DesactivarCanvasBuscarReserva()
    {
        canvasBuscarReserva.SetActive(false);

        EliminarObjetosHijoDeScrollView();
        inputFieldDniCliente.text = "";
        bot�nCancelarReserva.interactable = false;
    }
}
