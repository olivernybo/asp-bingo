const bingoCconnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()

bingoCconnection.on('BingoCallerRecieve', number => {
	
})

bingoCconnection.on('Sheet', sheet => {
	const tds = document.querySelectorAll('table .bingoCol')
	for (let i = 0; i < tds.length; i++)
		if (sheet[i])
			tds[i].innerText = sheet[i]
})

bingoCconnection.start()
	.then(() => {
		bingoCconnection.invoke('GetSheet')
	})
	.catch(err => console.error(err.toString()))