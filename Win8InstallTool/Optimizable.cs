namespace Win8InstallTool
{
	public interface Optimizable
	{
		string Name { get; }

		string Description { get; }

		void Optimize();
	}
}
