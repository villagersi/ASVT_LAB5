// See https://aka.ms/new-console-template for more information
using Lab5.Network.Common;

internal class Program
{
    private static object _locker = new object();

    private static async Task Main(string[] args)
    {
        var serverAdress = new Uri("udp://127.0.0.1:7777");
        var client = new NetUdpClient(serverAdress);
        Console.WriteLine($"Connect to server at {serverAdress}");

        var messageApi = new MessageApiClient(client);
        await ManageMessages(messageApi);
        client.Dispose();
    }

    private static async Task ManageMessages(IMessageApi messageApi)
    {
        PrintMenu();

        while (true)
        {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1)
            {
                Console.Write("Enter message: ");
                var message = Console.ReadLine() ?? string.Empty;
                await messageApi.SendMessage(message);
                Console.WriteLine($"Message sent: {message}");
            }

            if (key.Key == ConsoleKey.D2)
            {
                Console.Write("Enter your wish!!!: ");
                var message = Console.ReadLine() ?? string.Empty;
                await messageApi.SendMessage(message);
                Console.WriteLine($"Your message sent to Santa Claus: {message}");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        Console.ReadKey();
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Send message");
            Console.WriteLine("2 - Send wish to Santa");
            Console.WriteLine("-------");
        }
    }
}