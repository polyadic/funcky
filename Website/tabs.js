const panels = [...document.querySelectorAll('[role=tabpanel]')]
const tabs = [...document.querySelectorAll('[role=tab]')]

function updateActiveTab(activeTab) {
    const panel = document.getElementById(activeTab.getAttribute('aria-controls'))
    panels.forEach(p => p === panel ? p.classList.add('-active') : p.classList.remove('-active'))
    tabs.forEach(t => { t.setAttribute('aria-expanded', t === activeTab); t.setAttribute('aria-active', t === activeTab) })
}

updateActiveTab(tabs.filter(t => t.hasAttribute('data-default-tab'))[0])

tabs.forEach(tab => tab.addEventListener('click', _ => updateActiveTab(tab)))
