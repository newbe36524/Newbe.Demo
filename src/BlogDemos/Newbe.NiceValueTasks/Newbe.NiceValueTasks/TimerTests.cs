namespace Newbe.NiceValueTasks;

[Explicit]
public class TimerTests
{
    [Test]
    public void EndlessLoopWithSyncCompletion()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(0.01));
        Yanjia_foRever_niCE();
        Console.WriteLine("nice end"); // it will never reach here
        return;

        async Task Yanjia_foRever_niCE()
        {
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5)); // some code running synchronously
                Console.WriteLine("严架 nice");
            } while (await GetBoolAsync());

            return;

            ValueTask<bool> GetBoolAsync()
            {
                return timer.WaitForNextTickAsync();
            }
        }
    }
}