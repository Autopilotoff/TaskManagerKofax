const config = {
	tableHeader: ['Id', 'Process Name', 'Nonpaged SystemMemory Size64', 'Paged Memory Size64'],
	currentProcessesUrl: 'localhost:5159/TaskManager/GetCurrentProcessActions',
	pingTimeout: 5000,
	token: createGuid()
};

const tableContainer = document.getElementById('table-container');
const tableService = new TableService();
tableContainer.appendChild(tableService.createTable(config.tableHeader));

const currentProcessesUrl = `ws://${config.currentProcessesUrl}?token=${config.token}`;
const processesSocket = new WebSocket(currentProcessesUrl);

console.info('Processess webSocket is opening...');
processesSocket.onmessage = function (event) {
	const jsonData = JSON.parse(event.data);

	tableService.deleteTableRows(jsonData.deleted);
	tableService.updateTableRows(jsonData.updated);
	tableService.addTableRows(tableContainer, jsonData.added);
}

console.info('Processess ping is starting...');
let refreshIntervalId = null;
const pingUrl = `http://${config.currentProcessesUrl}?token=${config.token}`;

processesSocket.onopen = () => {
	refreshIntervalId = setInterval(() => {
		fetch(pingUrl, { mode: 'no-cors' });
	}, config.pingTimeout);
};

processesSocket.onclose = () => {
	console.info('... Processes webSocket closed.');
	clearInterval(refreshIntervalId);
};


window.addEventListener('beforeunload', function (e) {
	if (processesSocket && processesSocket.readyState !== WebSocket.CLOSED) {
		processesSocket.close();
		console.info('...Processes webSocket is closing.');
	}
});