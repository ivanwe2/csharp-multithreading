
using System.Collections.Concurrent;

ConcurrentQueue<string> requestQueue = new ConcurrentQueue<string>();

Thread monitoringThread = new Thread(() => MonitorQueue());
monitoringThread.Start();

Console.WriteLine("Server is running. Type 'exit' to stop.");

while (true)
{
    string? input = Console.ReadLine();
    if (input is null || input == "exit")
    {
        break;
    }

    requestQueue.Enqueue(input);
}

void MonitorQueue()
{
    while (true)
    {
        if (requestQueue.TryDequeue(out string? input))
        {
            Thread processingThread = new Thread(() => ProcessInput(input));
            processingThread.Start();
        }
        Thread.Sleep(100);
    }
}

void ProcessInput(string input)
{
    Thread.Sleep(2000);
    Console.WriteLine("Processed input: {0}", input);
}