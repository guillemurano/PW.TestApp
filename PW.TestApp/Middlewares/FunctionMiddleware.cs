using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PW.TestApp.Middlewares
{
    public class FunctionMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next) 
        {
            //pre-function execution
            var logger = context.GetLogger<FunctionMiddleware>();
            logger.LogInformation($"Start {context.FunctionDefinition.Name} function execution.");

            if (!context.BindingContext.BindingData.TryGetValue("TimerTrigger", out object _))
            {
                context.BindingContext.BindingData.TryGetValue("Headers", out object headers);
                var headersObject = JsonSerializer.Deserialize<Dictionary<string, string>>((string)headers);

                if (headersObject.TryGetValue("Authorization", out string authorizationHeader))
                {
                    var access = authorizationHeader == "123456" ?
                        Access.Authorized :
                        Access.Unauthorized;

                    context.Items.Add("Authorization", access);
                }
            }

            //function execution
            await next(context);

            //post-function execution
            logger.LogInformation($"End {context.FunctionDefinition.Name} function execution.");
        }
    }
}
