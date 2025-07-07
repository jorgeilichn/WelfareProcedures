using System.Net;
using System.Net.Http.Headers;
using System.Text;
using APIWelfareProcedures.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace APIWelfareProcedures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RequestPostBodyParameters requestBody; 
        private AuthSettings authSettings;
        private HttpClient post_client, get_client;
        private string accessToken = string.Empty;
        
        public AuthController(IOptionsSnapshot<RequestPostBodyParameters> configBody, 
            IOptionsSnapshot<AuthSettings> configAuth, 
            IHttpClientFactory clientFactory)
        {
            authSettings = configAuth.Value;
            requestBody = configBody.Value;
            post_client = clientFactory.CreateClient("welfare_client");
            get_client = clientFactory.CreateClient("welfare_client");
            
        }

        [HttpPost]
        [Route("getaccesstoken")]
        public async Task<ActionResult<ResponsePostParameters>> Post()
        {
            string post_uri = authSettings.post_uri;
            string bearerToken = authSettings.bearerToken;
            post_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            post_client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8 ");
                
            var serializedRequestBody = JsonConvert.SerializeObject(requestBody);
            var requestContent = new HttpRequestMessage(HttpMethod.Post, post_uri);
            requestContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            requestContent.Content = new StringContent(serializedRequestBody, Encoding.UTF8, "application/json");
            var result = await post_client.SendAsync(requestContent);
            if (result.IsSuccessStatusCode)
            {
                var jsonResult = await result.Content.ReadAsStringAsync();
                ResponsePostParameters response = JsonConvert.DeserializeObject<ResponsePostParameters>(jsonResult);
                this.accessToken = response.access_token;
                return Ok(response);
            }
            else
            {
                return BadRequest(result.ReasonPhrase + ": " + result.RequestMessage.RequestUri.ToString());
            }
        }
        
        [HttpGet]
        [Route("getprocedures")]
        public async Task<ActionResult<ResponseGetParameter>> Get([FromQuery]int start, [FromQuery]int limit)
        {
            string get_uri = authSettings.get_uri;
            //string base_uri = authSettings.base_uri + "/";
            //string url = base_uri + get_uri + "?start=" + start + "&limit=" + limit;
            get_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //get_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.accessToken);
            get_client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8 ");
            var requestContent = new HttpRequestMessage(HttpMethod.Get, get_uri);
            requestContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.accessToken); ;
            var result = await get_client.SendAsync(requestContent);
            //var response = await get_client.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                ResponseGetParameter response = JsonConvert.DeserializeObject<ResponseGetParameter>(responseContent);
                return Ok(response);
            }
            else
            {
                return BadRequest(result.ReasonPhrase + ": " + result.ReasonPhrase);
            }
        }
    }
}