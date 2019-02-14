'use strict';

var connection = new signalR.HubConnectionBuilder()
    .withUrl('/messenger') // On établie une connection avec notre Hub messenger
    .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()) // Avec une connection chiffrée
    .build();

// Quand on reçoit l'événement "ReceiveMessage", on va ajouter un élément de liste dans notre DOM
connection.on('ReceiveMessage', function (user, message) {
    var li = document.createElement('li');
    li.innerHTML = '<b>' + user + ' : </b>' + message;
    document.getElementById('messages').appendChild(li);
});

// On démarre la connection afin d'écouter les différents événements et pour pouvoir en émettre
connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById('envoie').addEventListener('click', function (event) {
    event.preventDefault();

    var user = document.getElementById('user').value;
    var message = document.getElementById('message').value;

    // Lorsque l'on valide notre formulaire, nous envoyons un événement "SendMessage" au serveur
    connection.invoke('SendMessage', user, message).catch(function (err) {
        return console.error(err.toString());
    });
});
