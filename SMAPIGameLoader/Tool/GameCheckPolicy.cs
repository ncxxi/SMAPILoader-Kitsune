namespace SMAPIGameLoader;

internal static class GameCheckPolicy
{
#if REQUIRE_GAME_CHECKS
    public static readonly bool RequireGameChecks = true;
#else
    public static readonly bool RequireGameChecks = false;
#endif
}
