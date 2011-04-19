using System;
using System.Reflection;
using Orchard.Environment;
using Orchard.FileSystems.AppData;
using Orchard.Logging;

namespace Orchard.FileSystems.Dependencies {
    public class DefaultAssemblyProbingFolder : IAssemblyProbingFolder {
        private const string BasePath = "Dependencies";
        private readonly IAppDataFolder _appDataFolder;
        private readonly IAssemblyLoader _assemblyLoader;

        public DefaultAssemblyProbingFolder(IAppDataFolder appDataFolder, IAssemblyLoader assemblyLoader) {
            _appDataFolder = appDataFolder;
            _assemblyLoader = assemblyLoader;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool AssemblyExists(string moduleName) {
            var path = PrecompiledAssemblyPath(moduleName);
            return _appDataFolder.FileExists(path);
        }

        public DateTime GetAssemblyDateTimeUtc(string moduleName) {
            var path = PrecompiledAssemblyPath(moduleName);
            return _appDataFolder.GetFileLastWriteTimeUtc(path);
        }

        public string GetAssemblyVirtualPath(string moduleName) {
            var path = PrecompiledAssemblyPath(moduleName);
            if (!_appDataFolder.FileExists(path))
                return null;

            return _appDataFolder.GetVirtualPath(path);
        }

        public Assembly LoadAssembly(string moduleName) {
            var path = PrecompiledAssemblyPath(moduleName);
            if (!_appDataFolder.FileExists(path))
                return null;

            return _assemblyLoader.Load(moduleName);
        }

        public void DeleteAssembly(string moduleName) {
            var path = PrecompiledAssemblyPath(moduleName);

            if (_appDataFolder.FileExists(path)) {
                Logger.Information("Deleting assembly for module \"{0}\" from probing directory", moduleName);
                _appDataFolder.DeleteFile(path);
            }
        }

        public void StoreAssembly(string moduleName, string fileName) {
            var path = PrecompiledAssemblyPath(moduleName);

            Logger.Information("Storing assembly file \"{0}\" for module \"{1}\"", fileName, moduleName);
            _appDataFolder.StoreFile(fileName, path);
            string mdbFrom = fileName + ".mdb";
            // This API will throw (on Windows) if the file path falls outside App_Data.
            // See PathValidation.ValidatePath.
            if (_appDataFolder.FileExists(mdbFrom))
                _appDataFolder.StoreFile(mdbFrom, path + ".mdb");
        }

        private string PrecompiledAssemblyPath(string moduleName) {
            return _appDataFolder.Combine(BasePath, moduleName + ".dll");
        }
    }
}