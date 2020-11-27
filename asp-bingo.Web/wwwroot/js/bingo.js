const bingoCconnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()

bingoCconnection.on('BingoCallerRecieve', number => {
	
})

bingoCconnection.on('Sheet', sheet => {
	const tds = document.querySelectorAll('table .col')
	for (let i = 0; i < tds.length; i++) {
		tds[i].innerText = sheet[i]
	}
})

bingoCconnection.start()
	.then(() => {
		bingoCconnection.invoke('GetSheet')
	})
	.catch(err => console.error(err.toString()))