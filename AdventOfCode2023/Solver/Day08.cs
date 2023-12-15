using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solver
{
    internal class Day08 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Haunted Wasteland";
        private class Element
        {
            public string Name { get; private set; } = "";
            public Element? Left { get; set; } = null;
            public Element? Right { get; set; } = null;

            public Element(string name)
            {
                Name = name;
            }
        }

        private Dictionary<string, Element> allElements = new Dictionary<string, Element>();
        private char[] moves = new char[0];

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day08(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Count moves
            int movePos = 0;
            int nbrOfMove = 0;
            Element currentElement = allElements["AAA"];
            do
            {
                currentElement = moves[movePos] == 'L' ? currentElement.Left : currentElement.Right;
                movePos = (movePos + 1) % moves.Length;
                nbrOfMove++;
            } while (currentElement.Name != "ZZZ");

            // Done
            return nbrOfMove.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Search loop length for each parrallel loops (Need a trick or search will last forever ;-)
            int movePos = 0;
            Element[] currentElements = Array.FindAll<Element>(allElements.Values.ToArray(), i => i.Name.EndsWith("A"));
            long[] cycleLength = new long[currentElements.Length];
            bool[] cycleConfirmed = new bool[currentElements.Length];
            int[] currentCycleLength = new int[currentElements.Length];
            do
            {
                for (int i = 0; i < currentElements.Length; i++)
                {
                    currentElements[i] = moves[movePos] == 'L' ? currentElements[i].Left : currentElements[i].Right;
                    currentCycleLength[i]++;
                    if (currentElements[i].Name.EndsWith("Z"))
                    {
                        if (cycleLength[i] == 0)
                            cycleLength[i] = currentCycleLength[i];
                        else if (cycleLength[i] == currentCycleLength[i])
                            cycleConfirmed[i] = true;
                        else
                            throw new Exception("Huston, we have a problem !");
                        currentCycleLength[i] = 0;
                    }
                }
                movePos = (movePos + 1) % moves.Length;
            } while (Array.FindAll<bool>(cycleConfirmed, e => e).Length != currentElements.Length);

            // Done
            return LCM(cycleLength).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // For LCM and GCD, thanks to https://www.geeksforgeeks.org/lcm-of-given-array-elements/
        //------------------------------------------------------------------------------------------------------------------------------------------------------ 
        private long LCM(long[] numbers)
        {
            return numbers.Aggregate((long x, long y) => x * y / GCD(x, y));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private long GCD(long a, long b)
        {
            if (b == 0) return a;
            return GCD(b, a % b);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allElements.Clear();

            // Get moves
            moves = rawData[0].ToCharArray();

            // Get all "Element"
            for (int pos = 2; pos < rawData.Count; pos++)
            {
                // Split data
                string[] data = Regex.Split(rawData[pos], @"[^A-Z0-9]+");

                // Make sure current element exist
                if (!allElements.ContainsKey(data[0])) allElements.Add(data[0], new Element(data[0]));

                // Add LEFT ref
                if (!allElements.ContainsKey(data[1])) allElements.Add(data[1], new Element(data[1]));
                allElements[data[0]].Left = allElements[data[1]];

                // Add RIGHT ref
                if (!allElements.ContainsKey(data[2])) allElements.Add(data[2], new Element(data[2]));
                allElements[data[0]].Right = allElements[data[2]];
            }
        }
    }
}