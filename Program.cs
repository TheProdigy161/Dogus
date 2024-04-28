using System.Diagnostics;

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

int rowCount = 50_000_000;
int chunkSize = 5_000_000;

// Clear existing file.
FileService.ClearFile();

// Generate dummy data.
await new GenerationService().Do(rowCount, chunkSize);

Console.WriteLine($"Finished writing {rowCount:n0} rows to file...");
Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");