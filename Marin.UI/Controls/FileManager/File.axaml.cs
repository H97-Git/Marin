﻿using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Marin.UI.Commands;
using Marin.UI.Helpers;
using Marin.UI.Models;
using Marin.UI.ViewModels.FileManager;
using ReactiveUI;
using Serilog;

namespace Marin.UI.Controls.FileManager;

public partial class File : UserControl
{
    #region Private Fields

    private FileViewModel? FileViewModel => DataContext as FileViewModel;
    private readonly DispatcherTimer _previewTimer;

    #endregion

    #region Private Methods

    private void OnFolderDoubleClick_ChangePath(object? sender, TappedEventArgs e)
    {
        Log.Debug("ChangePath on DoubleClick");
        StopTimer();
        if (FileViewModel is null) return;
        MessageBus.Current.SendMessage(new ChangePathCommand(FileViewModel.FullPath));
    }

    private void OnPointerPressed_StartTimerTickPreview(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed) return;
        if (FileViewModel is {IsImage: true}) return;
        _previewTimer.Start();
    }

    /// <summary>
    /// Setup the preview and start it.
    /// </summary>
    /// <param name="sender">Unused</param>
    /// <param name="e">Unused</param>
    private void OnPreviewTimerTick(object? sender, EventArgs e)
    {
        if (!_previewTimer.IsEnabled) return;
        if (FileViewModel is null) return;
        Log.Debug("PreviewTimerTick");
        _previewTimer.Stop();
        MessageBus.Current.SendMessage(new StartPreviewCommand(FileViewModel.FullPath));
    }

    // private void OnPointerExited_ClosePreview(object? sender, PointerEventArgs e) =>
    //     FileViewModel?.SendCommandToFileManager(new Command(CommandType.ClosePreview));

    private void OnPointerReleased_OpenInNewTab(object? sender, PointerReleasedEventArgs e)
    {
        Log.Debug("OpenInNewTab on PointerReleased");
        StopTimer();
        if (e.InitialPressMouseButton is not MouseButton.Middle) return;
        if (FileViewModel is null) return;

        var imagesInPath = PathHelper.GetAllImages(FileViewModel);
        if (imagesInPath is {Length: 0})
        {
            MessageBus.Current.SendMessage(new SendMessageToStatusBarCommand(InfoBarSeverity.Warning,
                "No images found in folder"));
            return;
        }

        var index = 0;
        if (FileViewModel.IsImage)
        {
            index = Array.IndexOf(imagesInPath, FileViewModel.FullPath);
        }

        var command = new OpenInNewTabCommand(index, imagesInPath);

        MessageBus.Current.SendMessage(command);
    }

    private void StopTimer()
    {
        if (_previewTimer.IsEnabled)
            _previewTimer.Stop();
    }


    private void Rename_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (FileViewModel is null) return;
        if (e.Key is Key.Escape)
        {
            FileViewModel.IsRenaming = false;
            return;
        }

        if (e.Key is not Key.Enter) return;
        MessageBus.Current.SendMessage(new RenameCommand(FileViewModel.FullPath, FileViewModel.FileName));
        FileViewModel.IsRenaming = false;
        e.Handled = true;
    }

    #endregion

    #region CTOR

    public File()
    {
        _previewTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200),
        };
        _previewTimer.Tick += OnPreviewTimerTick;
        InitializeComponent();
        FileControl.DoubleTapped += OnFolderDoubleClick_ChangePath;
        // FileControl.PointerExited += OnPointerExited_ClosePreview;
        FileControl.PointerPressed += OnPointerPressed_StartTimerTickPreview;
        FileControl.PointerReleased += OnPointerReleased_OpenInNewTab;

        FileControlGrid.Width = Settings.Instance.FileManagerPreference.FileWidth;
        // FileControlGrid.Height = Settings.Instance.FileManagerPreference.FileHeight;
    }

    #endregion

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        if (DataContext is FileViewModel {IsImage: true} fileViewModel)
            await fileViewModel.GetThumbnail();
        base.OnLoaded(e);
    }
}