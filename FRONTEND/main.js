document.getElementById('processBlacklistBtn').addEventListener('click', () => {
    const blacklistInput = document.getElementById('blacklistInput').value.trim();
    if (!blacklistInput) {
        alert("A feketelistás adat nem lehet üres!");
        return;
    }

    const blacklist = processBlacklistInput(blacklistInput);

    fetch('http://localhost:5000/api/censorship/blacklist', { 
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(blacklist)
    })
    .then(response => {
        if (!response.ok) {
            alert("Hiba történt az adatok mentésekor.");
            return;
        }
        loadBlacklist();                                                                                                    

        document.getElementById('blacklistInput').value = '';
    })
    .catch(err => {
        alert("Hiba történt: " + err.message);
    });
});

// Feketelistás szöveg feldolgozása
function processBlacklistInput(input) {
    const entries = input.split('\n');
    const blacklist = {};

    entries.forEach(entry => {
        const [word, alternatives] = entry.split('@');
        if (word && alternatives) {
            blacklist[word] = alternatives.split(',');
        }
    });

    return blacklist;
}

// Táblázat frissítése a mentett feketelistás szavakkal
function loadBlacklist() {
    fetch('http://localhost:5000/api/censorship/blacklist', {
        method: 'GET',
    })
    .then(response => response.json())
    .then(data => {
        const tbody = document.getElementById('blacklistTable').getElementsByTagName('tbody')[0];
        tbody.innerHTML = '';

        data.forEach(item => {
            const row = tbody.insertRow();
            const wordCell = row.insertCell(0);
            const alternativesCell = row.insertCell(1);
            const actionCell = row.insertCell(2); // Új oszlop a törléshez

            wordCell.textContent = item.word;
            alternativesCell.textContent = item.alternatives.join(', ');

            // Törlés gomb létrehozása
            const deleteBtn = document.createElement('button');
            deleteBtn.textContent = 'Törlés';
            deleteBtn.classList.add('btn', 'btn-danger', 'btn-sm');
            deleteBtn.addEventListener('click', () => deleteWord(item.word));

            actionCell.appendChild(deleteBtn);
        });
    })
    .catch(err => {
        alert("Hiba történt a feketelistás adatok betöltésekor: " + err.message);
    });
}

function deleteWord(word) {
    if (!confirm(`Biztosan törölni szeretnéd ezt a szót: "${word}"?`)) return;
        fetch(`http://localhost:5000/api/censorship/blacklist/${encodeURIComponent(word)}`, {
            method: 'DELETE'
        })
        .then(response => {
            if (!response.ok) {
                alert("Törlés nem sikerült.");
                return;
            }
            alert("Sikeres törlés.");
            loadBlacklist(); // Frissítjük a listát
        })
        .catch(err => {
            alert("Hiba történt: " + err.message);
        });
    }
