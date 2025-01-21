using System.Collections.Concurrent;

Queue<string> requestQueue = new Queue<string>();

int availableTickets = 10;
object ticketsLock = new();
Lock ticketsLockNew = new();

Thread monitoringThread = new Thread(() => MonitorQueue());
monitoringThread.Start();

Console.WriteLine("Server is running. \r\n Type b to book a ticket. \r\n Type c to cancel ticket. \r\n Type 'exit' to stop.");

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

    lock (ticketsLockNew)
    {

        if (input == "b")
        {
            if (availableTickets > 0)
            {
                availableTickets--;
                Console.WriteLine();
                Console.WriteLine("Seat is booked. {0} seats are still available", availableTickets);
            }
            else
            {
                Console.WriteLine("Tickets are not available");
            }
        }
        else if (input == "c")
        {
            if (availableTickets < 10)
            {

                availableTickets++;
                Console.WriteLine();
                Console.WriteLine("Seat is cancelled. {0} seats are still available", availableTickets);
            }
            else
            {
                Console.WriteLine("could not cancel");
            }
        }
    }
}