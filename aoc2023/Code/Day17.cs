namespace aoc2023.Code;

internal class Day17 : BaseDay
{
    enum Dir { Start, Up, Down, Left, Right };

    record XY(int X, int Y);

    record Item(XY Pos, Dir Where, int Steps);

    class Lavafall
    {
        readonly int[,] _map;
        readonly int _width, _height;

        public Lavafall(string[] data)
        {
            _width = data[0].Length;
            _height = data.Length;
            _map = new int[_height, _width];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _map[i, j] = data[i][j] - '0';
                }
            }
        }

        internal int MinimizeHeatLoss(XY start, bool ultraCrucibles)
        {
            var end = new XY(_width - 1, _height - 1);

            var dist = new Dictionary<Item, int>();
            var fringe = new PriorityQueue<Item, int>();

            var starter = new Item(start, Dir.Start, 0);
            dist[starter] = 0;
            fringe.Enqueue(starter, 0);

            while (fringe.TryDequeue(out var item, out var heatloss))
            {
                if (item.Pos == end)
                {
                    return heatloss;
                }

                foreach (var (next, nextDir, nextSteps) in GetNext(item, ultraCrucibles))
                {
                    if (next.X < 0 || next.X >= _width || next.Y < 0 || next.Y >= _height)
                    {
                        continue;
                    }

                    var newItem = new Item(next, nextDir, nextSteps);
                    var newCost = heatloss + _map[next.Y, next.X];

                    if (!dist.TryGetValue(newItem, out var nextCost) || newCost < nextCost)
                    {
                        dist[newItem] = newCost;

                        fringe.Enqueue(newItem, newCost);
                    }
                }
            }

            throw new InvalidDataException();
        }

        static IEnumerable<Item> GetNext(Item source, bool ultraCrucibles)
        {
            int SAME_CHECK = ultraCrucibles ? 9 : 2; // max 10 or 3 block in the same direction
            int TURN_CHECK = ultraCrucibles ? 3 : int.MinValue; // when can it turn, current dir is +1

            List<Item> turnMoves = source.Where switch
            {
                Dir.Start => [Left(0), Right(0), Up(0), Down(0)],
                Dir.Up or Dir.Down => source.Steps >= TURN_CHECK ? [Left(0), Right(0)] : [],
                Dir.Left or Dir.Right => source.Steps >= TURN_CHECK ? [Up(0), Down(0)] : [],

                _ => throw new NotImplementedException()
            };

            List<Item> sameMoves = source.Where switch
            {
                Dir.Up => source.Steps < SAME_CHECK ? [Up(source.Steps + 1)] : [],
                Dir.Down => source.Steps < SAME_CHECK ? [Down(source.Steps + 1)] : [],
                Dir.Left => source.Steps < SAME_CHECK ? [Left(source.Steps + 1)] : [],
                Dir.Right => source.Steps < SAME_CHECK ? [Right(source.Steps + 1)] : [],
                Dir.Start => [],

                _ => throw new NotImplementedException()
            };

            return turnMoves.Union(sameMoves);

            Item Left(int s) => new(new(source.Pos.X - 1, source.Pos.Y), Dir.Left, s);
            Item Right(int s) => new(new(source.Pos.X + 1, source.Pos.Y), Dir.Right, s);
            Item Up(int s) => new(new(source.Pos.X, source.Pos.Y - 1), Dir.Up, s);
            Item Down(int s) => new(new(source.Pos.X, source.Pos.Y + 1), Dir.Down, s);
        }
    }

    protected override object Part1() => new Lavafall(ReadAllLines(true)).MinimizeHeatLoss(new(0, 0), false);

    protected override object Part2() => new Lavafall(ReadAllLines(true)).MinimizeHeatLoss(new(0, 0), true);
}