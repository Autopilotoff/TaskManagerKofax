const config = {
	tableHeader: ['Id', 'Process Name', 'Nonpaged SystemMemory Size64', 'Paged Memory Size64'],
	currentProcessesUrl: 'ws://localhost:5159/TaskManager/SendCurrentProcessActions',
	requestInterval: 2000
};

const tableContainer = document.getElementById('table-container');
tableContainer.appendChild(createTable(config.tableHeader));


// setTimeout((tableContainer, jsonData) => {
// 	addTableRows(tableContainer, jsonData.added);
// }, 500, tableContainer, jsonData);

// setTimeout((jsonData) => {
// 	updateTableRows(jsonData.updated);
// }, 2000, jsonData);

// setTimeout((jsonData) => {
// 	deleteTableRows(jsonData.deleted);
// }, 4000, jsonData);


const processesSocket = new WebSocket(config.currentProcessesUrl);

processesSocket.onmessage = function (event) {
	const jsonData = JSON.parse(event.data);
	deleteTableRows(jsonData.deleted);
	updateTableRows(jsonData.updated);
	addTableRows(tableContainer, jsonData.added);
}

const refreshIntervalId = setInterval(requestData, config.requestInterval);
function requestData() {
	if (processesSocket && processesSocket.readyState == WebSocket.OPEN) {
		processesSocket.send('requestData');
	}
	else {
		clearInterval(refreshIntervalId);
	}
}

window.addEventListener('beforeunload', function (e) {
	if (processesSocket && processesSocket.readyState !== WebSocket.CLOSED) {
		processesSocket.close();
	}
});