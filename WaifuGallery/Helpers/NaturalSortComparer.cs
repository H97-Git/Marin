using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace WaifuGallery.Helpers;

public class NaturalSortComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var xx = Path.GetFileNameWithoutExtension(x);
        var yy = Path.GetFileNameWithoutExtension(y);
        Console.WriteLine($"{xx} vs {yy}");

        if (xx == null && yy == null) return 0;
        if (xx == null) return -1;
        if (yy == null) return 1;

        var nx = GetNumericValue(xx);
        var ny = GetNumericValue(yy);

        if (nx == null && ny == null) return 0;
        if (nx == null) return -1;
        if (ny == null) return 1;
        var nxx = (int) nx;
        var compareNumeric = nxx.CompareTo(ny);
        if (compareNumeric != 0)
            return compareNumeric;

        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
    }

    private static int? GetNumericValue(string s)
    {
        var match = Regex.Match(s, @"\d+");
        int result;
        var numValue = match.Success ? int.TryParse(match.Value, out result) : int.TryParse(s, out result);
        return numValue ? result : null;
    }
}