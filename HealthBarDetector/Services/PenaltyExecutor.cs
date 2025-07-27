using lyqbing.DGLAB;

namespace HealthBarDetector.Services
{
	/// <summary>
	/// 惩罚执行器
	/// </summary>
	public static class PenaltyExecutor
	{
		public static void Execute(PenaltyType type, int value)
		{
			switch (type)
			{
				case PenaltyType.SetStrength:
					DGLab.SetStrength.Set(value);
					break;
				case PenaltyType.AddStrength:
					DGLab.SetStrength.Add(value);
					break;
				case PenaltyType.SubStrength:
					DGLab.SetStrength.Sub(value);
					break;
				case PenaltyType.SetRandomStrength:
					DGLab.SetRandomStrength.Set(value);
					break;
				case PenaltyType.AddRandomStrength:
					DGLab.SetRandomStrength.Add(value);
					break;
				case PenaltyType.SubRandomStrength:
					DGLab.SetRandomStrength.Sub(value);
					break;
			}
		}
	}
}