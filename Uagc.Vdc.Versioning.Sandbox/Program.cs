namespace Uagc.Vdc.Versioning.Sandbox;

internal class Program
{
    static void Main(string[] args)
    {
        CC.Stripe(CC.HEADER);
        CC.Center(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "ERROR, Assembly Name not found.", CC.HEADER);
        CC.Center($"Version {AppVersion}, CC.HEADER);
        CC.Stripe(CC.HEADER);
    }
}