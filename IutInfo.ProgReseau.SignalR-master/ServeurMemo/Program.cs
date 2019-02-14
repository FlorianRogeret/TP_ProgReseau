using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServeurMemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Le serveur est allumé");

            try
            {
                #region Création et initialisation de la socket
                //Création de la socket 
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 1212);
                sock.Bind(iep);
                sock.Listen(5);
                #endregion

                while (true)
                {
                    #region Reception et envoie de la socket
                    //Réception de la socket
                    Socket seck_serv = sock.Accept();
                    //Envoie de la socket à Traitement par le biai d'un Thread afin que le processus ne soit pas bloquer par d'autres requêtes
                    ThreadPool.QueueUserWorkItem(Traitement, seck_serv);
                    #endregion
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }
        }

        /// <summary>
        /// Prend la Socket en paramètre d'entré et la traite afin d'enregistrer l'utilisateur et le message sur logs.txt
        /// </summary>
        /// <param name="arg"> La socket </param>
        static void Traitement(object arg)
        {
            Console.WriteLine("Traitement de la requête en cours");

            #region Préparation et réception de la Socket
            //Convertion de l'argument en socket
            Socket sock = (Socket)arg;

            //Recoit la taille du futur tableau
            int UserEtMessageLength = sock.ReceiveBufferSize;

            //Crée le tableau b_UserEtMessage avec la taille précédemment récupéré
            byte[] b_UserEtMessage = new byte[UserEtMessageLength];

            //Recois l'utilisateur et le message en bits
            sock.Receive(b_UserEtMessage);
            #endregion

            #region Conversion du message reçu
            //Décode le message (de byte en string)
            string UserEtMessage = Encoding.UTF8.GetString(b_UserEtMessage);
            
            //Sépare le message grace au caractère séparateur (&&&) et le stock dans TabUserEtMessage
            string[] TabUserEtMessage = UserEtMessage.Split(new string[] { "&&&" }, StringSplitOptions.None);
            //L'utilisateur sera la première case du tableau TabUserEtMessage
            string user = TabUserEtMessage[0];
            //Le message sera la deuxième case du tableau TabUserEtMessage
            string message = TabUserEtMessage[1];
            #endregion
            
            #region Préparation et enregistrement du message
            //path contient le chemin ou est le logs.txt
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..", "Logs.txt");

            //Concatene l'utilisateur et le message
            string text ="Le : " + DateTime.Now + " L'utilisateur : " + user + " a écrit : " + message;

            try
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    //Enregistre le tout dans logs.txt
                    sw.WriteLine(text);
                    Console.WriteLine("Le texte écrit par {0} le {1} a été enregistré avec succès !", user, text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Une erreur s'est produite lors de l'enregistrement du texte de {0} le {1}.\n {2}",
                    user, DateTime.Now, e.Message);
            }
            #endregion

            #region Fermeture de la Socket
            //Eteins la socket 
            sock.Shutdown(SocketShutdown.Both);
            //Et la ferme
            sock.Close();
            #endregion

        }
    }
}
