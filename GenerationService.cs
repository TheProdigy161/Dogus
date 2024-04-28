using System.Collections.Concurrent;
using Bogus;
using Dogus.Logging;
using Dogus.Models;

public class GenerationService
{
    private readonly Faker<Dog> _dog;
    
    private readonly BlockingCollection<List<Dog>> _piecesOfWorks = new BlockingCollection<List<Dog>>(100);
    
    public GenerationService()
    {
        _dog = new Faker<Dog>()
            .StrictMode(true)
            .RuleFor(d => d.Id, f => f.Random.Guid())
            .RuleFor(d => d.Name, f => f.Name.FirstName())
            .RuleFor(d => d.DateOfBirth, f => f.Date.PastDateOnly(10))
            .RuleFor(d => d.DateOfDeath, f => f.Date.RecentDateOnly(1));
    }

    public async Task Do(int rowCount, int chunkSize)
    {
        Console.WriteLine("Starting generation...");

        // Start consumers.
        var consumers = StartConsumers();
        
        // Get data.
        GetData(rowCount, chunkSize);

        // Wait for all consumers to be done.
        await Task.WhenAll(consumers);

        await FileService.Output(consumers.SelectMany(t => t.Result).ToList());
    }

    public List<Task<List<Dog>>> StartConsumers()
    {
        Console.WriteLine("Starting consumers...");

        var tasks = new List<Task<List<Dog>>>();

        for (int i = 0; i < Environment.ProcessorCount - 2; i++)
        {
            var task = Task.Run(Consumer);
            tasks.Add(task);
        }

        Console.WriteLine("Consumers started...");

        return tasks;
    }

    public List<Dog> Consumer()
    {
        var list = new List<Dog>();
        
        while (!_piecesOfWorks.IsCompleted)
        {
            List<Dog> data = null;
            try
            {
                data = _piecesOfWorks.Take();
            }
            catch (InvalidOperationException) { }

            if (data != null)
            {
                foreach (var pieceOfWork in data.ToList())
                {
                    list.Add(pieceOfWork);
                }
            }
        }

        return list;
    }

    public void GetData(int rowCount, int chunkSize)
    {
        Console.WriteLine("Populating work items...");

        var data = _dog.GenerateLazy(rowCount);
        var chunks = data.Chunk(chunkSize);

        int chunkCount = 0;
        int totalChunks = chunks.Count();

        foreach (var chunk in chunks)
        {
            _piecesOfWorks.Add(chunk.ToList());
            chunkCount++;
            Console.WriteLine($"Chunk {chunkCount}/{totalChunks} added.");
        }

        _piecesOfWorks.CompleteAdding();

        Console.WriteLine("Work items populated...");
    }
}