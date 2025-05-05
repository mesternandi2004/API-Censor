
// Fetch for blacklist process + send to backend

document.getElementById('processBlacklistBtn').addEventListener('click', () => {
    const blacklistInput = document.getElementById('blacklistInput').value.trim();
    if (!blacklistInput) {
        alert("The blacklist data can not be empty!");
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
            alert("An error occurred while saving the data.");
            return;
        }
        loadBlacklist();                                                                                                    

        document.getElementById('blacklistInput').value = '';
    })
    .catch(err => {
        alert("Error: " + err.message);
    });
});

// Blaclist text process
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

// Update table with saved blacklist words + delete button
function loadBlacklist() {
    fetch('http://localhost:5000/api/censorship/blacklist', {
        method: 'GET',
    })
    .then(response => response.json())
    .then(data => {
        const tbody = document.getElementById('blacklistTable').getElementsByTagName('tbody')[0];
        tbody.innerHTML = '';

        data.forEach(item => {
            //new row 
            const row = tbody.insertRow();
            const wordCell = row.insertCell(0);
            const alternativesCell = row.insertCell(1);
            const actionCell = row.insertCell(2); 

            //Upload columns
            wordCell.textContent = item.word;
            alternativesCell.textContent = item.alternatives.join(', ');

            //Delete button
            const deleteBtn = document.createElement('button');
            deleteBtn.textContent = 'Delete';
            deleteBtn.classList.add('btn', 'btn-danger', 'btn-sm');
            deleteBtn.addEventListener('click', () => deleteWord(item.word));

            actionCell.appendChild(deleteBtn);
        });
    })
    .catch(err => {
        alert("An error occurred while loading blacklist data.: " + err.message);
    });

}

//Delete the row data
function deleteWord(word) {
    if (!confirm(`Are you sure you want to delete this row?: "${word}"?`)) return;
        fetch(`http://localhost:5000/api/censorship/blacklist/${encodeURIComponent(word)}`, {
            method: 'DELETE'
        })
        .then(response => {
            if (!response.ok) {
                alert("The deletion was not successful.");
                return;
            }
            alert("Successful Delete.");
            loadBlacklist(); 
        })
        .catch(err => {
            alert("Error: " + err.message);
        });
    }
    // sends the text entered by the user to the backend, processes it, and then displays the result and two word clouds: one based on the original text, one based on the modified text. I will explain step by step:
    document.getElementById('processTextBtn').addEventListener('click', () => {
        const inputText = document.getElementById('Textinput').value.trim();
        if (!inputText) {
            alert("The text can not be empty!");
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
                throw new Error("An error occurred while processing the data");
            }
            return response.json();
        })
        .then(data => {
            const outputDiv = document.getElementById('outputText');
            outputDiv.innerHTML = data.processedText;
    
            // Tisztítjuk a HTML tagek eltávolítása érdekében
            // Clean the text by removing HTML tags
            const cleanProcessedText = data.processedText.replace(/<[^>]*>/g, '');

            // Szavak gyakoriságának számítása az eredeti és a feldolgozott szövegben
            // Calculate word frequencies in the original and processed text
            const originalWords = countWordFrequencies(data.originalText);
            const processedWords = countWordFrequencies(cleanProcessedText);

            // Kinyerjük a feketelistás szavakat a backend válaszából (feltételezve, hogy a backend ezt elküldi)
            // Extract the blacklisted words from the backend response (assuming it provides them)
            const blacklistFromBackend = data.blacklistWords || []; // Ha a backend nem küldi, akkor üres tömb
                                                                    // If the backend doesn't send it, use an empty array

            // Létrehozunk egy másolatot a feldolgozott szavak gyakoriságáról, hogy módosíthassuk
            // Create a copy of the processed word frequencies so we can modify it
            const modifiedWordCounts = { ...processedWords };

            // Eltávolítjuk a feketelistás szavakat a módosított szófelhőből
            // Remove blacklisted words from the modified word cloud
            blacklistFromBackend.forEach(blacklistedWord => {
                const lowerCaseBlacklistedWord = blacklistedWord.toLowerCase();
                if (modifiedWordCounts.hasOwnProperty(lowerCaseBlacklistedWord)) {
                    delete modifiedWordCounts[lowerCaseBlacklistedWord];
                }
            });
    
            // Meghatározzuk a globális max gyakoriságot az eredeti ÉS a módosított szövegben
            // We determine the global max frequency in the original AND the modified text
            const globalMax = getGlobalMaxCount(originalWords, modifiedWordCounts);
    
            // Render word clouds based on common max
            // Rendereljük szófelhőket a max alapján
            renderSimpleWordCloud("originalWordCloud", originalWords, globalMax);
            renderSimpleWordCloud("modifiedWordCloud", modifiedWordCounts, globalMax); // A szűrt gyakoriságokat használjuk // We use filtered frequencies
            
            //Displays the result (outputSection) and clears the input field.
            document.getElementById('outputSection').classList.remove('d-none');
            document.getElementById('Textinput').value = '';
        })
        .catch(err => {
            alert("Hiba: " + err.message);
        });
    });
    
    // A módosított szófelhő előállítása 
    // Create wordCloud 
    function getModifiedWords(processedWords, replacementWords) {
        return { ...processedWords };
    }
    
    // minus word that we changed
    function removeReplacedWords(text, replacementWords) {
        let result = text;
        const wordsToRemove = Object.keys(replacementWords);
    
        //All of the words we changed we delete
        wordsToRemove.forEach(word => {
            const regex = new RegExp('\\b' + word + '\\b', 'gi'); // Szóhatárok biztosítása // Ensuring word boundaries
            result = result.replace(regex, ''); // A cserélt szót eltávolítjuk // Remove the changed word
        });
    
        return result;
    }

    // Szófelhő megjelenítése adott szavak gyakorisága alapján
    // Renders a word cloud based on word frequencies
     function renderSimpleWordCloud(containerId, wordCounts, maxCount) {
        const container = document.getElementById(containerId);
    
           // Ellenőrizzük, hogy a konténer létezik-e
             // Check if the container exists
        if (!container) {
            console.error(`There is no  ${containerId} id.`);
            return;
        }
    
        container.innerHTML = ''; // Last worldcloud delete 
    
        const sortedWords = Object.entries(wordCounts).sort((a, b) => b[1] - a[1]);
    
        const maxFontSize = 60; // Starter size
        const minFontSize = 12; // Smallest size
    
        const wordList = document.createElement('div');
    
        sortedWords.forEach(([word, count]) => {
            const wordElement = document.createElement('div');
            // A Math.log for good skala
            const fontSize = Math.max(minFontSize, (maxFontSize * Math.log1p(count)) / Math.log1p(maxCount));
            wordElement.style.fontSize = `${fontSize}px`;
            wordElement.textContent = word;
            wordList.appendChild(wordElement);
        });
    
        container.appendChild(wordList);
    }
    
    // Megszámolja a szavak előfordulásait egy szövegben
    // Counts the frequency of each word in a text
    function countWordFrequencies(text) {
      
        // Szöveg megtisztítása és szavakra bontása (az összes betű kisbetűs lesz)
        // Clean text and split into words (convert all letters to lowercase)

        const words = text.toLowerCase().replace(/[.,!]/g, '').match(/\b[\p{L}]+\b/gu) || [];
        const wordCounts = {};
        for (const word of words) {
            wordCounts[word] = (wordCounts[word] || 0) + 1;
        }
        return wordCounts;
    }
    
    // Meghatározza a legnagyobb előfordulást a megadott szógyakoriságok közül
    // Determines the maximum word frequency across multiple word count objects
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
    
    // Alkalmazás indításakor betölti a feketelistát a táblázatba
    // Load blacklist from backend into table on page load
    window.onload = loadBlacklist;