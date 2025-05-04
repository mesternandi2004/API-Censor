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
    
    document.getElementById('processTextBtn').addEventListener('click', () => {
        const inputText = document.getElementById('Textinput').value.trim();
        if (!inputText) {
            alert("A szöveg nem lehet üres!");
            return;
        }
    
        fetch('http://localhost:5000/api/censorship/process', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ text: inputText })
        })
        .then(response => {
            if (!response.ok) {
                throw new Error("Hiba történt a feldolgozás során.");
            }
            return response.json();
        })
        .then(data => {
            const outputDiv = document.getElementById('outputText');
            outputDiv.innerHTML = data.processedText;
    
            // Tisztítjuk a HTML tagek eltávolítása érdekében
            const cleanProcessedText = data.processedText.replace(/<[^>]*>/g, '');
    
            // Szavak gyakoriságának számítása az eredeti és a feldolgozott szövegben
            const originalWords = countWordFrequencies(data.originalText);
            const processedWords = countWordFrequencies(cleanProcessedText);
    
            // Kinyerjük a feketelistás szavakat a backend válaszából (feltételezve, hogy a backend ezt elküldi)
            const blacklistFromBackend = data.blacklistWords || []; // Ha a backend nem küldi, akkor üres tömb
    
            // Létrehozunk egy másolatot a feldolgozott szavak gyakoriságáról, hogy módosíthassuk
            const modifiedWordCounts = { ...processedWords };
    
            // Eltávolítjuk a feketelistás szavakat a módosított szófelhőből
            blacklistFromBackend.forEach(blacklistedWord => {
                const lowerCaseBlacklistedWord = blacklistedWord.toLowerCase();
                if (modifiedWordCounts.hasOwnProperty(lowerCaseBlacklistedWord)) {
                    delete modifiedWordCounts[lowerCaseBlacklistedWord];
                }
            });
    
            // Meghatározzuk a globális max gyakoriságot az eredeti ÉS a módosított szövegben
            const globalMax = getGlobalMaxCount(originalWords, modifiedWordCounts);
    
            // Szófelhők renderelése közös max alapján
            renderSimpleWordCloud("originalWordCloud", originalWords, globalMax);
            renderSimpleWordCloud("modifiedWordCloud", modifiedWordCounts, globalMax); // A szűrt gyakoriságokat használjuk
    
            document.getElementById('outputSection').classList.remove('d-none');
            document.getElementById('Textinput').value = '';
        })
        .catch(err => {
            alert("Hiba: " + err.message);
        });
    });
    
    // A módosított szófelhő előállítása (erre most nincs külön szükség, mert a processedWords szűrésre kerül)
    function getModifiedWords(processedWords, replacementWords) {
        return { ...processedWords };
    }
    
    // Kivonjuk a cserélt szavakat a szövegből (erre a szófelhőhöz nincs szükség)
    function removeReplacedWords(text, replacementWords) {
        let result = text;
        const wordsToRemove = Object.keys(replacementWords);
    
        // Minden cserélt szót eltávolítunk a szövegből
        wordsToRemove.forEach(word => {
            const regex = new RegExp('\\b' + word + '\\b', 'gi'); // Szóhatárok biztosítása
            result = result.replace(regex, ''); // A cserélt szót eltávolítjuk
        });
    
        return result;
    }
    
    function renderSimpleWordCloud(containerId, wordCounts, maxCount) {
        const container = document.getElementById(containerId);
    
        if (!container) {
            console.error(`Nem található a ${containerId} elem.`);
            return;
        }
    
        container.innerHTML = ''; // Előző szófelhő törlése
    
        const sortedWords = Object.entries(wordCounts).sort((a, b) => b[1] - a[1]);
    
        const maxFontSize = 60; // Sokkal nagyobb kezdő betűméret
        const minFontSize = 12; // Kisebb alsó határ a több szintért
    
        const wordList = document.createElement('div');
    
        sortedWords.forEach(([word, count]) => {
            const wordElement = document.createElement('div');
            // A Math.log használata finomabb skálázást eredményezhet nagyobb gyakoriságkülönbségeknél
            const fontSize = Math.max(minFontSize, (maxFontSize * Math.log1p(count)) / Math.log1p(maxCount));
            wordElement.style.fontSize = `${fontSize}px`;
            wordElement.textContent = word;
            wordList.appendChild(wordElement);
        });
    
        container.appendChild(wordList);
    }
    
    function countWordFrequencies(text) {
        const words = text.toLowerCase().replace(/[.,!]/g, '').match(/\b[\p{L}]+\b/gu) || [];
        const wordCounts = {};
        for (const word of words) {
            wordCounts[word] = (wordCounts[word] || 0) + 1;
        }
        return wordCounts;
    }
    
    function getGlobalMaxCount(...wordCountsList) {
        let max = 1;
        for (const wordCounts of wordCountsList) {
            for (const count of Object.values(wordCounts)) {
                if (count > max) {
                    max = count;
                }
            }
        }
        return max;
    }
    
    
    window.onload = loadBlacklist;