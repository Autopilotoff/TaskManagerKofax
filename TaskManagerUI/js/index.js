const jsonData = {
	added: [
		{ id: 1, name: "John", email: "john@example.com", properties: "some" },
		{ id: 2, name: "Jane Doe", email: "jane@example.com", properties: "another" },
		{ id: 3, name: "Jack", email: "Jack@example.com", properties: "other" }
	],
	updated: [
		{ id: 1, name: "John Doe", email: "Doe@example.com", properties: "what" },
		{ id: 3, name: "Mad Jack", email: "Mad@example.com", properties: "who" }
	],
	deleted: [2]
};

const tableContainer = document.getElementById('table-container');
tableContainer.appendChild(createTable(jsonData.added));

setTimeout((tableContainer, jsonData) => {
	addTableRows(tableContainer, jsonData.added);
}, 500, tableContainer, jsonData);

setTimeout((jsonData) => {
	updateTableRows(jsonData.updated);
}, 2000, jsonData);

setTimeout((jsonData) => {
	deleteTableRows(jsonData.deleted);
}, 4000, jsonData);


let socket = new WebSocket("ws://localhost:5159/TaskManager/SendCurrentProcessActions");

socket.onmessage = function (event) {
	let message = event.data;
	let messageElem = document.createElement('div');
	messageElem.textContent = message;
	document.getElementById('table-container').prepend(messageElem);
}