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
            }
        });

        var helloResource = restApi.Root.AddResource("hello");
        helloResource.AddCorsPreflight(new CorsOptions
        {
            AllowOrigins = Cors.ALL_ORIGINS,
            AllowMethods = Cors.ALL_METHODS,
            AllowHeaders = Cors.DEFAULT_HEADERS,
        });
        helloResource.AddMethod("GET", new LambdaIntegration(helloLambda));

        var userResource = restApi.Root.AddResource("user");
        userResource.AddCorsPreflight(new CorsOptions
        {
            AllowOrigins = Cors.ALL_ORIGINS,
            AllowMethods = Cors.ALL_METHODS,
            AllowHeaders = Cors.DEFAULT_HEADERS,
        });
        userResource.AddMethod("POST", new LambdaIntegration(userLambda));
    }
}
