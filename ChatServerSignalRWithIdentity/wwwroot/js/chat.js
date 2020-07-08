class Message {
    constructor(username, text, CreatedUtc) {
        this.userName = username;
        this.text = text;
        this.CreatedUtc = CreatedUtc;
    }
}

// userName is declared in razor page.
const username = userName;
const textInput = document.getElementById('messageText');
//const CreatedUtcInput = document.getElementById('CreatedUtc');
const chat = document.getElementById('chat');
const messagesQueue = [];

document.getElementById('submitButton').addEventListener('click', () => {
    var currentdate = new Date();
    CreatedUtc.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })
});

document.getElementById('goSubmitButton').addEventListener('click', () => {
    var currentdate = new Date();
    CreatedUtc.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })
});

function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessageToPublicChat() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;
    
    let CreatedUtc = new Date();
    let message = new Message(username, text);
    sendMessageToPublicChatToHub(message);
}

function changeButtonFollow() {
    var elem = document.getElementById("followButton");
    if (elem.value == "Unfollow") elem.value = "Follow";
    else elem.value = "Unfollow";
}

function changeButtonBlock() {
    var elem = document.getElementById("blockButton");
    if (elem.value == "Unblock") elem.value = "Block";
    else elem.value = "Unblock";

    function changeButtonUnfollow() {
        var elem = document.getElementById("unfollowButton");
        if (elem.value == "Follow") elem.value = "Unfollow";
        else elem.value = "Follow";
    }

    function changeButtonUnblock() {
        var elem = document.getElementById("unblockButton");
        if (elem.value == "Block") elem.value = "Unblock";
        else elem.value = "Block";

function addMessageToChat(message) {
    let isCurrentUserMessage = message.userName === username;

    let container = document.createElement('div');
    container.className = isCurrentUserMessage ? "container darker" : "container";

    let sender = document.createElement('p');
    sender.className = "sender";
    sender.innerHTML = message.userName;
    let text = document.createElement('p');
    text.innerHTML = message.text;

    let CreatedUtc = document.createElement('span');
    CreatedUtc.className = isCurrentUserMessage ? "time-left" : "time-right";
    var currentdate = new Date();
    CreatedUtc.innerHTML = 
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })

    container.appendChild(sender);
    container.appendChild(text);
    container.appendChild(CreatedUtc);
    chat.appendChild(container);
}
