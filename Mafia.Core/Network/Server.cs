using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mafia.Core.Network
{
    public class Server
    {
        #region Attributes
        private static Socket socket;

        private static IPEndPoint endPoint;
        #endregion

        public Message AwaitMessage { get; set; }

        /// <summary>
        /// Construct the socket with the provided params
        /// </summary>
        /// <param name="adrFam"></param>
        /// <param name="socketType"></param>
        /// <param name="prot"></param>
        public Server(AddressFamily adrFam = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType prot = ProtocolType.Tcp)
        {
            socket = new Socket(adrFam, socketType, prot);
        }

        /// <summary>
        /// Fonction qui crée une Socket et se connecte à l'IP et port en paramètre
        /// </summary>
        /// <param name="IPaddress">Adresse IP serveur</param>
        /// <param name="port">Numéro de port de connexion serveur</param>
        public void ConnectSocket(string IPaddress, int port)
        {
            try
            {
                endPoint = new IPEndPoint(IPAddress.Parse(IPaddress), port);
                Console.WriteLine("Tentative de connexion à : {0}  port n° {1}", IPaddress, port);
                socket.Connect(endPoint);
                Console.WriteLine("Connexion réussie");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Echec de connexion : {0}", ex.ToString());
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Fonction qui récupère une trame à partir d'une Socket. Lit les données envoyées et transforme en chaîne de caractères
        /// </summary>
        /// <returns>Retourne la chaîne transformée</returns>
        public string GetTrame()
        {
            string trame;
            byte[] buffer = new byte[512];
            try
            {
                socket.Receive(buffer);
                trame = Encoding.ASCII.GetString(buffer);
                return trame;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur lors de la réception de données : {ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Send coordinates of seed played on this turn
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SendData(int x, int y)
        {
            byte[] msg = new byte[4];
            msg = Encoding.ASCII.GetBytes($"A:{x}{y}");
            socket.Send(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="p">Tell which player specifically receive this message, if null, the message is broadcasted</param>
        public void SendMessage(string message, Player p = null)
        {
            /* TODO SEND MESSAGE TO SERVER */
        }

        /// <summary>
        /// Get next 4 bytes sent by server
        /// </summary>
        /// <returns></returns>
        public string ReceiveData(byte size)
        {
            byte[] buffer = new byte[size];
            socket.Receive(buffer);
            Console.WriteLine(Encoding.ASCII.GetString(buffer).Trim());
            return Encoding.ASCII.GetString(buffer).Trim();
        }

        /// <summary>
        /// Fonction qui ferme la connexion au serveur en fermant la Socket
        /// </summary>
        public void CloseSocket()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Console.WriteLine("Socket correctement fermée");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur lors de la fermeture de la socket : {ex}");
            }
        }
    }
}
