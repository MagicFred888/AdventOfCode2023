using AdventOfCode2023.Solver;
using System.Security.AccessControl;

namespace AdventOfCode2023
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            int currentDay = DateTime.Now.Day;
            bool currentDayIsSolved = false;

            do
            {
                // For fun
                // Thanks to https://patorjk.com/software/taag/#p=display&v=3&f=Big&t=Type%20Something%20
                Console.Clear();
                Console.WriteLine(@"                           _                 _    ____   __  _____          _        ___   ___ ___  ____   ");
                Console.WriteLine(@"                  /\      | |               | |  / __ \ / _|/ ____|        | |      |__ \ / _ \__ \|___ \  ");
                Console.WriteLine(@"                 /  \   __| |_   _____ _ __ | |_| |  | | |_| |     ___   __| | ___     ) | | | | ) | __) | ");
                Console.WriteLine(@"                / /\ \ / _` \ \ / / _ \ '_ \| __| |  | |  _| |    / _ \ / _` |/ _ \   / /| | | |/ / |__ <  ");
                Console.WriteLine(@"               / ____ \ (_| |\ V /  __/ | | | |_| |__| | | | |___| (_) | (_| |  __/  / /_| |_| / /_ ___) | ");
                Console.WriteLine(@"              /_/    \_\__,_| \_/ \___|_| |_|\__|\____/|_|  \_____\___/ \__,_|\___| |____|\___/____|____/  ");
                Console.WriteLine("");

                // Pre-select day (to save time during challenge)
                int day = -1;
                if (DateTime.Now.Year == 2023 && DateTime.Now.Month == 12) day = DateTime.Now.Day;
                if (day == currentDay && currentDayIsSolved) day = -1;

                // Create solver object
                BaseSolver? solver = null;
                do
                {
                    Type? targetType = Type.GetType($"AdventOfCode2023.Solver.Day{day.ToString("00")}");
                    if (targetType != null)
                    {
                        object? newInstance = Activator.CreateInstance(targetType, new object[] { day });
                        if (newInstance != null)
                        {
                            solver = (BaseSolver)newInstance;
                        }
                    }

                    if (solver == null)
                    {
                        Console.Write("Please select day challenge you want: ");
                        string? input = Console.ReadLine();
                        day = input != null && Microsoft.VisualBasic.Information.IsNumeric(input) ? int.Parse(input) : -1;
                        if (day == -1) Environment.Exit(0);
                    }
                } while (solver == null);

                // Solve all 
                string answer = string.Empty;
                string[] answers = null;
                bool status = false;
                do
                {
                    // Sample 1
                    Console.WriteLine("");
                    Console.WriteLine("Solving sample 1:");
                    status = solver.SolveSample(RoundId.FirstRound, out answers);
                    Console.WriteLine($"--> {string.Join("\r\n--> ", answers)}");
                    if (!status) break;

                    // Challenge 1
                    Console.WriteLine("");
                    Console.WriteLine("Solving challenge 1:");
                    status = solver.SolveChallenge(RoundId.FirstRound, out answer);
                    Console.WriteLine($"--> {answer}");
                    if (!status) break;

                    // Sample 2
                    Console.WriteLine("");
                    Console.WriteLine("Solving sample 2:");
                    status = solver.SolveSample(RoundId.SecondRound, out answers);
                    Console.WriteLine($"--> {string.Join("\r\n--> ", answers)}");
                    if (!status) break;

                    // Challenge 2
                    Console.WriteLine("");
                    Console.WriteLine("Solving challenge 2:");
                    status = solver.SolveChallenge(RoundId.SecondRound, out answer);
                    Console.WriteLine($"--> {answer}");
                    if (!status) break;

                    // All solved for today, let ask user choose another day
                    if (day == currentDay) currentDayIsSolved = true;

                } while (false);

                // Wait to quit
                Console.WriteLine("");
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            } while (true);
        }
    }
}