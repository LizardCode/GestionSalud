using LizardCode.Framework.RestClient.RestException;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace LizardCode.Framework.RestClient
{
    public class Client
    {
        private readonly Dictionary<string, object> _Headers;
        private RestClientOptions RestClientOptions { get; set; }

        public Client(string URLService, Dictionary<string, object> Headers, int timeoutApi = 1200000)
        {
            _Headers = Headers;

            RestClientOptions = new RestClientOptions(URLService)
            {
                MaxTimeout = timeoutApi
            };

        }


        public async Task<bool> PutAsync(string method, object body, Dictionary<string, object> parameters, bool urlSegmentType = true)
        {
            var restClient = new RestSharp.RestClient(RestClientOptions);
       
            var request = new RestRequest(method, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };

            if (parameters != null)
                ADDRestParameters(ref request, parameters, urlSegmentType);
            
            if (body != null)
                ADDBody(ref request, body);
            
            ADDRestHeaders(ref request);

            var restResponse = await restClient.ExecuteAsync(request);
            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                case HttpStatusCode.NoContent:
                    throw new NotContentException();
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(restResponse.ErrorMessage);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException();
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException();
                case HttpStatusCode.NotFound:
                    return false;
                case HttpStatusCode.InternalServerError:
                    throw new Exception("Ha ocurrido un error interno al ejecutar la consulta - " + restResponse.ErrorMessage);
                default:
                    throw new Exception(restResponse.StatusDescription);
            }
        }

        public async Task<bool> DeleteAsync(string method, Dictionary<string, object> parameters, bool urlSegmentType = true)
        {
            var restClient = new RestSharp.RestClient(RestClientOptions);

            var request = new RestRequest(method, Method.Delete);

            if (parameters != null)
                ADDRestParameters(ref request, parameters, urlSegmentType);

            ADDRestHeaders(ref request);

            var restResponse = await restClient.ExecuteAsync(request);
            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(restResponse.ErrorMessage);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException();
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException();
                case HttpStatusCode.InternalServerError:
                    throw new Exception("Ha ocurrido un error interno al ejecutar la consulta - " + restResponse.ErrorMessage);
                default:
                    throw new Exception(restResponse.StatusDescription);
            }
        }       

        public async Task<T> GetAsync<T>(string method, Dictionary<string, object> parameters = null, bool urlSegmentType = true)
        {
            var restClient = new RestSharp.RestClient(RestClientOptions);

            var request = new RestRequest(method, Method.Get);

            if (parameters != null)
                ADDRestParameters(ref request, parameters, urlSegmentType);
            
            ADDRestHeaders(ref request);
     
            var restResponse = await restClient.ExecuteAsync(request);
            T obj;
            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    obj = JsonConvert.DeserializeObject<T>(restResponse.Content);
                    break;
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(restResponse.ErrorMessage, restResponse.ErrorException);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException();
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException();
                case HttpStatusCode.NotFound:
                    obj = default(T);
                    break;
                case HttpStatusCode.InternalServerError:
                    if (restResponse.ErrorMessage != null)
                    {
                        throw new Exception("InternalServerError: - " + restResponse.ErrorMessage);
                    }
                    else
                    {
                        throw new Exception("InternalServerError: - Not detected");
                    }

                default:
                    throw new Exception(restResponse.StatusDescription);
            }
            return obj;
        }

        public async Task<T> PostAsync<T>(string method, object body, Dictionary<string, object> parameters = null, bool urlSegmentType = true)
        {
            var restClient = new RestSharp.RestClient(RestClientOptions);

            RestRequest request = new RestRequest(method, Method.Post);
            if (parameters != null)
                ADDRestParameters(ref request, parameters, urlSegmentType);

            if (body != null)
                ADDBody(ref request, body);

            ADDRestHeaders(ref request);

            var restResponse = await restClient.ExecuteAsync(request);
            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<T>(restResponse.Content);
                case HttpStatusCode.Created:
                    return JsonConvert.DeserializeObject<T>(restResponse.Content);
                case HttpStatusCode.NoContent:
                    throw new NotContentException();
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(restResponse.ErrorMessage);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException();
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException();
                case HttpStatusCode.Conflict:
                    throw new ConflictException();
                case HttpStatusCode.InternalServerError:
                    throw new Exception("Ha ocurrido un error interno al ejecutar la consulta - " + restResponse.ErrorMessage);
                default:
                    throw new Exception(restResponse.StatusDescription);
            }
        }

        public async Task<T> PostXMLAsync<T>(string method, object body, Dictionary<string, object> parameters = null, bool urlSegmentType = true)
        {
            var restClient = new RestSharp.RestClient(RestClientOptions);

            var request = new RestRequest(method, Method.Post);

            if (parameters != null)
                ADDRestParameters(ref request, parameters, urlSegmentType); 
            
            if (body != null)
                request.AddParameter("text/xml", body, ParameterType.RequestBody);
            
            ADDRestHeaders(ref request);

            var restResponse = await restClient.ExecuteAsync(request);
            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    return JsonConvert.DeserializeObject<T>(restResponse.Content);
                case HttpStatusCode.NoContent:
                    throw new NotContentException();
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(restResponse.ErrorMessage);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException();
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException();
                case HttpStatusCode.Conflict:
                    throw new ConflictException();
                case HttpStatusCode.InternalServerError:
                    throw new Exception("Ha ocurrido un error interno al ejecutar la consulta - " + restResponse.ErrorMessage);
                default:
                    throw new Exception(restResponse.StatusDescription);
            }
        }


        private void ADDRestParameters(ref RestRequest request, Dictionary<string, object> parameters, bool urlSegmentType = true)
        {
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                if (parameter.Value.GetType() == typeof(string))
                {
                    request.AddParameter(parameter.Key, (object)parameter.Value.ToString(), urlSegmentType ? ParameterType.UrlSegment : ParameterType.QueryString);
                }
                else if (parameter.Value is DateTime)
                {
                    request.AddParameter(parameter.Key, (object)((DateTime)parameter.Value).ToString("s", (IFormatProvider)CultureInfo.InvariantCulture), urlSegmentType ? ParameterType.UrlSegment : ParameterType.QueryString);
                }
                else
                {
                    request.AddParameter(parameter.Key, (object)JsonConvert.SerializeObject(parameter.Value), urlSegmentType ? ParameterType.UrlSegment : ParameterType.QueryString);
                }
            }
        }

        private void ADDBody(ref RestRequest request, object body)
        {
            if (body == null)
            { return; }
            request.AddParameter("Application/Json", (object)JsonConvert.SerializeObject(body), ParameterType.RequestBody);
        }

        private void ADDRestHeaders(ref RestRequest request)
        {
            foreach (KeyValuePair<string, object> header in this._Headers)
            {
                if (header.Value != null)
                {
                    request.AddHeader(header.Key, header.Value.ToString());
                }
            }
        }
    }
}
