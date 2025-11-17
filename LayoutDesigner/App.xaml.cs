using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LayoutDesigner.Services;
using LayoutDesigner.Services.Interfaces;

namespace LayoutDesigner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ServiceContainer? _services;

        public static ServiceContainer Services => _services ??= new ServiceContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Register services
            RegisterServices();
        }

        private void RegisterServices()
        {
            // Register logger first for use in error handling
            Services.Register<IAppLogger, ConsoleLogger>();
            
            // Register ErrorHandlingService first since other services depend on it
            Services.Register<IErrorHandlingService, ErrorHandlingService>();
            Services.Register<IUndoRedoService, UndoRedoService>();
            Services.Register<ILayoutStorageService, LayoutStorageService>();
            Services.Register<IQrCodeService, QrCodeService>();
            Services.Register<IExportService, ExportService>();
            Services.Register<ISvgExportService, SvgExportService>();
            Services.Register<IAssetService, AssetService>();
            Services.Register<ITemplateService, TemplateService>();
            Services.Register<IDirtyTrackingService, DirtyTrackingService>();
        }
    }

    /// <summary>
    /// Simple dependency injection container
    /// </summary>
    public class ServiceContainer
    {
        private readonly Dictionary<Type, Type> _serviceTypes = new();
        private readonly Dictionary<Type, object> _singletonInstances = new();

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _serviceTypes[typeof(TInterface)] = typeof(TImplementation);
        }

        public T Resolve<T>()
        {
            var interfaceType = typeof(T);

            if (_singletonInstances.ContainsKey(interfaceType))
            {
                return (T)_singletonInstances[interfaceType];
            }

            if (_serviceTypes.ContainsKey(interfaceType))
            {
                var implementationType = _serviceTypes[interfaceType];

                // Get constructor with parameters
                var constructor = implementationType.GetConstructors().FirstOrDefault();

                if (constructor == null)
                {
                    throw new InvalidOperationException($"No public constructor found for {implementationType.Name}");
                }

                // Resolve constructor parameters
                var parameters = constructor.GetParameters();
                var parameterInstances = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterType = parameters[i].ParameterType;

                    // Recursively resolve dependencies
                    var resolveMethod = typeof(ServiceContainer).GetMethod(nameof(Resolve))!.MakeGenericMethod(parameterType);
                    parameterInstances[i] = resolveMethod.Invoke(this, null)!;
                }

                // Create instance with resolved parameters
                var instance = Activator.CreateInstance(implementationType, parameterInstances);

                if (instance != null)
                {
                    _singletonInstances[interfaceType] = instance;
                    return (T)instance;
                }
            }

            throw new InvalidOperationException($"Service of type {interfaceType.Name} is not registered.");
        }

        public static T? GetService<T>() where T : class
        {
            try
            {
                return App.Services.Resolve<T>();
            }
            catch
            {
                return null;
            }
        }
    }
}
