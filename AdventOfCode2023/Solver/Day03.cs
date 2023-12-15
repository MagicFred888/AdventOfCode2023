using Microsoft.VisualBasic;
using System.Drawing;
using System.Reflection;

namespace AdventOfCode2023.Solver
{
    internal class Day03 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Gear Ratios";

        private class NumberInfos
        {
            public int Number { get; private set; } = -1;
            public List<Point> Positions { get; private set; } = new List<Point>();

            public NumberInfos(int number, int xPos, int yPos, int length)
            {
                Number = number;
                for (int i = 0; i < length; i++)
                {
                    Positions.Add(new Point(xPos + i, yPos));
                }
            }

            public bool IsTouching(PartInfos partInfo)
            {
                if (partInfo.Position.X < Positions[0].X - 1 || partInfo.Position.X > Positions[Positions.Count - 1].X + 1) return false;
                if (partInfo.Position.Y < Positions[0].Y - 1 || partInfo.Position.Y > Positions[0].Y + 1) return false;
                return true;
            }
        }
        private class PartInfos
        {
            public string Part { get; private set; } = string.Empty;
            public Point Position { get; private set; } = new Point();

            public PartInfos(string part, int xPos, int yPos)
            {
                Part = part;
                Position = new Point(xPos, yPos);
            }
        }

        private List<NumberInfos> allNumber = new List<NumberInfos>();
        private List<PartInfos> allPart = new List<PartInfos>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day03(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Sum touching items
            int answer = 0;
            foreach (PartInfos partInfo in allPart)
            {
                foreach (NumberInfos numberInfo in allNumber)
                {
                    if (numberInfo.IsTouching(partInfo)) answer += numberInfo.Number;
                }
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Count * touching 2 numbers
            int answer = 0;
            foreach (PartInfos partInfo in allPart)
            {
                if (partInfo.Part != "*") continue;
                List<int> numbers = new List<int>();
                foreach (NumberInfos numberInfo in allNumber)
                {
                    if (numberInfo.IsTouching(partInfo))
                    {
                        numbers.Add(numberInfo.Number);
                    }
                }
                if (numbers.Count == 2) answer += numbers[0] * numbers[1];
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allNumber.Clear();
            allPart.Clear();
            for (int y = 0; y < rawData.Count; y++)
            {
                for (int x = 0; x < rawData[y].Length; x++)
                {
                    string currentSign = rawData[y].Substring(x, 1);
                    if (Information.IsNumeric(currentSign))
                    {
                        // Save number
                        int i = 0;
                        while (x + 1 + i < rawData[y].Length && Information.IsNumeric(rawData[y].Substring(x + 1 + i, 1)))
                        {
                            i++;
                        }
                        NumberInfos newInfo = new NumberInfos(int.Parse(rawData[y].Substring(x, i + 1)), x, y, i + 1);
                        allNumber.Add(newInfo);
                        x += i;
                    }
                    else if (currentSign != ".")
                    {
                        // Save part
                        allPart.Add(new PartInfos(currentSign, x, y));
                    }
                }
            }
        }
    }
}