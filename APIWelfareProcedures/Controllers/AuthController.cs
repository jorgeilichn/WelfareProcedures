using Microsoft.AspNetCore.Mvc;
using APIWelfareProcedures.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace APIWelfareProcedures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RequestPostBodyParameters requestBody;
        private string post_uri, bearerToken;
        
        public AuthController(IConfiguration config)
        {
            post_uri = config.GetSection("AuthSettings").GetSection("post_uri").ToString();
            bearerToken = config.GetSection("AuthSettings").GetSection("bearerToken").ToString();
            requestBody = new RequestPostBodyParameters()
            {
                grant_type = config.GetSection("RequestPostBodyParameters").GetSection("grant_type").ToString(),
                client_id = config.GetSection("RequestPostBodyParameters").GetSection("client_id").ToString(),
                client_secret = config.GetSection("RequestPostBodyParameters").GetSection("client_secret").ToString(),
                scope = config.GetSection("RequestPostBodyParameters").GetSection("scope").ToString(),
                username = config.GetSection("RequestPostBodyParameters").GetSection("username").ToString(),
                password = config.GetSection("RequestPostBodyParameters").GetSection("password").ToString(),
            };
        }

        [HttpPost]
        [Route("getaccesstoken")]
        public async Task<ActionResult<ApiResponse>> Post()
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent jsonContent = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                HttpResponseMessage result = await client.PostAsync(post_uri, jsonContent);
                if (!result.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Algo ha ido mal...");
                }
    
                string jsonResult = await result.Content.ReadAsStringAsync();
                ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(jsonResult);
                return Ok(response);
            }
        }
    }
}