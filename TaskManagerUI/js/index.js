const jsonData = {
	added: [
		{ Id: 1, ProcessName: "check", NonpagedSystemMemorySize64: "100", PagedMemorySize64: "100" },
	]
};

const tableContainer = document.getElementById('table-container');
tableContainer.appendChild(createTable(jsonData.added));


// setTimeout((tableContainer, jsonData) => {
// 	addTableRows(tableContainer, jsonData.added);
// }, 500, tableContainer, jsonData);

// setTimeout((jsonData) => {
// 	updateTableRows(jsonData.updated);
// }, 2000, jsonData);

// setTimeout((jsonData) => {
// 	deleteTableRows(jsonData.deleted);
// }, 4000, jsonData);


let socket = new WebSocket("ws://localhost:5159/TaskManager/SendCurrentProcessActions");

socket.onmessage = function (event) {
	const jsonData = JSON.parse(event.data);
	deleteTableRows(jsonData.deleted);
	updateTableRows(jsonData.updated);
	addTableRows(tableContainer, jsonData.added);
}

setInterval(requestData, 2000);
function requestData() {
	if (socket && socket.readyState == WebSocket.OPEN) {
		socket.send('requestData');
	}
}

window.addEventListener('beforeunload', function (e) {
	if (socket && socket.readyState !== WebSocket.CLOSED) {
		socket.close();
	}
});