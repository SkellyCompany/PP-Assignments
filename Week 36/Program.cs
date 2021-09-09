using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace DataParallelism {
	class Program {

		static int ARRAY_SIZE = 1_000_000_000;
		static int MAX_NUMBER = 100;

		// MARK: MAIN
		static void Main(string[] args) {
			// CompareSquaring();
			// CompareAdding();
			CompareInitialization();
		}

		// MARK: COMPARING
		private static void CompareSquaring() {
			int[] arraySequential = InitializeArray();
			SquareArraySequential(arraySequential);

			int[] arrayParallel = InitializeArray();
			SquareArrayParallel(arrayParallel);

			int[] arrayParallelPartitioned = InitializeArray();
			SquareArrayParallelPartitioned(arrayParallelPartitioned);
		}

		private static void CompareAdding() {
			int[] firstArray = InitializeArray();
			int[] secondArray = InitializeArray();

			AddTwoArraysSequential(firstArray, secondArray);
			AddTwoArraysParallel(firstArray, secondArray);
			AddTwoArraysParallelPartitioned(firstArray, secondArray);
		}

		private static void CompareInitialization() {
			InitializeRandomArraySequential();
			InitializeRandomArrayParallelPartitioned();
		}

		// MARK: INITIALIZATION
		private static int[] InitializeArray() {
			Stopwatch sw = Stopwatch.StartNew();
			var array = new int[ARRAY_SIZE];
			for (int i = 0; i < ARRAY_SIZE; i++) {
				array[i] = 10;
			}
			sw.Stop();
			Console.WriteLine("Array Initialization: " + sw.ElapsedMilliseconds);
			return array;
		}

		private static int[] InitializeRandomArraySequential() {
			Stopwatch sw = Stopwatch.StartNew();
			var array = new int[ARRAY_SIZE];
			var rand = new Random();
			for (int i = 0; i < ARRAY_SIZE; i++) {
				array[i] = rand.Next(MAX_NUMBER);
			}
			sw.Stop();
			Console.WriteLine("Sequential Random Array Initialization: " + sw.ElapsedMilliseconds);
			return array;
		}

		private static int[] InitializeRandomArrayParallelPartitioned() {
			Stopwatch sw = Stopwatch.StartNew();
			var array = new int[ARRAY_SIZE];
			Parallel.ForEach(
				Partitioner.Create(0, array.Length),
				new ParallelOptions(),
				() => { return new Random(); },
				(range, loopState, random) => {
					for (int i = range.Item1; i < range.Item2; i++) {
						array[i] = random.Next(MAX_NUMBER);
					}
					return random;
				}, _ => { });
			sw.Stop();
			Console.WriteLine("Parallel Random Array Initialization: " + sw.ElapsedMilliseconds);
			return array;
		}

		// MARK: SQUARING
		private static void SquareArraySequential(int[] array) {
			Stopwatch sw = Stopwatch.StartNew();
			for (int i = 0; i < array.Length; i++) {
				array[i] = array[i] * array[i];
			}
			sw.Stop();
			Console.WriteLine("Sequential array squaring: " + sw.ElapsedMilliseconds);
		}

		private static void SquareArrayParallel(int[] array) {
			Stopwatch sw = Stopwatch.StartNew();
			Parallel.For(0, array.Length, (i) => {
				array[i] = array[i] * array[i];
			});
			sw.Stop();
			Console.WriteLine("Parallel array squaring: " + sw.ElapsedMilliseconds);
		}

		private static void SquareArrayParallelPartitioned(int[] array) {
			Stopwatch sw = Stopwatch.StartNew();
			Parallel.ForEach(Partitioner.Create(0, array.Length), (range) => {
				for (int i = range.Item1; i < range.Item2; i++) {
					array[i] = array[i] * array[i];
				}
			});
			sw.Stop();
			Console.WriteLine("Partitioned Parallel array squaring: " + sw.ElapsedMilliseconds);
		}

		// MARK: ADDING
		private static void AddTwoArraysSequential(int[] firstArray, int[] secondArray) {
			Stopwatch sw = Stopwatch.StartNew();
			var resultArray = new int[ARRAY_SIZE];
			for (int i = 0; i < firstArray.Length; i++) {
				resultArray[i] = firstArray[i] + secondArray[i];
			}
			sw.Stop();
			Console.WriteLine("Sequential array adding: " + sw.ElapsedMilliseconds);
		}

		private static void AddTwoArraysParallel(int[] firstArray, int[] secondArray) {
			Stopwatch sw = Stopwatch.StartNew();
			var resultArray = new int[ARRAY_SIZE];
			Parallel.For(0, firstArray.Length, (i) => {
				resultArray[i] = firstArray[i] + secondArray[i];
			});
			sw.Stop();
			Console.WriteLine("Parallel array adding: " + sw.ElapsedMilliseconds);
		}

		private static void AddTwoArraysParallelPartitioned(int[] firstArray, int[] secondArray) {
			Stopwatch sw = Stopwatch.StartNew();
			var resultArray = new int[ARRAY_SIZE];
			Parallel.ForEach(Partitioner.Create(0, firstArray.Length), (range) => {
				for (int i = range.Item1; i < range.Item2; i++) {
					resultArray[i] = firstArray[i] + secondArray[i];
				}
			});
			sw.Stop();
			Console.WriteLine("Partitioned Parallel array adding: " + sw.ElapsedMilliseconds);
		}
	}
}
