using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WaifuGallery.Helpers;

[Obsolete]
public class NaturalSortComparer : IComparer<string>
{
    private static int? GetNumericValue(string s)
    {
        var match = Regex.Match(s, @"\d+");
        var numValue = match.Success ? int.TryParse(match.Value, out var result) : int.TryParse(s, out result);
        return numValue ? result : null;
    }

    public int Compare(string? x, string? y) // x = a1.txt, y = b2.txt
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var xx = Path.GetFileNameWithoutExtension(x); //xx = a1
        var yy = Path.GetFileNameWithoutExtension(y); //yy = b2

        if (xx is null && yy is null) return 0;
        if (xx is null) return -1;
        if (yy is null) return 1;

        var xExt = Path.GetExtension(x); //xExt = .txt
        var yExt = Path.GetExtension(y); //yExt = .txt

        var extCompare = xExt.CompareTo(yExt);
        if (extCompare != 0)
            return extCompare;

        var nx = GetNumericValue(xx); //nx = 1
        var ny = GetNumericValue(yy); //ny = 2

        if (nx is null || ny is null)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }

        var nxx = (int) nx;
        var nyy = (int) ny;
        var sx = x.Replace(nxx.ToString(), "");
        var sy = y.Replace(nyy.ToString(), "");
        if (sx == sy)
        {
            var compareNumeric = nxx.CompareTo(nyy);
            if (compareNumeric != 0)
                return compareNumeric;
        }

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
}