using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.IO;
using System.Runtime.Loader;

public class PdfGenerator : IDisposable
{
    private readonly IConverter _converter;
    private readonly CustomAssemblyLoadContext _context;

    public PdfGenerator()
    {
        _context = new CustomAssemblyLoadContext();
        var dllPath = Path.Combine(AppContext.BaseDirectory, "wkhtmltopdf", "libwkhtmltox.dll");
        _context.LoadUnmanagedLibrary(dllPath);

        _converter = new BasicConverter(new PdfTools());
    }

    public byte[] Generar(string html)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
            },
            Objects = {
                new ObjectSettings
                {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        return _converter.Convert(doc);
    }

    public void Dispose()
    {
        _context?.Unload();
    }
}

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }
}