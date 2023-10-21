// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Get all textarea elements and adjust their height dynamically as the user types.
const tx = document.getElementsByTagName("textarea");
for (let i = 0; i < tx.length; i++) {
    tx[i].setAttribute("style", "height:" + (tx[i].scrollHeight) + "px;overflow-y:hidden;");
    tx[i].addEventListener("input", OnInput, false);
}

// Dynamically adjust the height of a textarea element as the user types.
function OnInput() {
    this.style.height = 0;
    this.style.height = (this.scrollHeight) + "px";
}

// Select various buttons and form elements.
let optionsButtons = document.querySelectorAll(".option-button");
let advancedOptionButton = document.querySelectorAll(".adv-option-button");
let fontName = document.getElementById("fontName");
let fontSizeRef = document.getElementById("fontSize");
let writingArea = document.getElementById("text-input");
let linkButton = document.getElementById("createLink");
let alignButtons = document.querySelectorAll(".align");
let spacingButtons = document.querySelectorAll(".spacing");
let formatButtons = document.querySelectorAll(".format");
let scriptButtons = document.querySelectorAll(".script");

// Initialize the user interface.
const initializer = () => {
    // Highlight buttons as needed based on user actions.
    highlighter(alignButtons, true);
    highlighter(spacingButtons, true);
    highlighter(formatButtons, false);
    highlighter(scriptButtons, true);

    // Create options for font names and font sizes.
    fontList.map((value) => {
        let option = document.createElement("option");
        option.value = value;
        option.innerHTML = value;
        fontName.appendChild(option);
    });

    // Font size options (from 1 to 7).
    for (let i = 1; i <= 7; i++) {
        let option = document.createElement("option");
        option.value = i;
        option.innerHTML = i;
        fontSizeRef.appendChild(option);
    }

    // Set the default font size.
    fontSizeRef.value = 3;
};

// Handle user interactions and modify text accordingly.
const modifyText = (command, defaultUi, value) => {
    // Execute a command on the selected text.
    document.execCommand(command, defaultUi, value);
};

// Handle basic operations that don't require a value parameter.
optionsButtons.forEach((button) => {
    button.addEventListener("click", () => {
        modifyText(button.id, false, null);
    });
});

// Handle options that require a value parameter, such as colors and fonts.
advancedOptionButton.forEach((button) => {
    button.addEventListener("change", () => {
        modifyText(button.id, false, button.value);
    });
});

// Handle link creation.
linkButton.addEventListener("click", () => {
    let userLink = prompt("Enter a URL");
    if (/http/i.test(userLink)) {
        modifyText(linkButton.id, false, userLink);
    } else {
        userLink = "http://" + userLink;
        modifyText(linkButton.id, false, userLink);
    }
});

// Handle the "quote" button click event.
document.getElementById('quote').addEventListener('click', () => {
    modifyText('formatBlock', false, '<blockquote>');
});

// Highlight the clicked button based on user interactions.
const highlighter = (className, needsRemoval) => {
    className.forEach((button) => {
        button.addEventListener("click", () => {
            if (needsRemoval) {
                let alreadyActive = false;
                if (button.classList.contains("active")) {
                    alreadyActive = true;
                }
                highlighterRemover(className);
                if (!alreadyActive) {
                    button.classList.add("active");
                }
            } else {
                button.classList.toggle("active");
            }
        });
    });
};

// Remove highlights from all buttons in the given class.
const highlighterRemover = (className) => {
    className.forEach((button) => {
        button.classList.remove("active");
    });
};

// Initialize the user interface when the page loads.
window.onload = initializer();

// Monitor text selection changes and show/hide the options menu accordingly.
document.addEventListener('selectionchange', () => {
    const selection = window.getSelection();
    const options = document.querySelector('.options');

    if (selection.isCollapsed) {
        options.style.display = 'none'; // Hide the menu if nothing is selected.
    } else {
        const range = selection.getRangeAt(0);
        const rect = range.getBoundingClientRect();
        options.style.display = 'block';
        options.style.top = rect.bottom + 'px'; // Position the menu below the selected text.
        options.style.left = rect.left + 'px'; // Align the menu with the left side of the text.
    }
});

// Handle clearing and restoring placeholder text.
function clearPlaceholder(element) {
    if (element.innerHTML === 'Start writing your article here') {
        element.innerHTML = '';
    }
}

function restorePlaceholder(element) {
    if (element.innerHTML === '') {
        element.innerHTML = 'Start writing your article here';
    }
}
