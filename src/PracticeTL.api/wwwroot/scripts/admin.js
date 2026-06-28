const API = '/api/hero';
const tbody = document.getElementById('items');
let draggedRow = null;

async function loadHero() {
    const res = await fetch(API);
    const hero = await res.json();

    document.getElementById('hero-title').value = hero.title;
    document.getElementById('hero-subtitle').value = hero.subtitle;

    tbody.innerHTML = '';
    for (const stat of hero.stats) {
        tbody.appendChild(buildRow(stat));
    }
}

async function saveInfo() {
    const body = {
        title: document.getElementById('hero-title').value,
        subtitle: document.getElementById('hero-subtitle').value
    };
    const res = await fetch(API, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка при сохранении шапки'); return; }
    alert('Шапка сохранена');
}

function buildRow(stat) {
    const tr = document.createElement('tr');
    tr.dataset.id = stat.id;

    tr.innerHTML = `
        <td class="drag-handle">≡</td>
        <td>
            <select class="f-type">
                <option value="text" ${stat.type === 'text' ? 'selected' : ''}>text</option>
                <option value="img" ${stat.type === 'img' ? 'selected' : ''}>img</option>
            </select>
        </td>
        <td><input class="f-value" value="${escapeHtml(stat.value)}"></td>
        <td><input class="f-label" value="${escapeHtml(stat.label)}"></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;

    tr.querySelector('.save').addEventListener('click', () => saveItem(stat.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteItem(stat.id));

    const handle = tr.querySelector('.drag-handle');
    handle.addEventListener('mousedown', () => { tr.draggable = true; });
    tr.addEventListener('dragstart', () => { tr.classList.add('dragging'); draggedRow = tr; });
    tr.addEventListener('dragend', async () => {
        tr.classList.remove('dragging');
        tr.draggable = false;
        draggedRow = null;
        await saveOrder();
    });

    return tr;
}

tbody.addEventListener('dragover', (e) => {
    e.preventDefault();
    if (!draggedRow) return;
    const after = getRowAfter(e.clientY);
    if (after == null) {
        tbody.appendChild(draggedRow);
    } else {
        tbody.insertBefore(draggedRow, after);
    }
});

function getRowAfter(y) {
    const rows = [...tbody.querySelectorAll('tr:not(.dragging)')];
    let closest = { offset: Number.NEGATIVE_INFINITY, element: null };
    for (const row of rows) {
        const box = row.getBoundingClientRect();
        const offset = y - box.top - box.height / 2;
        if (offset < 0 && offset > closest.offset) {
            closest = { offset: offset, element: row };
        }
    }
    return closest.element;
}

async function saveOrder() {
    const ids = [...tbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(API + '/stats/reorder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadHero();
}

async function addItem() {
    const body = {
        type: document.getElementById('new-type').value,
        value: document.getElementById('new-value').value,
        label: document.getElementById('new-label').value
    };
    const res = await fetch(API + '/stats', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    document.getElementById('new-value').value = '';
    document.getElementById('new-label').value = '';
    await loadHero();
}

async function saveItem(id, tr) {
    const body = {
        type: tr.querySelector('.f-type').value,
        value: tr.querySelector('.f-value').value,
        label: tr.querySelector('.f-label').value
    };
    const res = await fetch(API + '/stats/' + id, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadHero();
}

async function deleteItem(id) {
    if (!confirm('Удалить элемент #' + id + '?')) return;
    const res = await fetch(API + '/stats/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadHero();
}

// Экранирование, чтобы значение не ломало атрибут value="..."
function escapeHtml(str) {
    return String(str)
        .replaceAll('&', '&amp;')
        .replaceAll('"', '&quot;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;');
}

loadHero();
