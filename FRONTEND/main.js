document.getElementById('processBlacklistBtn').addEventListener('click', () => {
    const blacklistInput = document.getElementById('blacklistInput').value.trim();
    if (!blacklistInput) {
        alert("A feketelistás adat nem lehet üres!");
        return;
    }

    const blacklist = processBlacklistInput(blacklistInput);

    fetch('http://localhost:5000/api/censorship/blacklist', { // itt a backend megfelelő portját használd!
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