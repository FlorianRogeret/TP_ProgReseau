using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IutInfo.ProgReseau.SignalR.WebUi.Hubs
{
    public sealed class MessengerHub : Hub // On déclare notre Hub
    {
        // Nous allons écouter les événements "SendMessage" envoyés par les clients
        public async Task SendMessage(string user, string message)
        {
            //Appelle d'EnvoiServeur pour enregistrer l'utilisateur et le message dans un log
            EnvoiServeur(user, message) ;

            // On envoie de façon asynchrone un événement "ReceiveMessage" aux différents clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public void EnvoiServeur(string user, string message)
        {
            try
            {
                #region Création et connection de la Socket
                //Création de la socket
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Connexion au serveur
                sock.Connect("127.0.0.1", 1212);
                #endregion

                #region Préparation et envoi des éléments au serveur
                //Concatène l'utilisateur et le message ainsi qu'un caractère séparteur 
                string UserEtMessage = user + "&&&" + message;
                
                //Transforme la chaine de caractère en un tableau de byte
                byte[] b_UserEtMessage = Encoding.UTF8.GetBytes(UserEtMessage);

                //Envoie la taille du tableau
                sock.SendBufferSize = b_UserEtMessage.Length;

                //Envoie la socket 
                sock.Send(b_UserEtMessage);
                #endregion

                #region Destruction de la socket
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }
        }

    }
}