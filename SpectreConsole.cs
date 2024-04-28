using Spectre.Console;

namespace Dogus.Logging;

public static class SpectreConsole
{
    public static void Output(List<Task[]> chunkedTasks)
    {
        AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                // new RemainingTimeColumn(),
                // new SpinnerColumn()
            })
            .Start(async ctx =>
            {
                List<ProgressTask> progressTasks = new List<ProgressTask>();

                for (int i = 1; i <= chunkedTasks.Count(); i++)
                {
                    ProgressTask progressTask = ctx.AddTask($"Chunk {i}", true, chunkedTasks[i - 1].Count());
                    progressTasks.Add(progressTask);
                }

                while (!ctx.IsFinished)
                {
                    chunkedTasks.ForEach(tasks =>
                    {
                        int completed = tasks.Count(t => t.IsCompleted);
                        progressTasks[chunkedTasks.IndexOf(tasks)].Increment(completed);
                    });
                }
            });
    }
}