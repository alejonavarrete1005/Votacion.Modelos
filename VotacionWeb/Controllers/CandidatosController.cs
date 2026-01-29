using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Votacion.Modelos.DTOs;

public class CandidatosController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CandidatosController(IHttpClientFactory httpClientFactory)
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

        var rolClaim = jwt.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return rolClaim switch
        {
            "Administrador" => 1,
            "Votante" => 2,
            "Candidato" => 3,
            _ => 0
        };
    }

    // INDEX

    public async Task<IActionResult> Index(int eleccionId)
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("VotacionAPI");
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

        // Rol del usuario (para la vista)
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;
        ViewBag.EsAdmin = rol == "1";

        ViewBag.EleccionId = eleccionId;

        return View(candidatos!);
    }

    // CREATE (GET)

    public IActionResult Create(int eleccionId)
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        // Solo ADMIN (rol 1)
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;
        if (rol != "1")
            return Unauthorized();

        ViewBag.EleccionId = eleccionId;
        return View();
    }

    // CREATE (POST)

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CandidatoDTO dto)
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        //  Solo ADMIN (rol 1)
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;
        if (rol != "1")
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            ViewBag.EleccionId = dto.EleccionId;
            return View(dto);
        }

        var client = _httpClientFactory.CreateClient("VotacionAPI");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/Candidatos", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "No se pudo crear el candidato");
            ViewBag.EleccionId = dto.EleccionId;
            return View(dto);
        }

        return RedirectToAction(nameof(Index), new { eleccionId = dto.EleccionId });
    }
}

