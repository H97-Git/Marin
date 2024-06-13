﻿using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.FileExplorer;

public sealed class FileViewModel : ViewModelBase
{
    #region Private Members

    private Bitmap? _thumbnail;
    private Size _imageSize;
    private bool _isImage;
    private readonly FileSystemInfo _fileSystemInfo;
    private readonly string _fileName = string.Empty;
    private string _createdTime = string.Empty;
    private string _lastAccessTime = string.Empty;
    private long _sizeInBytes;

    #endregion

    #region Public Properties

    public Bitmap? Thumbnail
    {
        get => _thumbnail;
        set
        {
            this.RaiseAndSetIfChanged(ref _thumbnail, value);
            ResizeThumbnail();
        }
    }

    public bool IsImage
    {
        get => _isImage;
        private set => this.RaiseAndSetIfChanged(ref _isImage, value);
    }

    public Size ImageSize
    {
        get => _imageSize;
        set => this.RaiseAndSetIfChanged(ref _imageSize, value);
    }

    public string FileName
    {
        get => _fileName;
        private init => this.RaiseAndSetIfChanged(ref _fileName, value);
    }

    public string LastAccessTime
    {
        get => _lastAccessTime;
        set => this.RaiseAndSetIfChanged(ref _lastAccessTime, value);
    }

    public string CreatedTime
    {
        get => _createdTime;
        set => this.RaiseAndSetIfChanged(ref _createdTime, value);
    }

    public long SizeInBytes
    {
        get => _sizeInBytes;
        set => this.RaiseAndSetIfChanged(ref _sizeInBytes, value);
    }
    
    public string SizeInHumanReadable
    {
        get
        {
            if (SizeInBytes < 1024)
                return $"{SizeInBytes} B";
            if (SizeInBytes < 1024 * 1024)
                return $"{SizeInBytes / 1024} KB";
            if (SizeInBytes < 1024 * 1024 * 1024)
                return $"{SizeInBytes / 1024 / 1024} MB";
            return $"{SizeInBytes / 1024 / 1024 / 1024} GB";
        }
    }

    public string FullPath => _fileSystemInfo.FullName;
    public string? ParentPath { get; }

    #endregion

    #region CTOR

    public FileViewModel(FileSystemInfo fileSystemInfo, Bitmap? thumbnail = null)
    {
        _fileSystemInfo = fileSystemInfo;
        switch (fileSystemInfo)
        {
            case DirectoryInfo directoryInfo:
                ParentPath = directoryInfo.Parent?.FullName;
                SizeInBytes = Helper.GetDirectorySizeInByte(fileSystemInfo);
                break;
            case FileInfo fileInfo:
                ParentPath = fileInfo.DirectoryName;
                SizeInBytes = fileInfo.Length;
                break;
        }

        CreatedTime = fileSystemInfo.CreationTime.ToString(CultureInfo.InvariantCulture);
        LastAccessTime = fileSystemInfo.LastAccessTime.ToString(CultureInfo.InvariantCulture);


        FileName = fileSystemInfo.Name;

        SizeInBytes = Helper.GetDirectorySizeInByte(fileSystemInfo);


        if (thumbnail is null) return;
        Thumbnail = thumbnail;
        IsImage = true;
    }

    #endregion

    #region Public Events

    public event EventHandler<Command>? OnSendCommandToFileExplorer;

    #endregion

    #region Private Methods

    private void ResizeThumbnail()
    {
        if (Thumbnail is null) return;
        var isPortrait = Thumbnail.Size.Width < Thumbnail.Size.Height;
        ImageSize = isPortrait
            ? Helper.GetScaledSizeByHeight(Thumbnail, 100)
            : Helper.GetScaledSizeByWidth(Thumbnail, 100);
    }

    #endregion

    #region Public Methods

    public void SendCommandToFileExplorer(Command command)
    {
        OnSendCommandToFileExplorer?.Invoke(this, command);
    }

    #endregion
}