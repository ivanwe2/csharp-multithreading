
Queue<int> queue = new Queue<int>();

ManualResetEventSlim consumeEvent = new(false);
ManualResetEventSlim produceEvent = new(true);

int consumerCount = 0;

ReaderWriterLockSlim readerWriterLock = new();
object lockConsumerCount = new();

Thread[] consumerThreads = new Thread[3];

for (int i = 0; i < 3; i++)
{
    consumerThreads[i] = new Thread(Consume);
    consumerThreads[i].Name = $"Consumer {i + 1}";
    consumerThreads[i].Start();
}

while (true)
{
    produceEvent.Wait();
    produceEvent.Reset();

    Console.WriteLine("To produce enter 'p' ");

    var input = Console.ReadLine() ?? string.Empty;
    if (input.ToLower() == "p")
    {
        for (int i = 0; i <= 10; i++)
        {
            try
            {
                readerWriterLock.EnterWriteLock();
                queue.Enqueue(i);
                Console.WriteLine($"Produced: {i} from thread: {Thread.CurrentThread.ManagedThreadId}");
            }
            finally
            {

                readerWriterLock.ExitWriteLock();
            }
        }

        consumeEvent.Set();
    }
}

void Consume()
{
    while (true)
    {
        consumeEvent.Wait();

        try
        {
            readerWriterLock.EnterReadLock();

            while (queue.TryDequeue(out int item))
            {
                Thread.Sleep(5000);

                Console.WriteLine($"Consumed: {item} from thread: {Thread.CurrentThread.Name}");
            }
        }
        finally
        {

            readerWriterLock.ExitReadLock();
        }

        lock (lockConsumerCount)
        {

            consumerCount++;
            if (consumerCount == 3)
            {
                consumeEvent.Reset();
                produceEvent.Set();
                consumerCount = 0;

                Console.WriteLine("*****************");
                Console.WriteLine("*** More please ***");
                Console.WriteLine("*****************");
            }
        }
    }
}