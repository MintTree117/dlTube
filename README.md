# dlTube

## Features

dlTube if free and lets you download youtube videos as video & audio, audio only, or video only

### Desktop (Avalonia Version)

- **YouTube Video Downloader**: Download mixed, video-only, or audio-only streams from YouTube.
- **FFmpeg Thumbnail Generation**: With FFmpeg installed, dlTube can generate thumbnails for the downloaded videos, making your collection visually appealing.
- **Quality Selection**: Choose the download quality, offering flexibility between saving data or enjoying high-definition videos.
- **Text Search**: Browse YouTube streams using text search to easily find the content you're looking for without leaving the app.
- **Private Archive Access**: Special users gain access to a private archive, offering exclusive content or features.

### Web Version

- **Server-Side Download**: dlTube on the web focuses on enabling users to download videos directly through the server, streamlining the process without the need for local processing.

### Console Version

- **Local Download**: The console version of dlTube is streamlined for straightforward, local video downloads, supporting mixed, video-only, or audio-only streams from YouTube.


## Prerequisites

Before installing dlTube, ensure you have the following prerequisites installed on your system:

- **.NET 8**: Required to run the application. Download and install from [here](https://dotnet.microsoft.com/download/dotnet/8.0).
- **FFmpeg** (Optional): Required only if you need support for file thumbnail images. Install FFmpeg following the instructions for your operating system.

## Installation Instructions

### Windows

1. Ensure you have .NET 8 installed.
2. (Optional) Install FFmpeg to enable file thumbnail generation.
3. Download the latest release of *Project Name* for Windows from the [releases page](<link-to-releases-page>).
4. Unzip the downloaded file to your desired location.
5. Open a command prompt in the application's directory.
6. Run the application with: dotnet YourApp.dll

### Linux Mint

1. Ensure you have .NET 8 installed: you can run these commands:
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update;
    sudo apt-get install -y apt-transport-https &&
    sudo apt-get update &&
    sudo apt-get install -y dotnet-runtime-8.0

2. (Optional) Install FFmpeg for file thumbnail support:
3. Download the latest release of *Project Name* for Linux from the [releases page](<link-to-releases-page>).
4. Extract the downloaded file to your desired location.
5. Open a terminal in the application's directory.
6. Run the application with: dotnet YourApp.dll


### macOS (Coming Soon)

Support for macOS is under development and will be available soon. Stay tuned for updates!

