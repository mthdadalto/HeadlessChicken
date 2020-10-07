namespace App.Render
{
	/// <summary>
	/// TR Render base
	/// </summary>
	public interface IRenderBase
	{
		/// <summary>
		/// Instance the GPU Starting point
		/// </summary>
		void Initialize();

		/// <summary>
		/// Update Render Configs
		/// </summary>
		/// <param name="config">new config file</param>
		void UpdateConfigs(RenderConfig config);

		/// <summary>
		/// Pass new instructions to be draw
		/// </summary>
		/// <param name="vbo">Array of triangle vertices(3f pos, 3f norm, 2f tex)</param>
		/// <param name="trianglesCount">Number of triangles(3 vertices)</param>
		/// <param name="flipHorizontal">Flip the output horizontally</param>
		/// <returns>PNG as byte array</returns>
		byte[] VboToPng(float[] vbo, int trianglesCount, bool flipHorizontal = false);
	}
	public interface IGetRender
	{
		IRenderBase GetRender();
	}
}
