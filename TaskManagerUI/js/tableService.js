class TableService {
    createTable(tableHeader) {
        const table = document.createElement('table');
        const thead = document.createElement('thead');
        const tbody = document.createElement('tbody');
    
        const headerRow = document.createElement('tr');
        tableHeader.forEach(column => {
            const th = document.createElement('th');
            th.textContent = column;
            headerRow.appendChild(th);
        });
        thead.appendChild(headerRow);
        table.appendChild(thead);
        table.appendChild(tbody);
    
        return table;
    }
    
    addTableRows(tableContainer, jsonData) {
        if (!jsonData || !jsonData.length) {
            return;
        }
        const columns = Object.keys(jsonData[0]);
        const tbody = tableContainer.querySelector('tbody');
    
        jsonData.forEach((rowData) => {
            tbody.appendChild(this.createTableRow(columns, rowData));
        });
    }
    
    updateTableRows(jsonData) {
        if (jsonData) {
            jsonData.forEach((rowData) => {
                this.updateTableRow(rowData);
            });
        }
    }
    
    deleteTableRows(jsonData) {
        if (jsonData) {
            jsonData.forEach((idx) => {
                this.deleteTableRow(idx);
            });
        }
    }
    
    createTableRow(columns, rowData) {
        const row = document.createElement('tr');
        row.setAttribute('id', rowData[columns[0]]);
    
        columns.forEach(column => {
            const td = document.createElement('td');
            td.setAttribute('name', column);
            td.textContent = rowData[column];
            row.appendChild(td);
        });
    
        return row;
    }
    
    updateTableRow(rowData) {
        const row = document.getElementById(rowData[Object.keys(rowData)[0]]);
        if (row && row.childNodes) {
            row.childNodes.forEach(td => {
                td.textContent = rowData[td.getAttribute("name")];
            });
        }
    }
    
    deleteTableRow(idx) {
        const row = document.getElementById(idx);
        if (row) {
            row.remove();
        }
    }
}
