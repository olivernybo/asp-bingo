const bingoConnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()
const historyList = document.querySelector('#history')

bingoConnection.on('BingoCallerRecieve', number => {
	const node = document.createElement('li')
	node.innerText = number
	historyList.prepend(node)
})

bingoConnection.on('Sheet', sheet => {
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

bingoConnection.on('RowsNeededForBingo', rowsNeeded => document.querySelector('#rowsNeeded').innerText = rowsNeeded == 4 ? 3 : rowsNeeded)

bingoConnection.on('NotBingo', () => {
	const bingoButton = document.querySelector('#bingoButton')
	bingoButton.disabled = false
	Swal.fire({
		title: 'Incorrect',
		text: 'You do not have banko!',
		icon: 'error',
		timer: 3000,
		timerProgressBar: true
	})
})

bingoConnection.on('GameOver', (name, className) => {
	Swal.fire({
		title: 'Game over',
		text: `${name} from ${className} got a full sheet!`,
		icon: 'warning',
		allowOutsideClick: false
	})
})

bingoConnection.on('StartingNewRound', () => {
	Swal.fire({
		title: 'Next round',
		text: 'The next round is starting!',
		icon: 'info',
		timer: 9500,
		timerProgressBar: true
	})
})

bingoConnection.on('StartingNewGame', () => {
	Swal.fire({
		title: 'New game',
		text: 'A new game is starting',
		icon: 'warning',
		didClose: () => location.reload(),
		allowOutsideClick: false
	})
})

bingoConnection.on('BingoCalled', (name, className) => {
	Swal.fire({
		title: 'Banko called',
		text: `${name} from ${className} got banko!`,
		icon: 'info',
		timer: 5000,
		timerProgressBar: true,
		allowOutsideClick: false
	})
})

bingoConnection.on('Victory', () => {
	Swal.fire({
		title: 'Banko!',
		text: 'You got banko!',
		icon: 'success',
		allowOutsideClick: false
	})
})

bingoConnection.on('History', history => {
	History.reverse()
	for (const number of history) {
		const node = document.createElement('li')
		node.innerText = number
		historyList.appendChild(node)
	}
})

bingoConnection.start()
	.then(() => {
		bingoConnection.invoke('GetSheet')
		bingoConnection.invoke('GetHistory')
		bingoConnection.invoke('GetRowsNeededForBingo')
		const bingoButton = document.querySelector('#bingoButton')
		bingoButton.addEventListener('click', () => {
			bingoConnection.invoke('CallBingo')
		})
		bingoButton.disabled = false
	})
	.catch(err => console.error(err.toString()))