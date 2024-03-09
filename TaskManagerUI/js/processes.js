const config = {
	tableHeader: ['Id', 'Process Name', 'Nonpaged SystemMemory Size64', 'Paged Memory Size64'],
	currentProcessesUrl: 'ws://localhost:5159/TaskManager/SendCurrentProcessActions',
	requestInterval: 2000
};

const tableContainer = document.getElementById('table-container');
tableContainer.appendChild(createTable(config.tableHeader));

const processesSocket = new WebSocket(config.currentProcessesUrl);
console.info('Processess webSocket is opening...');
processesSocket.onmessage = function (event) {
	const jsonData = JSON.parse(event.data);
	deleteTableRows(jsonData.deleted);
	updateTableRows(jsonData.updated);
	addTableRows(tableContainer, jsonData.added);
}

console.info('Processess watching is starting...');
const refreshIntervalId = setInterval(requestData, config.requestInterval);
function requestData() {
	if (!processesSocket 
		|| processesSocket.readyState === WebSocket.CLOSING 
		|| processesSocket.readyState === WebSocket.CLOSED) {
		clearInterval(refreshIntervalId);
		console.info('...Processess watching stopped.');
	}
	else if (processesSocket.readyState == WebSocket.OPEN) {
		processesSocket.send('data request');
	}
}

window.addEventListener('beforeunload', function (e) {
	if (processesSocket && processesSocket.readyState !== WebSocket.CLOSED) {
		processesSocket.close();
		console.info('...Processes webSocket is closing.');
	}
});