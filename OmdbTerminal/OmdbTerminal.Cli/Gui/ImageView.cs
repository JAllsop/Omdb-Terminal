using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Terminal.Gui;

namespace OmdbTerminal.Cli.Gui;

public class ImageView : View
{
    private Image<Rgba32>? _originalImage;
    private Rgba32[,]? _pixels;
    private int _renderedWidth = 0;
    private int _renderedHeight = 0;
    private string _currentUrl = string.Empty;

    public void LoadImageFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || url == "N/A" || _currentUrl == url)
        {
            return;
        }

        _currentUrl = url;

        Task.Run(async () =>
        {
            try
            {
                using var client = new HttpClient();
                var imageBytes = await client.GetByteArrayAsync(url);
                using var stream = new MemoryStream(imageBytes);
                var image = await Image.LoadAsync<Rgba32>(stream);

                Application.MainLoop.Invoke(() =>
                {
                    _originalImage?.Dispose();
                    _originalImage = image;
                    _pixels = null; // force re-render
                    SetNeedsDisplay();
                });
            }
            catch
            {
                // Fallback / fail silently
            }
        });
    }

    public override void Redraw(Rect bounds)
    {
        base.Redraw(bounds);

        if (_originalImage == null)
        {
            var text = "Loading...";
            Move(Math.Max(0, (bounds.Width - text.Length) / 2), Math.Max(0, bounds.Height / 2));
            Driver.AddStr(text);
            return;
        }

        // We need 1 console cell per 2 vertical pixels (half block)
        int targetWidth = bounds.Width;
        int targetHeight = bounds.Height * 2;

        if (targetWidth <= 0 || targetHeight <= 0) return;

        // Rebuild pixels cache if bounds changed
        if (_pixels == null || _renderedWidth != targetWidth || _renderedHeight != targetHeight)
        {
            _renderedWidth = targetWidth;
            _renderedHeight = targetHeight;

            using var resized = _originalImage.Clone(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(targetWidth, targetHeight),
                Mode = ResizeMode.Max
            }));

            _pixels = new Rgba32[resized.Width, resized.Height];
            for (int y = 0; y < resized.Height; y++)
            {
                for (int x = 0; x < resized.Width; x++)
                {
                    _pixels[x, y] = resized[x, y];
                }
            }
        }

        int drawWidth = _pixels.GetLength(0);
        int drawHeight = _pixels.GetLength(1);

        int offsetX = Math.Max(0, (bounds.Width - drawWidth) / 2);
        int offsetY = Math.Max(0, (bounds.Height - (drawHeight / 2)) / 2);

        for (int y = 0; y < drawHeight / 2; y++)
        {
            Move(offsetX, offsetY + y);

            for (int x = 0; x < drawWidth; x++)
            {
                var topColor = _pixels[x, y * 2];
                var bottomColor = (y * 2 + 1 < drawHeight) ? _pixels[x, y * 2 + 1] : topColor;

                var fg = GetClosestConsoleColor(bottomColor.R, bottomColor.G, bottomColor.B);
                var bg = GetClosestConsoleColor(topColor.R, topColor.G, topColor.B);

                Driver.SetAttribute(Driver.MakeAttribute(fg, bg));
                Driver.AddRune(new System.Rune('\u2584')); // Lower half block
            }
        }
    }

    private Terminal.Gui.Color GetClosestConsoleColor(byte r, byte g, byte b)
    {
        Terminal.Gui.Color closest = Terminal.Gui.Color.Black;
        double minDistance = double.MaxValue;

        var colors = new (Terminal.Gui.Color Color, byte R, byte G, byte B)[]
        {
            (Terminal.Gui.Color.Black, 0, 0, 0),
            (Terminal.Gui.Color.Blue, 0, 0, 128),
            (Terminal.Gui.Color.Green, 0, 128, 0),
            (Terminal.Gui.Color.Cyan, 0, 128, 128),
            (Terminal.Gui.Color.Red, 128, 0, 0),
            (Terminal.Gui.Color.Magenta, 128, 0, 128),
            (Terminal.Gui.Color.Brown, 128, 128, 0),
            (Terminal.Gui.Color.Gray, 192, 192, 192),
            (Terminal.Gui.Color.DarkGray, 128, 128, 128),
            (Terminal.Gui.Color.BrightBlue, 0, 0, 255),
            (Terminal.Gui.Color.BrightGreen, 0, 255, 0),
            (Terminal.Gui.Color.BrightCyan, 0, 255, 255),
            (Terminal.Gui.Color.BrightRed, 255, 0, 0),
            (Terminal.Gui.Color.BrightMagenta, 255, 0, 255),
            (Terminal.Gui.Color.BrightYellow, 255, 255, 0),
            (Terminal.Gui.Color.White, 255, 255, 255)
        };

        foreach (var c in colors)
        {
            double dist = Math.Pow(r - c.R, 2) + Math.Pow(g - c.G, 2) + Math.Pow(b - c.B, 2);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = c.Color;
            }
        }

        return closest;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _originalImage?.Dispose();
        }
        base.Dispose(disposing);
    }
}