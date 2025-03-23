using System.Reflection;

namespace ExampleHostApp;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}