using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace AdventOfCode2023.Solver
{
    internal class Day12 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Hot Springs";

        private class BatchInfos
        {
            public List<int> CorruptedLog { get; private set; } = new List<int>(); // 1 = operational (.), 0 = damaged (#), -1 = Unknow (?), 
            public List<int> DamagedPartsBatchInfo { get; private set; } = new List<int>();

            private static Dictionary<string, long> cacheDic = new Dictionary<string, long>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public BatchInfos(string rawData)
            {
                CorruptedLog = Array.ConvertAll<char, int>(rawData.Split(' ')[0].ToCharArray(), i => i == '#' ? 0 : (i == '.' ? 1 : -1)).ToList();
                DamagedPartsBatchInfo = Array.ConvertAll<string, int>(rawData.Split(' ')[1].Split(","), i => int.Parse(i)).ToList();
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public long NbrOfPossibleArrangements(int multiFactor)
            {
                if (multiFactor > 1)
                {
                    int iniSize = CorruptedLog.Count;
                    for (int i = 0; i < multiFactor - 1; i++)
                    {
                        CorruptedLog.Add(-1);
                        CorruptedLog.AddRange(CorruptedLog.GetRange(0, iniSize));
                    }
                    iniSize = DamagedPartsBatchInfo.Count;
                    for (int i = 0; i < multiFactor - 1; i++)
                    {
                        DamagedPartsBatchInfo.AddRange(DamagedPartsBatchInfo.GetRange(0, iniSize));
                    }
                }

                // Run the search
                List<int> data = new List<int>(CorruptedLog);
                return SolveThis(ref data, new Point());
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            private long SolveThis(ref List<int> corruptedLog, Point scanPos)
            {
                // Check if calculation is completed
                if (!corruptedLog.Contains(-1)) return 1;

                // Check if we have answer in cache
                string key = string.Join("", corruptedLog.GetRange(scanPos.X, corruptedLog.Count - scanPos.X)) + "+" + string.Join(".", DamagedPartsBatchInfo.GetRange(scanPos.Y, DamagedPartsBatchInfo.Count - scanPos.Y));
                if (cacheDic.ContainsKey(key))
                {
                    // Quick way
                    return cacheDic[key];
                }
                else
                {
                    // Calculation with 1
                    long answer = 0;
                    int firstUnknow = corruptedLog.FindIndex(x => x == -1);
                    Point scanPosBackup = new Point(scanPos.X, scanPos.Y);
                    corruptedLog[firstUnknow] = 1;
                    if (IsValid(ref corruptedLog, ref scanPosBackup)) answer += SolveThis(ref corruptedLog, scanPosBackup);

                    // Calculation with 0
                    corruptedLog[firstUnknow] = 0;
                    scanPosBackup = new Point(scanPos.X, scanPos.Y);
                    if (IsValid(ref corruptedLog, ref scanPosBackup)) answer += SolveThis(ref corruptedLog, scanPosBackup);

                    // Set data back, cache and return result
                    corruptedLog[firstUnknow] = -1;
                    cacheDic.Add(key, answer);
                    return answer;
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            private bool IsValid(ref List<int> newLog, ref Point startPos)
            {
                // Scan and compare
                Point newStartPos = new Point();
                int secSize = 0;
                int checkPos = startPos.Y;
                for (int pos = startPos.X; pos < newLog.Count; pos++)
                {
                    if (newLog[pos] == -1)
                    {
                        startPos = newStartPos;
                        return true;
                    }
                    if (newLog[pos] == 0)
                    {
                        secSize++;
                        if (checkPos >= DamagedPartsBatchInfo.Count || secSize > DamagedPartsBatchInfo[checkPos]) return false;
                    }
                    else if (secSize > 0)
                    {
                        if (checkPos >= DamagedPartsBatchInfo.Count)
                            return false;
                        if (secSize != DamagedPartsBatchInfo[checkPos])
                            return false;
                        secSize = 0;
                        checkPos++;
                        newStartPos = new Point(pos + 1, checkPos);
                    }
                }
                if (checkPos == DamagedPartsBatchInfo.Count - 1 && secSize == DamagedPartsBatchInfo[checkPos] ||
                    checkPos == DamagedPartsBatchInfo.Count && secSize == 0) return true;

                return false;
            }
        }

        private List<BatchInfos> allBatchInfos = new List<BatchInfos>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day12(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Done
            return allBatchInfos.Sum(bi => bi.NbrOfPossibleArrangements(1)).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Done
            return allBatchInfos.Sum(bi => bi.NbrOfPossibleArrangements(5)).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allBatchInfos.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allBatchInfos.Add(new BatchInfos(rawData[pos]));
            }
        }
    }
}