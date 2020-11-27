const bingoCconnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()
const historyList = document.querySelector('#history')

bingoCconnection.on('BingoCallerRecieve', number => {
	const node = document.createElement('li')
	node.innerText = number
	historyList.appendChild(node)
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

bingoCconnection.on('RowsNeededForBingo', rowsNeeded => document.querySelector('#rowsNeeded').innerText = rowsNeeded)

bingoCconnection.on('History', history => {
	for (const number of history) {
		const node = document.createElement('li')
		node.innerText = number
		historyList.appendChild(node)
	}
})

bingoCconnection.start()
	.then(() => {
		bingoCconnection.invoke('GetSheet')
		bingoCconnection.invoke('GetHistory')
		bingoCconnection.invoke('GetRowsNeededForBingo')
	})
	.catch(err => console.error(err.toString()))