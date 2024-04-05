using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
	public static class ImageProcesser
	{
		private const UInt32 SPI_GETDESKWALLPAPER = 0x73;
		private const int MAX_PATH = 260;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(UInt32 uAction, int uParam, string lpvParam, int fuWinIni);

		public static async Task<Texture2D> LoadDesktopBackgroundTextureOrNull(int width, int height)
		{
			try
			{
				string path = "";

				await Task.Factory.StartNew(() =>
				{
					string currentWallpaper = new string('\0', MAX_PATH);
					SystemParametersInfo(SPI_GETDESKWALLPAPER, currentWallpaper.Length, currentWallpaper, 0);
					path = currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0'));
				});

				// Load and set screen image
				var screenImageRawData = await System.IO.File.ReadAllBytesAsync(path);
				Texture2D screenTexture = await AsyncImageLoader.CreateFromImageAsync(screenImageRawData);

				// Set screen image
				var result = RemapTexture(screenTexture, width, height);
				result.filterMode = FilterMode.Point;

				return result;
			}
			catch
			{
				return null;
			}
		}

		public static Texture2D RemapTexture(Texture2D src, int destWidth, int destHeight)
		{
			Texture2D dest = new Texture2D(destWidth, destHeight);

			int srcWidth = src.width;
			int srcHeight = src.height;

			float offsetX = (float)srcWidth / destWidth;
			float offsetY = (float)srcHeight / destHeight;

			for (int curDestY = 0; curDestY < destHeight; curDestY++)
			{
				for (int curDestX = 0; curDestX < destWidth; curDestX++)
				{
					int curSrcX = (int)(curDestX * offsetX);
					int curSrcY = (int)(curDestY * offsetY);

					var color = src.GetPixel(curSrcX, curSrcY);
					dest.SetPixel(curDestX, curDestY, color);
				}
			}

			dest.Apply();
			return dest;
		}
	}
}
