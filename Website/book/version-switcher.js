// Adds a version switcher to the menu bar of the book.
// versions.json is written into the root of every book by Website/build.fsx.
(async () => {
    const versionsUrl = new URL(`${window.path_to_root ?? ""}versions.json`, document.baseURI);
    const bookRoot = new URL(".", versionsUrl);

    const response = await fetch(versionsUrl).catch(() => undefined);
    if (!response?.ok) {
        return;
    }

    const { current, versions } = await response.json();
    const buttons = document.querySelector(".menu-bar .right-buttons");
    if (versions.length < 2 || buttons === null) {
        return;
    }

    const select = document.createElement("select");
    select.setAttribute("aria-label", "Funcky version");
    select.title = "Funcky version";
    select.style.cssText = "margin: 0 8px; padding: 2px 4px; font: inherit; font-size: 0.7em;"
        + " color: var(--fg); background: var(--bg); border: 1px solid var(--icons); border-radius: 4px;";

    for (const version of versions) {
        const option = new Option(version.label, version.url, version.label === current, version.label === current);
        select.add(option);
    }

    select.addEventListener("change", async () => {
        // Stay on the same page when the other version has it, otherwise go to the start of that book.
        const page = location.pathname.startsWith(bookRoot.pathname)
            ? location.pathname.substring(bookRoot.pathname.length)
            : "";
        const samePage = select.value + page;
        const exists = page !== "" && await fetch(samePage, { method: "HEAD" }).then(r => r.ok, () => false);
        location.assign(exists ? samePage + location.hash : select.value);
    });

    buttons.prepend(select);
})();
