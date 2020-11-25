const connection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()

connection.on('BingoCaller', (message) => {
    console.log(message)
})

connection.start()
	.then(() => document.getElementById('sendButton').disabled = false)
	.catch(err => console.error(err.toString()))
