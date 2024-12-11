using System.Reflection.Metadata;

namespace Lab5.Network.Common;

public class Command 
{
    public ushort Code { get; set; }

    public Dictionary<string, object?> Arguments { get; set; } = new Dictionary<string, object?>();
}
