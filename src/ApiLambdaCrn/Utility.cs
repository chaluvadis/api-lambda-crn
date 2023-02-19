using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using System.Collections.Generic;

namespace ApiLambdaCrn;
public static class Utility
{
    public static string GetContextValue(App app, string key)
        => app.Node.TryGetContext(key)?.ToString();
    public static StackProps GetStackProps(string deploymentStage)
    {
        return new StackProps
        {
            StackName = $"{deploymentStage}-rest-api-stack",
            Description = $"This is a {deploymentStage} environment stack deployed on {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            Tags = new Dictionary<string, string>
            {
                { "Environment", deploymentStage },
                { "Application", "RestApi" }
            }
        };
    }

    public static IDictionary<string, string> GetDefaultResponseParameters()
    {
        return new Dictionary<string, string>
        {
            { "method.response.header.Content-Type", "'application/json'" },
            { "method.response.header.Access-Control-Allow-Origin", "'*'" },
            { "method.response.header.Access-Control-Allow-Credentials", "'true'" }
        };
    }

    public static IDictionary<string, string> GetDefaultResponseTemplate()
    {
        return new Dictionary<string, string>
        {
            { "application/json", "Empty" },
        };
    }

    public static IIntegrationResponse Get200Response()
    {
        return new IntegrationResponse
        {
            StatusCode = "200",
            ResponseTemplates = GetDefaultResponseTemplate(),
            ResponseParameters = GetDefaultResponseParameters()
        };
    }

    public static IIntegrationResponse Get400Responses()
    {
        return new IntegrationResponse
        {
            StatusCode = "400",
            ResponseTemplates = GetDefaultResponseTemplate(),
            ResponseParameters = GetDefaultResponseParameters()
        };
    }
    public static IIntegrationResponse Get500Responses()
    {
        return new IntegrationResponse
        {
            StatusCode = "500",
            ResponseTemplates = GetDefaultResponseTemplate(),
            ResponseParameters = GetDefaultResponseParameters()
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
    public static ILambdaIntegrationOptions GetLambdaIntegrationOptions()
    {
        return new LambdaIntegrationOptions
        {
            Proxy = true,
            PassthroughBehavior = PassthroughBehavior.NEVER,
            IntegrationResponses = GetIntegrationResponses()
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

    public static IModelOptions GetResponseModelOptions()
    {
        return new ModelOptions
        {
            ContentType = "application/json",
            ModelName = "ResponseModel",
            Schema = new JsonSchema
            {
                Schema = JsonSchemaVersion.DRAFT4,
                Title = "pollResponse",
                Type = JsonSchemaType.OBJECT,
                Properties = new Dictionary<string, IJsonSchema>
                {
                    { "message", new JsonSchema { Type = JsonSchemaType.STRING } },
                }
            }
        };
    }

    public static IModelOptions GetErrorResponseModelOptions()
    {
        return new ModelOptions
        {
            ContentType = "application/json",
            ModelName = "ErrorResponseModel",
            Schema = new JsonSchema
            {
                Schema = JsonSchemaVersion.DRAFT4,
                Title = "errorResponse",
                Type = JsonSchemaType.OBJECT,
                Properties = new Dictionary<string, IJsonSchema>
                {
                    { "message", new JsonSchema { Type = JsonSchemaType.STRING } }
                }
            }
        };
    }
}