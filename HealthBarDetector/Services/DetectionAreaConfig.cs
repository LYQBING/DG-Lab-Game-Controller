using System.Drawing;

namespace HealthBarDetector.Services
{
	/// <summary>
	/// 区域检测方案
	/// </summary>
	public enum DetectionType
	{
		/// <summary> 指定颜色最多时执行 </summary>
		MostFrequentColorIsTarget,
		/// <summary> 指定颜色最少时执行 </summary>
		MostFrequentColorIsNotTarget,
		/// <summary> 根据颜色已占据比例执行 </summary>
		TargetColorPercentage,
		/// <summary> 根据颜色未占据比例执行 </summary>
		TargetColorNotPercentage
	}

	/// <summary>
	/// 惩罚方案
	/// </summary>
	public enum PenaltyType
	{
		/// <summary> 设置强度 </summary>
		SetStrength,
		/// <summary> 增加强度 </summary>
		AddStrength,
		/// <summary> 减弱强度 </summary>
		SubStrength,
		/// <summary> 设置随机强度 </summary>
		SetRandomStrength,
		/// <summary> 增加随机强度 </summary>
		AddRandomStrength,
		/// <summary> 减弱随机强度 </summary>
		SubRandomStrength
	}

	/// <summary>
	/// 单个区域的惩罚配置单
	/// </summary>
	public class DetectionAreaConfig
	{
		public Rectangle Area { get; set; } = new(255,255,255,255); // 屏幕坐标区域
		public Color TargetColor { get; set; } = Color.Black; // 目标颜色
		public int Tolerance { get; set; } = 25; // 容差值
		public DetectionType DetectionType { get; set; } = DetectionType.MostFrequentColorIsTarget; // 处罚方案
		public float Threshold { get; set; } = 1; // 最佳百分比
		public PenaltyType PenaltyType { get; set; } = PenaltyType.SetStrength; // 惩罚类型
		public int PenaltyValueA { get; set; } = 30; // 惩罚值 A
		public bool Enabled { get; set; } = true; // 是否启用此检测区域
		public string Name { get; set; } = "新区域配置单"; // 区域名称

		public AreaTemp Temps { get; set; } = new();// 临时数据列表
		public string DebugString => $"{Name}：{(Enabled ? "已启用" : "已禁用")}";
		public System.Windows.Media.SolidColorBrush TargetBrush => new(System.Windows.Media.Color.FromArgb(TargetColor.A, TargetColor.R, TargetColor.G, TargetColor.B));
	}
	
	public class AreaTemp()
	{
		public double percent = 0;
		public Color color = Color.White;
	}
}