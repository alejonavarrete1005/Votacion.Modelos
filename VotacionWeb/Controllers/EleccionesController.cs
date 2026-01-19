using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Votacion.Modelos.DTOs;

public class EleccionesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EleccionesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/Elecciones");

        if (!response.IsSuccessStatusCode)
            return View(new List<EleccionDTO>());

        var json = await response.Content.ReadAsStringAsync();

        var elecciones = JsonSerializer.Deserialize<List<EleccionDTO>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return View(elecciones);
    }
}

