using System.IO;

namespace ImageTransferReceiveAPI.Controllers
{
	class Data
	{
		public string getImageOutput(string path)
		{
			string data = "";
			while(!File.Exists(path))
			{

			}
			data = File.ReadAllText(path);
			return data; 
		}
	}
}
