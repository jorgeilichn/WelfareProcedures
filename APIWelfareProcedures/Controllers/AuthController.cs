using System.Net;
using Microsoft.AspNetCore.Mvc;
using APIWelfareProcedures.Models;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace APIWelfareProcedures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RequestPostBodyParameters requestBody;
        private AuthSettings authSettings;
        private string post_uri, bearerToken;
        private HttpClient post_client, get_client;
        
        public AuthController(IOptionsSnapshot<RequestPostBodyParameters> configBody, 
            IOptionsSnapshot<AuthSettings> configAuth, 
            IHttpClientFactory clientFactory)
        {
            authSettings = configAuth.Value;
            post_uri = authSettings.post_uri;
            bearerToken = authSettings.bearerToken;
            requestBody = configBody.Value;
            post_client = clientFactory.CreateClient("welfare_client");
            get_client = clientFactory.CreateClient("welfare_client");
            
        }

        [HttpPost]
        [Route("getaccesstoken")]
        public async Task<ActionResult<ResponsePostParameters>> Post()
        {
            try
            {
                post_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                post_client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8 ");
                
                var serializedRequestBody = JsonConvert.SerializeObject(requestBody);
                var requestContent = new HttpRequestMessage(HttpMethod.Post, post_uri);
                requestContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                requestContent.Content = new StringContent(serializedRequestBody, Encoding.UTF8, "application/json");
                var result = await post_client.SendAsync(requestContent);
                if (result.StatusCode == HttpStatusCode.NotFound )
                {
                    return NotFound(result.RequestMessage.RequestUri.ToString());
                }
                else if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    return BadRequest(result.RequestMessage.RequestUri.ToString());
                }
                else if (!result.IsSuccessStatusCode)
                {
                    return BadRequest(result.RequestMessage.RequestUri.ToString());
                }

                var jsonResult = await result.Content.ReadAsStringAsync();
                ResponsePostParameters response = JsonConvert.DeserializeObject<ResponsePostParameters>(jsonResult);
                return Ok(response);
            }
            catch ( HttpRequestException ex)
            {
                throw new Exception(ex.Message); 
            }
            
        }
    }
}