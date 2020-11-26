const bingoCconnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()

bingoCconnection.on('BingoCallerRecieve', number => {
    console.log(number)
})

bingoCconnection.on('Sheet', sheet => console.log(sheet))

bingoCconnection.start()
	.then(() => {
		bingoCconnection.invoke('GetSheet')
	})
	.catch(err => console.error(err.toString()))