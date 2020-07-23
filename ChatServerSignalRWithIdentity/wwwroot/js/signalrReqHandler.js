//var connection = new signalR.HubConnectionBuilder()
//    .withUrl('/Home/Index')
//    .build();

//connection.on('receiveMessageToPublicChat', addMessageToChat);

//connection.start()
//    .catch(error => {
//        console.error(error.message);
//    });

//function sendMessageToPublicChatToHub(message) {
//    connection.invoke('sendMessageToPublicChat', message);
//}