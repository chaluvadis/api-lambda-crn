using Amazon.CDK;
namespace ApiLambdaCrn;
sealed class Program
{
    public static void Main()
    {
        var app = new App();
        var deploymentStage = Utility.GetContextValue(app, "deploymentStage");
        Utility.CreateStack(app, deploymentStage);
        app.Synth();
    }
}
