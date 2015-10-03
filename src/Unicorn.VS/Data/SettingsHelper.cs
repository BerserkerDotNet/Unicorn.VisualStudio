using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Unicorn.VS.Models;

namespace Unicorn.VS.Data
{
    public static class SettingsHelper
    {
        private const string SettingsDefaultEndPoint = "/unicornRemote.aspx";
        private const string UnicornConnectionsRoot = @"Unicorn\Connections";
        private const string UnicornSettingsRoot = @"Unicorn\Settings";
        private const string ConnectionIdKey = "Id";
        private const string ConnectionNameKey = "Name";
        private const string ConnectionUrlKey = "Url";
        private const string ConnectionTokenKey = "Token";
        private const string SettingsEndPointKey = "EndPoint";
        private const string SettingsAllowMultipleConfigsKey = "AllowMultipleConfigurations";

        public static void SaveConnection(UnicornConnection connectionViewModel)
        {
            var store = GetStore();
            EnsureCollectionExists(UnicornConnectionsRoot);
            var connectionPath = GetConnectionPath(connectionViewModel.Id);
            store.CreateCollection(connectionPath);
            store.SetString(connectionPath, ConnectionIdKey, connectionViewModel.Id);
            store.SetString(connectionPath, ConnectionNameKey, connectionViewModel.Name);
            store.SetString(connectionPath, ConnectionUrlKey, connectionViewModel.ServerUrl);
            if (!string.IsNullOrEmpty(connectionViewModel.Token))
                store.SetString(connectionPath, ConnectionTokenKey, connectionViewModel.Token);
        }

        public static void DeleteConnection(string id)
        {
            var fullName = GetConnectionPath(id);
            var store = GetStore();
            if (store.CollectionExists(fullName))
                store.DeleteCollection(fullName);
        }

        public static UnicornConnection GetConnectionInfo(string id)
        {
            EnsureCollectionExists(UnicornConnectionsRoot);
            var store = GetStore();
            var connectionPath = GetConnectionPath(id);
            if (!store.CollectionExists(connectionPath))
                return null;

            var unicornConnection = new UnicornConnection
            {
                Id = store.GetString(connectionPath,ConnectionIdKey, string.Empty),
                Name = store.GetString(connectionPath,ConnectionNameKey, string.Empty),
                ServerUrl = store.GetString(connectionPath,ConnectionUrlKey, string.Empty),
                Token = store.GetString(connectionPath,ConnectionTokenKey, string.Empty),
            };
            return unicornConnection;
        }

        public static IEnumerable<UnicornConnection> GetAllConnections()
        {
            EnsureCollectionExists(UnicornConnectionsRoot);
            return GetStore()
                .GetSubCollectionNames(UnicornConnectionsRoot)
                .Select(GetConnectionInfo);
        }

        public static bool IsConnectionExists(string id)
        {
            return GetAllConnections().Any(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetConnectionPath(string id)
        {
            return string.Format("{0}\\{1}", UnicornConnectionsRoot, id);
        }

        private static void EnsureCollectionExists(string unicornConfiguration)
        {
            var store = GetStore();
            if (!store.CollectionExists(unicornConfiguration))
                store.CreateCollection(unicornConfiguration);
        }

        private static WritableSettingsStore GetStore()
        {
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            return settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public static UnicornSettings GetSettings()
        {
            EnsureCollectionExists(UnicornSettingsRoot);
            var store = GetStore();
            return new UnicornSettings
            {
                AllowMultipleConfigurations = store.GetBoolean(UnicornSettingsRoot, SettingsAllowMultipleConfigsKey,false),
                EndPoint = store.GetString(UnicornSettingsRoot, SettingsEndPointKey, SettingsDefaultEndPoint)
            };
        }

        public static void SaveSettings(UnicornSettings settings)
        {
            EnsureCollectionExists(UnicornSettingsRoot);
            var store = GetStore();
            store.SetBoolean(UnicornSettingsRoot, SettingsAllowMultipleConfigsKey, settings.AllowMultipleConfigurations);
            store.SetString(UnicornSettingsRoot, SettingsEndPointKey, settings.EndPoint);
        }
    }
}