
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PlantCare.Models;

namespace PlantCare.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // INICIO

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("UsuarioId") != null)
        {
            return RedirectToAction("Index2");
        }

        return View("Index");
    }

    public IActionResult Index2()
    {
        return View("Index2");
    }

    // AGREGAR PLANTA

    [HttpGet]
    public IActionResult AgregarPlanta()
    {
        if (HttpContext.Session.GetString("UsuarioId") == null)
        {
            return RedirectToAction("Login");
        }

        return View("AgregarPlanta");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AgregarPlanta(Planta planta)
    {
        if (HttpContext.Session.GetString("UsuarioId") == null)
        {
            return RedirectToAction("Login");
        }

        if (planta == null)
        {
            ViewBag.Error = "La planta es obligatoria.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(planta.Nombre))
        {
            ViewBag.Error = "El nombre de la planta es obligatorio.";
            return View();
        }

        // Guardamos temporalmente la planta en sesión
        List<Planta> lista = ObtenerPlantas();

        planta.ID = lista.Count == 0 ? 1 : lista.Max(x => x.ID) + 1;

        planta.Descripción ??= "Planta agregada desde el formulario.";

        if (planta.TiempoRiego <= 0)
        {
            planta.TiempoRiego = 7;
        }

        lista.Add(planta);

        GuardarPlantas(lista);

        TempData["Success"] = $"{planta.Nombre} se agregó correctamente.";

        return RedirectToAction("MisPlantas");
    }

    // PLANTAS GUARDADAS EN SESIÓN

    public List<Planta> ObtenerPlantas()
    {
        string? json = HttpContext.Session.GetString("Plantas");

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Planta>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<Planta>>(json)
                   ?? new List<Planta>();
        }
        catch
        {
            return new List<Planta>();
        }
    }

    public void GuardarPlantas(List<Planta> plantas)
    {
        string json = JsonSerializer.Serialize(plantas);

        HttpContext.Session.SetString("Plantas", json);
    }

    // LOGIN

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("UsuarioId") != null)
        {
            return RedirectToAction("Index2");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(
        string? usuario,
        string? email,
        string? contraseña,
        string? password)
    {
        string? nombreUsuario = !string.IsNullOrWhiteSpace(usuario)
            ? usuario
            : email;

        string? clave = !string.IsNullOrWhiteSpace(contraseña)
            ? contraseña
            : password;

        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(clave))
        {
            ViewBag.Error = "Debe completar usuario y contraseña.";
            return View();
        }

        // Stored Procedure ObtenerUsuario
        Usuario usuarioLogueado = BD.ObtenerUsuario(nombreUsuario, clave);

        if (usuarioLogueado == null)
        {
            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        HttpContext.Session.SetString(
            "UsuarioId",
            usuarioLogueado.Id.ToString());

        HttpContext.Session.SetString(
            "UsuarioNombre",
            usuarioLogueado.NombreUsuario ??
            usuarioLogueado.UsuarioNombre ??
            usuarioLogueado.Nombre);

        return RedirectToAction("Index2");
    }

    // REGISTRO

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetString("UsuarioId") != null)
        {
            return RedirectToAction("Index2");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(Usuario usuario, string? confirmPassword)
    {
        if (usuario == null)
        {
            ViewBag.Error = "Debe completar todos los campos.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(usuario.Nombre) ||
            string.IsNullOrWhiteSpace(usuario.Apellido) ||
            string.IsNullOrWhiteSpace(usuario.Mail) ||
            string.IsNullOrWhiteSpace(usuario.UsuarioNombre) ||
            string.IsNullOrWhiteSpace(usuario.Contraseña))
        {
            ViewBag.Error = "Todos los campos son obligatorios.";
            return View();
        }

        if (!string.IsNullOrWhiteSpace(confirmPassword) &&
            usuario.Contraseña != confirmPassword)
        {
            ViewBag.Error = "Las contraseñas no coinciden.";
            return View();
        }

        // Stored Procedure ExisteUsuario
        if (BD.ExisteUsuario(usuario.UsuarioNombre, usuario.Mail))
        {
            ViewBag.Error = "Ya existe un usuario con ese nombre o correo.";
            return View();
        }

        if (!string.IsNullOrWhiteSpace(usuario.Teléfono) &&
            !long.TryParse(usuario.Teléfono, out _))
        {
            ViewBag.Error = "El teléfono tiene un formato inválido.";
            return View();
        }

        Usuario nuevoUsuario = new Usuario
        {
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Mail = usuario.Mail,
            UsuarioNombre = usuario.UsuarioNombre,
            Contraseña = usuario.Contraseña,
            Teléfono = usuario.Teléfono,
            Edad = usuario.Edad
        };

        // Stored Procedure RegistrarUsuario
        bool registroOk = BD.RegistrarUsuario(nuevoUsuario);

        if (!registroOk)
        {
            ViewBag.Error = "No se pudo registrar el usuario.";
            return View();
        }

        TempData["Success"] = "Registro exitoso. Iniciá sesión.";

        return RedirectToAction("Login");
    }

    // MIS PLANTAS

    public IActionResult MisPlantas(string? busqueda)
    {
        string? idUsuario = HttpContext.Session.GetString("UsuarioId");

        if (string.IsNullOrEmpty(idUsuario))
        {
            return RedirectToAction("Login");
        }

        // Stored Procedure ObtenerPlantasUsuario
        List<Planta> plantas = BD.ObtenerPlantasUsuario(int.Parse(idUsuario));

        // Sumamos las plantas que temporalmente están en sesión
        plantas.AddRange(ObtenerPlantas());

        // Buscador por nombre
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            plantas = plantas
                .Where(p => p.Nombre != null &&
                            p.Nombre.Contains(
                                busqueda,
                                StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        ViewBag.Plantas = plantas;

        return View("MisPlantas");
    }

    // OTRAS PÁGINAS

    public IActionResult Explorar()
    {
        return View("Explorar");
    }

    public IActionResult Explorar2()
    {
        return View("Explorar2");
    }

    public IActionResult Comunidad()
    {
        return View("Comunidad");
    }

    public IActionResult Guias()
    {
        return View("Guias");
    }

    public IActionResult Guias2()
    {
        return View("Guias2");
    }

    public IActionResult Nosotros()
    {
        return View("Nosotros");
    }

    public IActionResult Ayuda()
    {
        return View("Ayuda");
    }

    public IActionResult Contacto()
    {
        return View("Contacto");
    }

    public IActionResult Terminos()
    {
        return View("Terminos");
    }

    public IActionResult Consejos()
    {
        return View("Consejos");
    }

    public IActionResult Recordatorios()
    {
        return View("Recordatorios");
    }

    public IActionResult Perfil()
    {
        return View("Perfil");
    }

    // CERRAR SESIÓN

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    // PRIVACIDAD

    public IActionResult Privacy()
    {
        return View();
    }

    // ERROR

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ??
                        HttpContext.TraceIdentifier
        });
    }
}
