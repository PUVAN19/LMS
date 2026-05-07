document.addEventListener("DOMContentLoaded", function () {

    const multi = document.querySelector(".custom-multiselect");
    const searchInput = multi.querySelector(".search-input");
    const dropdown = multi.querySelector(".dropdown-list");
    const listBox = multi.querySelector(".authors-list");

    // Show dropdown when search box is clicked
    searchInput.addEventListener("focus", () => {
        multi.classList.add("active");
    });

    // Hide dropdown if clicked outside
    document.addEventListener("click", (e) => {
        if (!multi.contains(e.target)) {
            multi.classList.remove("active");
        }
    });

    // Search filter for ASP.NET ListBox
    searchInput.addEventListener("keyup", function () {
        let filter = searchInput.value.toLowerCase();

        for (let i = 0; i < listBox.options.length; i++) {
            let text = listBox.options[i].text.toLowerCase();
            listBox.options[i].style.display = text.includes(filter) ? "" : "none";
        }
    });

});
