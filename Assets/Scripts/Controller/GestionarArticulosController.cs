using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Buffers.Text;
using System.IO;
using UnityEngine.Networking;
//SI ES POSIBLE, PANTALLA PARA AÑADIR ARTÍCULOS A LA BASE DE DATOS
//IMÁGENES
public class GestionarArticulosController : MonoBehaviour
{
    public GameObject ventanaModificar;
    public GameObject botonModificar;
    public GameObject botonAdd;
    public GameObject canvasArticulos;
    public RectTransform espacio;
    public Scrollbar scrollbarLista;
    public TMP_InputField selNombre;//usar contains
    public GameObject baseA;
    public MétodosAPIController instanceMétodosAPIController;
    public TMP_InputField setNombre;
    public TMP_InputField setPrecio;
    public TMP_Dropdown setCategoria;
    public TextMeshProUGUI AID;
    public TextMeshProUGUI tBoton;
    public Articulo articuloSeleccionado;
    public RawImage imagenRAW;
    public Sprite imagenPrueba;
    public ImageManager instanceImage;
    //public ImageDownloader instanceDownloader;
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
        instanceImage = ImageManager.instanceImage;
        //instanceDownloader=ImageDownloader.instanceDownloader;
        //instanceDownloader.DownloadImage();
        cargarLista();
    }
    void Awake()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
        //instanceDownloader = ImageDownloader.instanceDownloader;
    }
    public void volver()
    {
        SceneManager.LoadScene("Main");
    }
    public void cerrarVentana()
    {
        ventanaModificar.SetActive(false);
        setNombre.text = "";
        setPrecio.text = "";
        setCategoria.value = 0;
        AID.text = "";
        botonAdd.SetActive(false);
        botonModificar.SetActive(false);
        cargarLista();
    }
    public void abrirVentanaAdd()
    {
        articuloSeleccionado = null;
        ventanaModificar.SetActive(true);
        botonAdd.SetActive(true);
        imagenRAW.gameObject.SetActive(false);
    }
    public void abrirVentanaModificar(Articulo a)
    {
        setNombre.text = ""+a.nombre;
        setPrecio.text = ""+a.precio;
        if (a.categoria.ToUpper().Equals("ENTRANTES"))
        {
            setCategoria.value = 0;
        }
        if (a.categoria.ToUpper().Equals("PLATOS"))
        {
            setCategoria.value = 1;
        }
        if (a.categoria.ToUpper().Equals("BEBIDAS"))
        {
            setCategoria.value = 2;
        }
        else
        {
            setCategoria.value = 3;
        }
        AID.text = "Articulo:"+a.id;
        articuloSeleccionado = a;
        imagenRAW.gameObject.SetActive(true);
        cargarImagen();
        ventanaModificar.SetActive(true);
        botonModificar.SetActive(true);
    }
    public async void cargarLista()
    {
        await cargarLista("");
    }
    public async Task cargarLista(string nombre)
    {
        scrollbarLista.value = 1;
        foreach (Transform t in espacio.transform)
        {
            Destroy(t.gameObject);
            Debug.Log("Se ha eliminado: " + t);
        }
        //LimpiarBotones();
        List<Articulo> lista;

        string cad = await instanceMétodosAPIController.GetDataAsync("articulo/obtenerTodos/");
        Debug.Log("CADENA:" + cad);
        lista = JsonConvert.DeserializeObject<List<Articulo>>(cad);
        int i = 0;
        espacio.sizeDelta = new Vector2(1570, 150 * lista.Count);
        foreach (Articulo a in lista)
        {
            if (nombre == null || a.nombre.ToUpper().Contains(nombre.ToUpper()))
            {
                crearBoton(a, i);
                Debug.Log("Creado botón: " + a.id);
                i++;
            }
        }
    }

    public void mostrarTBoton()
    {
        tBoton.gameObject.SetActive(true);
    }
    public void ocultarTBoton()
    {
        tBoton.gameObject.SetActive(false);
    }

    public void crearBoton(Articulo a, int num)
    {
        GameObject botonA = Instantiate(baseA, espacio, true);
        botonA.transform.position = new Vector2(950, 820 - num * 150);
        botonA.transform.SetParent(espacio);
        //botonA.AddComponent<CanvasRenderer>();
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        botonA.name = "Articulo-" + a.id;
        GameObject nom = botonA.transform.Find("Nombre").gameObject;
        TextMeshProUGUI textoN = nom.GetComponent<TextMeshProUGUI>();
        textoN.text = a.nombre;
        GameObject pre = botonA.transform.Find("Precio").gameObject;
        TextMeshProUGUI textoP = pre.GetComponent<TextMeshProUGUI>();
        textoP.text = ""+a.precio;
        GameObject cat = botonA.transform.Find("Categoria").gameObject;
        TextMeshProUGUI textoC = cat.GetComponent<TextMeshProUGUI>();
        textoC.text = a.categoria;
        //Categoría debería ser un combobox EN EL PANEL DE EDITAR, AQUÍ TIENE QUE SER UN TEXTO MÁS
        // Tal vez montar una tabla con índices (es decir crear un encabezado donde se escriban las columnas)
        /*if (Usuario.Idioma.CompareTo("Espa�ol") == 0)
        {
            texto.text = "Pedido " + p.id;
            textoF.text = "Mesa " + instanceGestionarMesasController.getNumMesa(p.mesa);
        }
        else
        {
            texto.text = "Order " + p.id;
            textoF.text = "Table " + instanceGestionarMesasController.getNumMesa(p.mesa);
        }*/
        GameObject modificar = botonA.transform.Find("Modificar").gameObject;
        Button mod = modificar.AddComponent<Button>();
        mod.onClick.AddListener(() => abrirVentanaModificar(a));
        GameObject eliminar = botonA.transform.Find("Eliminar").gameObject;
        Button del = eliminar.AddComponent<Button>();
        del.onClick.AddListener(() => eliminarArticulo(a.id));


        GameObject modIMG = modificar.transform.Find("Image").gameObject;
        GameObject modTexto = modIMG.transform.Find("Text (TMP)").gameObject;
        TextMeshProUGUI textoMod = modTexto.GetComponent<TextMeshProUGUI>();
        GameObject delIMG = eliminar.transform.Find("Image").gameObject;
        GameObject delTexto = delIMG.transform.Find("Text (TMP)").gameObject;
        TextMeshProUGUI textoDel = delTexto.GetComponent<TextMeshProUGUI>();
        if (Usuario.Idioma.CompareTo("Español") != 0)
        {
            Debug.Log("Debería traducirse");
            textoMod.text = "Modify";
            textoDel.text = "Delete";
        }
        else
        {
            textoMod.text = "Modificar";
            textoDel.text = "Eliminar";
        }
    }
    public void crear()
    {
        crearArticulo();
    }
    public void modificar()
    {
        modificarArticulo();
    }
    public async Task crearArticulo()
    {
        string cad = await instanceMétodosAPIController.GetDataAsync("articulo/maxID/");
        int IDA = JsonConvert.DeserializeObject<int>(cad)+1;
        string nombre = setNombre.text;
        float precio = float.Parse(setPrecio.text);
        string categoria = setCategoria.options[setCategoria.value].text;
        Articulo a = new Articulo(IDA, nombre, precio, categoria);
        string cad2 = await instanceMétodosAPIController.PostDataAsync("articulo/crearArticulo/",a);
        Debug.Log(cad2);
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad2);
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Estado cambiado correctamente");
            articuloSeleccionado = null;
        }
        else
        {
            Debug.Log("Error al finalizar factura");
        }
        ventanaModificar.SetActive(false);
        cargarLista();
    }
    public async Task modificarArticulo()
    {
        int IDA = articuloSeleccionado.id;
        string nombre = setNombre.text;
        float precio = float.Parse(setPrecio.text);
        string categoria = setCategoria.options[setCategoria.value].text;
        Articulo a = new Articulo(IDA, nombre, precio, categoria);
        string cad = await instanceMétodosAPIController.PutDataAsync("articulo/modificar/", a);
        Debug.Log(cad);
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Estado cambiado correctamente");
        }
        else
        {
            Debug.Log("Error al finalizar factura");
        }
        ventanaModificar.SetActive(false);
        cargarLista();
    }
    public async Task crearArticulo(Articulo a)
    {
        string cad = await instanceMétodosAPIController.PutDataAsync("articulo/crearArticulo/", a);
        Debug.Log(cad);
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Estado cambiado correctamente");
        }
        else
        {
            Debug.Log("Error al finalizar factura");
        }
    }
    public async Task modificarArticulo(Articulo a)
    {
        string cad = await instanceMétodosAPIController.PutDataAsync("articulo/modificar/",a);
        Debug.Log(cad);
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Estado cambiado correctamente");
        }
        else
        {
            Debug.Log("Error al finalizar factura");
        }
    }
    public async Task eliminarArticulo(int idA)
    {
        string cad = await instanceMétodosAPIController.DeleteDataAsync("articulo/borrar/" + idA);
        Debug.Log(cad);
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Estado cambiado correctamente");
        }
        else
        {
            Debug.Log("Error al finalizar factura");
        }
        cargarLista();
    }
    public void filtrarArticulos()
    {
        cargarLista(selNombre.text);
    }

    public void cargarImagen()
    {
        cargarImagenAux();
    }
    public async Task cargarImagenAux()
    {
        try
        {
            instanceImage.imageDisplay = imagenRAW;
            string cad = await instanceMétodosAPIController.GetDataAsync("articulo/tieneImagen/"+articuloSeleccionado.id);
            Debug.Log(cad);
            bool resultado = JsonConvert.DeserializeObject<bool>(cad);
            if (!resultado) throw new Exception("No tiene imagen");
            instanceImage.DownloadImage(articuloSeleccionado.id);
        }
        catch (Exception e)
        {
            Debug.Log("No se encuentra el artículo");
            imagenRAW.texture = imagenPrueba.texture;
        }
    }
    public void subirImagen()
    {
        instanceImage.OpenFileAndUpload(articuloSeleccionado.id);
        cargarImagenAux();
    }
}
