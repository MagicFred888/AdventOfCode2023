using System.Drawing;

namespace AdventOfCode2023.Solver
{
    internal class Day13 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Point of Incidence";

        public enum SymmetryType
        {
            Line = 0,
            Column = 1
        }

        private class PatternNote
        {
            public bool[,] field { get; private set; } = new bool[0, 0];

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public PatternNote(List<string> fieldData)
            {
                field = new bool[fieldData[0].Length, fieldData.Count];
                for (int y = 0; y < fieldData.Count; y++)
                {
                    for (int x = 0; x < fieldData[y].Length; x++)
                    {
                        field[x, y] = fieldData[y].Substring(x, 1) == "#";
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public int GetRow(int y)
            {
                int answer = 0;
                for (int x = 0; x <= field.GetUpperBound(0); x++)
                {
                    if (field[x, y])
                        answer = answer | 1 << (field.GetUpperBound(0) - x);
                }
                return answer;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public int GetCol(int x)
            {
                int answer = 0;
                for (int y = 0; y <= field.GetUpperBound(1); y++)
                {
                    if (field[x, y]) answer = answer | 1 << (field.GetUpperBound(1) - y);
                }
                return answer;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public List<int> GetSymmetries(SymmetryType symmetryType, Point? smudgePos = null)
            {
                // Get data with required smudge position
                List<int> data = new List<int>();
                if (smudgePos != null) field[smudgePos.Value.X, smudgePos.Value.Y] = !field[smudgePos.Value.X, smudgePos.Value.Y];
                if (symmetryType == SymmetryType.Line)
                {
                    for (int y = 0; y <= field.GetUpperBound(1); y++)
                    {
                        data.Add(GetRow(y));
                    }
                }
                else
                {
                    for (int x = 0; x <= field.GetUpperBound(0); x++)
                    {
                        data.Add(GetCol(x));
                    }
                }
                if (smudgePos != null) field[smudgePos.Value.X, smudgePos.Value.Y] = !field[smudgePos.Value.X, smudgePos.Value.Y];

                // Search all symetries in required direction
                List<int> result = new List<int>();
                for (int i = 0; i < data.Count - 1; i++)
                {
                    if (data[i] != data[i + 1]) continue;
                    bool ok = true;
                    for (int j = 1; j < data.Count; j++)
                    {
                        int x1 = i - j;
                        int x2 = i + 1 + j;
                        if (x1 < 0 || x2 >= data.Count) break;
                        if (data[x1] != data[x2])
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok && !result.Contains(i + 1)) result.Add(i + 1);
                }
                if (result.Count == 0) result.Add(0);
                return result;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public int GetSymmetriesWithSmudges(SymmetryType symmetryType)
            {
                // Scan to get all possible symetries
                List<int> possibleAnswers = new List<int>();
                for (int x = 0; x <= field.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= field.GetUpperBound(1); y++)
                    {
                        foreach (int i in GetSymmetries(symmetryType, new Point(x, y)))
                        {
                            if (!possibleAnswers.Contains(i)) possibleAnswers.Add(i);
                        }
                    }
                }

                // Clean results
                if (possibleAnswers.Count > 2 && possibleAnswers.Contains(0)) possibleAnswers.Remove(0); // 0 do not interrest us in this case
                if (possibleAnswers.Count == 2) // Need see if we can remove original value (we must find a new one...
                {
                    int ori = GetSymmetries(symmetryType)[0];
                    if (possibleAnswers.Contains(ori)) possibleAnswers.Remove(ori);
                }
                return possibleAnswers[0];
            }
        }

        private List<PatternNote> allPatternNotes = new List<PatternNote>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day13(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Done
            return allPatternNotes.Aggregate(0, (result, i) => result + 100 * i.GetSymmetries(SymmetryType.Line)[0] + i.GetSymmetries(SymmetryType.Column)[0]).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Get smudge position and solution with it
            int answer = 0;
            for (int i = 0; i < allPatternNotes.Count; i++)
            {
                int column = 0;
                int line = 100 * allPatternNotes[i].GetSymmetriesWithSmudges(SymmetryType.Line);
                if (line == 0) column = allPatternNotes[i].GetSymmetriesWithSmudges(SymmetryType.Column);
                answer += line + column;
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allPatternNotes.Clear();
            List<string> lines = new List<string>();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                if (rawData[pos] == string.Empty)
                {
                    allPatternNotes.Add(new PatternNote(lines));
                    lines.Clear();
                }
                else
                {
                    lines.Add(rawData[pos]);
                }
            }
            allPatternNotes.Add(new PatternNote(lines));
        }
    }
}