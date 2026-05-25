using System;
using System.Globalization;

class Program
{
    const double EPS = 1e-10;

    static int n;
    static double[,] cost;

    static double[,] simplexTable;
    static string[] rowLabels;
    static string[] colLabels;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        while (true)
        {
            Console.WriteLine("\nПРАКТИЧНА РОБОТА №6");
            Console.WriteLine("РОЗВ'ЯЗАННЯ ЗАДАЧІ ПРО ПРИЗНАЧЕННЯ");
            Console.WriteLine("1 - Ввести матрицю вручну");
            Console.WriteLine("2 - Завантажити мій варіант 12");
            Console.WriteLine("3 - Показати матрицю");
            Console.WriteLine("4 - Розв'язати угорським методом");
            Console.WriteLine("5 - Розв'язати симплекс-методом через МЖВ");
            Console.WriteLine("0 - Вихід");
            Console.Write("Ваш вибір: ");

            int choice = int.Parse(Console.ReadLine());

            if (choice == 0)
                break;

            switch (choice)
            {
                case 1:
                    InputMatrix();
                    break;
                case 2:
                    LoadVariant12();
                    break;
                case 3:
                    if (CheckData()) PrintMatrix(cost);
                    break;
                case 4:
                    if (CheckData()) SolveHungarian();
                    break;
                case 5:
                    if (CheckData()) SolveSimplexByMJV();
                    break;
                default:
                    Console.WriteLine("Невірний вибір.");
                    break;
            }
        }
    }

    static void InputMatrix()
    {
        Console.Write("Введіть розмір квадратної матриці n: ");
        n = int.Parse(Console.ReadLine());

        cost = new double[n, n];

        Console.WriteLine("\nВведіть матрицю вартостей:");

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write($"C[{i + 1},{j + 1}] = ");
                cost[i, j] = double.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("Матрицю збережено.");
    }

    static void LoadVariant12()
    {
        n = 4;

        cost = new double[,]
        {
            { 11,  5,  9, 10 },
            {  6,  9,  6,  5 },
            {  7, 11, 10,  8 },
            { 10,  5,  7,  9 }
        };

        Console.WriteLine("Завантажено варіант 12.");
    }

    static bool CheckData()
    {
        if (cost == null)
        {
            Console.WriteLine("Спочатку потрібно ввести або завантажити матрицю.");
            return false;
        }

        return true;
    }






    static void SolveHungarian()
    {
        Console.WriteLine("\nЗгенерований протокол обчислення:");
        Console.WriteLine("\nМатриця вартостей:");
        PrintMatrix(cost);

        double[,] work = CopyMatrix(cost);

        Console.WriteLine("Пошук мінімальних елементів у кожному рядку:");

        for (int i = 0; i < n; i++)
        {
            double min = work[i, 0];

            for (int j = 1; j < n; j++)
                if (work[i, j] < min)
                    min = work[i, j];

            Console.WriteLine($"В рядку {i + 1} знайдено min: {min:F0}");

            for (int j = 0; j < n; j++)
                work[i, j] -= min;
        }

        Console.WriteLine("\nМатриця після віднімання мінімальних елементів у рядках:");
        PrintMatrix(work);

        Console.WriteLine("Пошук мінімальних елементів у кожному стовпці:");

        for (int j = 0; j < n; j++)
        {
            double min = work[0, j];

            for (int i = 1; i < n; i++)
                if (work[i, j] < min)
                    min = work[i, j];

            Console.WriteLine($"В стовпці {j + 1} знайдено min: {min:F0}");

            for (int i = 0; i < n; i++)
                work[i, j] -= min;
        }

        Console.WriteLine("\nМатриця після віднімання мінімальних елементів у стовпцях:");
        PrintMatrix(work);

        Console.WriteLine("Пошук оптимальних призначень:");

        int[] assignment = HungarianAlgorithm(cost);

        Console.WriteLine("\nМатриця вартостей, в якій відмічено призначення:");

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (assignment[i] == j)
                    Console.Write($"[{cost[i, j]:F0}]".PadLeft(8));
                else
                    Console.Write($"{cost[i, j]:F0}".PadLeft(8));
            }

            Console.WriteLine();
        }

        PrintAssignmentResult(assignment, "угорським методом");
    }

    static int[] HungarianAlgorithm(double[,] matrix)
    {
        int size = matrix.GetLength(0);

        double[] u = new double[size + 1];
        double[] v = new double[size + 1];
        int[] p = new int[size + 1];
        int[] way = new int[size + 1];

        for (int i = 1; i <= size; i++)
        {
            p[0] = i;
            int j0 = 0;

            double[] minv = new double[size + 1];
            bool[] used = new bool[size + 1];

            for (int j = 1; j <= size; j++)
                minv[j] = double.PositiveInfinity;

            do
            {
                used[j0] = true;

                int i0 = p[j0];
                double delta = double.PositiveInfinity;
                int j1 = 0;

                for (int j = 1; j <= size; j++)
                {
                    if (used[j]) continue;

                    double cur = matrix[i0 - 1, j - 1] - u[i0] - v[j];

                    if (cur < minv[j])
                    {
                        minv[j] = cur;
                        way[j] = j0;
                    }

                    if (minv[j] < delta)
                    {
                        delta = minv[j];
                        j1 = j;
                    }
                }

                for (int j = 0; j <= size; j++)
                {
                    if (used[j])
                    {
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        minv[j] -= delta;
                    }
                }

                j0 = j1;
            }
            while (p[j0] != 0);

            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            }
            while (j0 != 0);
        }

        int[] assignment = new int[size];

        for (int j = 1; j <= size; j++)
            assignment[p[j] - 1] = j - 1;

        return assignment;
    }






    static void SolveSimplexByMJV()
    {
        Console.WriteLine("\nЗгенерований протокол обчислення:");
        Console.WriteLine("\nМатриця вартостей:");
        PrintMatrix(cost);

        Console.WriteLine("Пошук оптимального розв'язку задачі лінійного програмування:");
        PrintLpStatement();

        BuildSimplexTable();

        Console.WriteLine("\nВхідна симплекс-таблиця:");
        PrintSimplexTable();

        Console.WriteLine("\nПошук опорного розв'язку:");
        bool supportFound = FindSupportSolution();

        if (!supportFound)
        {
            Console.WriteLine("Опорний розв'язок не знайдено.");
            return;
        }

        Console.WriteLine("\nЗнайдено опорний розв'язок:");
        PrintCurrentX();

        Console.WriteLine("\nПошук оптимального розв'язку:");
        bool optimalFound = FindOptimalSolution();

        if (!optimalFound)
        {
            Console.WriteLine("Оптимальний розв'язок не знайдено.");
            return;
        }

        Console.WriteLine("\nЗнайдено оптимальний розв'язок:");
        PrintCurrentX();

        int[] assignment = GetAssignmentFromTable();

        PrintAssignmentResult(assignment, "симплекс-методом через МЖВ");
    }

    static void BuildSimplexTable()
    {
        int variableCount = n * n;
        int rows = 2 * n + 1;
        int cols = variableCount + 1;

        simplexTable = new double[rows, cols];
        rowLabels = new string[rows];
        colLabels = new string[cols];

        for (int j = 0; j < variableCount; j++)
            colLabels[j] = $"-x{j + 1}";

        colLabels[cols - 1] = "1";

        int row = 0;

        for (int i = 0; i < n; i++)
        {
            rowLabels[row] = $"y{row + 1}";

            for (int j = 0; j < n; j++)
                simplexTable[row, Index(i, j)] = 1;

            simplexTable[row, cols - 1] = 1;
            row++;
        }

        for (int j = 0; j < n; j++)
        {
            rowLabels[row] = $"y{row + 1}";

            for (int i = 0; i < n; i++)
                simplexTable[row, Index(i, j)] = -1;

            simplexTable[row, cols - 1] = -1;
            row++;
        }

        rowLabels[rows - 1] = "Z";

        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                simplexTable[rows - 1, Index(i, j)] = cost[i, j];

        simplexTable[rows - 1, cols - 1] = 0;
    }

    static bool FindSupportSolution()
    {
        while (true)
        {
            int pivotRow = FindNegativeFreeTermRow();

            if (pivotRow == -1)
                return true;

            int pivotCol = FindNegativeElementInRow(pivotRow);

            if (pivotCol == -1)
                return false;

            Console.WriteLine($"\nРозв'язувальний рядок: {rowLabels[pivotRow]}");
            Console.WriteLine($"Розв'язувальний стовпець: {colLabels[pivotCol]}");

            ModifiedJordanStep(pivotRow, pivotCol);
            PrintSimplexTable();
        }
    }

    static bool FindOptimalSolution()
    {
        while (true)
        {
            int pivotCol = FindNegativeElementInZRow();

            if (pivotCol == -1)
                return true;

            int pivotRow = FindResolvingRowForOptimal(pivotCol);

            if (pivotRow == -1)
                return false;

            Console.WriteLine($"\nРозв'язувальний рядок: {rowLabels[pivotRow]}");
            Console.WriteLine($"Розв'язувальний стовпець: {colLabels[pivotCol]}");

            ModifiedJordanStep(pivotRow, pivotCol);
            PrintSimplexTable();
        }
    }

    static int FindNegativeFreeTermRow()
    {
        int lastCol = simplexTable.GetLength(1) - 1;
        int zRow = simplexTable.GetLength(0) - 1;

        for (int i = 0; i < zRow; i++)
        {
            if (simplexTable[i, lastCol] < -EPS)
                return i;
        }

        return -1;
    }

    static int FindNegativeElementInRow(int row)
    {
        int lastCol = simplexTable.GetLength(1) - 1;

        for (int j = 0; j < lastCol; j++)
        {
            if (simplexTable[row, j] < -EPS)
                return j;
        }

        return -1;
    }

    static int FindNegativeElementInZRow()
    {
        int zRow = simplexTable.GetLength(0) - 1;
        int lastCol = simplexTable.GetLength(1) - 1;

        for (int j = 0; j < lastCol; j++)
        {
            if (simplexTable[zRow, j] < -EPS)
                return j;
        }

        return -1;
    }

    static int FindResolvingRowForOptimal(int pivotCol)
    {
        int lastCol = simplexTable.GetLength(1) - 1;
        int zRow = simplexTable.GetLength(0) - 1;

        int bestRow = -1;
        double bestRatio = double.MaxValue;

        for (int i = 0; i < zRow; i++)
        {
            if (simplexTable[i, pivotCol] > EPS)
            {
                double ratio = simplexTable[i, lastCol] / simplexTable[i, pivotCol];

                if (ratio < bestRatio)
                {
                    bestRatio = ratio;
                    bestRow = i;
                }
            }
        }

        return bestRow;
    }

    static void ModifiedJordanStep(int pivotRow, int pivotCol)
    {
        int rows = simplexTable.GetLength(0);
        int cols = simplexTable.GetLength(1);

        double[,] old = CopyMatrix(simplexTable);
        double pivot = old[pivotRow, pivotCol];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (i == pivotRow && j == pivotCol)
                {
                    simplexTable[i, j] = 1.0 / pivot;
                }
                else if (i == pivotRow)
                {
                    simplexTable[i, j] = old[i, j] / pivot;
                }
                else if (j == pivotCol)
                {
                    simplexTable[i, j] = -old[i, j] / pivot;
                }
                else
                {
                    simplexTable[i, j] =
                        (old[i, j] * pivot - old[i, pivotCol] * old[pivotRow, j]) / pivot;
                }
            }
        }

        string oldRowLabel = rowLabels[pivotRow];

        if (colLabels[pivotCol].StartsWith("-"))
            rowLabels[pivotRow] = colLabels[pivotCol].Substring(1);
        else
            rowLabels[pivotRow] = colLabels[pivotCol];

        colLabels[pivotCol] = "-" + oldRowLabel;
    }

    static int[] GetAssignmentFromTable()
    {
        int[] assignment = new int[n];

        for (int i = 0; i < n; i++)
            assignment[i] = -1;

        int lastCol = simplexTable.GetLength(1) - 1;
        int zRow = simplexTable.GetLength(0) - 1;

        for (int i = 0; i < zRow; i++)
        {
            if (rowLabels[i].StartsWith("x"))
            {
                int variableNumber = int.Parse(rowLabels[i].Substring(1)) - 1;

                if (simplexTable[i, lastCol] > 0.5)
                {
                    int worker = variableNumber / n;
                    int job = variableNumber % n;

                    assignment[worker] = job;
                }
            }
        }

        return assignment;
    }





    static void PrintLpStatement()
    {
        Console.WriteLine("\nПостановка задачі:");

        Console.Write("Z = ");

        bool first = true;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (!first)
                    Console.Write(" + ");

                Console.Write($"{cost[i, j]:F0}x{Index(i, j) + 1}");
                first = false;
            }
        }

        Console.WriteLine(" -> min");

        Console.WriteLine("\nПерехід до задачі максимізації функції мети Z':");

        Console.Write("Z' = ");

        first = true;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (!first)
                    Console.Write(" ");

                Console.Write($"- {cost[i, j]:F0}x{Index(i, j) + 1}");
                first = false;
            }
        }

        Console.WriteLine(" -> max");

        Console.WriteLine("\nОбмеження:");

        for (int i = 0; i < n; i++)
        {
            Console.Write("- ");

            for (int j = 0; j < n; j++)
            {
                if (j > 0)
                    Console.Write(" - ");

                Console.Write($"x{Index(i, j) + 1}");
            }

            Console.WriteLine(" + 1 >= 0");
        }

        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                    Console.Write(" + ");

                Console.Write($"x{Index(i, j) + 1}");
            }

            Console.WriteLine(" - 1 >= 0");
        }

        Console.WriteLine("\nx[j]>=0, j=1," + (n * n));
    }

    static void PrintSimplexTable()
    {
        int rows = simplexTable.GetLength(0);
        int cols = simplexTable.GetLength(1);

        Console.WriteLine("\nСимплекс-таблиця:");

        Console.Write("        ");

        for (int j = 0; j < cols; j++)
            Console.Write($"{colLabels[j],8}");

        Console.WriteLine();

        Console.WriteLine(new string('-', 10 + cols * 8));

        for (int i = 0; i < rows; i++)
        {
            Console.Write($"{rowLabels[i],6} = ");

            for (int j = 0; j < cols; j++)
                Console.Write($"{simplexTable[i, j],8:F2}");

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    static void PrintCurrentX()
    {
        int variableCount = n * n;
        double[] x = new double[variableCount];

        int lastCol = simplexTable.GetLength(1) - 1;
        int zRow = simplexTable.GetLength(0) - 1;

        for (int i = 0; i < zRow; i++)
        {
            if (rowLabels[i].StartsWith("x"))
            {
                int index = int.Parse(rowLabels[i].Substring(1)) - 1;

                if (index >= 0 && index < variableCount)
                    x[index] = simplexTable[i, lastCol];
            }
        }

        Console.Write("X = (");

        for (int i = 0; i < variableCount; i++)
        {
            Console.Write($"{x[i]:F2}");

            if (i < variableCount - 1)
                Console.Write("; ");
        }

        Console.WriteLine(")");
    }

    static void PrintAssignmentResult(int[] assignment, string methodName)
    {
        Console.WriteLine($"\nМатриця призначень, отримана {methodName}:");

        int[,] matrix = new int[n, n];
        double total = 0;

        for (int i = 0; i < n; i++)
        {
            int j = assignment[i];

            if (j >= 0)
            {
                matrix[i, j] = 1;
                total += cost[i, j];
            }
        }

        PrintAssignmentMatrix(matrix);

        Console.WriteLine("\nОптимальні призначення:");

        for (int i = 0; i < n; i++)
        {
            int j = assignment[i];

            if (j >= 0)
                Console.WriteLine($"A{i + 1} -> B{j + 1}, вартість = {cost[i, j]:F2}");
        }

        Console.WriteLine("\nЗагальна вартість робіт:");
        Console.Write("S = ");

        for (int i = 0; i < n; i++)
        {
            int j = assignment[i];

            if (j >= 0)
            {
                Console.Write($"{cost[i, j]:F2}");

                if (i < n - 1)
                    Console.Write(" + ");
            }
        }

        Console.WriteLine($" = {total:F2}");
    }

    static void PrintMatrix(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
                Console.Write($"{matrix[i, j],8:F2}");

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    static void PrintAssignmentMatrix(int[,] matrix)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write($"{matrix[i, j],5}");

            Console.WriteLine();
        }
    }





    static double[,] CopyMatrix(double[,] source)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);

        double[,] copy = new double[rows, cols];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                copy[i, j] = source[i, j];

        return copy;
    }

    static int Index(int i, int j)
    {
        return i * n + j;
    }
}