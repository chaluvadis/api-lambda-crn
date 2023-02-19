using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace ApiLambdaCrn;
public class RestApiStack : Stack
{
    internal RestApiStack(Construct scope, string id, IStackProps props, string deploymentStage) : base(scope, id, props)
    {
        IFunction helloLambda = Function.FromFunctionName(this,
                    $"{deploymentStage}-hello-world-function",
                    $"{deploymentStage}-hello-world-lambda");

        IFunction userLambda = Function.FromFunctionName(this,
                    $"{deploymentStage}-user-lambda-function",
                    $"{deploymentStage}-user-handler-lambda");

        var restApi = new RestApi(this, "RestApi", new RestApiProps
        {
            RestApiName = $"Rest API {deploymentStage}",
            Description = $"{deploymentStage} api stack deployed on {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            DeployOptions = new StageOptions
            {
                StageName = deploymentStage,
                TracingEnabled = true,
            },
            DefaultCorsPreflightOptions = new CorsOptions
            {
                AllowOrigins = Cors.ALL_ORIGINS,
                AllowMethods = Cors.ALL_METHODS,
                AllowHeaders = Cors.DEFAULT_HEADERS,
            }
        });

        Model responseModel = restApi.AddModel("ResponseModel", Utility.GetResponseModelOptions());
        Model errorResponseModel = restApi.AddModel("ErrorResponseModel", Utility.GetErrorResponseModelOptions());
        var methodOptions = new MethodOptions
        {
            MethodResponses = new IMethodResponse[]
            {
                new MethodResponse
                {
                    StatusCode = "200",
                    ResponseModels = new Dictionary<string, IModel>
                    {
                        { "application/json", responseModel }
                    },
                    ResponseParameters = new Dictionary<string, bool>
                    {
                        { "method.response.header.Content-Type", true },
                        { "method.response.header.Access-Control-Allow-Origin", true },
                        { "method.response.header.Access-Control-Allow-Credentials", true }
                    }
                },
                new MethodResponse
                {
                    StatusCode = "400",
                    ResponseModels = new Dictionary<string, IModel>
                    {
                        { "application/json", errorResponseModel }
                    },
                    ResponseParameters = new Dictionary<string, bool>
                    {
                        { "method.response.header.Content-Type", true },
                        { "method.response.header.Access-Control-Allow-Origin", true },
                        { "method.response.header.Access-Control-Allow-Credentials", true }
                    }
                },
                new MethodResponse
                {
                    StatusCode = "500",
                    ResponseModels = new Dictionary<string, IModel>
                    {
                        { "application/json", errorResponseModel }
                    },
                    ResponseParameters = new Dictionary<string, bool>
                    {
                        { "method.response.header.Content-Type", true },
                        { "method.response.header.Access-Control-Allow-Origin", true },
                        { "method.response.header.Access-Control-Allow-Credentials", true }
                    }
                }
            }
        };

        var helloResource = restApi.Root.AddResource("hello");
        var helloIntegration = new LambdaIntegration(helloLambda, Utility.GetLambdaIntegrationOptions());
        helloResource.AddMethod("GET", helloIntegration, methodOptions);

        var userResource = restApi.Root.AddResource("user");
        var userIntegration = new LambdaIntegration(userLambda, Utility.GetLambdaIntegrationOptions());
        userResource.AddMethod("POST", userIntegration, methodOptions);
    }
}
