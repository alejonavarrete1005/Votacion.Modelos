using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Votacion.Modelos.DTOs;

public class CandidatosController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CandidatosController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index(int eleccionId)
    {
        var client = _httpClientFactory.CreateClient("VotacionAPI");

        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"api/Candidatos/eleccion/{eleccionId}");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "No se pudieron cargar los candidatos";
            return View(new List<CandidatoDTO>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var candidatos = JsonSerializer.Deserialize<List<CandidatoDTO>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        ViewBag.EleccionId = eleccionId;

        return View(candidatos!);
    }
}
