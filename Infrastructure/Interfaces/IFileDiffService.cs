using System.Diagnostics;

namespace Infrastructure.Utilities
{
	public enum DiffServicePriority
	{
		First,
		Second,
		Third
	};

	public interface IFileDiffService
	{
		Process ShowDiffs(string fileNameRight, string fileNameLeft);
		bool IsSupported { get; }
		DiffServicePriority Priority { get; }
	}
}