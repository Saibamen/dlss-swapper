﻿using System;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLSS_Swapper.UserControls
{
    public partial class ManuallyAddGameControl : UserControl
    {
        public ManuallyAddGameControl(string gamePath)
        {
            this.InitializeComponent();

            DataContext = new ManuallyAddGameModel(this, gamePath);
        }


        string[] customCoverValidFileTypes =
        [
            ".png",
            ".jpg",
            ".jpeg",
            ".webp",
            ".bmp",
        ];
        DataPackageOperation coverDragDropAcceptedOperation = DataPackageOperation.None;
        string coverDragDropDragUIOverrideCaption = string.Empty;

        async void CoverButton_DragEnter(object sender, DragEventArgs e)
        {
            // This thing likes to break, so I took the advice from this thread https://github.com/microsoft/microsoft-ui-xaml/issues/8108

            // Default to this.
            coverDragDropAcceptedOperation = DataPackageOperation.None;
            coverDragDropDragUIOverrideCaption = string.Empty;

            e.AcceptedOperation = coverDragDropAcceptedOperation;
            e.DragUIOverride.Caption = coverDragDropDragUIOverrideCaption;

            // This await messes things up. So what we do is also handle in CoverButton_DragOver which will have hopefully
            // mean this code is finished by then.
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 1)
            {
                var storageFile = items[0] as StorageFile;

                if (storageFile is null)
                {
                    coverDragDropAcceptedOperation = DataPackageOperation.None;
                    coverDragDropDragUIOverrideCaption = $"storageFile is null";
                }
                else if (customCoverValidFileTypes.Contains(storageFile.FileType.ToLower()))
                {
                    coverDragDropAcceptedOperation = DataPackageOperation.Copy;
                    coverDragDropDragUIOverrideCaption = "Add custom cover";
                }
                else
                {
                    coverDragDropAcceptedOperation = DataPackageOperation.None;
                    coverDragDropDragUIOverrideCaption = $"\"{storageFile.FileType}\" is an invalid file type";
                }
            }
            else
            {
                coverDragDropAcceptedOperation = DataPackageOperation.None;
                coverDragDropDragUIOverrideCaption = "You may only drag over a single file for a cover";
            }
        }

        void CoverButton_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = coverDragDropAcceptedOperation;
            e.DragUIOverride.Caption = coverDragDropDragUIOverrideCaption;
        }

        async void CoverButton_Drop(object sender, DragEventArgs e)
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 1)
            {
                var storageFile = items[0] as StorageFile;

                if (storageFile is null)
                {
                    Logger.Error($"storageFile is null");
                }
                else if (customCoverValidFileTypes.Contains(storageFile.FileType.ToLower()))
                {
                    using (var stream = await storageFile.OpenStreamForReadAsync())
                    {
                        if (DataContext is ManuallyAddGameModel manuallyAddGameModel)
                        {
                            manuallyAddGameModel.Game.AddCustomCover(stream);
                        }
                    }
                }
                else
                {
                    Logger.Error($"\"{storageFile.FileType}\" is an invalid file type");
                }
            }
            else
            {
                Logger.Error("You may only drag over a single cover");
            }
        }
    }
}
