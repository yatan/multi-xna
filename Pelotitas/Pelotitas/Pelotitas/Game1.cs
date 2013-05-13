using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;
using System.Threading;

namespace Pelotitas
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState teclado = new KeyboardState();
        NetClient cliente;
        NetPeerConfiguration config;
        String mi_id;

        List<Pelele> listaPeleles = new List<Pelele>();

        Texture2D texturePelota;
        Vector2 miPosicion;

        float totaltiempo = 0;
        float tiempoanterior = 0;
        float contadorticks = 0;
        float ticks = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            miPosicion = new Vector2(300, 200);
            config = new NetPeerConfiguration("chat");


            cliente = new NetClient(config);
            cliente.Start();
            NetOutgoingMessage hail = cliente.CreateMessage("This is the hail message");
            cliente.Connect("199.195.254.103", 14242, hail);
            mi_id = NetUtility.ToHexString(cliente.UniqueIdentifier);
            System.Console.WriteLine("Mi id de conexion es: " + mi_id);

            cliente.RegisterReceivedCallback(new SendOrPostCallback(GotMessage)); 

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texturePelota = Content.Load<Texture2D>("pelota");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            cliente.Disconnect("QUIT");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            teclado = Keyboard.GetState();

            // Allows the game to exit
            if (teclado.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            if (teclado.IsKeyDown(Keys.Left))
            {
                miPosicion.X-=2;
            }
            if (teclado.IsKeyDown(Keys.Right))
            {
                miPosicion.X+=2;
            }



            
            //ENVIO DE DATOS CADA 1 SEGUNDO
            tiempoanterior = totaltiempo;

            totaltiempo = (float)gameTime.TotalGameTime.TotalSeconds;

            contadorticks = totaltiempo - tiempoanterior;
            ticks += contadorticks;


            if (ticks > 0.05)
            {
                NetOutgoingMessage om = cliente.CreateMessage();
                om.Write("PELOTA");
                om.Write(miPosicion.X.ToString());
                om.Write(miPosicion.Y.ToString());
                cliente.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                //Output("Sending '" + text + "'");
                cliente.FlushSendQueue();
                //Console.WriteLine(contadorticks.ToString());
                ticks = 0f;
            }
           
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(texturePelota, miPosicion, Color.Red);
            foreach (Pelele pelele in listaPeleles)
            {
                spriteBatch.Draw(texturePelota, pelele.getPosition(), Color.Green);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = cliente.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        Output(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        /*
                        if (status == NetConnectionStatus.Connected)
                            s_form.EnableInput();
                        else
                            s_form.DisableInput();

                        if (status == NetConnectionStatus.Disconnected)
                            s_form.button2.Text = "Connect";
                        */
                        string reason = im.ReadString();
                        Output(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        string chat = im.ReadString();
                        string uid = im.ReadString();
                        string posX = im.ReadString();
                        string posY = im.ReadString();
                        Boolean isRegistred = false;
                        foreach (Pelele pelele in listaPeleles)
                        {
                            if (pelele.uid == uid)
                            {
                                pelele.setPosition(Convert.ToInt16(posX), Convert.ToInt16(posY));
                                isRegistred = true;
                            }
                        }
                        if (isRegistred == false)
                        {
                            listaPeleles.Add(new Pelele(uid));
                        }
                        Output(chat);
                        break;
                    default:
                        Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }

        private static void Output(string text)
        {
            Console.WriteLine(text);
        }
    }
}
