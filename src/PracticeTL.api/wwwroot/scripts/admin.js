let draggedRow = null;

function getRowAfter(tbody, y) {
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

function enableDropZone(tbody) {
    tbody.addEventListener('dragover', (e) => {
        e.preventDefault();
        if (!draggedRow || draggedRow.parentElement !== tbody) return;
        const after = getRowAfter(tbody, e.clientY);
        if (after == null) {
            tbody.appendChild(draggedRow);
        } else {
            tbody.insertBefore(draggedRow, after);
        }
    });
}

function attachDrag(tr, onReorder) {
    const handle = tr.querySelector('.drag-handle');
    handle.addEventListener('mousedown', () => { tr.draggable = true; });
    tr.addEventListener('dragstart', () => { tr.classList.add('dragging'); draggedRow = tr; });
    tr.addEventListener('dragend', async () => {
        tr.classList.remove('dragging');
        tr.draggable = false;
        draggedRow = null;
        await onReorder();
    });
}

function escapeHtml(str) {
    return String(str)
        .replaceAll('&', '&amp;')
        .replaceAll('"', '&quot;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;');
}

const HERO_API = '/api/hero';
const heroTbody = document.getElementById('hero-items');
enableDropZone(heroTbody);

async function loadHero() {
    const res = await fetch(HERO_API);
    const hero = await res.json();

    document.getElementById('hero-title').value = hero.title;
    document.getElementById('hero-subtitle').value = hero.subtitle;

    heroTbody.innerHTML = '';
    for (const stat of hero.stats) {
        heroTbody.appendChild(buildStatRow(stat));
    }
}

async function saveInfo() {
    const body = {
        title: document.getElementById('hero-title').value,
        subtitle: document.getElementById('hero-subtitle').value
    };
    const res = await fetch(HERO_API, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка при сохранении шапки'); return; }
    alert('Шапка сохранена');
}

function buildStatRow(stat) {
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

    tr.querySelector('.save').addEventListener('click', () => saveStat(stat.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteStat(stat.id));
    attachDrag(tr, saveStatsOrder);
    return tr;
}

async function saveStatsOrder() {
    const ids = [...heroTbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(HERO_API + '/stats/reorder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadHero();
}

async function addStat() {
    const body = {
        type: document.getElementById('new-type').value,
        value: document.getElementById('new-value').value,
        label: document.getElementById('new-label').value
    };
    const res = await fetch(HERO_API + '/stats', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    document.getElementById('new-value').value = '';
    document.getElementById('new-label').value = '';
    await loadHero();
}

async function saveStat(id, tr) {
    const body = {
        type: tr.querySelector('.f-type').value,
        value: tr.querySelector('.f-value').value,
        label: tr.querySelector('.f-label').value
    };
    const res = await fetch(HERO_API + '/stats/' + id, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadHero();
}

async function deleteStat(id) {
    if (!confirm('Удалить элемент #' + id + '?')) return;
    const res = await fetch(HERO_API + '/stats/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadHero();
}

const TEAM_API = '/api/team';
const teamTbody = document.getElementById('team-items');
enableDropZone(teamTbody);

async function loadTeam() {
    const res = await fetch(TEAM_API);
    const members = await res.json();
    teamTbody.innerHTML = '';
    for (const m of members) {
        teamTbody.appendChild(buildMemberRow(m));
    }
}

function buildMemberRow(m) {
    const tr = document.createElement('tr');
    tr.dataset.id = m.id;
    tr.innerHTML = `
        <td class="drag-handle">≡</td>
        <td>
            <img class="photo-preview" src="${escapeHtml(m.photo)}" alt="">
            <input class="f-photo" value="${escapeHtml(m.photo)}" placeholder="/media/main/team/...">
        </td>
        <td><input class="f-name" value="${escapeHtml(m.name)}"></td>
        <td><input class="f-position" value="${escapeHtml(m.position)}"></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;

    const photoInput = tr.querySelector('.f-photo');
    const preview = tr.querySelector('.photo-preview');
    photoInput.addEventListener('input', () => { preview.src = photoInput.value; });

    tr.querySelector('.save').addEventListener('click', () => saveMember(m.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteMember(m.id));
    attachDrag(tr, saveTeamOrder);
    return tr;
}

async function saveTeamOrder() {
    const ids = [...teamTbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(TEAM_API + '/reorder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadTeam();
}

async function addMember() {
    const body = {
        name: document.getElementById('new-name').value,
        position: document.getElementById('new-position').value,
        photo: document.getElementById('new-photo').value
    };
    const res = await fetch(TEAM_API, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    ['new-name', 'new-position', 'new-photo'].forEach(id => document.getElementById(id).value = '');
    await loadTeam();
}

async function saveMember(id, tr) {
    const body = {
        name: tr.querySelector('.f-name').value,
        position: tr.querySelector('.f-position').value,
        photo: tr.querySelector('.f-photo').value
    };
    const res = await fetch(TEAM_API + '/' + id, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadTeam();
}

async function deleteMember(id) {
    if (!confirm('Удалить сотрудника #' + id + '?')) return;
    const res = await fetch(TEAM_API + '/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadTeam();
}

const PLATFORM_API = '/api/platform';
const platformTbody = document.getElementById('platform-items');
enableDropZone(platformTbody);

async function loadPlatform() {
    const res = await fetch(PLATFORM_API);
    const items = await res.json();
    platformTbody.innerHTML = '';
    for (const it of items) {
        platformTbody.appendChild(buildPlatformRow(it));
    }
}

function buildPlatformRow(it) {
    const tr = document.createElement('tr');
    tr.dataset.id = it.id;
    tr.innerHTML = `
        <td class="drag-handle">≡</td>
        <td><input class="f-year" value="${escapeHtml(it.year)}"></td>
        <td><input class="f-name" value="${escapeHtml(it.name)}"></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;
    tr.querySelector('.save').addEventListener('click', () => savePlatform(it.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deletePlatform(it.id));
    attachDrag(tr, savePlatformOrder);
    return tr;
}

async function savePlatformOrder() {
    const ids = [...platformTbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(PLATFORM_API + '/reorder', {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadPlatform();
}

async function addPlatform() {
    const body = {
        year: document.getElementById('new-pf-year').value,
        name: document.getElementById('new-pf-name').value
    };
    const res = await fetch(PLATFORM_API, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    ['new-pf-year', 'new-pf-name'].forEach(id => document.getElementById(id).value = '');
    await loadPlatform();
}

async function savePlatform(id, tr) {
    const body = {
        year: tr.querySelector('.f-year').value,
        name: tr.querySelector('.f-name').value
    };
    const res = await fetch(PLATFORM_API + '/' + id, {
        method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadPlatform();
}

async function deletePlatform(id) {
    if (!confirm('Удалить продукт #' + id + '?')) return;
    const res = await fetch(PLATFORM_API + '/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadPlatform();
}

const BRANDS_API = '/api/brands';
const brandsTbody = document.getElementById('brands-items');
enableDropZone(brandsTbody);

async function loadBrands() {
    const res = await fetch(BRANDS_API);
    const brands = await res.json();
    brandsTbody.innerHTML = '';
    for (const b of brands) {
        brandsTbody.appendChild(buildBrandRow(b));
    }
}

function buildBrandRow(b) {
    const tr = document.createElement('tr');
    tr.dataset.id = b.id;
    tr.innerHTML = `
        <td class="drag-handle">≡</td>
        <td>
            <img class="photo-preview photo-preview--logo" src="${escapeHtml(b.logo)}" alt="">
            <input class="f-logo" value="${escapeHtml(b.logo)}" placeholder="/media/main/hotels/...">
        </td>
        <td><input class="f-name" value="${escapeHtml(b.name)}"></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;
    const logoInput = tr.querySelector('.f-logo');
    const preview = tr.querySelector('.photo-preview');
    logoInput.addEventListener('input', () => { preview.src = logoInput.value; });
    tr.querySelector('.save').addEventListener('click', () => saveBrand(b.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteBrand(b.id));
    attachDrag(tr, saveBrandsOrder);
    return tr;
}

async function saveBrandsOrder() {
    const ids = [...brandsTbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(BRANDS_API + '/reorder', {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadBrands();
}

async function addBrand() {
    const body = {
        name: document.getElementById('new-br-name').value,
        logo: document.getElementById('new-br-logo').value
    };
    const res = await fetch(BRANDS_API, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    ['new-br-name', 'new-br-logo'].forEach(id => document.getElementById(id).value = '');
    await loadBrands();
}

async function saveBrand(id, tr) {
    const body = {
        name: tr.querySelector('.f-name').value,
        logo: tr.querySelector('.f-logo').value
    };
    const res = await fetch(BRANDS_API + '/' + id, {
        method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadBrands();
}

async function deleteBrand(id) {
    if (!confirm('Удалить бренд #' + id + '?')) return;
    const res = await fetch(BRANDS_API + '/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadBrands();
}

loadHero();
loadTeam();
loadPlatform();
loadBrands();
