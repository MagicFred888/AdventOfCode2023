using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode2023.Solver
{
    enum RoundId
    {
        FirstRound = 0,
        SecondRound = 1,
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------
    internal abstract class BaseSolver
    {
        private int day = -1;
        private class DataSet
        {
            public string TestFileName = string.Empty;
            public List<string> Data = new List<string>();
            public string[] RoundIdAnswers = new string[2];
        }

        // To be used by children class to solve challenges
        protected List<string> rawData = new List<string>();

        // For internal use only
        private List<string> challengeData = new List<string>();
        private List<DataSet> sampleDataSet = new List<DataSet>();
        private Stopwatch stopwatch = new Stopwatch();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public BaseSolver(int day)
        {
            // Base data folder
            this.day = day;
            challengeData.Clear();
            sampleDataSet.Clear();
            string dataFolder = @$"..\..\..\Data\Day{day.ToString("00")}\";

            // Load challenge data
            string challengeDataFilePath = $"{dataFolder}Challenge.txt";
            if (File.Exists(challengeDataFilePath)) challengeData = File.ReadAllLines(challengeDataFilePath).ToList();

            // Load sample data
            string[] sampleDataFilePath = Directory.GetFiles(dataFolder, "Sample*.txt");
            foreach (string filePath in sampleDataFilePath)
            {
                // Split file name
                string cleanFileName = Path.GetFileNameWithoutExtension(filePath);
                cleanFileName = cleanFileName.Substring(cleanFileName.IndexOf("_") + 1);
                string[] answers = cleanFileName.Split('_');

                // Load, check and save sample
                if (answers.Length > 0)
                {
                    DataSet newSet = new DataSet();
                    newSet.TestFileName = Path.GetFileName(filePath);
                    newSet.Data = File.ReadAllLines(filePath).ToList();
                    if (answers.Length >= 1 && answers[0] != "X") newSet.RoundIdAnswers[0] = answers[0];
                    if (answers.Length >= 2 && answers[1] != "X") newSet.RoundIdAnswers[1] = answers[1];
                    if (newSet.RoundIdAnswers[0] != null || newSet.RoundIdAnswers[1] != null) sampleDataSet.Add(newSet);
                }
            }

            // Print title
            Console.WriteLine("");
            string title = $"Day {day}: {PuzzleTitle}";
            Console.WriteLine(title);
            Console.WriteLine(new string('^', title.Length));
            Console.WriteLine("");
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool SolveSample(RoundId roundId, out string[] resultString)
        {
            // Test sample
            bool allTestPassed = true;
            List<string> results = new List<string>();
            int testId = 1;
            foreach (DataSet ds in sampleDataSet)
            {
                // Check if must be tested 
                if (ds.RoundIdAnswers[(int)roundId] == null) continue;

                // Do test
                rawData = ds.Data;
                stopwatch.Restart();
                string answer = roundId == RoundId.FirstRound ? GetSolution1() : GetSolution2();
                stopwatch.Stop();
                if (answer == ds.RoundIdAnswers[(int)roundId])
                {
                    results.Add($"SAMPLE {testId} PASSED: {answer} found in {GetProperUnitAndRounding(stopwatch.Elapsed.TotalMilliseconds)}");
                }
                else
                {
                    results.Add($"SAMPLE {testId} FAILED: Found {answer} instead of {ds.RoundIdAnswers[(int)roundId]}");
                    allTestPassed = false;
                }
            }

            // Give result
            resultString = results.ToArray();
            return allTestPassed;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool SolveChallenge(RoundId roundId, out string resultString)
        {
            // Test sample
            rawData = challengeData;
            if (challengeData.Count == 0)
            {
                resultString = $"NO CHALLENGE DATA FOUND ! Please make sure you save your puzzle input into Data\\Day{day.ToString("00")}\\Challenge.txt !";
                return false;
            }
            stopwatch.Restart();
            string answer = roundId == RoundId.FirstRound ? GetSolution1() : GetSolution2();
            stopwatch.Stop();
            resultString = $"{answer} found in {GetProperUnitAndRounding(stopwatch.Elapsed.TotalMilliseconds)}";
            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected void DebugPrint(object[,] tmpTable, Dictionary<string, string> convDic, string missing)
        {
            // To visualize Matrix
            Debug.WriteLine("");
            for (int y = 0; y <= tmpTable.GetUpperBound(1); y++)
            {
                string line = string.Empty;
                for (int x = 0; x <= tmpTable.GetUpperBound(0); x++)
                {
                    string tmpVal = tmpTable[x, y].ToString();

                    if (convDic == null)
                    {
                        line += tmpVal;
                    }
                    else
                    {
                        if (convDic.ContainsKey(tmpVal))
                        {
                            line += convDic[tmpVal];
                        }
                        else
                        {
                            line += missing;
                        }
                    }
                }
                Debug.WriteLine(line);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private string GetProperUnitAndRounding(double totalMilliseconds)
        {
            // Change scale
            double duration = totalMilliseconds;
            string unit = "[ms]";
            if (totalMilliseconds < 1)
            {
                duration *= 1000;
                unit = "[μs]";
            }
            else if (totalMilliseconds >= 1000)
            {
                duration /= 1000;
                unit = "[s]";
            }

            // Choose rounding
            int nbrOfDecimals = 1;
            if (duration < 100 && duration >= 10)
            {
                nbrOfDecimals = 2;
            }
            else if (duration < 10)
            {
                nbrOfDecimals = 3;
            }

            // Done
            return $"{Math.Round(duration, nbrOfDecimals).ToString()} {unit}";
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract string PuzzleTitle { get; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract string GetSolution1();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract string GetSolution2();
    }
}