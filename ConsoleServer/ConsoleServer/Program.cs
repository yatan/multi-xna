using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;

namespace ConsoleServer
{
    class Program
    {

        private static NetServer s_server;

        //static Player jugador;
        static List<Player> lista_jugadores;

        private static void Application_Idle()
        {
            while (true)
            {
                NetIncomingMessage im;
                while ((im = s_server.ReadMessage()) != null)
                {
                    String[] envio;
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
                            if (status == NetConnectionStatus.Connected)
                            {
                                Player player = new Player(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier));
                                lista_jugadores.Add(player);
                                Output("Player con id: " + player.UID + " se ha conectado.");
                            }
                            else
                            {
                                string reason = im.ReadString();
                                Output(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            }
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                Player player = new Player(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier));
                                envio = new String[2];
                                envio[0] = "LOGOUT";
                                envio[1] = NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier);
                                enviar(envio, im);
                                lista_jugadores.Remove(player);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            // incoming chat message from a client
                            string chat = im.ReadString();
                            //String[] envio;
                            if (chat == "POS")
                            {
                                //Buscamos en la lista de jugadores el que envia los datos para almacenarlos
                                foreach (Player jugador in lista_jugadores)
                                {
                                    if (jugador.UID == NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier))
                                    {
                                        if (jugador.is_loged() == false)
                                        {
                                            envio = new String[1];
                                            envio[0] = "LOGOUT";
                                            enviar(envio, im);
                                        }
                                        else
                                        {
                                            String pos_x = im.ReadString();
                                            String pos_y = im.ReadString();
                                            String mapa = im.ReadString();

                                            envio = new String[4];
                                            jugador.updatePos(Convert.ToInt16(pos_x), Convert.ToInt16(pos_y));
                                            jugador.mapa = Convert.ToInt16(mapa);


                                            envio[0] = "POS";
                                            envio[1] = pos_x.ToString();
                                            envio[2] = pos_y.ToString();
                                            envio[3] = jugador.mapa.ToString();

                                            Output("Posicion x: " + pos_x + " y: " + pos_y + " mapa: " + mapa);

                                            //Enviamos la posicion al resto de la gente
                                            enviar(envio, im);
                                        }
                                    }
                                }

                            }
                            else if (chat == "PELOTA")
                            {
                                //Buscamos en la lista de jugadores el que envia los datos para almacenarlos
                                foreach (Player jugador in lista_jugadores)
                                {
                                    if (jugador.UID == NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier))
                                    {
                                        if (jugador.is_loged() == false)
                                        {
                                            envio = new String[1];
                                            envio[0] = "LOGOUT";
                                            enviar(envio, im);
                                        }
                                        else
                                        {
                                            String pos_x = im.ReadString();
                                            String pos_y = im.ReadString();

                                            envio = new String[5];
                                            jugador.updatePos(Convert.ToInt16(pos_x), Convert.ToInt16(pos_y));



                                            envio[0] = "PELOTA";
                                            envio[1] = jugador.UID;
                                            envio[2] = jugador.nick;
                                            envio[3] = pos_x.ToString();
                                            envio[4] = pos_y.ToString();

                                            //Eliminar debug text
                                            Output("Posicion x: " + pos_x + " y: " + pos_y);

                                            //Enviamos la posicion al resto de la gente
                                            enviar(envio, im);
                                        }
                                    }
                                }
                            }
                            else if (chat == "LOGIN")
                            {
                                //Nick de logeo
                                String log_nick = im.ReadString();

                                //Buscamos si ya esta logeado
                                foreach (Player jugador in lista_jugadores)
                                {
                                    if (jugador.nick == log_nick)
                                    {
                                        if (jugador.UID != NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier))
                                        {
                                            //Se elimina el jugador del array de jugadores
                                            //jugador.UID = NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier);
                                            //jugador.login(log_nick);
                                        }
                                    }
                                    Output("Login de:" + log_nick);
                                    jugador.login(log_nick);
                                }
                                /*
                                Player player = new Player(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier));
                                lista_jugadores.Add(player);
                                 * */
                                
                            }
                            else
                            {
                                envio = new String[1];
                                envio[0] = "Error";
                                enviar(envio, im);
                            }

                            
                            break;
                        default:
                            Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }


        public static void enviar(String[] datos, NetIncomingMessage im)
        {
            NetOutgoingMessage om = s_server.CreateMessage();
            foreach (String item in datos)
            {
                om.Write(item);
            }

            om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier));


            // broadcast this to all connections, except sender
            List<NetConnection> all = s_server.Connections; // get copy
            all.Remove(im.SenderConnection);

            if (all.Count > 0)
            {
                //Output("Broadcasting '" + envio + "'");
                
                //om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: ");
                s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
            }

        }

        private static void Output(string text)
        {
            Console.WriteLine(text);
        }

        static void Main(string[] args)
        {
            // set up network
            NetPeerConfiguration config = new NetPeerConfiguration("chat");
            config.MaximumConnections = 100;
            config.Port = 14242;
            lista_jugadores = new List<Player>();
            s_server = new NetServer(config);
            //Console.WriteLine("HOLA");
            s_server.Start();
            Application_Idle();
            Console.ReadLine();
        }
    }
}
