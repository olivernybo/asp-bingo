const bingoCconnection = new signalR.HubConnectionBuilder().withUrl('/bingohub').build()

bingoCconnection.on('BingoCallerRecieve', number => {
	
})

bingoCconnection.on('Sheet', sheet => {
	// let currentRow = 0
	// let childCount = 0
	// let lastNum = null
	// for (const number of sheet) {
	// 	const strNumber = (number + '').padStart(2, '0')
	// 	if (lastNum && strNumber[0] == lastNum[0]) currentRow++
	// 	else if 
	// 	else currentRow = 0

	// 	let colIndex = number == 90 ? '8' : strNumber.charAt(0)

	// 	const td = document.querySelector(`#row${currentRow} .col${colIndex}`)
	// 	console.log(`#row${currentRow} .col${colIndex}`)
	// 	td.innerHTML = number

	// 	lastNum = strNumber

	// 	//if (currentRow > 2) currentRow = 0
	// }
})

bingoCconnection.start()
	.then(() => {
		bingoCconnection.invoke('GetSheet')
	})
	.catch(err => console.error(err.toString()))