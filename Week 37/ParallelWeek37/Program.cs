using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelWeek37
{
	class Program
	{
		private readonly int _fiveSeconds = 5;
		private readonly int _tenSeconds = 10;


		static void Main(string[] args)
		{
			Program program = new Program();
			program.Exercise2();
		}

		//Exercise 1

		void Exercise1()
		{
			Console.WriteLine("Exercise 1");
			RunSequential();
			RunParallel();
		}

		void RunSequential()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Exercise1Method5();
			Exercise1Method10();
			stopwatch.Stop();
			Console.WriteLine("Total time: " + stopwatch.ElapsedMilliseconds / 1000.0f);
		}

		void RunParallel()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Task task1 = Task.Factory.StartNew(Exercise1Method5);
			Task task2 = Task.Factory.StartNew(Exercise1Method10);
			Task.WaitAll(task1, task2);
			stopwatch.Stop();
			Console.WriteLine("Total time: " + stopwatch.ElapsedMilliseconds / 1000.0f);
		}

		void Exercise1Method5()
		{
			Thread.Sleep(_fiveSeconds * 1000);
		}

		void Exercise1Method10()
		{
			Thread.Sleep(_tenSeconds * 1000);
		}

		//Exercise 2
		void Exercise2()
		{
			Console.WriteLine("Exercise 2");
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = cancellationTokenSource.Token;
			Stopwatch stopwatch = Stopwatch.StartNew();	
			Task task1 = Task.Factory.StartNew(() => Exercise2Method5(token), token);
			Task task2 = Task.Factory.StartNew(() => Exercise2Method10(token), token);
			try
			{
				Console.ReadKey();
				cancellationTokenSource.Cancel();
				Task.WaitAll(task1, task2);
				Console.WriteLine("Tasks Completed: " + task1.Status);
				Console.WriteLine("Tasks Completed: " + task2.Status);
			}
			catch (AggregateException e)
			{
				e.Handle(e =>
				{
					Console.WriteLine(e.Message);
					return true;
				});
				Console.WriteLine("Tasks Error: " + task1.Status);
				Console.WriteLine("Tasks Error: " + task2.Status);
			}
			stopwatch.Stop();
			Console.WriteLine("Total time: " + stopwatch.ElapsedMilliseconds / 1000.0f);
		}
		void Exercise2Method5(CancellationToken token)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (stopwatch.Elapsed < TimeSpan.FromSeconds(_fiveSeconds))
			{
				if (token.IsCancellationRequested)
				{
					token.ThrowIfCancellationRequested();
				}
			}
			stopwatch.Stop();
		}

		void Exercise2Method10(CancellationToken token)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (stopwatch.Elapsed < TimeSpan.FromSeconds(_tenSeconds))
			{
				if (token.IsCancellationRequested)
				{
					token.ThrowIfCancellationRequested();
				}
			}
			stopwatch.Stop();
		}
	}
}
