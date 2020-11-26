const bingoCconnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()

bingoCconnection.on('BingoCallerRecieve', number => {
    console.log(number)
})

bingoCconnection.start()
	.then(() => {})
	.catch(err => console.error(err.toString()))