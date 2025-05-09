namespace Newbe.NiceValueTasks;

[Explicit]
public class ValueTasksTests
{
    [Test]
    public void EndlessLoopWithSyncCompletion()
    {
        Yanjia_foRever_niCE();
        Console.WriteLine("nice end"); // it will never reach here
        return;

        async Task Yanjia_foRever_niCE()
        {
            do
            {
                Thread.Sleep(100); // some code running synchronously
                Console.WriteLine("严架 nice");
            } while (await GetBoolAsync());

            return;

            ValueTask<bool> GetBoolAsync()
            {
                return ValueTask.FromResult(true);
            }
        }
    }

    [Test]
    public void EndlessLoopWithSyncCompletion2()
    {
        Yanjia_foRever_niCE();
        Console.WriteLine("nice end"); // it will never reach here
        return;

        async Task Yanjia_foRever_niCE()
        {
            do
            {
                Thread.Sleep(100); // some code running synchronously
                Console.WriteLine("严架 nice");
            } while (await GetBoolAsync());

            return;

            async ValueTask<bool> GetBoolAsync()
            {
                return true;
            }
        }
    }


    [Test]
    public void EndlessLoopWithSyncCompletion3()
    {
        Yanjia_foRever_niCE();
        Console.WriteLine("nice end"); // it will never reach here
        return;

        async Task Yanjia_foRever_niCE()
        {
            do
            {
                Thread.Sleep(100); // some code running synchronously
                Console.WriteLine("严架 nice");
            } while (await GetBoolAsync());

            return;

            async ValueTask<bool> GetBoolAsync()
            {
                return await ValueTask.FromResult(true);
            }
        }
    }

    [Test]
    public void EndlessLoopWithAsyncCompletion()
    {
        Yanjia_foRever_niCE();
        Console.WriteLine("nice end"); // it will reach here
        return;

        async Task Yanjia_foRever_niCE()
        {
            do
            {
                Thread.Sleep(100); // some code running synchronously
                Console.WriteLine("严架 nice");
            } while (await GetBoolAsync());

            return;

            async ValueTask<bool> GetBoolAsync()
            {
                await Task.Delay(100);
                return true;
            }
        }
    }
}