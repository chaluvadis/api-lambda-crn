using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;

namespace ApiLambdaCrn;
public static class Utility
{
    public static string GetContextValue(App app, string key)
    {
        return app.Node.TryGetContext(key)?.ToString();
    }
    public static StackProps GetStackProps(string deploymentStage)
    {
        return new StackProps
        {
            StackName = $"rest-api-{deploymentStage}-stack",
            Description = $"This is a {deploymentStage} environment stack deployed on {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            Tags = new Dictionary<string, string>
            {
                { "Environment", deploymentStage },
                { "Application", "RestApi" }
            }
        };
    }

    public static IIntegrationResponse Get200Response()
    {
        return new IntegrationResponse
        {
            StatusCode = "200",
            ResponseTemplates = new Dictionary<string, string>
            {
                { "application/json", "Empty" },
            }
        };
    }

    public static IIntegrationResponse Get400Responses()
    {
        return new IntegrationResponse
        {
            StatusCode = "400",
            ResponseTemplates = new Dictionary<string, string>
            {
                { "application/json", "Empty" },
            }
        };
    }

    public static IIntegrationResponse Get500Responses()
    {
        return new IntegrationResponse
        {
            StatusCode = "500",
            ResponseTemplates = new Dictionary<string, string>
            {
                { "application/json", "Empty" },
            }
        };
    }

    public static IIntegrationResponse[] GetIntegrationResponses()
    {
        return new IIntegrationResponse[]
        {
            Get200Response(),
            Get400Responses(),
            Get500Responses()
        };
    }

    public static LambdaIntegrationOptions GetLambdaIntegrationOptions()
    {
        return new LambdaIntegrationOptions
        {
            Proxy = true,
            IntegrationResponses = GetIntegrationResponses()
        };
    }

    public static bool IsProduction(string deploymentStage)
    {
        return deploymentStage == "prod";
    }

    public static IQuotaSettings GetQuotaSettings()
    {
        return new QuotaSettings
        {
            Limit = 10000,
            Offset = 0,
            Period = Period.DAY,
        };
    }

    public static ResourceOptions GetResourceOptions()
    {
        return new ResourceOptions
        {
            DefaultCorsPreflightOptions = new CorsOptions
            {
                AllowOrigins = Cors.ALL_ORIGINS,
                AllowMethods = Cors.ALL_METHODS,
            },
            DefaultMethodOptions = new MethodOptions
            {
                AuthorizationType = AuthorizationType.NONE,
            },
        };
    }
    public static void CreateStack(App app, string deploymentStage)
    {
        var stackId = $"{deploymentStage}-rest-api-stack";
        _ = new RestApiStack(
            app,
            stackId,
            GetStackProps(deploymentStage),
            deploymentStage
        );
    }
    public static MethodOptions GetMethodOptions()
    {
        return new MethodOptions
        {
            MethodResponses = new[]
            {
                new MethodResponse
                {
                    StatusCode = "200",
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
                    ResponseParameters = new Dictionary<string, bool>
                    {
                        { "method.response.header.Content-Type", true },
                        { "method.response.header.Access-Control-Allow-Origin", true },
                        { "method.response.header.Access-Control-Allow-Credentials", true }
                    }
                },
                new MethodResponse
                {
                    StatusCode= "500",
                    ResponseParameters = new Dictionary<string, bool>
                    {
                        { "method.response.header.Content-Type", true },
                        { "method.response.header.Access-Control-Allow-Origin", true },
                        { "method.response.header.Access-Control-Allow-Credentials", true }
                    }
                }
            }
        };
    }
}