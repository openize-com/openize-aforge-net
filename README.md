# Openize.AForge

**Openize.AForge**  This is a fork of the [AForge.Net](https://github.com/andrewkirillov/AForge.NET) project, which includes the Imaging library. This version has been translated into the NetStandard 2.0 framework, and uses [Aspose.Drawing](https://docs.aspose.com/drawing/net/) as a graphics engine, which allows you to create cross-platform applications using lastest .Net platforms.

## Platform dependence

Openize.AForge can be used to develop applications on Windows Desktop (x86, x64), Windows Server (x86, x64), Windows Azure, Windows Embedded (CE 6.0 R2), as well as Linux x64. The supported platforms include Net Core 3.1, Net6.0, Net7.0, Net8.0.

## New Features & Enhancements in Version 25.2
 - Change FileFormat to Openize
 - Update Aspose.Drawing Engine


## Getting Started with Openize.AForge for .NET

Are you ready to give Openize.AForge a try? Simply execute 

```
Install-Package Openize.AForge
```

from Package Manager Console in Visual Studio to fetch the NuGet package. If you already have AForge.Imaging.Net and want to upgrade the version, please execute 

```
Update-Package Openize.AForge
```

 to get the latest version.
 
## Product License
 - [Openize.AForge is distributed under LGPL license](http://www.gnu.org/licenses/lgpl.html)
 - [Aspose.Drawing .NET is distributed under Aspose EULA license](https://www.conholdate.app/viewer/view/4Y8UNm7laVFjMAd0r/aspose_end-user-license-agreement_2024-05-16.pdf);


## Usage example

### Export Apply grayscale filter

```csharp
//Set License for Aspose.Drawing
System.Drawing.AsposeDrawing.License license = new System.Drawing.AsposeDrawing.License();
license.SetLicense("Aspose.Drawing.License.lic");

//Create grayscale filter
var grayscaleFilter = new Openize.AForge.Imaging.Filters.Grayscale(0.2126, 0.7152, 0.0722);

//open image
using (var bmp = (Bitmap)Image.FromFile(@"sample.bmp"))

//convert image to grayscale
using (var grayscaleImage = grayscaleFilter.Apply(bmp))
{
    //save grayscale image
    grayscaleImage.Save(@"grayscale.png", ImageFormat.Png);
}
```

You can see other examples directly on the [AForge website](https://www.aforgenet.com/framework/features/).
