using System.Numerics;

namespace Lab5.Network.Common;

public static class EncodeHelper
{
    public static uint NearestBase2(uint find) 
    {
        uint r = 1;
        while (r < find) 
        {
            r *= 2;
        }

        return r;
    }

    public static byte Encode5BitTo8(ushort value) 
    {
        var n = NearestBase2(value);
        var l = (byte)(BitOperations.Log2(n) + 1);
        return (byte)(l << 3);
    }

    public static ushort Decode5BitFrom8(byte value) 
    {
        var s = value >> 3;
        var n = Math.Pow(2, s - 1);
        var n1 = NearestBase2((uint)n);
        return (ushort)n1;
    }
}
