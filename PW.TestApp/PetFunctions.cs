using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PW.TestApp.Entities;
using PW.TestApp.Services;

namespace PW.TestApp
{
    public class PetFunctions
    {
        private readonly IPetService _petService;

        public PetFunctions(IPetService petService)
        {
            _petService = petService ?? throw new ArgumentNullException(nameof(petService));
        }

        /// <summary>
        /// Get Pet By Id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="executionContext"></param>
        /// <param name="id">Pet Id</param>
        /// <returns></returns>
        [Function(nameof(GetPetByIdAsync))]
        public async Task<HttpResponseData> GetPetByIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pet/{id}")] HttpRequestData req,
            FunctionContext executionContext, int id)
        {
            HttpResponseData response;

            if (!executionContext.Items.ContainsKey("Authorization"))
            {
                response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.WriteString(ErrorMessage.BadRequest);
            }
            else if (executionContext.Items["Authorization"].ToString() == Access.Unauthorized)
            {
                response = req.CreateResponse(HttpStatusCode.Unauthorized);
                response.WriteString(ErrorMessage.Unauthorized);
            }
            else
            {
                var pet = await _petService.SingleOrDefaultAsync(p => p.Id == id);

                if (pet == default(Pet))
                {
                    response = req.CreateResponse(HttpStatusCode.NotFound);
                    response.WriteString(ErrorMessage.NotFound);
                }
                else
                {
                    var camelSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

                    var serializedObject = JsonConvert.SerializeObject(pet, camelSettings);

                    response = req.CreateResponse(HttpStatusCode.OK);

                    response.Headers.Add("Content-Type", "text/json; charset=utf-8");
                    response.Headers.TryAddWithoutValidation("Date", DateTime.UtcNow.ToString("o"));

                    response.WriteString(serializedObject);
                }
            }

            return response;
        }

        [Function(nameof(GetPetsAsync))]
        public async Task<HttpResponseData> GetPetsAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pets")] HttpRequestData req,
            FunctionContext executionContext)
        {
            HttpResponseData response;

            if (!executionContext.Items.ContainsKey("Authorization"))
            {
                response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.WriteString(ErrorMessage.BadRequest);
            }
            else if (executionContext.Items["Authorization"].ToString() == Access.Unauthorized)
            {
                response = req.CreateResponse(HttpStatusCode.Unauthorized);
                response.WriteString(ErrorMessage.Unauthorized);
            }
            else
            {
                var pets = await _petService.GetAsync();

                if (pets == default(List<Pet>))
                {
                    response = req.CreateResponse(HttpStatusCode.NotFound);
                    response.WriteString(ErrorMessage.NotFound);
                }
                else
                {
                    var camelSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

                    var serializedObject = JsonConvert.SerializeObject(pets, camelSettings);

                    response = req.CreateResponse(HttpStatusCode.OK);

                    response.Headers.Add("Content-Type", "text/json; charset=utf-8");
                    response.Headers.TryAddWithoutValidation("Date", DateTime.UtcNow.ToString("o"));

                    response.WriteString(serializedObject);
                }
            }

            return response;
        }


        [Function(nameof(AddPetAsync))]
        public async Task<HttpResponseData> AddPetAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pet")] HttpRequestData req,
            FunctionContext executionContext)
        {
            HttpResponseData response;

            if (!executionContext.Items.ContainsKey("Authorization"))
            {
                response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.WriteString(ErrorMessage.BadRequest);
            }
            else if (executionContext.Items["Authorization"].ToString() == Access.Unauthorized)
            {
                response = req.CreateResponse(HttpStatusCode.Unauthorized);
                response.WriteString(ErrorMessage.Unauthorized);
            }
            else 
            {
                //Read body to get object
                var requestBody = string.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }

                var pet = JsonConvert.DeserializeObject<Pet>(requestBody);

                pet = await _petService.AddAsync(pet);

                var camelSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

                var serializedObject = JsonConvert.SerializeObject(pet, camelSettings);

                response = req.CreateResponse(HttpStatusCode.OK);

                response.Headers.Add("Content-Type", "text/json; charset=utf-8");
                response.Headers.TryAddWithoutValidation("Date", DateTime.UtcNow.ToString("o"));
                response.Headers.Add("Location", UriHelper.Encode(req.Url));

                response.WriteString(serializedObject);
            }

            return response;
        }

        [Function(nameof(TickEveryTenSeconds))]
        public void TickEveryTenSeconds([TimerTrigger("*/10 * * * * *", RunOnStartup = true)] TimerInfo timer, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(TickEveryTenSeconds));
            logger.LogInformation($"Tic-Tac -------> { DateTime.UtcNow.ToString("HH:mm:ss") } <-----------------");
        }
    }
}
