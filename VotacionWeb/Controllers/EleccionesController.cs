using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
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

    // GET: Elecciones/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Elecciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EleccionDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/Elecciones", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "No se pudo crear la elección");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Elecciones/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"api/Elecciones/{id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Index));

        var json = await response.Content.ReadAsStringAsync();
        var eleccion = JsonSerializer.Deserialize<EleccionDTO>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return View(eleccion);
    }

    // POST: Elecciones/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await client.DeleteAsync($"api/Elecciones/{id}");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Candidatos(int id)
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var candidatosResponse =
            await client.GetAsync($"api/Elecciones/{id}/candidatos");

        if (!candidatosResponse.IsSuccessStatusCode)
            return View(new List<CandidatoDTO>());

        var candidatosJson = await candidatosResponse.Content.ReadAsStringAsync();

        var candidatos = JsonSerializer.Deserialize<List<CandidatoDTO>>(
            candidatosJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

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
