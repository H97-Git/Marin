using System;
using System.Collections.Generic;
using System.IO;

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
        var numValue = int.TryParse(s, out var result);
        return numValue ? result : null;
    }
}
