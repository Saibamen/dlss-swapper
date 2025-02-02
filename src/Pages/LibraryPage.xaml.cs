﻿using System;
using DLSS_Swapper.Data;
using DLSS_Swapper.UserControls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLSS_Swapper.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {
        public LibraryPageModel ViewModel { get; private set; }

        public LibraryPage()
        {
            InitializeComponent();
            ViewModel = new LibraryPageModel(this);
        }

        private void MainGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // via: https://stackoverflow.com/a/41141249
            var columns = Math.Ceiling(MainGridView.ActualWidth / 400);
            ((ItemsWrapGrid)MainGridView.ItemsPanelRoot).ItemWidth = e.NewSize.Width / columns;
        }

        private async void MainGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            MainGridView.SelectedIndex = -1;

            if (e.AddedItems[0] is DLLRecord dllRecord)
            {
                var dialog = new EasyContentDialog(XamlRoot)
                {
                    Title = DLLManager.Instance.GetAssetTypeName(dllRecord.AssetType),
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Close,
                    Content = new DLSSRecordInfoControl(dllRecord),
                };
                await dialog.ShowAsync();
            }
        }
    }
}
