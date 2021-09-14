using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PW.TestApp.Middlewares
{
    public static class FunctionContextExtensions
    {
        public static HttpRequestData GetHttpRequestData(this FunctionContext functionContext)
        {
            try
            {
                var keyValuePair = functionContext.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
                var functionBindingsFeature = keyValuePair.Value;
                var type = functionBindingsFeature.GetType();
                var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
                return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
            }
            catch
            {
                return null;
            }
        }

        public static HttpResponseData SetHttpResponseData(this FunctionContext functionContext)
        {
            // A workaround for this can be found here
            // https://github.com/Azure/azure-functions-dotnet-worker/issues/530
            
            throw new NotImplementedException(nameof(SetHttpResponseData));

        }


    }
}
