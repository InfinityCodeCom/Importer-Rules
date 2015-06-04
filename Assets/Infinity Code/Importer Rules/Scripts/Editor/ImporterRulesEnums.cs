﻿/*     INFINITY CODE 2013-2015      */
/*   http://www.infinity-code.com   */

namespace InfinityCode.ImporterRules
{
    public enum ImporterRulesTypes
    {
        texture,
        model,
        audio,
        movie,
        trueTypeFont
    }

    public enum ImporterRulesPathComparer
    {
        allAssets,
        startWith,
        contains,
        regex
    }
}