﻿using R8.Lib.xUnit.TestOrder;

using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

using System;
using System.IO;

using Xunit;

namespace R8.Lib.FileHandlers.Test
{
    [TestOrder(1)]
    public class FileHandlersTests : TestClassBase
    {
        [Fact, TestOrder(0)]
        public void CallSaveImage()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Folder = Constants.Assets,
                HierarchicallyFolderNameByDate = false,
                RealFilename = true,
                OverwriteFile = true,
                TestDevelopment = true,
                Watermark = new MyFileConfigurationWatermark(Constants.WatermarkFile)
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.jpg", file.FilePath);
        }

        [Fact, TestOrder(1)]
        public void CallSaveImage_WithWatermark_Png()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new PngEncoder(),
                Folder = Constants.Assets,
                HierarchicallyFolderNameByDate = false,
                RealFilename = true,
                OverwriteFile = true,
                TestDevelopment = true,
                Watermark = new MyFileConfigurationWatermark(Constants.WatermarkFile)
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.png", file.FilePath);
        }

        [Fact, TestOrder(2)]
        public void CallSaveImage_WithWatermark_Bmp()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new BmpEncoder(),
                Folder = Constants.Assets,
                HierarchicallyFolderNameByDate = false,
                RealFilename = true,
                OverwriteFile = true,
                TestDevelopment = true,
                Watermark = new MyFileConfigurationWatermark(Constants.WatermarkFile)
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.bmp", file.FilePath);
        }

        [Fact, TestOrder(3)]
        public void CallSave_StreamNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((FileStream)null).Save(null, null));
        }

        [Fact, TestOrder(4)]
        public void CallSave_NameNull()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            Assert.Throws<ArgumentNullException>(() => fileStream.Save(null, null));
        }

        [Fact, TestOrder(5)]
        public void CallSaveImage_WithWatermark_Gif()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new GifEncoder(),
                Folder = Constants.Assets,
                HierarchicallyFolderNameByDate = false,
                RealFilename = true,
                OverwriteFile = true,
                TestDevelopment = true,
                Watermark = new MyFileConfigurationWatermark(Constants.WatermarkFile)
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.gif", file.FilePath);
        }

        [Fact, TestOrder(6)]
        public void CallSave_WithoutExtension()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            Assert.Throws<NullReferenceException>(() => fileStream.Save("test", null));
        }

        [Fact, TestOrder(7)]
        public void CallSave_Zip()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidZipFile2, FileMode.Open);
            var file = fileStream.Save<MyFileConfiguration>("valid.zip", cfg =>
            {
                cfg.Folder = Constants.Assets;
                cfg.HierarchicallyFolderNameByDate = false;
                cfg.RealFilename = true;
                cfg.OverwriteFile = true;
                cfg.TestDevelopment = true;
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/valid.zip", file.FilePath);
        }

        [Fact, TestOrder(8)]
        public void CallSave_Pdf()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidPdfFile, FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationPdf>("test.pdf", cfg =>
            {
                cfg.GhostScriptDllPath = Constants.GhostScriptFile;
                cfg.Folder = Constants.Assets;
                cfg.HierarchicallyFolderNameByDate = false;
                cfg.RealFilename = true;
                cfg.OverwriteFile = true;
                cfg.TestDevelopment = true;
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.pdf", file.FilePath);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test_thumbnail.jpg", file.ThumbnailPath);
        }

        [Fact, TestOrder(9)]
        public void CallSave_Image()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.Save<MyFileConfigurationImage>("test.jpg", cfg =>
            {
                cfg.ImageEncoder = new JpegEncoder();
                cfg.Folder = Constants.Assets;
                cfg.HierarchicallyFolderNameByDate = false;
                cfg.RealFilename = true;
                cfg.OverwriteFile = true;
                cfg.TestDevelopment = true;
                cfg.Watermark = new MyFileConfigurationWatermark(Constants.WatermarkFile);
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.jpg", file.FilePath);
        }

        [Fact, TestOrder(10)]
        public void CallSaveImage_WithWatermark_Jpg()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Folder = Constants.Assets,
                HierarchicallyFolderNameByDate = false,
                RealFilename = true,
                OverwriteFile = true,
                TestDevelopment = true,
                Watermark = new MyFileConfigurationWatermark(Constants.WatermarkFile)
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.jpg", file.FilePath);
        }

        [Fact, TestOrder(11)]
        public void CallSaveImage_NullStream()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => ((Stream)null).SaveImage(null, null));
        }

        [Fact, TestOrder(12)]
        public void CallSaveImage_NullName()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            Assert.Throws<ArgumentNullException>(() => fileStream.SaveImage(null, null));
        }

        [Fact, TestOrder(13)]
        public void CallSaveImage_Resize()
        {
            // Act
            using var fileStream = new FileStream(Constants.ValidImageFile, FileMode.Open);
            var file = fileStream.SaveImage("test", new MyFileConfigurationImage
            {
                ImageEncoder = new JpegEncoder(),
                Folder = Constants.Assets,
                HierarchicallyFolderNameByDate = false,
                RealFilename = true,
                OverwriteFile = true,
                TestDevelopment = true,
                Watermark = new MyFileConfigurationWatermark(Constants.WatermarkFile),
                ResizeToSize = 300
            });

            Assert.NotNull(file);
            Assert.Equal("E:/Work/Develope/Libraries/R8.Lib/R8.Lib.FileHandlers.Test/assets/test.jpg", file.FilePath);
        }
    }
}