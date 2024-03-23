namespace dlTubeBlazor.Client;

public static class Utils
{
    public static void WriteLine( Exception e )
    {
        Console.WriteLine( $"{e} : {e.Message}" );
    }
}