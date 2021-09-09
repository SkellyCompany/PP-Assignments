using System;
using System.Diagnostics;
using System.Threading;

namespace ParallelAssignmentOne
{
	class Program
	{
		static int invalidNumber;
		static int lockNumber;
		static int mutexNumber;
		static int semaphoreNumber;
		static int interlockedNumber;
		static bool bad;
		private static object lockObject = new object();
		private static Mutex mutexObject = new Mutex();
		private static Semaphore semaphoreObject = new Semaphore(0, 5);
		private static SemaphoreSlim semaphoreSlimObject = new SemaphoreSlim(5, 5);
		private static string calculation;

		public enum CalculationMethod { Invalid, Lock, Mutex, Semaphore, SemaphoreSlim, Interlocked, All }

		static void Main(string[] args)
		{	
			Console.WriteLine();
			CalculationMethod calculationMethod = CalculationMethod.Interlocked;
			Thread thread1 = new Thread(()=>DoWork(1_000_000, calculationMethod));
			Thread thread2 = new Thread(()=>DoWork(1_000_000, calculationMethod));
			thread1.Start();
			thread2.Start();
			thread1.Join();
			thread2.Join();
			Console.ReadLine();
		}

		public static Stopwatch StartWrite()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		public static void StopWrite(Stopwatch stopwatch, int number)
		{
			if (bad)
			{
				stopwatch.Stop();
				Console.WriteLine(calculation);
				Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds / 1000.0f);
				Console.WriteLine("Number: " + number);
			}
			bad = true;
		}

		public static void DoWork(int times, CalculationMethod calculationMethod)
		{
			if (calculationMethod == CalculationMethod.Invalid)
			{
				Invalid(times);
			}
			if (calculationMethod == CalculationMethod.Lock)
			{
				Lock(times);
			}
			if (calculationMethod == CalculationMethod.Mutex)
			{
				Mutex(times);
			}
			if (calculationMethod == CalculationMethod.Semaphore)
			{
				Semaphore(times);
			}
			if (calculationMethod == CalculationMethod.SemaphoreSlim)
			{
				SemaphoreSlim(times);
			}
			if (calculationMethod == CalculationMethod.Interlocked)
			{
				Interlock(times);
			}
			if (calculationMethod == CalculationMethod.All)
			{
				Invalid(times);
				Lock(times);
				Mutex(times);
				Semaphore(times);
				SemaphoreSlim(times);
				Interlock(times);
			}
		}

		public static void Invalid(int times)
		{
			Stopwatch stopwatch = StartWrite();
			calculation = "Invalid";
			for (int i = 0; i < times; i++)
			{
				invalidNumber++;
			}
			StopWrite(stopwatch, invalidNumber);
		}

		public static void Lock(int times)
		{
			Stopwatch stopwatch = StartWrite();
			calculation = "Lock";
			for (int i = 0; i < times; i++)
			{
				lock (lockObject)
				{
					lockNumber++;
				}
			}
			StopWrite(stopwatch, lockNumber);
		}

		public static void Mutex(int times)
		{
			Stopwatch stopwatch = StartWrite();
			calculation = "Mutex";
			for (int i = 0; i < times; i++)
			{
				mutexObject.WaitOne();
				mutexNumber++;
				mutexObject.ReleaseMutex();
			}
			StopWrite(stopwatch, mutexNumber);
		}

		public static void Semaphore(int times)
		{
			Stopwatch stopwatch = StartWrite();
			calculation = "Semaphore";
			for (int i = 0; i < times; i++)
			{
				semaphoreObject.WaitOne();
				semaphoreNumber++;
				semaphoreObject.Release();
			}
			StopWrite(stopwatch, semaphoreNumber);
		}

		public static void SemaphoreSlim(int times)
		{
			Stopwatch stopwatch = StartWrite();
			calculation = "SemaphoreSlim";
			for (int i = 0; i < times; i++)
			{
				semaphoreSlimObject.Wait();
				semaphoreNumber++;
				semaphoreSlimObject.Release();
			}
			StopWrite(stopwatch, semaphoreNumber);
		}

		public static void Interlock(int times)
		{
			Stopwatch stopwatch = StartWrite();
			calculation = "Interlock";
			for (int i = 0; i < times; i++)
			{
				Interlocked.Increment(ref interlockedNumber);
			}
			StopWrite(stopwatch, interlockedNumber);
		}
	}
}
