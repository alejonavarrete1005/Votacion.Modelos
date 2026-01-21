using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Votacion.Modelos.DTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


public class EleccionesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EleccionesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private int ObtenerRolDesdeToken()
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return 0;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var rolClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return rolClaim switch
        {
            "Administrador" => 1,
            "Votante" => 2,
            "Candidato" => 3,
            _ => 0
        };
    }


    private bool EsAdministrador()
    {
        return ObtenerRolDesdeToken() == 1;
    }

    private HttpClient ClienteConToken()
    {
        var token = HttpContext.Session.GetString("JWT");
        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
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

        ViewBag.Rol = ObtenerRolDesdeToken();

        return View(elecciones);
    }


    public async Task<IActionResult> Details(int id)
    {
        var client = ClienteConToken();
        var response = await client.GetAsync($"api/Elecciones/{id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Index));

        var json = await response.Content.ReadAsStringAsync();
        var eleccion = JsonSerializer.Deserialize<EleccionDTO>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(eleccion);
    }

  
    // CREATE (SOLO ADMIN)
    public IActionResult Create()
    {
        if (!EsAdministrador())
            return Unauthorized();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EleccionDTO dto)
    {
        if (!EsAdministrador())
            return Unauthorized();

        if (!ModelState.IsValid)
            return View(dto);

        var client = ClienteConToken();
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

    // EDIT (SOLO ADMIN)
    public async Task<IActionResult> Edit(int id)
    {
        if (!EsAdministrador())
            return Unauthorized();

        var client = ClienteConToken();
        var response = await client.GetAsync($"api/Elecciones/{id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Index));

        var json = await response.Content.ReadAsStringAsync();
        var eleccion = JsonSerializer.Deserialize<EleccionDTO>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(eleccion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EleccionDTO dto)
    {
        if (!EsAdministrador())
            return Unauthorized();

        if (!ModelState.IsValid)
            return View(dto);

        var client = ClienteConToken();
        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await client.PutAsync($"api/Elecciones/{id}", content);
        return RedirectToAction(nameof(Index));
    }
    
    // DELETE (SOLO ADMIN)
    public async Task<IActionResult> Delete(int id)
    {
        if (!EsAdministrador())
            return Unauthorized();

        var client = ClienteConToken();
        var response = await client.GetAsync($"api/Elecciones/{id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Index));

        var json = await response.Content.ReadAsStringAsync();
        var eleccion = JsonSerializer.Deserialize<EleccionDTO>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(eleccion);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!EsAdministrador())
            return Unauthorized();

        var client = ClienteConToken();
        await client.DeleteAsync($"api/Elecciones/{id}");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Candidatos(int id)
    {
        var client = ClienteConToken();

        var candidatosResponse =
            await client.GetAsync($"api/Elecciones/{id}/candidatos");

        if (!candidatosResponse.IsSuccessStatusCode)
            return View(new List<CandidatoDTO>());

        var candidatosJson = await candidatosResponse.Content.ReadAsStringAsync();
        var candidatos = JsonSerializer.Deserialize<List<CandidatoDTO>>(
            candidatosJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var yaVotoResponse = await client.GetAsync($"api/Votos/ya-voto/{id}");
        bool yaVoto = false;

        if (yaVotoResponse.IsSuccessStatusCode)
        {
            var json = await yaVotoResponse.Content.ReadAsStringAsync();
            yaVoto = JsonSerializer.Deserialize<bool>(json);
        }

        ViewBag.YaVoto = yaVoto;
        ViewBag.EleccionId = id;

        return View(candidatos);
    }
}
