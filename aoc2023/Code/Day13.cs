namespace aoc2023.Code;

internal class Day13 : BaseDay
{
    static IEnumerable<int> FindSameColumns(List<string> rows) => Enumerable.Range(0, rows[0].Length - 1).Where(x => SameColumns(x, x + 1, rows));

    static IEnumerable<int> FindSameRows(List<string> rows) => Enumerable.Range(0, rows.Count - 1).Where(x => SameRows(x, x + 1, rows));

    static bool CheckHorizontal(int row, List<string> rows)
    {
        var up = row - 1;
        var down = row + 2;
        while (up >= 0 && down < rows.Count)
        {
            if (!SameRows(up, down, rows))
            {
                return false;
            }
            up--;
            down++;
        }
        return true;
    }

    static bool CheckVertical(int col, List<string> rows)
    {
        var left = col - 1;
        var right = col + 2;
        while (left >= 0 && right < rows[0].Length)
        {
            if (!SameColumns(left, right, rows))
            {
                return false;
            }
            left--;
            right++;
        }
        return true;
    }

    static bool SameColumns(int i, int j, List<string> rows)
    {
        for (int row = 0; row < rows.Count; row++)
        {
            if (rows[row][i] != rows[row][j])
            {
                return false;
            }
        }
        return true;
    }

    static bool SameRows(int i, int j, List<string> rows)
    {
        for (int col = 0; col < rows[0].Length; col++)
        {
            if (rows[i][col] != rows[j][col])
            {
                return false;
            }
        }
        return true;
    }

    static bool HaveVertical(List<string> rows, int skipCol, out int summary)
    {
        summary = 0;
        var vertical = FindSameColumns(rows).ToList();

        foreach (var col in vertical)
        {
            if (col != skipCol && CheckVertical(col, rows))
            {
                summary = 1 + col;

                return true;
            }
        }

        return false;
    }

    static bool HaveHorizontal(List<string> rows, int skipRow, out int summary)
    {
        summary = 0;
        var horizontal = FindSameRows(rows).ToList();

        foreach (var row in horizontal)
        {
            if (row != skipRow && CheckHorizontal(row, rows))
            {
                summary = (1 + row) * 100;

                return true;
            }
        }

        return false;
    }

    static int Process(List<string> rows, bool fix)
    {
        if (HaveVertical(rows, -1, out var a))
        {
            if (!fix)
            {
                return a;
            }
            foreach (var f in Fix(rows))
            {
                if (HaveVertical(f, a - 1, out var c))
                {
                    return c;
                }
                if (HaveHorizontal(f, -1, out var d))
                {
                    return d;
                }
            }
        }

        if (HaveHorizontal(rows, -1, out var b))
        {
            if (!fix)
            {
                return b;
            }
            foreach (var f in Fix(rows))
            {
                if (HaveVertical(f, -1, out var c))
                {
                    return c;
                }
                if (HaveHorizontal(f, (b - 1) / 100, out var d))
                {
                    return d;
                }
            }
        }

        throw new InvalidProgramException();
    }

    static IEnumerable<List<string>> Fix(List<string> rows)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            for (int j = 0; j < rows[i].Length; j++)
            {
                var copy = rows.ToList();
                var row = copy[i].ToCharArray();

                row[j] = row[j] == '#' ? '.' : '#';
                copy[i] = new string(row);

                yield return copy;
            }
        }
    }

    int Smash(bool fix)
    {
        var summary = 0;
        var data = ReadAllText().Split(Environment.NewLine).ToList();

        while (data.Count > 0)
        {
            List<string> rows = [.. data.TakeWhile(x => !string.IsNullOrWhiteSpace(x))];
            data.RemoveRange(0, 1 + rows.Count);

            summary += Process(rows, fix);
        }

        return summary;
    }

    protected override object Part1() => Smash(false);

    protected override object Part2() => Smash(true);
}