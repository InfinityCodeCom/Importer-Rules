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