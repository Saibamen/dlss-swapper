﻿using DLSS_Swapper.Data;
using Microsoft.UI.Xaml;
using System;

namespace DLSS_Swapper
{
    public class Settings
    {
        private static Settings? _instance;
        public static Settings Instance => _instance ??= Settings.FromJson();

        // We default this to false to prevent saves firing when loading from json.
        private bool _autoSave;
        private bool _hasShownWarning;
        public bool HasShownWarning
        {
            get { return _hasShownWarning; }
            set
            {
                if (_hasShownWarning != value)
                {
                    _hasShownWarning = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _hasShownMultiplayerWarning;
        public bool HasShownMultiplayerWarning
        {
            get { return _hasShownMultiplayerWarning; }
            set
            {
                if (_hasShownMultiplayerWarning != value)
                {
                    _hasShownMultiplayerWarning = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _hideNonDLSSGames;
        public bool HideNonDLSSGames
        {
            get { return _hideNonDLSSGames; }
            set
            {
                if (_hideNonDLSSGames != value)
                {
                    _hideNonDLSSGames = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _groupGameLibrariesTogether = true;
        public bool GroupGameLibrariesTogether
        {
            get { return _groupGameLibrariesTogether; }
            set
            {
                if (_groupGameLibrariesTogether != value)
                {
                    _groupGameLibrariesTogether = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private ElementTheme _appTheme = ElementTheme.Default;
        public ElementTheme AppTheme
        {
            get { return _appTheme; }
            set
            {
                if (_appTheme != value)
                {
                    _appTheme = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _allowDebugDlls;
        public bool AllowDebugDlls
        {
            get { return _allowDebugDlls; }
            set
            {
                if (_allowDebugDlls != value)
                {
                    _allowDebugDlls = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _allowUntrusted;
        public bool AllowUntrusted
        {
            get { return _allowUntrusted; }
            set
            {
                if (_allowUntrusted != value)
                {
                    _allowUntrusted = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private DateTimeOffset _lastRecordsRefresh = DateTimeOffset.MinValue;
        public DateTimeOffset LastRecordsRefresh
        {
            get { return _lastRecordsRefresh; }
            set
            {
                if (_lastRecordsRefresh != value)
                {
                    _lastRecordsRefresh = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private ulong _lastPromptWasForVersion;
        public ulong LastPromptWasForVersion
        {
            get { return _lastPromptWasForVersion; }
            set
            {
                if (_lastPromptWasForVersion != value)
                {
                    _lastPromptWasForVersion = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        // Don't forget to change this back to off.
        private LoggingLevel _loggingLevel = LoggingLevel.Error;
        public LoggingLevel LoggingLevel
        {
            get { return _loggingLevel; }
            set
            {
                if (_loggingLevel != value)
                {
                    _loggingLevel = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private uint _enabledGameLibraries = uint.MaxValue;
        public uint EnabledGameLibraries
        {
            get { return _enabledGameLibraries; }
            set
            {
                if (_enabledGameLibraries != value)
                {
                    _enabledGameLibraries = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _wasLoadingGames;
        public bool WasLoadingGames
        {
            get { return _wasLoadingGames; }
            set
            {
                if (_wasLoadingGames != value)
                {
                    _wasLoadingGames = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _dontShowManuallyAddingGamesNotice;
        public bool DontShowManuallyAddingGamesNotice
        {
            get { return _dontShowManuallyAddingGamesNotice; }
            set
            {
                if (_dontShowManuallyAddingGamesNotice != value)
                {
                    _dontShowManuallyAddingGamesNotice = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private bool _hasShownAddGameFolderMessage;
        public bool HasShownAddGameFolderMessage
        {
            get { return _hasShownAddGameFolderMessage; }
            set
            {
                if (_hasShownAddGameFolderMessage != value)
                {
                    _hasShownAddGameFolderMessage = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        private WindowPositionRect _lastWindowSizeAndPosition = new();
        public WindowPositionRect LastWindowSizeAndPosition
        {
            get { return _lastWindowSizeAndPosition; }
            set
            {
                if (_lastWindowSizeAndPosition != value)
                {
                    _lastWindowSizeAndPosition = value;
                    if (_autoSave)
                    {
                        SaveJson();
                    }
                }
            }
        }

        /*
        public List<string> Directories { get; set; } = new List<string>();

        public void AddDirectory(string directory)
        {
            if (Directories.Contains(directory))
            {
                return;
            }

            Directories.Add(directory);

            if (_autoSave)
            {
                SaveJson();
            }
        }

        public void RemoveDirectory(string directory)
        {
            Directories.Remove(directory);

            if (_autoSave)
            {
                SaveJson();
            }
        }
        */

        private void SaveJson()
        {
            AsyncHelper.RunSync(() => Storage.SaveSettingsJsonAsync(this));
        }

        private static Settings FromJson()
        {
            Settings? settings = null;

            var settingsFromJson = AsyncHelper.RunSync(Storage.LoadSettingsJsonAsync);
            // If we couldn't load settings then save the defaults.
            if (settingsFromJson is null)
            {
                settings = new Settings();
                settings.SaveJson();
            }
            else
            {
                settings = settingsFromJson;
            }

            // Re-enable auto save.
            settings._autoSave = true;
            return settings;
        }
    }
}
