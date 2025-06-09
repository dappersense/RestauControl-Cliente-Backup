using Assets.Scripts.Model;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMesaController : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    public static ButtonMesaController buttonSeleccionadoParaBorrar;

    [Header("Referencias")]
    // Imagen contenedora que limitar� los botones (debe tener RectTransform)
    public RectTransform containerRect;

    public RectTransform rectTransform; // RectTransform del bot�n generado
    private bool isDragging = false;
    private static int cantComensalesMesaSeleccionadaParaEditarOBorrar;

    EditarRestauranteController instanceEditarRestauranteController;

    public static ButtonMesaController InstanceButtonMesaController { get; private set; }

    void Awake()
    {
        if (InstanceButtonMesaController == null)
        {
            InstanceButtonMesaController = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceEditarRestauranteController = EditarRestauranteController.InstanceEditarRestauranteController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnButton()
    {
        // Defino la posici�n de spawn (centro del contenedor)
        Vector2 spawnPos = Vector2.zero;

        // Defino el tama�o predeterminado del bot�n (180x100)
        Vector2 defaultSize = new Vector2(180, 100);

        // Creo un Rect que representar�a el �rea del nuevo bot�n
        Rect newButtonRect = new Rect(spawnPos - defaultSize / 2f, defaultSize);

        // Compruebo si en la posici�n de spawn ya hay un bot�n (que se solape)
        ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();

        // Reviso si ya hay un bot�n en la zona central
        foreach (ButtonMesaController btn in buttons)
        {
            if (btn.rectTransform == null)
                continue;

            Vector2 otherPos = btn.rectTransform.anchoredPosition;
            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;

            //if (Mathf.Abs(otherPos.x - spawnPos.x) < marginX && Mathf.Abs(otherPos.y - spawnPos.y) < marginY)
            if (Mathf.Abs(otherPos.x - spawnPos.x) < (otherSize.x / 2f + defaultSize.x / 2f) && Mathf.Abs(otherPos.y - spawnPos.y) < (otherSize.y / 2f + defaultSize.y / 2f))
            {
                Debug.Log("Ya existe una mesa en el centro. No se crear� una nuevo.");

                string cad = "";
                if (Usuario.Idioma.CompareTo("Espa�ol") == 0) 
                {
                    cad = "Ya existe una mesa en el centro.";
                    StartCoroutine(instanceEditarRestauranteController.MovimientoCartelDeMadera(2f, cad, 0f, 12f));
                }
                else
                {
                    cad = "There is already a table in the center.";
                    StartCoroutine(instanceEditarRestauranteController.MovimientoCartelDeMadera(2f, cad, 7.5f, 12f));
                }
                return;
            }
        }

        instanceEditarRestauranteController.GetButtonA�adirMesa().interactable = false;
        Debug.Log("Es null?: " + instanceEditarRestauranteController.GetContenedorAsignarComensales());
        instanceEditarRestauranteController.GetContenedorAsignarComensales().SetActive(true);
    }

    // Detecta cuando se presiona el bot�n (inicia el arrastre)
    public void OnPointerDown(PointerEventData eventData)
    {
        // Compruebo si el bot�n no es interactuable
        Button btn = GetComponent<Button>();
        if (btn != null && !btn.interactable)
        {
            return; // Salgo si el bot�n no es interactuable
        }

        // Verifica si este objeto es hijo del contenedorPadre
        if (transform.parent != containerRect.transform)
        {
            Debug.Log("Este bot�n no es hijo del contenedor permitido.");
            return; // Salir si no es hijo
        }

        // Si se pulsa con clic derecho, se marca este bot�n
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (buttonSeleccionadoParaBorrar != null)
            {
                DesactivarCaretsInputFieldBot�nMesa(buttonSeleccionadoParaBorrar);
                ComprobarQueElInputFieldNoEst�Vac�o(buttonSeleccionadoParaBorrar); // Si est� vac�o se poen la cantidad de comensales que ten�a antes la mesa
            }
            
            // Actualizo la referencia del bot�n seleccionado
            buttonSeleccionadoParaBorrar = this;

            cantComensalesMesaSeleccionadaParaEditarOBorrar = int.Parse(buttonSeleccionadoParaBorrar.transform.Find("InputField").GetComponent<TMP_InputField>().text);
            PonerTodosLosBotonesEnBlanco();
            PonerBot�nSeleccionadoConC�rculoAmarillo(buttonSeleccionadoParaBorrar);
            ActivarInputFieldDelBot�nMesa(buttonSeleccionadoParaBorrar);
            //Debug.Log("Bot�n marcado: " + gameObject.name);

            instanceEditarRestauranteController.ActivarPapelera();
            
            isDragging = false; // No queremos que inicie un arrastre con clic derecho.
            return;
        }
        // Si se pulsa con clic izquierdo
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            DesactivarPapeleraEInputFieldBtnMesa();
        }
    }

    public void DesactivarPapeleraEInputFieldBtnMesa()
    {
        // Si el clic izquierdo es en algo que no es el bot�n papelera, se desmarca
        if (!gameObject.CompareTag("TrashButton"))
        {
            // Si se ha marcado alg�n bot�n previamente y se hace clic en otro elemento,
            // se deselecciona el bot�n marcado.
            if (buttonSeleccionadoParaBorrar != null)
            {
                // Pongo el color del bot�n seleccionado para borrar en blanco
                Image imgButtonAEliminar = buttonSeleccionadoParaBorrar.transform.Find("Imagen Circle").GetComponent<Image>();
                imgButtonAEliminar.color = Color.white; 

                DesactivarCaretsInputFieldBot�nMesa(buttonSeleccionadoParaBorrar);
                ComprobarQueElInputFieldNoEst�Vac�o(buttonSeleccionadoParaBorrar); // Si est� vac�o se poen la cantidad de comensales que ten�a antes la mesa
            }
            buttonSeleccionadoParaBorrar = null;

            instanceEditarRestauranteController.DesactivarPapelera();
            //Debug.Log("Selecci�n desmarcada");
        }
        isDragging = true;
    }

    private void ActivarInputFieldDelBot�nMesa(ButtonMesaController buttonSeleccionadoParaBorrar)
    {
        GameObject inputFieldInstance = buttonSeleccionadoParaBorrar.transform.Find("InputField").gameObject;

        TMP_Text textComponent = inputFieldInstance.transform.Find("Text Area/Text").GetComponent<TMP_Text>();

        TMP_Text textPlaceHolder = inputFieldInstance.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

        // Desactivo Raycast Target para que no bloqueen interacci�n con el bot�n
        TMP_SelectionCaret caret = inputFieldInstance.GetComponentInChildren<TMP_SelectionCaret>();
        if (caret != null)
        {
            // Desactivo raycastTarget del Caret
            caret.raycastTarget = true;
        }
        else
        {
            Debug.Log("Caret no encontrado!!!!!!!!!!!!!!!!!");
        }

        inputFieldInstance.GetComponent<Image>().raycastTarget = true;
        textPlaceHolder.raycastTarget = true;
        textComponent.raycastTarget = true;

        TMP_InputField inputField = inputFieldInstance.GetComponent<TMP_InputField>();
        inputField.Select();
    }

    private void DesactivarCaretsInputFieldBot�nMesa(ButtonMesaController buttonSeleccionadoParaBorrar)
    {
        GameObject inputFieldInstance = buttonSeleccionadoParaBorrar.transform.Find("InputField").gameObject;
        TMP_Text textComponent = inputFieldInstance.transform.Find("Text Area/Text").GetComponent<TMP_Text>();

        TMP_Text textPlaceHolder = inputFieldInstance.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

        // Desactivo Raycast Target para que no bloqueen interacci�n con el bot�n
        TMP_SelectionCaret caret = inputFieldInstance.GetComponentInChildren<TMP_SelectionCaret>();
        if (caret != null)
        {
            // Desactivo raycastTarget del Caret
            caret.raycastTarget = false;
        }
        else
        {
            Debug.Log("Caret no encontrado!!!!!!!!!!!!!!!!!");
        }

        inputFieldInstance.GetComponent<Image>().raycastTarget = false;
        textPlaceHolder.raycastTarget = false;
        textComponent.raycastTarget = false;
    }

    private void ComprobarQueElInputFieldNoEst�Vac�o(ButtonMesaController buttonSeleccionadoParaBorrar)
    {
        TMP_InputField inputField = buttonSeleccionadoParaBorrar.transform.Find("InputField").GetComponent<TMP_InputField>();

        string textInputField = inputField.text.Trim();

        // InputField no est� vac�o
        if (textInputField.Length > 0 && textInputField.CompareTo("0") != 0)
        {
            return;
        }
        else // InputField vac�o, por lo que se pone el valor que ten�a antes
        {
            int cantCom = 0;
            try
            {
                string[] arrayNombreBtnMesa = buttonSeleccionadoParaBorrar.name.Split("-");
                List<Mesa> mesas = instanceEditarRestauranteController.GetMesasRest();
                foreach (Mesa mesa in mesas)
                {
                    if (mesa.Id.Equals(int.Parse(arrayNombreBtnMesa[1])))
                    {
                        cantCom = mesa.CantPers;
                    }
                }
            }
            catch
            {
                //Debug.Log("Pasa por el catch:"+cantComensalesMesaSeleccionadaParaEditarOBorrar);
                cantCom = cantComensalesMesaSeleccionadaParaEditarOBorrar;
            }

            //Debug.Log("Pasa por el catch2:"+cantCom);
            inputField.text = ""+ cantCom;
        }
    }

    public void PonerTodosLosBotonesEnBlanco()
    {
        foreach (Transform child in containerRect.gameObject.transform)
        {
            Image img = child.transform.Find("Imagen Circle").GetComponent<Image>();
            img.color = Color.white;
        }
    }

    private void PonerBot�nSeleccionadoConC�rculoAmarillo(ButtonMesaController buttonSeleccionadoParaBorrar)
    {
        Debug.Log("Name button: " + buttonSeleccionadoParaBorrar.name);
        Image img = buttonSeleccionadoParaBorrar.transform.Find("Imagen Circle").GetComponent<Image>();
        PonerColorCorrectoAImg(img, "#FFE700");
    }

    public void PonerColorCorrectoAImg(Image img, string hexadecimal)
    {
        Color newColor;
        // Intento convertir el string hexadecimal a Color
        if (UnityEngine.ColorUtility.TryParseHtmlString(hexadecimal, out newColor))
        {
            img.color = newColor;
        }
        else
        {
            Debug.LogError("El formato del color hexadecimal es inv�lido.");
        }
    }

    // Mientras se arrastra, actualiza la posici�n y la limita al contenedor
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && rectTransform != null)
        {
            Vector2 newPos = rectTransform.anchoredPosition + eventData.delta;
            newPos = ClampToContainer(newPos);
            if (!WillOverlap(newPos))
            {
                rectTransform.anchoredPosition = newPos;
            }
        }
    }

    // Cambia el tama�o del bot�n al usar la rueda del mouse
    public void OnScroll(PointerEventData eventData)
    {
        if(rectTransform != null && containerRect != null)
        {
            float scaleChange = eventData.scrollDelta.y * 0.1f;
            Vector3 newScale = rectTransform.localScale + new Vector3(scaleChange, scaleChange, 0);

            // Defino los l�mites de escala
            newScale.x = Mathf.Clamp(newScale.x, 1f, 3f);
            newScale.y = Mathf.Clamp(newScale.y, 1f, 3f);
            newScale.z = 1;

            // Calculo el tama�o del bot�n con la nueva escala
            Vector2 newSize = rectTransform.rect.size * newScale;
            Vector2 newPos = rectTransform.anchoredPosition;

            // Verifico si cabe dentro del contenedor
            Vector2 halfContainer = containerRect.rect.size * 0.5f;
            Vector2 halfButton = newSize * 0.5f;

            bool fitsInside =
                newPos.x - halfButton.x >= -halfContainer.x &&
                newPos.x + halfButton.x <= halfContainer.x &&
                newPos.y - halfButton.y >= -halfContainer.y &&
                newPos.y + halfButton.y <= halfContainer.y;

            // Verifica si el nuevo tama�o causar�a superposici�n con otro bot�n
            bool overlapsWithOther = WillOverlapWithSize(newSize);

            // Si cabe dentro del contenedor y no se superpone con otro bot�n, aplico la nueva escala
            if (fitsInside && !overlapsWithOther)
            {
                rectTransform.localScale = newScale;
            }
        }
    }

    // M�todo para clamping: limita la posici�n del bot�n dentro de los l�mites del contenedor
    private Vector2 ClampToContainer(Vector2 pos)
    {
        if (containerRect == null || rectTransform == null)
            return pos;

        // Tama�o del contenedor (�rea disponible)
        Vector2 containerSize = containerRect.rect.size;
        // Tama�o del bot�n (considerando la escala actual)
        Vector2 buttonSize = rectTransform.rect.size * rectTransform.localScale;

        // Suponiendo que el pivote es (0.5, 0.5) para ambos, se calculan los m�rgenes:
        Vector2 halfContainer = containerSize * 0.5f;
        Vector2 halfButton = buttonSize * 0.5f;

        float minX = -halfContainer.x + halfButton.x;
        float maxX = halfContainer.x - halfButton.x;
        float minY = -halfContainer.y + halfButton.y;
        float maxY = halfContainer.y - halfButton.y;

        float clampedX = Mathf.Clamp(pos.x, minX, maxX);
        float clampedY = Mathf.Clamp(pos.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }

    // Comprueba si, en la nueva posici�n, este bot�n se superpondr�a con otro
    private bool WillOverlap(Vector2 newPos)
    {
        if (rectTransform == null)
            return false;

        // Calcula el rect�ngulo del bot�n en la posici�n nueva
        Vector2 mySize = rectTransform.rect.size * rectTransform.localScale;
        Rect myRect = new Rect(newPos - mySize / 2f, mySize);

        // Obtengo todos los ButtonMesaController hijos del contenedor
        ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();
        foreach (ButtonMesaController btn in buttons)
        {
            // Se ignora el bot�n actual
            if (btn == this || btn.rectTransform == null)
                continue;

            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;
            Rect otherRect = new Rect(btn.rectTransform.anchoredPosition - otherSize / 2f, otherSize);

            // Si se superponen, se retorna true
            if (myRect.Overlaps(otherRect))
                return true;
        }
        return false;
    }

    private bool WillOverlapWithSize(Vector2 newSize)
    {
        if (rectTransform == null)
            return false;

        // Calculo el rect�ngulo del bot�n con el nuevo tama�o
        Vector2 myPosition = rectTransform.anchoredPosition;
        Rect myRect = new Rect(myPosition - newSize / 2f, newSize);

        // Obtengo todos los botones dentro del contenedor
        ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();
        foreach (ButtonMesaController btn in buttons)
        {
            if (btn == this || btn.rectTransform == null)
                continue;

            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;
            Rect otherRect = new Rect(btn.rectTransform.anchoredPosition - otherSize / 2f, otherSize);

            // Si el nuevo tama�o se superpone con otro bot�n, retorna true
            if (myRect.Overlaps(otherRect))
                return true;
        }
        return false;
    }

    public void GestionarPapelera()
    {
        Debug.Log("He pulsado papelera; Bot�n marcado: " + buttonSeleccionadoParaBorrar.gameObject.name);

        string nombreBot�n = buttonSeleccionadoParaBorrar.gameObject.name;
        // Si el bot�n seleccionado para ser borrado no est� registrado en la BDD, lo eliino s�lo en la memoria.
        if (nombreBot�n.CompareTo("Button") == 0)
        {
            Destroy(buttonSeleccionadoParaBorrar.gameObject);
            StartCoroutine(instanceEditarRestauranteController.ActualizarIDMesasEnMapa());
        }
        else // El bot�n seleccionado para ser eliminado/borrado est� registrado en la BDD 
        {
            Debug.Log("Button con n�mero");

            string[] array = nombreBot�n.Split("-");
            Debug.Log("N�mero:" + array[1] + "*");
            int num = int.Parse(array[1]);
            instanceEditarRestauranteController.GestionarEliminarMesaEnBDDAsync(num);
        }

        // Una vez usada la papelera, la desactivo
        instanceEditarRestauranteController.DesactivarPapelera();
    }

    
}
