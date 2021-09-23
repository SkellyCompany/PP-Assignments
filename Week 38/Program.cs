using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Week_38 {
	class Program {
		static readonly int LARGE_ARRAY_SIZE = 100_000_000;
		static readonly int MAX_RANDOM_VALUE = 100;
		static object lockObject = new object();

		static void Main(string[] args) {
			// var array = InitializeLargeArrayOfNumbers();
			// Time(() => {
			// 	var result = GetSum1(array);
			// 	Console.WriteLine("Result: " + result);
			// }, "Sequential Sum");
			// Time(() => {
			// 	var result = GetSum2(array);
			// 	Console.WriteLine("Result: " + result);
			// }, "Dummy Parallel Sum");
			// Time(() => {
			// 	var result = GetSum3(array);
			// 	Console.WriteLine("Result: " + result);
			// }, "Smarty Parallel Sum");
			// Time(() => {
			// 	var result = GetSum5(array);
			// 	Console.WriteLine("Result: " + result);
			// }, "PLINQ Sum");

			var randomArray = InitializeRandomArrayOfNumbers();
			Time(() => {
				var result = GetMostFrequentNumber1(randomArray);
				Console.WriteLine("Result: " + string.Join(", ", result));
			}, "Most Frequent Number");
		}

		static decimal[] InitializeLargeArrayOfNumbers() {
			var array = new decimal[LARGE_ARRAY_SIZE];
			for (int i = 0; i < array.Length; i++) {
				array[i] = i;
			}
			return array;
		}

		static int[] InitializeRandomArrayOfNumbers() {
			var array = new int[LARGE_ARRAY_SIZE];
			var random = new Random();
			for (int i = 0; i < array.Length; i++) {
				array[i] = random.Next(MAX_RANDOM_VALUE);
			}
			return array;
		}

		static decimal GetSum1(decimal[] numbers) {
			decimal sum = 0;
			foreach (var number in numbers) {
				sum += number;
			}
			return sum;
		}

		static decimal GetSum2(decimal[] numbers) {
			decimal sum = 0;
			Parallel.ForEach(
				Partitioner.Create(0, numbers.Length),
				(range) => {
					decimal localSum = 0;
					for (int i = range.Item1; i < range.Item2; i++) {
						localSum += numbers[i];
					}
					lock (lockObject) {
						sum += localSum;
					}
				}
			);
			return sum;
		}

		static decimal GetSum3(decimal[] numbers) {
			decimal sum = 0;
			Parallel.ForEach(
				Partitioner.Create(0, numbers.Length),
				() => new Decimal(0),
				(range, loopState, localSum) => {
					for (int i = range.Item1; i < range.Item2; i++) {
						localSum += numbers[i];
					}
					return localSum;
				},
				(localSum) => {
					lock (lockObject) {
						sum += localSum;
					}
				}
			);
			return sum;
		}

		static decimal GetSum4(decimal[] numbers) {
			decimal sum = 0;
			Parallel.ForEach(
				Partitioner.Create(0, numbers.Length),
				(range) => {
					decimal localSum = 0;
					for (int i = range.Item1; i < range.Item2; i++) {
						localSum += numbers[i];
					}
					lock (lockObject) {
						sum += localSum;
					}
				}
			);
			return sum;
		}

		static decimal GetSum5(decimal[] numbers) {
			return (from number in numbers.AsParallel() select number).Aggregate(new Decimal(0), (acc, val) => {
				return acc += val;
			});
		}

		static int[] GetMostFrequentNumber1(int[] numbers) {
			Dictionary<int, int> results = new Dictionary<int, int>();

			// MAP & REDUCE
			Parallel.ForEach(
				Partitioner.Create(0, numbers.Length),
				() => new Dictionary<int, int>(),
				(range, loopState, localResults) => {
					for (int i = range.Item1; i < range.Item2; i++) {
						if (localResults.ContainsKey(numbers[i])) {
							localResults[numbers[i]] = localResults[numbers[i]]++;
						} else {
							localResults[numbers[i]] = 1;
						}
					}
					return localResults;
				},
				(localResults) => {
					foreach (int key in localResults.Keys) {
						if (results.ContainsKey(key)) {
							lock (lockObject) {
								results[key] = results[key] += localResults[key];
							}
						} else {
							lock (lockObject) {
								results[key] = 1;
							}
						}
					}
				}
			);

			// POSTPROCESS
			return (from key in results.Keys orderby results[key] descending select key).Take(10).ToArray();
		}

		static void Time(Action action, String taskName) {
			Stopwatch watch = Stopwatch.StartNew();
			action.Invoke();
			watch.Stop();
			Console.WriteLine(taskName + " - Time elapsed: " + watch.ElapsedMilliseconds);
		}
	}
}
