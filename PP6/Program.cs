using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PP6
{
    class Program
    {
        private static int[,] _matrix;
        private static int[] _vector;
        private static int[] _result;
        private static List<int> _resultList = new List<int>();

        static void Main(string[] args)
        {
            int size = 10000;
            int maxDegreesOfParallelism = 4;
            CalculateInOrder(size);
            CalculateInParallel(size, maxDegreesOfParallelism);
            CalculateInParallelWithForEach(size, maxDegreesOfParallelism);

        }

        private static void CalculateInOrder(int size)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeInputData(size);
            _result = MultiplicationHelper.Multiply(_matrix, _vector);
            stopWatch.Stop();
            Console.WriteLine($"[In-Order Algorithm] Time elapsed: {stopWatch.Elapsed} ms");
        }

        private static void CalculateInParallel(int size, int degreesOfParallelism)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeInputData(size);

            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = degreesOfParallelism
            };

            Parallel.For(0, size, options, (index) =>
            {
                var matrixRow = Enumerable.Range(0, _matrix.GetLength(1))
                    .Select(x => _matrix[index, x])
                    .ToArray();
                _result[index] = MultiplicationHelper.Multiply(matrixRow, _vector);
            });
            stopWatch.Stop();
            Console.WriteLine($"[Parallel Algorithm] Time elapsed: {stopWatch.Elapsed} ms");
        }

        private static void CalculateInParallelWithForEach(int size, int degreesOfParallelism)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeInputData(size);

            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = degreesOfParallelism
            };

            var matrixRowCollection = Enumerable.Range(0, _matrix.GetLength(0))
                .Select(row => 
                        Enumerable.Range(0, _matrix.GetLength(1))
                        .Select(column => _matrix[row, column])
                        .ToArray());

            Parallel.ForEach(matrixRowCollection, 
                options, 
                (row) =>
                {
                    var multiplicationResult = MultiplicationHelper.Multiply(row, _vector);
                    _resultList.Add(multiplicationResult);
                }
                );
            stopWatch.Stop();
            Console.WriteLine($"[Parallel Algorithm using a ForEach construct] Time elapsed: {stopWatch.Elapsed} ms");
        }

        private static void InitializeInputData(int vectorLength)
        {
            _matrix = DataGenerator.GenerateMatrix(vectorLength);
            _vector = DataGenerator.GenerateVector(vectorLength);
        }

        private static void PrintInput()
        {
            Console.WriteLine("Initial matrix:");
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    Console.Write($"{_matrix[i, j]}\t");
                }
                Console.Write('\n');
            }

            Console.WriteLine("Initial vector:");
            for (int i = 0; i < _vector.Length; i++)
            {
                Console.Write($"{_vector[i]}\t");
            }
            Console.Write('\n');
        }

        private static void PrintOutput()
        {

            Console.WriteLine("Resulting vector:");
            for (int i = 0; i < _result.Length; i++)
            {
                Console.Write($"{_result[i]}\t");
            }
            Console.Write('\n');
        }
    }
}

