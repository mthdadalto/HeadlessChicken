using System.Numerics;
using Veldrid;
using System;
using System.Runtime.InteropServices;
using Veldrid.SPIRV;
using System.Threading;

namespace App.Render
{
    /// <summary>
    /// Multiplatfom Treadreader render
    /// </summary>
    public class VeldridRender : IRenderBase
    {
        readonly int threadId;
        void CheckThread()
        {
            int currentId = Thread.CurrentThread.ManagedThreadId;
            Logger.AddLog("Thread " + currentId);
            if (threadId != currentId) { throw new NotSupportedException("GPU is single-thread only, using Thread " + currentId + " instead of creation Thread " + threadId); }
        }

        #region Constructors
        VeldridRender(GraphicsDevice graphicsDevice)
        {
            threadId = Thread.CurrentThread.ManagedThreadId;
            this.graphicsDevice = graphicsDevice;
            resourceFactory = this.graphicsDevice.ResourceFactory;

            Logger.AddLog("VeldridRender..ctor Thread: " + threadId + ", Backend: " + graphicsDevice.BackendType);
        }

        /// <summary>
        /// Initialize using Vulkan drivers
        /// </summary>
        /// <returns>New render</returns>
        public static VeldridRender InitFromVulkan(bool invertBGR = false)
        {
            VeldridRender render = new VeldridRender(GraphicsDevice.CreateVulkan(new GraphicsDeviceOptions()));

            render.MakeSpirvShaders(invertBGR);
            render.FlipVertical = true;
            return render;
        }

        /// <summary>
        /// Initialize using Direct3D 11 drivers
        /// </summary>
        /// <returns>New render</returns>
        public static VeldridRender InitFromD3D11(bool invertBGR = false)
        {
            VeldridRender render = new VeldridRender(GraphicsDevice.CreateD3D11(new GraphicsDeviceOptions()));

            render.MakeSpirvShaders(invertBGR);
            render.FlipVertical = false;
            return render;
        }

        /// <summary>
        /// Initialize using Metal drivers
        /// </summary>
        /// <param name="handle">Provide a MetalKit View handle</param>
        /// <returns>Metal based render</returns>
        public static VeldridRender InitFromMetal(IntPtr handle)
        {
            try
            {
                var gd = GraphicsDevice.CreateMetal(new GraphicsDeviceOptions(), new SwapchainDescription(SwapchainSource.CreateUIView(handle), 20, 20, null, false));
                gd.ResourceFactory.CreateSwapchain(new SwapchainDescription(SwapchainSource.CreateUIView(handle), 20, 20, null, false)).Resize(20,20);
                gd.WaitForIdle();

                VeldridRender render = new VeldridRender(gd);


                byte[] data = Helpers.GetAssetByteArray("App.Shaders.Shader.metal");

                render.Shaders = new Shader[]
                {
                    render.graphicsDevice.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, data, "shader_vertex", true)),
                    render.graphicsDevice.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, data, "shader_fragment", true))
                };
                return render;
            }
            catch (Exception ex)
            {
                Logger.AddLog(ex);
            }
            return null;
        }
        
        public static VeldridRender InitFromMetal(SwapchainSource scs)
        {
            try
            {
                var gd = GraphicsDevice.CreateMetal(new GraphicsDeviceOptions(), new SwapchainDescription(scs, 20, 20, null, false));

                gd.WaitForIdle();

                gd.ResizeMainWindow(270, 800);

                VeldridRender render = new VeldridRender(gd);

                byte[] data = Helpers.GetAssetByteArray("App.Shaders.Shader.metal");

                render.Shaders = new Shader[]
                {
                    render.graphicsDevice.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, data, "shader_vertex", true)),
                    render.graphicsDevice.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, data, "shader_fragment", true))
                };
                return render;
            }
            catch (Exception ex)
            {
                Logger.AddLog(ex);
            }
            return null;
        }

        public static VeldridRender InitDesk()
        {
            if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan))
            {
                return InitFromVulkan();
            }
            else if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Direct3D11))
            {
                return InitFromD3D11();
            }
            throw new NotSupportedException("No supported GPU device found for auto");
        }
        #endregion


        #region Fields/Properties
        readonly GraphicsDevice graphicsDevice;
        readonly ResourceFactory resourceFactory;

        Shader[] Shaders { get; set; }

        ResourceLayout uniformsLayout;
        ResourceSet _uniformsResourceSet;

        Texture rubber;
        Texture _offscreenColor;

        ResourceLayout textureLayout;
        TextureView _textureView;
        ResourceSet _textureSet;


        DeviceBuffer _vertBuffer;
        DeviceBuffer _fragLightBuffer;
        DeviceBuffer _fragMaterialBuffer;

        Pipeline _pipeline;

        Texture _offscreenReadOut;
        Framebuffer _offscreenFB;

        /// <summary>
        /// Should the png output be vertival flipped
        /// </summary>
        public bool FlipVertical { get; set; } = false;

        //readonly RgbaFloat background = new RgbaFloat(0, 0, 0, 0);
        readonly RgbaFloat background = new RgbaFloat(0.7f, 0, 0, 0.5f);

        FragLightUniformsInfo FragLightUniforms = new FragLightUniformsInfo();
        FragMaterialUniformsInfo FragMaterialUniforms = new FragMaterialUniformsInfo();
        #endregion

        #region IRenderBase implementation

        public void Initialize()
        {
            Logger.AddLog("VeldridRender.Initialize: no-use");
        }

        public void UpdateConfigs(RenderConfig config)
        {
            Logger.AddLog("VeldridRender.UpdateConfigs");
            CheckThread();

            try
            {
                try { rubber?.Dispose(); } catch { }
                try { _textureView?.Dispose(); } catch { }
                try { textureLayout?.Dispose(); } catch { }
                try { _textureSet?.Dispose(); } catch { }

                try { _vertBuffer?.Dispose(); } catch { }
                try { _fragLightBuffer?.Dispose(); } catch { }
                try { _fragMaterialBuffer?.Dispose(); } catch { }

                try { uniformsLayout?.Dispose(); } catch { }
                try { _uniformsResourceSet?.Dispose(); } catch { }

                try { _offscreenReadOut?.Dispose(); } catch { }
                try { _offscreenFB?.Dispose(); } catch { }
                try { _offscreenColor?.Dispose(); } catch { }

                try { _pipeline?.Dispose(); } catch { }

                #region Textures
                ProcessImage tmpTexture = Helpers.ImageProcessing.PngToRgba8(new ProcessImage { Load = Helpers.GetAssetByteArray("App.rubber4.png"), Type = ProcessImageType.PNG });
                rubber = resourceFactory.CreateTexture(TextureDescription.Texture2D((uint)tmpTexture.Width, (uint)tmpTexture.Height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled));

                graphicsDevice.UpdateTexture(rubber, tmpTexture.Load, 0, 0, 0, (uint)tmpTexture.Width, (uint)tmpTexture.Height, 1, 0, 0);

                _textureView = resourceFactory.CreateTextureView(rubber);
                ResourceLayoutElementDescription[] textureLayoutDescriptions =
                {
                    new ResourceLayoutElementDescription("Tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("Samp", ResourceKind.Sampler, ShaderStages.Fragment)
                };
                textureLayout = resourceFactory.CreateResourceLayout(new ResourceLayoutDescription(textureLayoutDescriptions));
                _textureSet = resourceFactory.CreateResourceSet(new ResourceSetDescription(textureLayout, _textureView, graphicsDevice.LinearSampler));
                #endregion

                #region Uniforms
                _vertBuffer = resourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
                _fragLightBuffer = resourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
                _fragMaterialBuffer = resourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

                ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
                {
                    new ResourceLayoutElementDescription("ModelViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("LightInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("MaterialInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment)
                };
                uniformsLayout = resourceFactory.CreateResourceLayout(new ResourceLayoutDescription(resourceLayoutElementDescriptions));
                _uniformsResourceSet = resourceFactory.CreateResourceSet(new ResourceSetDescription(uniformsLayout, _vertBuffer, _fragLightBuffer, _fragMaterialBuffer));
                #endregion

                var VertBufferDescription = new VertexLayoutDescription(
                            new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                            new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
                            new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));


                //Pipeline
                _offscreenReadOut = resourceFactory.CreateTexture(TextureDescription.Texture2D((uint)config.Width, (uint)config.Height, 1, 1, PixelFormat.R32_G32_B32_A32_Float, TextureUsage.Staging));

                _offscreenColor = resourceFactory.CreateTexture(TextureDescription.Texture2D((uint)config.Width, (uint)config.Height, 1, 1, PixelFormat.R32_G32_B32_A32_Float, TextureUsage.RenderTarget));

                _offscreenFB = resourceFactory.CreateFramebuffer(new FramebufferDescription(null, _offscreenColor));

                GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription()
                {
                    BlendState = BlendStateDescription.SingleDisabled,
                    RasterizerState = new RasterizerStateDescription()
                    {
                        CullMode = FaceCullMode.None,
                        FillMode = PolygonFillMode.Solid,
                        FrontFace = FrontFace.CounterClockwise,
                        DepthClipEnabled = true,
                        ScissorTestEnabled = true
                    },
                    PrimitiveTopology = PrimitiveTopology.TriangleList,
                    ResourceLayouts = new ResourceLayout[] { uniformsLayout, textureLayout },
                    Outputs = _offscreenFB.OutputDescription,
                    ShaderSet = new ShaderSetDescription()
                    {
                        Shaders = Shaders,
                        VertexLayouts = new VertexLayoutDescription[] { VertBufferDescription }
                    },
                    ResourceBindingModel = ResourceBindingModel.Improved
                };
                _pipeline = resourceFactory.CreateGraphicsPipeline(ref pipelineDescription);


                FragLightUniforms.Lightdirection = new Vector4(Vector3.Normalize(config.LightDirection), 0);
                FragLightUniforms.Lightambient = new Vector4(config.LightAmbient, 0);
                FragLightUniforms.Lightdiffuse = new Vector4(config.LightDiffuse, 0);
                FragLightUniforms.Lightspecular = new Vector4(config.LightSpecular, 0);

                FragMaterialUniforms.Materialdiffuse = new Vector4(config.MaterialDiffuse, 0);
                FragMaterialUniforms.Materialspecular = new Vector4(config.MaterialSpecular, 0);
                FragMaterialUniforms.Materialshininess = new Vector4(config.MaterialShininess, 0, 0, 0);

                FragMaterialUniforms.ViewDir = new Vector4(Vector3.Normalize(config.CameraPosition), 0);

                Vector3 camPos = new Vector3(config.CameraPosition.X, config.CameraPosition.Y, config.CameraPosition.Z);
                Vector3 camLookAt = new Vector3(config.CameraLookAt.X, config.CameraLookAt.Y, config.CameraLookAt.Z);
                Matrix4x4 model = Matrix4x4.Identity * Matrix4x4.CreateTranslation(config.ModelTranslation.X, config.ModelTranslation.Y, config.ModelTranslation.Z);
                model *= Matrix4x4.CreateFromAxisAngle(new Vector3(1.0f, 0, 0), config.ModelRotation.X)
                * Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1.0f, 0), config.ModelRotation.Y)
                * Matrix4x4.CreateFromAxisAngle(new Vector3(0, 0, 1.0f), config.ModelRotation.Z);
                Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(DegreesToRadians(config.CameraFOV), config.Width / (float)config.Height, config.CameraNear, config.CameraFar);
                Matrix4x4 view = Matrix4x4.CreateLookAt(camPos, camLookAt - camPos, -Vector3.UnitX);
                Matrix4x4 MVP = model * view * projection;

                graphicsDevice.UpdateBuffer(_vertBuffer, 0, MVP);
                graphicsDevice.UpdateBuffer(_fragLightBuffer, 0, FragLightUniforms);
                graphicsDevice.UpdateBuffer(_fragMaterialBuffer, 0, FragMaterialUniforms);

                graphicsDevice.WaitForIdle();

            }
            catch (Exception ex)
            {
                Logger.AddLog(ex);
            }
        }


        public byte[] VboToPng(float[] vbo, int trianglesCount, bool flipHorizontal = false)
        {
            Logger.AddLog("VeldridRender.VboToPng");
            CheckThread();
            try
            {
                uint size = (uint)vbo.Length * 4;
                if(size%16 != 0) { size += 16 - size % 16; }

                DeviceBuffer vertexBuffer = resourceFactory.CreateBuffer(new BufferDescription(size, BufferUsage.VertexBuffer));
                graphicsDevice.UpdateBuffer(vertexBuffer, 0, vbo);
                graphicsDevice.WaitForIdle();

                CommandList cl = resourceFactory.CreateCommandList();
                cl.Begin();

                cl.SetFramebuffer(_offscreenFB);
                cl.ClearColorTarget(0, background);
                cl.SetFullViewport(0);

                //cl.UpdateBuffer(vertexBuffer, 0, vbo);
                cl.SetVertexBuffer(0, vertexBuffer);

                cl.SetPipeline(_pipeline);

                cl.SetGraphicsResourceSet(0, _uniformsResourceSet);
                cl.SetGraphicsResourceSet(1, _textureSet);

                cl.Draw((uint)(trianglesCount * 3));

                //transfer GPU drawing to CPU readable one
                cl.CopyTexture(_offscreenFB.ColorTargets[0].Target, _offscreenReadOut);

                cl.End();

                graphicsDevice.SubmitCommands(cl);
                //Thread.Sleep(5);
                graphicsDevice.WaitForIdle();
                //Thread.Sleep(5);

                MappedResourceView<byte> view = graphicsDevice.Map<byte>(_offscreenReadOut, MapMode.Read);

                byte[] tmp = new byte[view.SizeInBytes];

                Marshal.Copy(view.MappedResource.Data, tmp, 0, (int)view.SizeInBytes);

                graphicsDevice.Unmap(_offscreenReadOut);

                return Helpers.ImageProcessing.Rgba32ToPng(
                        new ProcessImage { Load = tmp, Width = (int)_offscreenReadOut.Width, Height = (int)_offscreenReadOut.Height, Type = ProcessImageType.RGBA32 },
                        flipHorizontal,
                        FlipVertical)
                    .Load;
            }
            catch (Exception ex)
            {
                Logger.AddLog(ex);
            }

            return null;
        }
        #endregion

        #region Helpers
        void MakeSpirvShaders(bool invertBGR = false)
        {
            var vertex = Helpers.GetAssetByteArray("App.Shaders.render.vert");
            var fragment = Helpers.GetAssetByteArray("App.Shaders.render" + (invertBGR ? "Inverted" : "") + ".frag");
            Shaders = resourceFactory.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, vertex, "main"),
                    new ShaderDescription(ShaderStages.Fragment, fragment, "main")
                    );
        }

        float DegreesToRadians(float degrees)
        {
            return degrees * ((float)Math.PI / 180f);
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    struct FragLightUniformsInfo
    {
        public Vector4 Lightdirection;
        public Vector4 Lightambient;
        public Vector4 Lightdiffuse;
        public Vector4 Lightspecular;
    }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    struct FragMaterialUniformsInfo
    {
        public Vector4 Materialdiffuse;
        public Vector4 Materialspecular;
        public Vector4 ViewDir;
        public Vector4 Materialshininess;

    }
}
