using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LightDemo
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D lightTexture;
        private Effect lightEffect;
        private Effect polygons;
        private Vector2 pos;
        private Color lightColor = Color.LightBlue;
        private Vector4[] walls;
        private RenderTarget2D finalTarget;

        private Triangle t1, t2;
        private Square s1;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _graphics.PreferMultiSampling = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            pos = new Vector2();
            t1 = new Triangle(new VertexPositionColor[] { new VertexPositionColor(new Vector3(160.0f, 200.0f, 0), Color.Red), new VertexPositionColor(new Vector3(200.0f, 140.0f, 0), Color.Green), new VertexPositionColor(new Vector3(250.0f, 200.0f, 0), Color.Blue) });
            t2 = new Triangle(new VertexPositionColor[] { new VertexPositionColor(new Vector3(300.0f, 200.0f, 0), Color.PaleVioletRed), new VertexPositionColor(new Vector3(340.0f, 140.0f, 0), Color.LightGreen), new VertexPositionColor(new Vector3(390.0f, 200.0f, 0), Color.LightBlue) });
            s1 = new Square(new VertexPositionColor[] { new VertexPositionColor(new Vector3(300, 300, 0), Color.White), new VertexPositionColor(new Vector3(350, 300, 0), Color.White), new VertexPositionColor(new Vector3(350, 350, 0), Color.White), new VertexPositionColor(new Vector3(300, 350, 0), Color.White) });
            List<Vector4> wallList = new List<Vector4>();
            wallList.AddRange(t1.Walls);
            wallList.AddRange(t2.Walls);
            wallList.AddRange(s1.Walls);
            walls = wallList.ToArray();
            finalTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None, 32, RenderTargetUsage.DiscardContents);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            lightTexture = Content.Load<Texture2D>("Sprites\\light");
            lightEffect = Content.Load<Effect>("Shaders\\lightEffect");
            polygons = Content.Load<Effect>("Shaders\\polygonEffect");
            polygons.Parameters["walls"].SetValue(walls);
            polygons.Parameters["WorldViewProjection"].SetValue(Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1));
            lightEffect.Parameters["walls"].SetValue(walls);
            polygons.Parameters["lightColor"].SetValue(lightColor.ToVector4());
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            pos.X = (float)Mouse.GetState().X;
            pos.Y = (float)Mouse.GetState().Y;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(finalTarget);
            GraphicsDevice.Clear(Color.Black);

            RasterizerState rasterizerState = new RasterizerState() { MultiSampleAntiAlias = true, CullMode = CullMode.None };
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Additive;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            polygons.Parameters["lightPos"].SetValue(pos);
            foreach (EffectPass pass in polygons.CurrentTechnique.Passes)
            {
                pass.Apply();
                t1.Draw(GraphicsDevice);
                t2.Draw(GraphicsDevice);
                s1.Draw(GraphicsDevice);
            }

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            lightEffect.Parameters["lightPos"].SetValue(pos - new Vector2(150, 150));
            lightEffect.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Draw(lightTexture, pos - new Vector2(150, 150), lightColor);
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, rasterizerState);
            _spriteBatch.Draw(finalTarget, new Vector2(), Color.White);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
