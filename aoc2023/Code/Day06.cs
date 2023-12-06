namespace aoc2023.Code;

internal class Day06 : BaseDay
{
    record Race(long Length, long Record);

    static long CountWays(Race race)
    {
        var r = 0L;
        for (var speed = 0L; speed <= race.Length; speed++)
        {
            var dist = speed * (race.Length - speed);
            if (dist > race.Record)
            {
                r++;
            }
        }
        return r;
    }

    static long Solve(IEnumerable<Race> races) => races.Aggregate(1L, (x, y) => x * CountWays(y));

    protected override object Part1()
    {
        var races = new List<Race>();
        if (_testRun)
        {
            races.Add(new(7, 9));
            races.Add(new(15, 40));
            races.Add(new(30, 200));
        }
        else
        {
            races.Add(new(48, 296));
            races.Add(new(93, 1928));
            races.Add(new(85, 1236));
            races.Add(new(95, 1391));
        }

        return Solve(races);
    }

    protected override object Part2()
    {
        var races = new List<Race>();
        if (_testRun)
        {
            races.Add(new(71530, 940200));
        }
        else
        {
            races.Add(new(48938595, 296192812361391));
        }

        return Solve(races);
    }
}