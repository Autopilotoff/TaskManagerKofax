
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

function addTableRows(tableContainer, jsonData) {
    if (!jsonData || !jsonData.length) {
        return;
    }
    const columns = Object.keys(jsonData[0]);
    const tbody = tableContainer.querySelector('tbody');

    jsonData.forEach((rowData) => {
        tbody.appendChild(createTableRow(columns, rowData));
    });
}

function updateTableRows(jsonData) {
    jsonData.forEach((rowData) => {
        updateTableRow(rowData);
    });
}

function deleteTableRows(jsonData) {
    jsonData.forEach((idx) => {
        deleteTableRow(idx);
    });
}

function createTableRow(columns, rowData) {
    const row = document.createElement('tr');
    row.setAttribute('id', rowData[Object.keys(rowData)[0]]);

    columns.forEach(column => {
        const td = document.createElement('td');
        td.setAttribute('name', column);
        td.textContent = rowData[column];
        row.appendChild(td);
    });

    return row;
}

function updateTableRow(rowData) {
    const row = document.getElementById(rowData[Object.keys(rowData)[0]]);
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