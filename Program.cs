using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System; 
using System.IO;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace Tutorial
{
     partial class Program
    {
        private static IWindow window;
        private static GL Gl;
        private static IKeyboard primaryKeyboard;

        private const int Width = 1000;
        private const int Height = 800;

        private static BufferObject<float> Vbo;
        private static BufferObject<uint> Ebo;
        private static VertexArrayObject<float, uint> Vao;
        //private static Texture Texture;
        private static Shader Shader;

        //Setup the camera's location, directions, and movement speed
        private static Vector3 CameraPosition = new Vector3(0.5f, 0.3f, 3.0f);
        private static Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        private static Vector3 CameraUp = Vector3.UnitY;
        private static Vector3 CameraDirection = Vector3.Zero;
        private static float CameraYaw = -90f;
        private static float CameraPitch = 0f;
        private static float CameraZoom = 45f;

        //Used to track change in mouse movement to allow for moving of the Camera
        private static Vector2 LastMousePosition;
         private static float[] Vertices;

        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        private static ImGuiController controller;
        private static IInputContext inputContext;
        private static float differenceX = 0;
        private static float differenceY = 0;

        [STAThread]
        private static void Main(string[] args)
        {
         //  readsdf(@"./_vertics.csv");
            CreateGlwin();
        }
        private static void CreateGlwin()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Position = new Vector2D<int>(80, 0);
            options.Title = "道路面積計算";
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Closing += OnClose;
            window.FramebufferResize += Onresize;
            window.Run();

        }
        private static void Onresize(Vector2D<int> e)
        {
            Size si = new Size();
            si.Width = window.Size.X;si.Height = window.Size.Y;
            Gl.Viewport(si);
        }
        private static void LdGPU()
        {
            Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
            Vao = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 4, 0);
            Vao.VertexAttributePointer(1, 1, VertexAttribPointerType.Float, 4, 3);

            Shader = new Shader(Gl, "shader.vert", "shader.frag");
            //Texture = new Texture(Gl, "silk.png");

            ///////////ImGuiController
            ImGuiFontConfig  fontConfig = new ImGuiFontConfig( @"c:\Windows\Fonts\msgothic.ttc",16);
        
        }
        private static void OnLoad()
        {
            IInputContext input = window.CreateInput();
            primaryKeyboard = input.Keyboards.FirstOrDefault();
            if (primaryKeyboard != null)
            {
                primaryKeyboard.KeyDown += KeyDown;
            }
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Normal;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
            }

            Gl = GL.GetApi(window);

            //LdGPU();
/*
            Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
            Vao = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 4, 0);
            Vao.VertexAttributePointer(1, 1, VertexAttribPointerType.Float, 4, 3);

            Shader = new Shader(Gl, "shader.vert", "shader.frag");
            //Texture = new Texture(Gl, "silk.png");
*/
            ///////////ImGuiController
            ImGuiFontConfig  fontConfig = new ImGuiFontConfig( @"c:\Windows\Fonts\msgothic.ttc",16);
        

            controller = new ImGuiController(
                Gl,//gl = window.CreateOpenGL(), // load OpenGL
                window, // pass in our window
                inputContext = window.CreateInput(), // create an input context
                fontConfig
            );
   
        }
        private static unsafe void OnUpdate(double deltaTime)
        {
            var moveSpeed = 1.0f * (float) deltaTime;

            if (primaryKeyboard.IsKeyPressed(Key.W))
            {
                //Move forwards
                CameraPosition += moveSpeed * CameraFront;
            }
            if (primaryKeyboard.IsKeyPressed(Key.S))
            {
                //Move backwards
                CameraPosition -= moveSpeed * CameraFront;
            }
            if (primaryKeyboard.IsKeyPressed(Key.A))
            {
                //Move left
                CameraPosition -= Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.D))
            {
                //Move right
                CameraPosition += Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.Q))
            {
                //Move right
                differenceX -=1.0f;
            }
            if (primaryKeyboard.IsKeyPressed(Key.Z))
            {
                //Move right
                differenceX += 1.0f;
            }
        }
        
        static float slider1 = 90.0f;
        static float onoffs = 0.0f;
        static  System.Boolean onoff=true;
        static string menseki="0平方";
        //static string comments="";
        static byte[] hhh;
        private static unsafe void OnRender(double deltaTime)
        {
            if(hhh==null)hhh = new byte[19];
         //  if(readed==0)return;
            Gl.Enable(EnableCap.DepthTest);
            Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            if(readed==1){
                Vao.Bind();
                //Texture.Bind();
                Shader.Use();
                //Shader.SetUniform("uTexture0", 0);

                //Use elapsed time to convert to radians to allow our cube to rotate over time
                //var difference = (float) (window.Time * 100);

                var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(differenceY)) *  Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(differenceX));
                var view = Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
                var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(CameraZoom), Width / Height, 0.1f, 100.0f);

                Shader.SetUniform("u_command",slider1);
                 
                Shader.SetUniform("uModel", model);
                Shader.SetUniform("uView", view);
                Shader.SetUniform("uProjection", projection);
                if(onoff){
                    onoffs =1.0f;
                   // Shader.SetUniform("uHeighAngle", 1.0f);
                }else{
                    onoffs =0.0f;
                   // 
                }
                Shader.SetUniform("u_HeighAngle", onoffs);
                ////////
                
                ////////
                //We're drawing with just vertices and no indices, and it takes 36 vertices to have a six-sided textured cube
                Gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)Vertices.Length);
                //Gl.DrawArrays(PrimitiveType.Points, 0, (uint)Vertices.Length);
            }
                        // Make sure ImGui is up-to-date
            controller.Update((float) deltaTime);
            var io = ImGuiNET.ImGui.GetIO();
            
            
         
            ImGuiNET.ImGui.Begin("データ");
            
            ImGuiNET.ImGui.InputText("ファイル", hhh, 20);
            
            if (ImGuiNET.ImGui.Button("DXF読み込み")) {
               Opend();
               //DxfRead();
            }
            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("変形済みを読み込み")) {
               //Opend();
               readsdf(@"./vertics.csv");
               LdGPU();
               readed =1;

            }
            ImGuiNET.ImGui.Separator();
            if(ImGuiNET.ImGui.RadioButton("高さ", !onoff)){
                onoff=false;
            }
            ImGuiNET.ImGui.SameLine();
            if(ImGuiNET.ImGui.RadioButton("角度",onoff)){
                onoff=true;
            } 
            ImGuiNET.ImGui.SliderFloat("レンジ", ref slider1, 90.0f, 180.0f);
            ImGuiNET.ImGui.Separator();
            if (ImGuiNET.ImGui.Button("面積計算")) {
                areastl();
                //menseki ="100平方";
            }
            ImGuiNET.ImGui.LabelText(menseki,"");

            //ImGuiNET.ImGui.LabelText(comments,"");
            ImGuiNET.ImGui.End();
                            // Make sure ImGui renders too!
            
            controller.Render();
        }

        private static unsafe void OnMouseMove(IMouse mouse, Vector2 position)
        {
            var lookSensitivity = 0.1f;
            if(mouse.IsButtonPressed(MouseButton.Right)==true){
                if (LastMousePosition == default) { LastMousePosition = position; }
                else
                {
                    var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                    var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                    LastMousePosition = position;

                    CameraYaw += xOffset;
                    CameraPitch -= yOffset;

                    //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
                    CameraPitch = Math.Clamp(CameraPitch, -89.0f, 89.0f);

                    CameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(CameraPitch));
                    CameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(CameraPitch));
                    CameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(CameraPitch));
                    CameraFront = Vector3.Normalize(CameraDirection);
                }
            }
        }

        private static unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            //We don't want to be able to zoom in too close or too far away so clamp to these values
            CameraZoom = Math.Clamp(CameraZoom - scrollWheel.Y, 1.0f, 45f);
        }

        private static void OnClose()
        {
            Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
            Shader.Dispose();
            //Texture.Dispose();
             // Dispose our controller first
            controller?.Dispose();

            // Dispose the input context
            inputContext?.Dispose();

                
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            if (key == Key.Escape)
            {
                window.Close();
            }
        }
    }
}
