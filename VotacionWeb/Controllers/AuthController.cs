using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Votacion.Modelos.DTOs;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
   
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Debe ingresar correo y contraseña";
            return View();
        }

        var client = _httpClientFactory.CreateClient("VotacionAPI");

        var payload = new
        {
            email = email,
            password = password
        };

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("api/Auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

        // Obtener token
        var token = data.GetProperty("token").GetString();

        if (string.IsNullOrEmpty(token))
        {
            ViewBag.Error = "Error al generar el token";
            return View();
        }

        //  Guardar token en sesión
        HttpContext.Session.SetString("JWT", token);

        // (opcional) guardar datos del usuario
        HttpContext.Session.SetString("UsuarioNombre",
            data.GetProperty("nombreCompleto").GetString() ?? "");

        HttpContext.Session.SetString("UsuarioRol",
            data.GetProperty("rol").GetInt32().ToString());

        return RedirectToAction("Index", "Elecciones");
    }

    [HttpPost]
    public async Task<IActionResult> Register(UsuarioRegisterDTO dto)
    {
        var client = _httpClientFactory.CreateClient("VotacionAPI");

        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/Auth/register", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "No se pudo registrar el usuario";
            return View(dto);
        }

        // Luego de registrar → ir al login
        return RedirectToAction("Login");
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

}

