using System;
using System.Text;

namespace lab1_shumeiko_v11
{
    internal class Program
    {
        // --- ЗАДАЧА 1: МЕТОД НЬЮТОНА ---

        // Функція f(x) для першого рівняння
        public static double F1(double x)
        {
            return Math.Pow(x, 3) - 4 * Math.Pow(x, 2) - 15 * x + 18;
        }

        // Перша похідна f'(x) для першого рівняння
        public static double F1_prime(double x)
        {
            return 3 * Math.Pow(x, 2) - 8 * x - 15;
        }

        // Друга похідна f''(x) для першого рівняння
        public static double F1_double_prime(double x)
        {
            return 6 * x - 8;
        }

        // Реалізація методу Ньютона
        public static void SolveWithNewtonMethod(double epsilon)
        {
            Console.WriteLine("--- Задача 1: Метод Ньютона ---");
            Console.WriteLine("Рівняння: x^3 - 4x^2 - 15x + 18 = 0");

            double x0 = -4.0; // Початкове наближення

            // Апріорна оцінка кількості ітерацій
            // Для методу Ньютона n >= log2(log(|x0-x*|/epsilon)) + 1, що складно оцінити без x*
            // Використаємо іншу оцінку, пов'язану з m1 та M2
            // n >= log2((log(epsilon_0 / epsilon) / log(M2 / (2*m1))) + 1) - теж складно
            // Простіша оцінка: |x_n - x*| <= (M2 / 2m1) * |x_{n-1} - x*|^2
            // Для квадратичної збіжності, кількість правильних знаків подвоюється.
            // Це не дає точної формули для n, але демонструє швидкість.
            // Однак, можна зробити грубу оцінку, припускаючи, що похибка зменшується квадратично.
            // |x1 - x0| = 0.769; |x0 - x*| ~ 1.
            // |e_k+1| ~ C * |e_k|^2.
            // log|e_k+1| ~ log(C) + 2*log|e_k|.
            // Це показує, що кількість ітерацій буде дуже малою.
            // Формула для апріорної оцінки: n >= log2(log(|b-a|/epsilon)/log(2))
            Console.WriteLine($"\nАпріорна оцінка:");
            Console.WriteLine("Метод Ньютона має квадратичну швидкість збіжності.");
            Console.WriteLine("Очікується дуже мала кількість ітерацій (зазвичай 4-7) для досягнення високої точності.");


            double x_current = x0;
            double x_next;
            int iteration = 0;

            Console.WriteLine("\nПрактичний розрахунок:");
            Console.WriteLine("k | x_k | f(x_k) | f'(x_k) | x_{k+1} | |x_{k+1}-x_k|");
            Console.WriteLine("----------------------------------------------------------------------------------");

            do
            {
                double fx = F1(x_current);
                double f_prime_x = F1_prime(x_current);

                if (Math.Abs(f_prime_x) < 1e-10)
                {
                    Console.WriteLine("Похідна близька до нуля. Метод не може бути застосований.");
                    return;
                }

                x_next = x_current - fx / f_prime_x;

                Console.WriteLine($"{iteration,-2} | {x_current,12:F8} | {fx,13:F8} | {f_prime_x,13:F8} | {x_next,13:F8} | {Math.Abs(x_next - x_current),12:F8}");

                if (Math.Abs(x_next - x_current) < epsilon)
                {
                    break;
                }

                x_current = x_next;
                iteration++;

            } while (iteration < 100); // Обмеження на кількість ітерацій

            Console.WriteLine("----------------------------------------------------------------------------------");
            Console.WriteLine($"Результат: Мінімальний корінь x* = {x_next:F6}");
            Console.WriteLine($"Кількість ітерацій: {iteration + 1}\n");
        }


        // --- ЗАДАЧА 2: МЕТОД ПРОСТОЇ ІТЕРАЦІЇ ---

        // Ітераційна функція phi(x) для другого рівняння
        public static double Phi2(double x)
        {
            // x = (10x^2 - 11x - 70)^(1/3)
            double expression = 10 * Math.Pow(x, 2) - 11 * x - 70;
            return Math.Sign(expression) * Math.Pow(Math.Abs(expression), 1.0 / 3.0);
        }

        // Похідна ітераційної функції phi'(x)
        public static double Phi2_prime(double x)
        {
            double base_expr = 10 * Math.Pow(x, 2) - 11 * x - 70;
            if (Math.Abs(base_expr) < 1e-10) return double.MaxValue; // Уникаємо ділення на нуль
            double derivative_numerator = 20 * x - 11;
            double derivative_denominator = 3 * Math.Pow(base_expr, 2.0 / 3.0);
            return derivative_numerator / derivative_denominator;
        }


        // Реалізація методу простої ітерації
        public static void SolveWithSimpleIteration(double epsilon)
        {
            Console.WriteLine("--- Задача 2: Метод простої ітерації ---");
            Console.WriteLine("Рівняння: x^3 - 10x^2 + 11x + 70 = 0");

            double x0 = 6.5; // Початкове наближення
            double a = 6.5, b = 7.5; // Інтервал ізоляції

            // Апріорна оцінка кількості ітерацій
            // n >= (ln(|x1-x0|/epsilon) + ln(1/(1-q))) / ln(1/q)
            // q = max|phi'(x)| на [a, b]
            double q = Math.Max(Math.Abs(Phi2_prime(a)), Math.Abs(Phi2_prime(b)));
            double x1 = Phi2(x0);
            double diff_x1_x0 = Math.Abs(x1 - x0);

            int n_apriori = 0;
            if (q < 1 && diff_x1_x0 > 0)
            {
                // Формула: |x_n - x*| <= (q^n / (1-q)) * |x1 - x0|.
                // Треба знайти n, щоб (q^n / (1-q)) * |x1 - x0| < epsilon
                // q^n < epsilon * (1-q) / |x1 - x0|
                // n * log(q) < log(epsilon * (1-q) / |x1 - x0|)
                // n > log(epsilon * (1-q) / |x1 - x0|) / log(q)
                double required_val = epsilon * (1 - q) / diff_x1_x0;
                n_apriori = (int)Math.Ceiling(Math.Log(required_val) / Math.Log(q));
            }

            Console.WriteLine($"\nАпріорна оцінка:");
            Console.WriteLine($"Коефіцієнт стиснення q на інтервалі [{a}, {b}] ~ {q:F4}");
            if (n_apriori > 0)
            {
                Console.WriteLine($"Очікувана кількість ітерацій для досягнення точності {epsilon}: {n_apriori}");
            }
            else
            {
                Console.WriteLine("Неможливо розрахувати апріорну кількість ітерацій (q >= 1 або початкове наближення не змінюється).");
            }


            double x_current = x0;
            double x_next;
            int iteration = 0;

            Console.WriteLine("\nПрактичний розрахунок:");
            Console.WriteLine("k | x_k | x_{k+1}=phi(x_k) | |x_{k+1}-x_k|");
            Console.WriteLine("-------------------------------------------------------");

            do
            {
                x_next = Phi2(x_current);

                Console.WriteLine($"{iteration,-2} | {x_current,12:F8} | {x_next,16:F8} | {Math.Abs(x_next - x_current),12:F8}");

                if (Math.Abs(x_next - x_current) < epsilon)
                {
                    break;
                }

                x_current = x_next;
                iteration++;

            } while (iteration < 100); // Обмеження на кількість ітерацій

            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine($"Результат: Максимальний корінь x* = {x_next:F6}");
            Console.WriteLine($"Кількість ітерацій: {iteration + 1}\n");
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            double epsilon; // Точність
            Console.WriteLine("Введіть точність:");
            epsilon = Convert.ToDouble(Console.ReadLine());
            SolveWithNewtonMethod(epsilon);
            SolveWithSimpleIteration(epsilon);
        }
    }
}
