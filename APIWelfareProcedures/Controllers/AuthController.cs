﻿using System.Net;
using System.Net.Http.Headers;
using System.Text;
using APIWelfareProcedures.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;

namespace APIWelfareProcedures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RequestPostBodyParameters requestBody; 
        private AuthSettings authSettings;
        private HttpClient post_client, get_client;
        private string accessToken = String.Empty;
        protected APIResponse _response;
        
        public AuthController(IOptionsSnapshot<RequestPostBodyParameters> configBody, 
            IOptionsSnapshot<AuthSettings> configAuth, 
            IHttpClientFactory clientFactory)
        {
            authSettings = configAuth.Value;
            requestBody = configBody.Value;
            post_client = clientFactory.CreateClient("welfare_client");
            get_client = clientFactory.CreateClient("welfare_client");
            _response = new APIResponse();
        }

        private void ConfigRequestHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8 ");
        }

        [HttpPost]
        [Route("getaccesstoken")]
        public async Task<ActionResult<APIResponse>> Post()
        {
            string post_uri = authSettings.post_uri;
            string bearerToken = authSettings.bearerToken;
            ConfigRequestHeaders(post_client);
            var serializedRequestBody = JsonConvert.SerializeObject(requestBody);
            var requestContent = new HttpRequestMessage(HttpMethod.Post, post_uri);
            requestContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            requestContent.Content = new StringContent(serializedRequestBody, Encoding.UTF8, "application/json");
            var result = await post_client.SendAsync(requestContent);
            if (result.IsSuccessStatusCode)
            {
                var jsonResult = await result.Content.ReadAsStringAsync();
                ResponsePostParameters response = JsonConvert.DeserializeObject<ResponsePostParameters>(jsonResult);
                accessToken = response.access_token;
                _response.statusCode = HttpStatusCode.OK;
                _response.Result = response;
                return Ok(_response);
            }
            else
            {
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Result = null;
                _response.ErrorMessages.Add("Error: " + result.ReasonPhrase);
                _response.ErrorMessages.Add("URL: " + result.RequestMessage.RequestUri);
                                            
                return BadRequest(_response);
            }
        }
        
        [HttpGet]
        [Route("getprocedures")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery]int start, [FromQuery]int limit)
        {
            if (limit <= 0 || limit < start || start < 0)
            {
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Result = null;
                _response.ErrorMessages.Add("Error: El valor de los parámetros start y limit tiene restricciones");
                _response.ErrorMessages.Add("limit mayor que cero");
                _response.ErrorMessages.Add("limit < start");
                _response.ErrorMessages.Add("start >= cero");
                return BadRequest(_response);
            }
            string get_uri = authSettings.get_uri+ "?start=" + start + "&limit=" + limit;
            ConfigRequestHeaders(get_client);
            var requestContent = new HttpRequestMessage(HttpMethod.Get, get_uri);
            requestContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); ;
            var result = await get_client.SendAsync(requestContent);
            if (result.IsSuccessStatusCode)
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                ResponseGetParameters response = JsonConvert.DeserializeObject<ResponseGetParameters>(responseContent);
                _response.statusCode = HttpStatusCode.OK;
                _response.Result = response;
                return Ok(_response);
            }
            else
            {
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Result = null;
                _response.ErrorMessages.Add("Error: " + result.ReasonPhrase);
                _response.ErrorMessages.Add("URL: " + result.RequestMessage.RequestUri);
                return BadRequest(_response);
            }
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetWelfareProcedures([FromQuery] int start, int limit)
        {
            if (limit <= 0 || limit < start || start < 0)
            {
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Result = null;
                _response.ErrorMessages.Add("Error: El valor de los parámetros start y limit tiene restricciones");
                _response.ErrorMessages.Add("limit mayor que cero");
                _response.ErrorMessages.Add("limit < start");
                _response.ErrorMessages.Add("start >= cero");
                return BadRequest(_response);
            }
            string post_uri = authSettings.post_uri;
            string bearerToken = authSettings.bearerToken;
            ConfigRequestHeaders(post_client);
            var serializedRequestBody = JsonConvert.SerializeObject(requestBody);
            var requestPostContent = new HttpRequestMessage(HttpMethod.Post, post_uri);
            requestPostContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            requestPostContent.Content = new StringContent(serializedRequestBody, Encoding.UTF8, "application/json");
            var postResult = await post_client.SendAsync(requestPostContent);
            if (postResult.IsSuccessStatusCode)
            {
                var jsonPostResult = await postResult.Content.ReadAsStringAsync();
                ResponsePostParameters postResponse = JsonConvert.DeserializeObject<ResponsePostParameters>(jsonPostResult);
                accessToken = postResponse.access_token;
                string get_uri = authSettings.get_uri+ "?start=" + start + "&limit=" + limit;
                ConfigRequestHeaders(get_client);
                var requestContent = new HttpRequestMessage(HttpMethod.Get, get_uri);
                requestContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); ;
                var result = await get_client.SendAsync(requestContent);
                if (result.IsSuccessStatusCode)
                {
                    var jsonResult = await result.Content.ReadAsStringAsync();
                    ResponseGetParameters response = JsonConvert.DeserializeObject<ResponseGetParameters>(jsonResult);
                    _response.statusCode = HttpStatusCode.OK;
                    _response.Result = response;
                    return Ok(_response);
                }
                else
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.ErrorMessages.Add("Error: " + result.ReasonPhrase);
                    _response.ErrorMessages.Add("URL: " + result.RequestMessage.RequestUri);
                    return BadRequest(_response);
                }
            }
            else
            {
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Result = null;
                _response.ErrorMessages.Add("Error: " + postResult.ReasonPhrase);
                _response.ErrorMessages.Add("URL: " + postResult.RequestMessage.RequestUri);
                return BadRequest(_response);
            }
        }
    }
}