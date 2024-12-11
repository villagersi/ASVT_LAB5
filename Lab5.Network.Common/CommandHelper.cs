
using System.Text;
using System.Text.Json;

namespace Lab5.Network.Common;

public static class CommandHelper
{
    public static bool TryDeserializeCommand(this byte[] requestArray, out Command? command)
    {
        var commandCode = requestArray[1] - 0x31;
        var lengthBase = requestArray[2];
        var dataLength =  EncodeHelper.Decode5BitFrom8(lengthBase);
        Console.WriteLine($"> base: {lengthBase}");
        Console.WriteLine($"> len: {dataLength}");

        command = null;

        if (requestArray[3] != Const.STX && requestArray[4 + dataLength] != Const.ETX)
        {
            Console.WriteLine("Wrong message");
            return false;
        }

        var dataList = new List<byte>();
        for (int i = 4; i < requestArray.Length - 1; i++)
        {
            if (requestArray[i] == 0) 
            {
                break;
            }

            dataList.Add(requestArray[i]);
        }

        var dataBytes = dataList.ToArray();

        Console.WriteLine($"> code: {commandCode}");
        var base64Json = Encoding.ASCII.GetString(dataBytes);
        Console.WriteLine($"> base64: {base64Json}");
        var jsonBytes = Convert.FromBase64String(base64Json);

        var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonBytes);

        Console.WriteLine($"> raw: [{ToReadableByteArray(requestArray)}]");

        command = new Command()
        {
            Code = (byte)commandCode,
            Arguments = data ?? new Dictionary<string, object?>()
        };
        Console.WriteLine($"> json: {JsonSerializer.Serialize(command)}");
        return true;
    }

    public static byte[] SerializeCommand(this Command command)
    {
        var bytesList = new List<byte>
        {
            Const.SOH,
            (byte)(command.Code + 0x31)
        };

        Console.WriteLine($"< code: {command.Code}");
        var argsJson = JsonSerializer.Serialize(command.Arguments);
        var jsonBytes = Encoding.ASCII.GetBytes(argsJson);

        var base64Json = Convert.ToBase64String(jsonBytes);
        var base64Bytes = Encoding.ASCII.GetBytes(base64Json);
        Console.WriteLine($"< json: {JsonSerializer.Serialize(command)}");
        Console.WriteLine($"< base64: {base64Json}");

        var base2Length = EncodeHelper.Encode5BitTo8((ushort)base64Bytes.Length);
        var nearest2Length = EncodeHelper.NearestBase2((ushort)base64Bytes.Length);
        Array.Resize(ref base64Bytes, (int)nearest2Length); 

        Console.WriteLine($"< base: {base2Length}");
        Console.WriteLine($"< len: {nearest2Length}");


        bytesList.Add(base2Length);

        bytesList.Add(Const.STX);
        bytesList.AddRange(base64Bytes);
        bytesList.Add(Const.ETX);

        var full = bytesList.ToArray();
        Console.WriteLine($"< raw: [{ToReadableByteArray(full)}]");
        return full;
    }

    private static string ToReadableByteArray(byte[] bytes)
    {
        return string.Join(", ", bytes);
    }
}