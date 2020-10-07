using System.Numerics;
using System.Xml.Serialization;


namespace App.Render
{
    public class RenderConfig
	{
		[XmlIgnore]
		public string FileName = "";

		public int Width = 800;
		public int Height = 270;

		public string Resolution
		{
			get
			{
				string tmp = resolutionX + "/" + resolutionY;
				if (tmp.Equals(DefaultResolution)) { return "Default"; }
				if (tmp.Equals(LowResolution)) { return "Low"; }
				return tmp;
			}
			set
			{
				string tmp = value;
				//Pre sets
				if (string.IsNullOrEmpty(tmp))
				{
					tmp = DefaultResolution;
				}
				if (tmp.Equals("Default") || value.Equals("New"))
				{
					tmp = DefaultResolution;
				}
				if (value.Equals("Low") || value.Equals("Old"))
				{
					tmp = LowResolution;
				}
				//numeric resulotions
				try
				{
					string[] vals = tmp.Split('/');
					resolutionX = int.Parse(vals[0]);
					resolutionY = int.Parse(vals[1]);
				}
				catch
				{
					resolutionX = 400;
					resolutionY = 175;
				}
			}
		}
		public const string LowResolution = "140/75";
		public const string DefaultResolution = "400/175";

		internal int resolutionX = 400;
		internal int resolutionY = 175;

		public bool FlipVertical = true;
		public double FailBelow = 1.6;
		public double AdvisoryBelow = 3;

		public string TextureFile = "rubber4.png";

		public Vector3 CameraPosition = new Vector3(250.343f, 91.105f, 214.252f);
		public Vector3 CameraLookAt = new Vector3(-153.807f, -4.407f, -142.568f);
		public float CameraFOV = 40.0f;
		public float CameraNear = 0.125f;
		public float CameraFar = float.PositiveInfinity;

		public Vector3 ModelTranslation = new Vector3(0, 0, 0);
		public Vector3 ModelRotation = new Vector3(0, 0, 0);

		public Vector3 LightDirection = new Vector3(2.0f, 0.0f, -3.0f);
		public Vector3 LightAmbient = new Vector3(0.02f, 0.02f, 0.02f);
		public Vector3 LightDiffuse = new Vector3(0.8f, 0.8f, 0.9f);
		public Vector3 LightSpecular = new Vector3(0.3f, 0.3f, 0.3f);

		public Vector3 MaterialSpecular = new Vector3(0.3f, 0.3f, 0.4f);
		public Vector3 MaterialDiffuse = new Vector3(0.05f);

		public float MaterialShininess = 16.0f;

		public static RenderConfig Default => new RenderConfig
		{
			TextureFile = "rubber4.png",

			Width = 800,
			Height = 270,

			FailBelow = 1.6,
			AdvisoryBelow = 3,

			CameraPosition = new Vector3(250.0f, 0, 215.0f),
			CameraLookAt = new Vector3(0, 0, -35.0f),
			CameraFOV = 14.0f,
			CameraNear = 0.01f,
			CameraFar = 10000.0f,

			ModelTranslation = new Vector3(0, -87.5f, 0),
			ModelRotation = new Vector3(0, 0, 0.0f),

			LightDirection = new Vector3(1.7f, 0.2f, -1.0f),
			LightAmbient = new Vector3(0.02f, 0.02f, 0.02f),
			LightDiffuse = new Vector3(0.8f, 0.8f, 0.9f),
			LightSpecular = new Vector3(0.2f, 0.2f, 0.25f),

			MaterialSpecular = new Vector3(0.2f, 0.2f, 0.3f),
			MaterialDiffuse = new Vector3(0.05f),

			MaterialShininess = .0f
		};

		public static RenderConfig SnapSkanDefault => new RenderConfig
		{
			TextureFile = "snapskan.png",
			Resolution = "Low",

			Width = 698,
			Height = 299,

			FailBelow = 1.6,
			AdvisoryBelow = 3,

			CameraPosition = new Vector3(180.0f, 0, 230.0f),
			CameraLookAt = new Vector3(0, 0, 0),
			CameraFOV = 30.0f,
			CameraNear = 1.0f,
			CameraFar = 10000.0f,

			ModelTranslation = new Vector3(0, -85.0f, 50.0f),
			ModelRotation = new Vector3(0, 0.0f, 0.0f),

			LightDirection = new Vector3(0.888888f, 0.2f, -1.0f),
			LightAmbient = new Vector3(0.02f, 0.02f, 0.02f),
			LightDiffuse = new Vector3(0.8f, 0.8f, 0.9f),
			LightSpecular = new Vector3(0.3f, 0.3f, 0.3f),

			MaterialSpecular = new Vector3(0.3f, 0.3f, 0.4f),
			MaterialDiffuse = new Vector3(0.05f),

			MaterialShininess = 16.0f
		};
	}
}
