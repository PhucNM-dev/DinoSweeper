public static class Assets
{
    private const string BaseUri = "pack://application:,,,/Assets/";

    public static string Path(string fileName) => BaseUri + fileName;
}
