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

function createTable(jsonData) {
	const table = document.createElement('table');
	const thead = document.createElement('thead');
	const tbody = document.createElement('tbody');
	
	const columns = Object.keys(jsonData[0]);
	
	const headerRow = document.createElement('tr');
	columns.forEach(column => {
		const th = document.createElement('th');
		th.textContent = column;
		headerRow.appendChild(th);
	});
	thead.appendChild(headerRow);
	table.appendChild(thead);
	table.appendChild(tbody);

	return table;
}

setTimeout((tableContainer, jsonData) => {
	const columns = Object.keys(jsonData.added[0]);
	const tbody = tableContainer.querySelector('tbody');
	jsonData.added.forEach((rowData) => {
		tbody.appendChild(addTableRow(columns, rowData));
	});
}, 500, tableContainer, jsonData);

setTimeout((jsonData) => {
	jsonData.updated.forEach((rowData) => {
		updateTableRow(rowData);
	});
}, 2000, jsonData);

setTimeout((jsonData) => {
	jsonData.deleted.forEach((idx) => {
		deleteTableRow(idx);
	});
}, 4000, jsonData);

function addTableRow(columns, rowData) {
	const row = document.createElement('tr');
	row.setAttribute('id', rowData['id']);
	columns.forEach(column => {
		const td = document.createElement('td');
		td.setAttribute('name', column);
		td.textContent = rowData[column];
		row.appendChild(td);
	});

	return row;
}

function updateTableRow(rowData) {
	const row = document.getElementById(rowData['id']);
	if (row && row.childNodes) {
		row.childNodes.forEach(td => {
			td.textContent = rowData[td.getAttribute("name")];
		});
	}
}

function deleteTableRow(idx) {
	const row = document.getElementById(idx);
	if (row) {
		row.remove();
	}
}