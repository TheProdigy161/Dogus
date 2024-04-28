using System.Text;
using Dogus.Models;

public static class FileService
{
    public static void ClearFile()
    {
        // Delete file if exists.
        if (File.Exists("./Outputs/dog.txt"))
        {
            File.Delete("./Outputs/dog.txt");
        }
    }

    public static async Task Output(List<Dog> list)
    {
        Console.WriteLine("Writing to file...");

        using (var outFile = File.Create("./Output/dog.txt"))
        {    
            foreach (var dog in list)
            {
                await outFile.WriteAsync(Encoding.UTF8.GetBytes($"{dog.Id},{dog.Name},{dog.DateOfBirth},{dog.DateOfDeath},{dog.Age},{dog.IsDead}\n"));
            }
        }
    }
}