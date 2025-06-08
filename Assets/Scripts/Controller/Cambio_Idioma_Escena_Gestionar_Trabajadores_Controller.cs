using Assets.Scripts.Model;
using TMPro;
using UnityEngine;

public class Cambio_Idioma_Escena_Gestionar_Trabajadores_Controller : MonoBehaviour
{
    // Textos en canvas Gestionar Trabajadores
    [SerializeField] private TMP_Text textGestTrab;
    [SerializeField] private TMP_Text textRegistrarYA�adirGestTrab;
    [SerializeField] private TMP_Text textNombreGestTrab;
    [SerializeField] private TMP_Text textRolGestTrab;
    [SerializeField] private TMP_Text textGuardarGestTrab;
    [SerializeField] private TMP_Text textEliminarGestTrab;

    [SerializeField] private TMP_Text textAdvertenciaContenedorAdvertenciaCambiarDeGerente;
    [SerializeField] private TMP_Text textTextoEntrePar�ntesisContenedorAdvertenciaCambiarDeGerente;
    [SerializeField] private TMP_Text textBtnCancelarContenedorAdvertenciaCambiarDeGerente;
    [SerializeField] private TMP_Text textBtnConfirmarContenedorAdvertenciaCambiarDeGerente;

    [SerializeField] private TMP_Text textAdvertenciaContenedorAdvertenciaEliminarTrabajador;
    [SerializeField] private TMP_Text textBtnCancelarContenedorAdvertenciaEliminarTrabajador;
    [SerializeField] private TMP_Text textBtnConfirmarContenedorAdvertenciaEliminarTrabajador;

    [SerializeField] private TMP_Text textErrorContenedorErrorAlGuardarTrabajador;
    [SerializeField] private TMP_Text textBtnOkayContenedorErrorAlGuardarTrabajador;


    // Textos en canvas Registrar Usuario
    [SerializeField] private TMP_Text textEncabezadoRegistrarUsuario;
    [SerializeField] private TMP_Text textNombreRegistrarUsuario;
    [SerializeField] private TMP_Text textContrase�aRegistrarUsuario;
    [SerializeField] private TMP_Text textRepetirContrase�aRegistrarUsuario;
    [SerializeField] private TMP_Text textBtnConfirmarRegistrarUsuario;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldNombreRegistrarUsuario;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldPasswordRegistrarUsuario;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldRepeatPasswordRegistrarUsuario;


    // Start is called before the first frame update
    void Start()
    {
        PonerTextosEnIdiomaCorrecto();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PonerTextosEnIdiomaCorrecto()
    {
        switch (Usuario.Idioma)
        {
            case "Espa�ol":
                // Textos en canvas Gestionar Trabajadores
                textGestTrab.text = "GESTIONAR TRABAJADORES";
                textRegistrarYA�adirGestTrab.text = "Registrar y A�adir";
                textNombreGestTrab.text = "Nombre";
                textRolGestTrab.text = "Rol";
                textGuardarGestTrab.text = "Guardar";
                textEliminarGestTrab.text = "Eliminar";

                textAdvertenciaContenedorAdvertenciaCambiarDeGerente.text = "�ADVERTENCIA!";
                textTextoEntrePar�ntesisContenedorAdvertenciaCambiarDeGerente.text = "(Este cambio se guardar� autom�ticamente)";
                textBtnCancelarContenedorAdvertenciaCambiarDeGerente.text = "Cancelar";
                textBtnConfirmarContenedorAdvertenciaCambiarDeGerente.text = "Confirmar";

                textAdvertenciaContenedorAdvertenciaEliminarTrabajador.text = "�ADVERTENCIA!";
                textBtnCancelarContenedorAdvertenciaEliminarTrabajador.text = "Cancelar";
                textBtnConfirmarContenedorAdvertenciaEliminarTrabajador.text = "Confirmar";

                textErrorContenedorErrorAlGuardarTrabajador.text = "Error";
                textBtnOkayContenedorErrorAlGuardarTrabajador.text = "Okay";

                // Textos en canvas Registrar Usuario
                textEncabezadoRegistrarUsuario.text = "REGISTRAR Y A�ADIR TRABAJADOR";
                textNombreRegistrarUsuario.text = "Nombre:";
                textContrase�aRegistrarUsuario.text = "Contrase�a:";
                textRepetirContrase�aRegistrarUsuario.text = "Confirmar contrase�a:";
                textBtnConfirmarRegistrarUsuario.text = "Confirmar";
                textPlaceHolderInputFieldNombreRegistrarUsuario.text = " M�nimo 3 caracteres...";
                textPlaceHolderInputFieldPasswordRegistrarUsuario.text = " M�nimo 6 caracteres...";
                textPlaceHolderInputFieldRepeatPasswordRegistrarUsuario.text = " Repetir contrase�a...";
                

                break;

            case "English":
                // Textos en canvas Gestionar Trabajadores
                textGestTrab.text = "MANAGE WORKERS";
                textRegistrarYA�adirGestTrab.text = "Register and Add";
                textNombreGestTrab.text = "Name";
                textRolGestTrab.text = "Rol";
                textGuardarGestTrab.text = "Save";
                textEliminarGestTrab.text = "Delete";

                textAdvertenciaContenedorAdvertenciaCambiarDeGerente.text = "WARNING!";
                textTextoEntrePar�ntesisContenedorAdvertenciaCambiarDeGerente.text = "(This change will be saved automatically)";
                textBtnCancelarContenedorAdvertenciaCambiarDeGerente.text = "Cancel";
                textBtnConfirmarContenedorAdvertenciaCambiarDeGerente.text = "Confirm";

                textAdvertenciaContenedorAdvertenciaEliminarTrabajador.text = "WARNING!";
                textBtnCancelarContenedorAdvertenciaEliminarTrabajador.text = "Cancel";
                textBtnConfirmarContenedorAdvertenciaEliminarTrabajador.text = "Confirm";

                textErrorContenedorErrorAlGuardarTrabajador.text = "Error";
                textBtnOkayContenedorErrorAlGuardarTrabajador.text = "Okay";

                // Textos en canvas Registrar Usuario
                textEncabezadoRegistrarUsuario.text = "REGISTER AND ADD WORKER";
                textNombreRegistrarUsuario.text = "Name:";
                textContrase�aRegistrarUsuario.text = "Password:";
                textRepetirContrase�aRegistrarUsuario.text = "Confirm password:";
                textBtnConfirmarRegistrarUsuario.text = "Confirm";
                textPlaceHolderInputFieldNombreRegistrarUsuario.text = " Minimum 3 characters...";
                textPlaceHolderInputFieldPasswordRegistrarUsuario.text = " Minimum 6 characters...";
                textPlaceHolderInputFieldRepeatPasswordRegistrarUsuario.text = " Repeat password...";

                break;
        }

    }
}
