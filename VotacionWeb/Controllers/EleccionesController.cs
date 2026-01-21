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
    public async Task<IActionResult> Candidatos(int id)
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // 1 Obtener candidatos de la elección
        var candidatosResponse = await client.GetAsync($"api/Elecciones/{id}/candidatos");

        if (!candidatosResponse.IsSuccessStatusCode)
            return View(new List<CandidatoDTO>());

        var candidatosJson = await candidatosResponse.Content.ReadAsStringAsync();

        var candidatos = JsonSerializer.Deserialize<List<CandidatoDTO>>(
            candidatosJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // 2️ Verificar si ya votó en esta elección
        var yaVotoResponse = await client.GetAsync($"api/Votos/ya-voto/{id}");

        bool yaVoto = false;

        if (yaVotoResponse.IsSuccessStatusCode)
        {
            var yaVotoJson = await yaVotoResponse.Content.ReadAsStringAsync();
            yaVoto = JsonSerializer.Deserialize<bool>(yaVotoJson);
        }

        ViewBag.YaVoto = yaVoto;
        ViewBag.EleccionId = id;

        return View(candidatos);
    }
}

