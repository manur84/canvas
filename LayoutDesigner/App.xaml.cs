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
            Services.Register<ILayoutStorageService, LayoutStorageService>();
            Services.Register<IQrCodeService, QrCodeService>();
            Services.Register<IExportService, ExportService>();
            Services.Register<IAssetService, AssetService>();
            Services.Register<IUndoRedoService, UndoRedoService>();
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
                var instance = Activator.CreateInstance(implementationType);

                if (instance != null)
                {
                    _singletonInstances[interfaceType] = instance;
                    return (T)instance;
                }
            }

            throw new InvalidOperationException($"Service of type {interfaceType.Name} is not registered.");
        }
    }
}
