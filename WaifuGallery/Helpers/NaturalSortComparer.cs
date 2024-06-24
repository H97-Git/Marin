using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WaifuGallery.Helpers;

public class NaturalSortComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var xx = Path.GetFileNameWithoutExtension(x);
        var yy = Path.GetFileNameWithoutExtension(y);

        if (xx is null && yy is null) return 0;
        if (xx is null) return -1;
        if (yy is null) return 1;

        var xExt = Path.GetExtension(x);
        var yExt = Path.GetExtension(y);

        var extCompare = xExt.CompareTo(yExt);
        if (extCompare != 0)
            return extCompare;

        var nx = GetNumericValue(xx);
        var ny = GetNumericValue(yy);

        if (nx is null && ny is null) return 0;
        if (nx is null) return -1;
        if (ny is null) return 1;
        var nxx = (int) nx;
        var compareNumeric = nxx.CompareTo(ny);
        if (compareNumeric != 0)
            return compareNumeric;

        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);

        // if (x is null && y is null) return 0;
        // if (x is null) return -1;
        // if (y is null) return 1;
        //
        // var xx = Path.GetFileNameWithoutExtension(x);
        // var yy = Path.GetFileNameWithoutExtension(y);
        //
        // if (xx is null && yy is null) return 0;
        // if (xx is null) return -1;
        // if (yy is null) return 1;
        //
        // var nx = GetNumericValue(xx);
        // var ny = GetNumericValue(yy);
        //
        // if (nx is null && ny is null) return 0;
        // if (nx is null) return -1;
        // if (ny is null) return 1;
        // var nxx = (int) nx;
        // var compareNumeric = nxx.CompareTo(ny);
        // if (compareNumeric != 0)
        //     return compareNumeric;
        //
        // return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
    }

    private static int? GetNumericValue(string s)
    {
        var match = Regex.Match(s, @"\d+");
        int result;
        var numValue = match.Success ? int.TryParse(match.Value, out result) : int.TryParse(s, out result);
        return numValue ? result : null;
    }
}