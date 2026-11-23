using GroveGames.DependencyInjection;

namespace GroveGames.Mathematics.Unity
{
    public static class MathematicsContainerBuilderExtensions
    {
        public static IContainerBuilder AddRandomNumberGenerator(this IContainerBuilder builder)
        {
            return builder.AddSingleton<IRandomNumberGenerator>(_ => new RandomNumberGenerator());
        }
    }
}
