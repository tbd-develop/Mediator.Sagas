﻿using System.Text;
using Xunit;

namespace Integration.Base;

public class TestOutputRedirect : TextWriter
{
    public override Encoding Encoding { get; } = null!;
    private readonly ITestOutputHelper _helper;

    public TestOutputRedirect(ITestOutputHelper helper)
    {
        _helper = helper;

        Console.SetOut(this);
    }

    public override void WriteLine(string? value)
    {
        try
        {
            _helper.WriteLine(value ?? string.Empty);
        }
        catch
        {
            // If the helper is unavailable, avoid the exception it raises
        }
    }

    public override ValueTask DisposeAsync()
    {
        Console.SetOut(Console.Out);

        return base.DisposeAsync();
    }
}