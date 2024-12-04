using Microsoft.AspNetCore.Mvc;
using ZXing;
using ZXing.Common;

namespace Hospitalmicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateBarCodeController : ControllerBase
    {
        [ApiController]
        [Route("api/[controller]")]
        public class BarcodeController : ControllerBase
        {
            [HttpGet("generate")]
            public IActionResult GenerateBarcode(string content)
            {
                // Check if the content is null or empty
                if (string.IsNullOrEmpty(content))
                {
                    return BadRequest("Content cannot be empty. Please provide valid content to generate the barcode.");
                }

                // Define the barcode writer
                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128, // Barcode format (CODE_128)
                    Options = new EncodingOptions
                    {
                        Height = 100,   // Height of the barcode
                        Width = 300,    // Width of the barcode
                        Margin = 1      // Margin around the barcode
                    }
                };
                var pixelData = barcodeWriter.Write(content);

                // Convert pixel data to an image (PNG format)
                using var bitmap = new  System.Drawing.Bitmap(pixelData.Width, pixelData.Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                                 System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                bitmap.UnlockBits(bitmapData);

                // Convert the bitmap to a memory stream
                using var ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                // Return the image as a FileStreamResult
                return File(ms.ToArray(), "image/png");
            }
        }
    }
}
