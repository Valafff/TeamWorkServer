using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.BLL.Services
{
	//Работа класса - картинки для администраторов, клиентов, и блюд хранятся в разных папках (обязательное условие!)
	//Для чтения/записи картинки нужно указать id, целевую папку _path, название файла _filename с расширением, то что надо записать _array
	internal class ImageWorks
	{
		static public void ImageWriter(int _Id, string _path, string _fileName, byte[] _array)
		{
			try
			{
				if (!Directory.Exists(_path + "\\" + _Id))
				{
					Directory.CreateDirectory(_path + "\\" + _Id);
				}
				using (FileStream fs = new FileStream(_path + "\\" + _Id + "\\" + _fileName, FileMode.Create))
				{
					fs.Write(_array);
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Ошибка ImageWriter");
				throw;
			}
		}

		static public byte[] ImageReader(int _Id, string _path, string _fileName)
		{
			try
			{
				FileInfo file = new FileInfo(_path + "\\" + _Id + "\\" + _fileName);
				byte[] buffer = new byte[file.Length];
				using (FileStream fs = new FileStream(_path + "\\" + _Id + "\\" + _fileName, FileMode.Open))
				{
					fs.Read(buffer, 0, buffer.Length);
					return buffer;
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Ошибка ImageReader");
				throw;
			}
		}
	}
}
