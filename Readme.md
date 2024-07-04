# Waifu Gallery (Name Subject to Change)

Waifu Gallery is an image viewer made in C# with AvaloniaUI. This application is designed to be used efficiently with either a mouse or a keyboard, allowing for easy one-handed operation. The application features a tab-oriented interface, file exploration and management, customizable keyboard shortcuts, and more.

## Features

- **Tab-Oriented Interface**: Manage multiple image viewing sessions with a tabbed interface.
- **Keyboard and Mouse Support**: Navigate and use the application with either a mouse or keyboard.
- **File Explorer/Manager**: Browse and manage your image files directly within the application.
- **Image Previewer**: Click and hold on a folder to preview images inside, with up to 3 levels of depth for subfolder exploration.
- **Customizable Keyboard Shortcuts**: Set your preferred keyboard shortcuts for various actions.
- **Hide UI Elements in Full Screen**: Focus on your images by hiding UI elements when in full-screen mode.
- **Session Management**: Save your sessions (open tabs) for later use.
- **No Image Editing**: This application is focused on viewing images and does not include image editing features like cropping, adjusting brightness, or contrast.

## Installation

To run Waifu Gallery, follow these steps:

1. **Clone the Repository**:
    ```bash
    git clone https://github.com/H97-Git/waifu-gallery.git
    cd waifu-gallery
    ```

2. **Install Dependencies**:
   Make sure you have the .NET SDK installed. You can download it from [here](https://dotnet.microsoft.com/download).

3. **Build the Project**:
    ```bash
    dotnet build
    ```

4. **Run the Application**:
    ```bash
    dotnet run
    ```

## Usage

### Keyboard Shortcuts (Not yet implemented)

The application comes with customizable keyboard shortcuts. To configure these shortcuts, go to the settings menu and set your preferred key bindings for various actions.

### File Explorer

The file explorer allows you to navigate your file system and manage your image files. You can perform basic file operations such as copy, cut, paste, and delete.

### Image Previewer

Click and hold on a folder (or press `P`) to preview the images inside. The previewer supports exploring up to 3 levels of subfolders to display images.

### Full Screen Mode

Press the fullscreen toggle button or use the keyboard shortcut to enter fullscreen mode. In fullscreen mode, UI elements are hidden to provide an unobstructed view of your images.

### Session Management (Not yet implemented)

Your open tabs can be saved and restored later, allowing you to pick up where you left off.

## Contributing

We welcome contributions to the project. If you find any issues or have suggestions for new features, please open an issue on GitHub. To contribute code:

1. **Fork the Repository**:
   Click the "Fork" button at the top right of this page.

2. **Clone Your Fork**:
    ```bash
    git clone https://github.com/H97-Git/waifu-gallery.git
    cd waifu-gallery
    ```

3. **Create a Branch**:
    ```bash
    git checkout -b feature/your-feature-name
    ```

4. **Commit Your Changes**:
    ```bash
    git commit -m "Add some feature"
    ```

5. **Push to the Branch**:
    ```bash
    git push origin feature/your-feature-name
    ```

6. **Open a Pull Request**:
   Open a pull request on GitHub and provide a description of your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [AvaloniaUI](https://avaloniaui.net/) for the cross-platform UI framework.
- [FluentAvaloniaUI](https://www.nuget.org/packages/FluentAvaloniaUI) for their beautiful controls.
- [Magick.NET](https://www.nuget.org/packages/Magick.NET.Core) for image processing.
- [NaturalSort](https://www.nuget.org/packages/NaturalSort.Extension/) for natural sorting of file names.
- [PanAndZoom](https://www.nuget.org/packages/Avalonia.Controls.PanAndZoom) for zooming and panning images in Tab.
- [SharpCompress](https://www.nuget.org/packages/SharpCompress) for archives extraction.

---