const connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build()

//Disable send button until connection is established
document.getElementById('sendButton').disabled = true

connection.on('ReceiveMessage', (user, message) => {
    const msg = message.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
    const encodedMsg = user + ' says ' + msg
    const li = document.createElement('li')
    li.textContent = encodedMsg
    document.getElementById('messagesList').appendChild(li)
});

connection.start()
	.then(() => document.getElementById('sendButton').disabled = false)
	.catch(err => console.error(err.toString()))

document.getElementById('sendButton').addEventListener('click', event => {
    const user = document.getElementById('userInput').value
    const message = document.getElementById('messageInput').value
    connection.invoke('SendMessage', user, message).catch(err => console.error(err.toString()))
    event.preventDefault()
})