using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Application.Behaviours;
using Application.Abstractions.Messaging;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            var assm = Assembly.GetExecutingAssembly();
            services.AddValidatorsFromAssembly(assm);
            services.AddHandlersFromAssembly(assm);
        }

        private static void AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .ToList();

            foreach (var type in types)
            {
                foreach (var iface in type.GetInterfaces().Where(i => i.IsGenericType))
                {
                    var def = iface.GetGenericTypeDefinition();
                    var args = iface.GetGenericArguments();

                    if (def == typeof(ICommandHandler<,>))
                    {
                        services.AddTransient(type);

                        var commandType = args[0];
                        var responseType = args[1];
                        var decoratorType = typeof(ValidatingCommandHandler<,>).MakeGenericType(commandType, responseType);
                        var capturedType = type;
                        var capturedIface = iface;

                        services.AddTransient(capturedIface, sp =>
                        {
                            var inner = sp.GetRequiredService(capturedType);
                            return Activator.CreateInstance(decoratorType, inner, sp)!;
                        });
                    }
                    else if (def == typeof(IQueryHandler<,>))
                    {
                        services.AddTransient(iface, type);
                    }
                }
            }
        }
    }
}
