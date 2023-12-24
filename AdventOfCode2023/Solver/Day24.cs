

using Microsoft.Z3;

namespace AdventOfCode2023.Solver
{
    internal class Day24 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Never Tell Me The Odds";

        private class HailstoneInfos
        {
            public (double X, double Y, double Z) Position { get; private set; } = (0, 0, 0);
            public (double Vx, double Vy, double Vz) Velocity { get; private set; } = (0, 0, 0);

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public HailstoneInfos(string rawData)
            {
                double[] data = rawData.Replace("@", ",").Replace(" ", "").Split(',').ToList().ConvertAll(i => double.Parse(i)).ToArray();
                Position = (data[0], data[1], data[2]);
                Velocity = (data[3], data[4], data[5]);
            }
        }

        private List<HailstoneInfos> allHailstoneInfos = new List<HailstoneInfos>();
        private double[] testArea = new double[2];

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day24(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Make some calculation...
            testArea = allHailstoneInfos.Count < 10 ? new double[] { 7, 27 } : new double[] { 200000000000000, 400000000000000 };
            int answer = 0;
            for (int i = 0; i < allHailstoneInfos.Count; i++)
            {
                for (int j = i + 1; j < allHailstoneInfos.Count; j++)
                {
                    if (Intersect(allHailstoneInfos[i], allHailstoneInfos[j])) answer++;
                }
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Not from me but found online, will have not been able to solve it without reading that sample : https://pastebin.com/fkpZWn8X
            long answer = SolveHailImpact(allHailstoneInfos);

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private long SolveHailImpact(List<HailstoneInfos> hailstoneInfos)
        {
            Context ctx = new Context();
            Microsoft.Z3.Solver solver = ctx.MkSolver();

            // Coordinates of the stone we need throw
            IntExpr x = ctx.MkIntConst("x");
            IntExpr y = ctx.MkIntConst("y");
            IntExpr z = ctx.MkIntConst("z");

            // Velocity of the stone we need throw
            IntExpr vx = ctx.MkIntConst("vx");
            IntExpr vy = ctx.MkIntConst("vy");
            IntExpr vz = ctx.MkIntConst("vz");

            // We want to find 9 variables (x, y, z, vx, vy, vz, t0, t1, t2) so a system of 9 equations should be enough
            for (var i = 0; i < 3; i++)
            {
                var t = ctx.MkIntConst($"t{i}"); // time for the stone to reach the hail
                HailstoneInfos hsi = hailstoneInfos[i];

                IntNum px = ctx.MkInt(Convert.ToInt64(hsi.Position.X));
                IntNum py = ctx.MkInt(Convert.ToInt64(hsi.Position.Y));
                IntNum pz = ctx.MkInt(Convert.ToInt64(hsi.Position.Z));

                IntNum pvx = ctx.MkInt(Convert.ToInt64(hsi.Velocity.Vx));
                IntNum pvy = ctx.MkInt(Convert.ToInt64(hsi.Velocity.Vy));
                IntNum pvz = ctx.MkInt(Convert.ToInt64(hsi.Velocity.Vz));

                ArithExpr xLeft = ctx.MkAdd(x, ctx.MkMul(t, vx)); // x + t * vx
                ArithExpr yLeft = ctx.MkAdd(y, ctx.MkMul(t, vy)); // y + t * vy
                ArithExpr zLeft = ctx.MkAdd(z, ctx.MkMul(t, vz)); // z + t * vz

                ArithExpr xRight = ctx.MkAdd(px, ctx.MkMul(t, pvx)); // px + t * pvx
                ArithExpr yRight = ctx.MkAdd(py, ctx.MkMul(t, pvy)); // py + t * pvy
                ArithExpr zRight = ctx.MkAdd(pz, ctx.MkMul(t, pvz)); // pz + t * pvz

                solver.Add(t >= 0); // time should always be positive - we don't want solutions for negative time
                solver.Add(ctx.MkEq(xLeft, xRight)); // x + t * vx = px + t * pvx
                solver.Add(ctx.MkEq(yLeft, yRight)); // y + t * vy = py + t * pvy
                solver.Add(ctx.MkEq(zLeft, zRight)); // z + t * vz = pz + t * pvz
            }

            // Solve
            solver.Check();
            var model = solver.Model;

            // Get x, y and z
            var rx = model.Eval(x);
            var ry = model.Eval(y);
            var rz = model.Eval(z);

            // Return result
            return Convert.ToInt64(rx.ToString()) + Convert.ToInt64(ry.ToString()) + Convert.ToInt64(rz.ToString());
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool Intersect(HailstoneInfos h1, HailstoneInfos h2)
        {
            // Compute intersection point
            double multiFactor = 10000; // Without it, rounding bring wrong results, struggle a lot to find why...
            (double X, double Y) intersection = GetIntersection((h1.Position.X, h1.Position.Y), (h1.Position.X + multiFactor * h1.Velocity.Vx, h1.Position.Y + multiFactor * h1.Velocity.Vy), (h2.Position.X, h2.Position.Y), (h2.Position.X + multiFactor * h2.Velocity.Vx, h2.Position.Y + multiFactor * h2.Velocity.Vy));

            // Intersect
            if (intersection.X == double.PositiveInfinity || intersection.X == double.NegativeInfinity || intersection.Y == double.PositiveInfinity || intersection.Y == double.NegativeInfinity) return false;

            // In target ?
            if (intersection.X < testArea[0] || intersection.X > testArea[1] || intersection.Y < testArea[0] || intersection.Y > testArea[1]) return false;

            // Both in future
            bool t1 = Math.Sign(intersection.X - h1.Position.X) == Math.Sign(h1.Velocity.Vx);
            bool t2 = Math.Sign(intersection.X - h2.Position.X) == Math.Sign(h2.Velocity.Vx);
            return t1 && t2;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public (double X, double Y) GetIntersection((double X, double Y) p1, (double X, double Y) p2, (double X, double Y) p3, (double X, double Y) p4)
        {
            try
            {
                double x = ((p1.X * p2.Y - p1.Y * p2.X) * (p3.X - p4.X) - (p1.X - p2.X) * (p3.X * p4.Y - p3.Y * p4.X)) / ((p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X));
                double y = ((p1.X * p2.Y - p1.Y * p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X * p4.Y - p3.Y * p4.X)) / ((p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X));
                return (x, y);
            }
            catch
            {
                return (double.NaN, double.NaN);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allHailstoneInfos.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allHailstoneInfos.Add(new HailstoneInfos(rawData[pos]));
            }
        }
    }
}