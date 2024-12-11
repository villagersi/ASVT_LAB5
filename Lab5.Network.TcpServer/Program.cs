using Lab5.Network.Common.CarApi;

internal class Program
{
    public static void Main(string[] args)
    {
        var listenAddress = new Uri("tcp://0.0.0.0:5555");

        var carApi = new CarApi(); // Создаем экземпляр CarApi

        var tcpServer = new ApiTcpServer(carApi, listenAddress); // Передаем CarApi в сервер
        Console.WriteLine($"Start server and listen at {listenAddress}");
        tcpServer.StartAsync().Wait();
    }
}
