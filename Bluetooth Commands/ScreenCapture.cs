using OpenCvSharp;
using System;
using Tesseract;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using OpenCvSharp.Extensions;

public class ScreenCapture
{
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static Bitmap CaptureScreen()
    {
        Rect rect = new Rect();
        GetWindowRect(GetDesktopWindow(), ref rect);

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
        }

        return bmp;
    }

    public static Bitmap PreprocessImage(Bitmap bmp)
    {
        // Convert Bitmap to Mat
        Mat src = BitmapConverter.ToMat(bmp);

        // Convert to grayscale
        Mat gray = new Mat();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

        // Apply Otsu threshold
        Mat binary = new Mat();
        Cv2.Threshold(gray, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

        Bitmap resultBitmap = BitmapConverter.ToBitmap(binary);

        return resultBitmap;
    }

    private static Pix BitmapToPix(Bitmap bmp)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            bmp.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;
            return Pix.LoadFromMemory(stream.ToArray());
        }
    }

    public static System.Drawing.Point FindTextOnScreen(string searchText)
    {
        try
        {
            Bitmap bmp = CaptureScreen();

            // Save the captured screen for debugging
            string capturedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "captured_screen.png");
            bmp.Save(capturedPath, ImageFormat.Png);
            Console.WriteLine($"Captured screen saved at: {capturedPath}");

            // Preprocess the image to enhance OCR detection
            Bitmap processedBmp = PreprocessImage(bmp);

            // Save the preprocessed image for debugging
            string preprocessedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "preprocessed_screen.png");
            processedBmp.Save(preprocessedPath, ImageFormat.Png);
            Console.WriteLine($"Preprocessed screen saved at: {preprocessedPath}");

            // Get the absolute path to the tessdata directory
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
            string tessdataPath = Path.Combine(projectRoot, "tessdata");

            var ocr = new TesseractEngine(tessdataPath, "eng", EngineMode.Default);

            using (var page = ocr.Process(BitmapToPix(processedBmp)))
            {
                Console.WriteLine($"OCR Text: {page.GetText()}"); // Debugging output to see OCR results

                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                        {
                            string word = iter.GetText(PageIteratorLevel.Word);
                            Console.WriteLine($"Detected word: {word}, Bounding Box: {rect}");

                            if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                            {
                                int deltaX = rect.X1;
                                int deltaY = rect.Y1;
                                return new System.Drawing.Point(deltaX, deltaY);
                            }
                        }
                    } while (iter.Next(PageIteratorLevel.Word));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}");
            Console.WriteLine($"Inner Exception: {ex.InnerException}");
        }

        return System.Drawing.Point.Empty; // Return an empty point if the text is not found
    }
}
