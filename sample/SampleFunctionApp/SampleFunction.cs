using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace SampleFunctionApp
{
    public class SampleFunction
    {
        private readonly ILogger _logger;
        private readonly IValidator<SampleInput> _validator;

        public SampleFunction(ILoggerFactory loggerFactory, IValidator<SampleInput> validator)
        {
            _logger = loggerFactory.CreateLogger<SampleFunction>();
            _validator = validator;
        }

        [Function(nameof(SampleFunctionWithBody))]
        [OpenApiOperation(operationId: nameof(SampleFunctionWithBody), tags: new[] { "sample" }, Summary = "Sample input validation of body.")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SampleInput), Required = true)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ProblemDetails), Summary = "Invalid input supplied")]
        public async Task<HttpResponseData> SampleFunctionWithBody([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var input = await req.ReadFromJsonAsync<SampleInput>();
            if (input is null)
            {
                var error = req.CreateResponse(HttpStatusCode.BadRequest);
                await error.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = "Missing body"
                    });

                return error;
            }

            var validationResult = await _validator.ValidateAsync(input);
            if (!validationResult.IsValid)
            {
                var error = req.CreateResponse(HttpStatusCode.BadRequest);
                await error.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = "One or more validation errors occurred.",
                        Extensions = {
                            ["errors"] = validationResult.Errors.Select(e => new
                                {
                                    e.ErrorCode,
                                    e.PropertyName,
                                    e.ErrorMessage
                                })
                        }
                    });

                return error;
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function(nameof(SampleFunctionWithQuery))]
        [OpenApiOperation(operationId: nameof(SampleFunctionWithQuery), tags: new[] { "sample" }, Summary = "Sample input validation of query string.")]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Type = typeof(string), Summary = "The name")]
        [OpenApiParameter(name: "value", In = ParameterLocation.Query, Type = typeof(int), Summary = "The value")]
        [OpenApiParameter(name: "description", In = ParameterLocation.Query, Type = typeof(string), Summary = "The value")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(ProblemDetails), Summary = "Invalid input supplied")]
        public async Task<HttpResponseData> SampleFunctionWithQuery([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, string name, int value, string description)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var input = new SampleInput { Name = name, Value = value, Description = description };

            var validationResult = await _validator.ValidateAsync(input);
            if (!validationResult.IsValid)
            {
                var error = req.CreateResponse(HttpStatusCode.BadRequest);
                await error.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = "One or more validation errors occurred.",
                        Extensions = {
                            ["errors"] = validationResult.Errors.Select(e => new
                                {
                                    e.ErrorCode,
                                    e.PropertyName,
                                    e.ErrorMessage
                                })
                        }
                    });

                return error;
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
