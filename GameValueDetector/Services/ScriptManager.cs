using GameValueDetector.Models;
using Newtonsoft.Json;
using System.IO;

namespace GameValueDetector.Services
{
	/// <summary>
	/// 脚本管理器
	/// </summary>
	public static class ScriptManager
    {
		/// <summary>
		/// 获取指定文件夹下的所有脚本文件
		/// </summary>
		/// <param name="folderPath">文件夹路径</param>
		/// <returns>文件列表</returns>
		public static List<string> GetScriptFiles(string folderPath)
        {
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
				return [];
			}
            return [.. Directory.GetFiles(folderPath, "*.json").Select(Path.GetFileName)];
        }

		/// <summary>
		/// 加载指定文件夹下的配置文件
		/// </summary>
		/// <param name="folderPath">文件路径</param>
		/// <param name="fileName">文件名称</param>
		/// <returns>游戏监控配置类</returns>
		public static GameMonitorConfig? LoadConfig(string folderPath, string fileName)
        {
            string path = Path.Combine(folderPath, fileName);
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<GameMonitorConfig>(json);
        }
    }
}