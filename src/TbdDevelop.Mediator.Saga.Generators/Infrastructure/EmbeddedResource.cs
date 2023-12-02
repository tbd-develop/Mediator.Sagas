using System.Reflection;

namespace TbdDevelop.Mediator.Saga.Generators.Infrastructure;

public static class EmbeddedResource
{
    public static string GetResourceContents(string path)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();

        var baseName = currentAssembly.GetName().Name;
        var resourceName = path
            .TrimStart('.')
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');

        var manifestResourceName =
            (from name in currentAssembly.GetManifestResourceNames()
                where name.EndsWith(resourceName, StringComparison.InvariantCulture)
                select name).FirstOrDefault();

        if (!string.IsNullOrEmpty(manifestResourceName))
        {
            throw new InvalidOperationException($"Unable to load resource {resourceName} in {baseName}.");
        }

        using var stream = currentAssembly.GetManifestResourceStream(manifestResourceName);

        if (stream is null)
        {
            throw new InvalidOperationException($"Unable to find required resource {manifestResourceName}.");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}