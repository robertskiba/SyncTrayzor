using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SyncTrayzor.Services.Config
{
    public class FolderConfiguration
    {
        public string ID { get; set; }
        public bool IsWatched { get; set; }
        public bool NotificationsEnabled { get; set; }

        public FolderConfiguration()
        {
        }

        public FolderConfiguration(string id, bool isWatched, bool notificationsEnabled)
        {
            this.ID = id;
            this.IsWatched = isWatched;
            this.NotificationsEnabled = notificationsEnabled;
        }

        public FolderConfiguration(FolderConfiguration other)
        {
            this.ID = other.ID;
            this.IsWatched = other.IsWatched;
            this.NotificationsEnabled = other.NotificationsEnabled;
        }

        public override string ToString()
        {
            return $"<Folder ID={this.ID} IsWatched={this.IsWatched} NotificationsEnabled={this.NotificationsEnabled}>";
        }
    }

    public class EnvironmentalVariableCollection : Dictionary<string, string>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // Used to use XElement.Load(reader.ReadSubtree()), but that effectively closed the reader
            // and nothing else would get parsed.
            var root = XElement.Parse(reader.ReadOuterXml());
            foreach (var element in root.Elements("Item"))
            {
                this.Add(element.Element("Key").Value, element.Element("Value").Value);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var elements = this.Select(item =>
            {
                return new XElement("Item",
                    new XElement("Key", item.Key),
                    new XElement("Value", item.Value)
                );
            });
            foreach (var element in elements)
            {
                element.WriteTo(writer);
            }
        }
    }

    [XmlRoot("Configuration")]
    public class Configuration
    {
        public const int CurrentVersion = 4;
        public const double DefaultSyncthingConsoleHeight = 100;

        [XmlAttribute("Version")]
        public int Version
        {
            get { return CurrentVersion; }
            set
            {
                if (CurrentVersion != value)
                    throw new InvalidOperationException($"Can't deserialize config of version {value} (expected {CurrentVersion})");
            }
        }

        public bool ShowTrayIconOnlyOnClose { get; set; }
        public bool MinimizeToTray { get; set; }
        public bool CloseToTray { get; set; }
        public bool ShowDeviceConnectivityBalloons { get; set; }
        public bool ShowSynchronizedBalloonEvenIfNothingDownloaded { get; set; }
        public string SyncthingAddress { get; set; }
        public bool StartSyncthingAutomatically { get; set; }
        public string SyncthingApiKey { get; set; }
        public EnvironmentalVariableCollection SyncthingEnvironmentalVariables { get; set; }
        public bool SyncthingUseCustomHome { get; set; }
        public bool SyncthingDenyUpgrade { get; set; }
        public bool SyncthingRunLowPriority { get; set; }

        [XmlArrayItem("Folder")]
        public List<FolderConfiguration> Folders { get; set; }

        public bool NotifyOfNewVersions { get; set; }
        public bool ObfuscateDeviceIDs { get; set; }

        [XmlIgnore]
        public Version LatestNotifiedVersion { get; set; }
        [XmlElement("LatestNotifiedVersion")]
        public string LatestNotifiedVersionRaw
        {
            get { return this.LatestNotifiedVersion == null ? null : this.LatestNotifiedVersion.ToString(); }
            set { this.LatestNotifiedVersion = value == null ? null : new Version(value); }
        }

        public bool UseComputerCulture { get; set; }
        public double SyncthingConsoleHeight { get; set; }
        public WindowPlacement WindowPlacement { get; set; }
        public double SyncthingWebBrowserZoomLevel { get; set; }
        public int LastSeenInstallCount { get; set; }
        public string SyncthingPath { get; set; }
        public string SyncthingCustomHomePath { get; set; }
        public bool DisableHardwareRendering { get; set; }

        public Configuration()
        {
            // Default configuration is for a portable setup.

            this.ShowTrayIconOnlyOnClose = false;
            this.MinimizeToTray = false;
            this.CloseToTray = true;
            this.ShowSynchronizedBalloonEvenIfNothingDownloaded = false;
            this.ShowDeviceConnectivityBalloons = true;
            this.SyncthingAddress = "localhost:8384";
            this.StartSyncthingAutomatically = true;
            this.SyncthingApiKey = null;
            this.SyncthingEnvironmentalVariables = new EnvironmentalVariableCollection();
            this.SyncthingUseCustomHome = true;
            this.SyncthingDenyUpgrade = false;
            this.SyncthingRunLowPriority = false;
            this.Folders = new List<FolderConfiguration>();
            this.NotifyOfNewVersions = true;
            this.ObfuscateDeviceIDs = true;
            this.LatestNotifiedVersion = null;
            this.UseComputerCulture = true;
            this.SyncthingConsoleHeight = Configuration.DefaultSyncthingConsoleHeight;
            this.WindowPlacement = null;
            this.SyncthingWebBrowserZoomLevel = 0;
            this.LastSeenInstallCount = 0;
            this.SyncthingPath = @"%EXEPATH%\syncthing.exe";
            this.SyncthingCustomHomePath = @"%EXEPATH%\data\syncthing";
            this.DisableHardwareRendering = false;
        }

        public Configuration(Configuration other)
        {
            this.ShowTrayIconOnlyOnClose = other.ShowTrayIconOnlyOnClose;
            this.MinimizeToTray = other.MinimizeToTray;
            this.CloseToTray = other.CloseToTray;
            this.ShowSynchronizedBalloonEvenIfNothingDownloaded = other.ShowSynchronizedBalloonEvenIfNothingDownloaded;
            this.ShowDeviceConnectivityBalloons = other.ShowDeviceConnectivityBalloons;
            this.SyncthingAddress = other.SyncthingAddress;
            this.StartSyncthingAutomatically = other.StartSyncthingAutomatically;
            this.SyncthingApiKey = other.SyncthingApiKey;
            this.SyncthingEnvironmentalVariables = other.SyncthingEnvironmentalVariables;
            this.SyncthingUseCustomHome = other.SyncthingUseCustomHome;
            this.SyncthingDenyUpgrade = other.SyncthingDenyUpgrade;
            this.SyncthingRunLowPriority = other.SyncthingRunLowPriority;
            this.Folders = other.Folders.Select(x => new FolderConfiguration(x)).ToList();
            this.NotifyOfNewVersions = other.NotifyOfNewVersions;
            this.ObfuscateDeviceIDs = other.ObfuscateDeviceIDs;
            this.LatestNotifiedVersion = other.LatestNotifiedVersion;
            this.UseComputerCulture = other.UseComputerCulture;
            this.SyncthingConsoleHeight = other.SyncthingConsoleHeight;
            this.WindowPlacement = other.WindowPlacement;
            this.SyncthingWebBrowserZoomLevel = other.SyncthingWebBrowserZoomLevel;
            this.LastSeenInstallCount = other.LastSeenInstallCount;
            this.SyncthingPath = other.SyncthingPath;
            this.SyncthingCustomHomePath = other.SyncthingCustomHomePath;
            this.DisableHardwareRendering = other.DisableHardwareRendering;
        }

        public override string ToString()
        {
            return $"<Configuration ShowTrayIconOnlyOnClose={this.ShowTrayIconOnlyOnClose} MinimizeToTray={this.MinimizeToTray} CloseToTray={this.CloseToTray} " +
                $"ShowDeviceConnectivityBalloons={this.ShowDeviceConnectivityBalloons} SyncthingAddress={this.SyncthingAddress} StartSyncthingAutomatically={this.StartSyncthingAutomatically} " +
                $"SyncthingApiKey={this.SyncthingApiKey} SyncthingEnvironmentalVariables=[{String.Join(" ", this.SyncthingEnvironmentalVariables)}] " +
                $"SyncthingUseCustomHome={this.SyncthingUseCustomHome} SyncthingDenyUpgrade={this.SyncthingDenyUpgrade} SyncthingRunLowPriority={this.SyncthingRunLowPriority} " +
                $"Folders=[{String.Join(", ", this.Folders)}] NotifyOfNewVersions={this.NotifyOfNewVersions} LatestNotifiedVersion={this.LatestNotifiedVersion} " +
                $"ObfuscateDeviceIDs={this.ObfuscateDeviceIDs} UseComputerCulture={this.UseComputerCulture} SyncthingConsoleHeight={this.SyncthingConsoleHeight} WindowPlacement={this.WindowPlacement} " +
                $"SyncthingWebBrowserZoomLevel={this.SyncthingWebBrowserZoomLevel} LastSeenInstallCount={this.LastSeenInstallCount} SyncthingPath={this.SyncthingPath} " +
                $"SyncthingCustomHomePath={this.SyncthingCustomHomePath} ShowSynchronizedBalloonEvenIfNothingDownloaded={this.ShowSynchronizedBalloonEvenIfNothingDownloaded} " +
                $"DisableHardwareRendering={this.DisableHardwareRendering}>";
        }
    }
}
