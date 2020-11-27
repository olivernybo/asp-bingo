const bingoCconnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()

bingoCconnection.on('BingoCallerRecieve', number => {
	
})

bingoCconnection.on('Sheet', sheet => {
	const tds = document.querySelectorAll('table .bingoCol')
	for (let i = 0; i < tds.length; i++) {
		tds[i].addEventListener('click', () => {
			if (tds[i].classList.contains('bingoMarked')) tds[i].classList.remove('bingoMarked')
			else tds[i].classList.add('bingoMarked')
		})
		if (sheet[i])
			tds[i].innerText = sheet[i]
	}
})

bingoCconnection.start()
	.then(() => {
		bingoCconnection.invoke('GetSheet')
	})
	.catch(err => console.error(err.toString()))