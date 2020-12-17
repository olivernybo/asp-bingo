const connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build()

//Disable send button until connection is established
document.getElementById('sendButton').disabled = true

connection.on('ReceiveMessage', (user, color, message, id) => {
    const msg = message.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
    const encodedMsg = `<span data-session-id="${id}" style="color: ${color};">${user}</span>: ${msg}`
    const li = document.createElement('li')
    li.innerHTML = encodedMsg
	const list = document.getElementById('messagesList')
	list.prepend(li)
})

connection.on('ReceiveSystemMessage', message => {
    const msg = message.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
    const encodedMsg = `<span>System</span>: ${msg}`
	const li = document.createElement('li')
	li.style.color = 'white'
	li.style.backgroundColor = 'black'
    li.innerHTML = encodedMsg
	const list = document.getElementById('messagesList')
	list.prepend(li)
})

connection.start()
	.then(() => document.getElementById('sendButton').disabled = false)
	.catch(err => console.error(err.toString()))

const sendButton = document.getElementById('sendButton')
const messageInput = document.getElementById('messageInput')
sendButton.addEventListener('click', event => {
	const message = messageInput.value
	if (!message.replace(/\s+/g, '').length) return
	connection.invoke('SendMessage', message).catch(err => console.error(err.toString()))
	messageInput.value = ''
    event.preventDefault()
})

messageInput.addEventListener('keyup', e => {
	if (e.key === 'Enter') sendButton.click()
	e.preventDefault()
})