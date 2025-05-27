
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
            const row = tbody.insertRow();
            const wordCell = row.insertCell(0);
            const alternativesCell = row.insertCell(1);
            const actionCell = row.insertCell(2); 

            wordCell.textContent = item.word;
            alternativesCell.textContent = item.alternatives.join(', ');

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
    
            
            
            const cleanProcessedText = data.processedText.replace(/<[^>]*>/g, '');

            
            
            const originalWords = countWordFrequencies(data.originalText);
            const processedWords = countWordFrequencies(cleanProcessedText);

            const blacklistFromBackend = data.blacklistWords || []; 
                                                                    

            const modifiedWordCounts = { ...processedWords };

            blacklistFromBackend.forEach(blacklistedWord => {
                const lowerCaseBlacklistedWord = blacklistedWord.toLowerCase();
                if (modifiedWordCounts.hasOwnProperty(lowerCaseBlacklistedWord)) {
                    delete modifiedWordCounts[lowerCaseBlacklistedWord];
                }
            });
    
            const globalMax = getGlobalMaxCount(originalWords, modifiedWordCounts);
    
            renderSimpleWordCloud("originalWordCloud", originalWords, globalMax);
            renderSimpleWordCloud("modifiedWordCloud", modifiedWordCounts, globalMax); // A szűrt gyakoriságokat használjuk // We use filtered frequencies
            
            document.getElementById('outputSection').classList.remove('d-none');
            document.getElementById('Textinput').value = '';
        })
        .catch(err => {
            alert("Hiba: " + err.message);
        });
    });
    
    // Create wordCloud 
    function getModifiedWords(processedWords, replacementWords) {
        return { ...processedWords };
    }
    
    function removeReplacedWords(text, replacementWords) {
        let result = text;
        const wordsToRemove = Object.keys(replacementWords);
    
        wordsToRemove.forEach(word => {
            const regex = new RegExp('\\b' + word + '\\b', 'gi');
            result = result.replace(regex, ''); 
        });
    
        return result;
    }

    
    // Renders a word cloud
     function renderSimpleWordCloud(containerId, wordCounts, maxCount) {
        const container = document.getElementById(containerId);
    
           
        if (!container) {
            console.error(`There is no  ${containerId} id.`);
            return;
        }
    
        container.innerHTML = ''; 
    
        const sortedWords = Object.entries(wordCounts).sort((a, b) => b[1] - a[1]);
    
        const maxFontSize = 60; 
        const minFontSize = 12; 
    
        const wordList = document.createElement('div');
    
        sortedWords.forEach(([word, count]) => {
            const wordElement = document.createElement('div');
           
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