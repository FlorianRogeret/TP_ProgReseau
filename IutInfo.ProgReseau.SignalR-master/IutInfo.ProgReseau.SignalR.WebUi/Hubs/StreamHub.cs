using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace IutInfo.ProgReseau.SignalR.WebUi.Hubs
{
    public sealed class StreamHub : Hub // Ce Hub fait de l'émission de données en continue
    {
        public ChannelReader<int> Counter(int countTo, int sleepTime)
        {
            // On créée un channel pouvant être consommé par plusieurs writers et readers en simultané
            var channel = Channel.CreateUnbounded<int>();

            // On enregistre une tâche qui va s'éxécuter de façon asynchrone afin de na pas bloquer le thread
            // Ainsi, nous pourrons renvoyer un reader au client pour qu'ils puissent consommer le flux que l'on génère dans le writer
            _ = StreamAsync(channel.Writer, countTo, sleepTime);

            // On retourne le flux aux clients
            return channel.Reader;
        }

        // On défini notre tâche qui sera chargée de générer des données
        private async Task StreamAsync(ChannelWriter<int> writer, int countTo, int sleepTime)
        {
            // Nous faisons un compteur fini, mais on pourrait très bien imaginer avoir une boucle infinie
            // Par exemple, pour une appli de bourse, le cours ne s'arrête jamais d'évoluer, on ne souhaite donc pas s'arrêter
            for (int i = 0; i < countTo; i++)
            {
                await writer.WriteAsync(i); // On écrit dans le channel en le bloquant
                await Task.Delay(sleepTime); // On bloque l'éxécution de la tâche
            }

            // Comme nous sommes sur une tâche finie, nous devons essayer de libérer le writer
            writer.TryComplete();
        }
    }
}