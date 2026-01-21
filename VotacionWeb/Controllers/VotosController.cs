using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace VotacionWeb.Controllers
{
    public class VotosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VotosController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Votar(int candidatoId)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("VotacionAPI");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(new
            {
                candidatoId
            });

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/Votos", content);

            if (!response.IsSuccessStatusCode)
            {
                var apiError = await response.Content.ReadAsStringAsync();
                TempData["Error"] = string.IsNullOrEmpty(apiError)
                    ? "No se pudo registrar el voto"
                    : apiError;

                return RedirectToAction("Index", "Elecciones");
            }

            TempData["Success"] = "Voto registrado correctamente";
            return RedirectToAction("Index", "Elecciones");
        }
    }
}
