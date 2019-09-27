using Microsoft.Extensions.DependencyInjection;

namespace LanguageExt.Bson.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Set up the languageext bson serializers
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLanguageExtBsonSerializers(this IServiceCollection services)
        {
            LanguageExtBsonSerializer.Setup();
            return services;
        }
    }
}