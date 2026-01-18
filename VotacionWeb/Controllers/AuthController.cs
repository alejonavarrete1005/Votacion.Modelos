using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

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

    // POST: /Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var client = _httpClientFactory.CreateClient("VotacionAPI");

        var json = JsonSerializer.Serialize(new
        {
            email,
            password
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/Auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

        var token = data.GetProperty("token").GetString();

        // Guardar token en sesión
        HttpContext.Session.SetString("JWT", token!);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}

