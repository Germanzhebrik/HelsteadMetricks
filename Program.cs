using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HalsteadMetricsFromFile
{
    class Program
    {
        // Список операторов Rust
        private static readonly HashSet<string> RustOps = new HashSet<string>
        {
            "fn", "let", "mut", "if", "else", "match", "return", "struct", "impl", "impl",
            "for", "while", "loop", "in", "break", "continue", "println!", "Vec", "String",
            "+", "-", "*", "/", "%", "=", "==", "!=", ">", "<", ">=", "<=", "&&", "||",
            "->", "=>", "..", "::", ".", ",", ";", ":", "(", ")", "[", "]", "{", "}"
        };

        static void Main(string[] args)
        {
            // Имя файла, который лежит в папке с программой
            string fileName = "input.txt";

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Ошибка: Файл {fileName} не найден в папке с программой!");
                return;
            }

            // Читаем весь текст из файла
            string rustCode = File.ReadAllText(fileName);

            Analyze(rustCode);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void Analyze(string code)
        {
            // Очистка комментариев
            string cleanCode = Regex.Replace(code, @"//.*|/\*[\s\S]*?\*/", "");

            // Токенизация
            var tokens = Regex.Matches(cleanCode, @"("".*?""|[\w!]+|[^\w\s])")
                              .Cast<Match>()
                              .Select(m => m.Value)
                              .Where(v => !string.IsNullOrWhiteSpace(v))
                              .ToList();

            var opsMap = new Dictionary<string, int>();
            var opersMap = new Dictionary<string, int>();

            foreach (var t in tokens)
            {
                if (RustOps.Contains(t))
                {
                    if (!opsMap.ContainsKey(t)) opsMap[t] = 0;
                    opsMap[t]++;
                }
                else
                {
                    if (!opersMap.ContainsKey(t)) opersMap[t] = 0;
                    opersMap[t]++;
                }
            }

            // Метрики
            int n1 = opsMap.Count;
            int n2 = opersMap.Count;
            int N1 = opsMap.Values.Sum();
            int N2 = opersMap.Values.Sum();
            int n = n1 + n2;
            int N = N1 + N2;
            double V = N * Math.Log(n, 2);

            // Вывод таблицы
            Console.WriteLine("{0,-3} | {1,-12} | {2,-4} || {3,-3} | {4,-12} | {5,-4}", "j", "Оператор", "f1j", "i", "Операнд", "f2i");
            Console.WriteLine(new string('-', 60));

            var oL = opsMap.OrderByDescending(x => x.Value).ToList();
            var rL = opersMap.OrderByDescending(x => x.Value).ToList();
            for (int k = 0; k < Math.Max(n1, n2); k++)
            {
                Console.WriteLine("{0,-3} | {1,-12} | {2,-4} || {3,-3} | {4,-12} | {5,-4}",
                    k < n1 ? (k + 1).ToString() : "", k < n1 ? oL[k].Key : "", k < n1 ? oL[k].Value.ToString() : "",
                    k < n2 ? (k + 1).ToString() : "", k < n2 ? rL[k].Key : "", k < n2 ? rL[k].Value.ToString() : "");
            }

            Console.WriteLine($"\nСловарь (n): {n} | Длина (N): {N} | Объем (V): {V:F2}");
        }
    }
}