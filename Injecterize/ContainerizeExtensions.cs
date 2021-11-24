using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using os_ms_students.utils;

namespace Injecterize
{
    public static class InjecterizeExtensions
    {
        private static ILogger TryToDetermineLogger(IServiceCollection serviceCollection)
        {
            var currentSc = serviceCollection?.BuildServiceProvider();
            ILogger logger = currentSc?.GetService<ILogger>();
            if (logger == null)
            {
                var logProvider = currentSc?.GetRequiredService<ILoggerFactory>();
                logger = logProvider?.CreateLogger("Injecterize-Logging");
            }

            return logger;
        }


        public static void AddInjecterized(
            this IServiceCollection services, InjecterizeOptions options = null, ILogger logger = null)
        {
            var aLogger = DetermineLogging(services, logger);
            options = CheckOptions(options);
            aLogger?.LogDebug("Starting Injecterize Scanning");
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(InjecterizeAttribute), true).Length > 0)
                {
                    InjecterizeAttribute InjecterizeAttribute =
                        (InjecterizeAttribute) Attribute.GetCustomAttribute(type, typeof(InjecterizeAttribute));
                    if (InjecterizeAttribute != null)
                    {
                        aLogger?.LogDebug($"Found Type {type.FullName} with Injecterize Enabled");
                        DetermineAndRegister(type, InjecterizeAttribute, services, options, aLogger);
                    }
                }
            }
        }

        private static void RegisterServiceWithServiceInterface(InstanceScope scope, Type theType, Type theServiceType,
            IServiceCollection serviceCollection, ILogger aLogger)
        {
            switch (scope)
            {
                case InstanceScope.Scope:
                    serviceCollection.AddScoped(theServiceType, theType);
                    break;
                case InstanceScope.Singleton:
                    serviceCollection.AddSingleton(theServiceType, theType);
                    break;
                case InstanceScope.Transient:
                    serviceCollection.AddTransient(theServiceType, theType);
                    break;
                default:
                    aLogger?.LogError($"Error with Type : {theType.FullName} .. No Valid Scope Defined");
                    break;
            }
        }

        private static void RegisterService(InstanceScope scope, Type theType, IServiceCollection serviceCollection,
            ILogger aLogger)
        {
            switch (scope)
            {
                case InstanceScope.Scope:
                    aLogger?.LogDebug($"Type :{theType.FullName} requested to be registered as Scoped");
                    serviceCollection.AddScoped(theType);
                    break;
                case InstanceScope.Singleton:
                    aLogger?.LogDebug($"Type :{theType.FullName} requested to be registered as Singleton");
                    serviceCollection.AddSingleton(theType);
                    break;
                case InstanceScope.Transient:
                    aLogger?.LogDebug($"Type :{theType.FullName} requested to be registered as Transient");
                    serviceCollection.AddTransient(theType);
                    break;
                default:
                    aLogger?.LogError($"Error with Type : {theType.FullName} .. No Valid Scope Defined");
                    break;
            }
        }


        private static InjecterizeOptions CheckOptions(InjecterizeOptions options)
        {
            return options ??= new InjecterizeOptions();
        }

        private static ILogger DetermineLogging(IServiceCollection services, ILogger logger)
        {
            ILogger aLogger;
            if (logger == null)
            {
                aLogger = TryToDetermineLogger(services);
            }
            else
            {
                aLogger = logger;
            }

            return aLogger;
        }

        private static void DetermineAndRegister(Type type, InjecterizeAttribute InjecterizeAttribute,
            IServiceCollection services, InjecterizeOptions InjecterizeOptions, ILogger aLogger)
        {
            if (InjecterizeAttribute.InterfaceToUse == null)
            {
                aLogger?.LogDebug($"Type :{type.FullName} No Specific Interface Defined as part of Attribute");
                if (InjecterizeOptions.TryRegisterWithFirstInterface)
                {
                    aLogger?.LogDebug(
                        $"Injecterize Options to Check for First Interface Is Enabled , checking for Type :{type.FullName}");

                    var interfaceType = type.GetInterfaces().FirstOrDefault();
                    if (interfaceType != null)
                    {
                        if (services.Any(descriptor =>
                            descriptor.ImplementationType != null && descriptor.ServiceType == interfaceType))
                        {
                            aLogger?.LogWarning(
                                $"Injecterize Determined Interface for Type :{type.FullName} was already registered ,will register as instance type only");
                            RegisterService(InjecterizeAttribute.TargetScope, type, services, aLogger);
                        }
                        else
                        {
                            aLogger?.LogDebug(
                                $"Will Register Type :{type.FullName} with Service Interface of : {interfaceType.FullName} as Scoped");
                            RegisterServiceWithServiceInterface(InjecterizeAttribute.TargetScope, type, interfaceType,
                                services, aLogger);
                        }
                    }
                }
                else
                {
                    aLogger?.LogDebug($"Will Register Type :{type.FullName} as Scoped");
                    RegisterService(InjecterizeAttribute.TargetScope, type, services, aLogger);
                }
            }
            else
            {
                if (services.Any(descriptor =>
                    descriptor.ImplementationType != null &&
                    descriptor.ServiceType == InjecterizeAttribute.InterfaceToUse))
                {
                    aLogger?.LogWarning(
                        $"Injecterize Determined Interface {InjecterizeAttribute.InterfaceToUse} on Type :{type.FullName} was already registered ,will register as instance type only");
                    RegisterService(InjecterizeAttribute.TargetScope, type, services, aLogger);
                }
                else
                {
                    aLogger?.LogDebug(
                        $"Will Register Type :{type.FullName} as Service :{InjecterizeAttribute.InterfaceToUse} as Scoped");
                    RegisterServiceWithServiceInterface(InjecterizeAttribute.TargetScope, type,
                        InjecterizeAttribute.InterfaceToUse, services, aLogger);
                }
            }
        }
    }
}