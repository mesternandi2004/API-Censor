
📌 Project Overview

This project is a full-stack application developed for the Full-Stack Software Development course. It processes a given text by replacing blacklisted words with alternatives, ensuring even distribution and avoiding repetition, while preserving case sensitivity. The frontend and backend communicate via RESTful API.
🏗️ Architecture

    Frontend: HTML, CSS, JavaScript (with Bootstrap)

    Backend: ASP.NET Core (C#)

    Communication: REST API


🔧 Functionality
🎯 Input

    Blacklist & Alternatives: Users input blacklist entries in the format
    word@alternative-1,alternative-2,...
    (One entry per line in a textarea)

    Main Text: Users paste a long text into a separate textarea.

⚙️ Backend Processing

    Parses blacklist and input text.

    Replaces blacklisted words with random alternatives:

        Even distribution across occurrences

        No repetition within the same sentence

        Preserves capitalization (e.g., "Word" → "Choice", "word" → "choice")

🖼️ Output

    Returns a modified version of the text, showing both the original and replaced word:

        Original word = red badge

        New word = green badge

    Example Output Format:
    The (fast|rapid) fox jumps over the (lazy|sleepy) dog.
    (Displayed visually as: fast in red badge, rapid in green badge)

☁️ Word Cloud

Displays two vertical word clouds side-by-side:
Original Text	Modified Text
Words sorted by frequency, from most to least	
Font size proportional to word frequency	

    No external libraries used for visualization. Built using plain HTML and CSS.
