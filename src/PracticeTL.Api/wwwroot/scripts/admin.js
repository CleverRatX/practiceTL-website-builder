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

function toggleAcc(head) {
    head.parentElement.classList.toggle('open');
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
        <td>
            <input class="f-value" value="${escapeHtml(stat.value)}">
            <div class="upload-row">
                <input type="file" class="f-file" accept="image/*">
                <button type="button" class="upload">Загрузить</button>
            </div>
        </td>
        <td><input class="f-label" value="${escapeHtml(stat.label)}"></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;

    tr.querySelector('.save').addEventListener('click', () => saveStat(stat.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteStat(stat.id));
    tr.querySelector('.upload').addEventListener('click', () => uploadFile(tr.querySelector('.f-file'), tr.querySelector('.f-value')));
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
            <div class="upload-row">
                <input type="file" class="f-file" accept="image/*">
                <button type="button" class="upload">Загрузить</button>
            </div>
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
    tr.querySelector('.upload').addEventListener('click', () => uploadFile(tr.querySelector('.f-file'), photoInput));
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
        <td><textarea class="f-desc" rows="2">${escapeHtml(it.description)}</textarea></td>
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
        name: document.getElementById('new-pf-name').value,
        description: document.getElementById('new-pf-desc').value
    };
    const res = await fetch(PLATFORM_API, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    ['new-pf-year', 'new-pf-name', 'new-pf-desc'].forEach(id => document.getElementById(id).value = '');
    await loadPlatform();
}

async function savePlatform(id, tr) {
    const body = {
        year: tr.querySelector('.f-year').value,
        name: tr.querySelector('.f-name').value,
        description: tr.querySelector('.f-desc').value
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
            <div class="upload-row">
                <input type="file" class="f-file" accept="image/*">
                <button type="button" class="upload">Загрузить</button>
            </div>
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
    tr.querySelector('.upload').addEventListener('click', () => uploadFile(tr.querySelector('.f-file'), logoInput));
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

const DIRECTIONS_API = '/api/directions';
const directionsTbody = document.getElementById('directions-items');
enableDropZone(directionsTbody);

async function loadDirections() {
    const res = await fetch(DIRECTIONS_API);
    const items = await res.json();
    directionsTbody.innerHTML = '';
    for (const d of items) {
        directionsTbody.appendChild(buildDirectionRow(d));
    }
}

function buildDirectionRow(d) {
    const tr = document.createElement('tr');
    tr.dataset.id = d.id;
    tr.innerHTML = `
        <td class="drag-handle">≡</td>
        <td><input class="f-name" value="${escapeHtml(d.name)}"></td>
        <td><input class="f-badges" value="${escapeHtml(d.badges)}"></td>
        <td><textarea class="f-content" rows="3">${escapeHtml(d.content)}</textarea></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;
    tr.querySelector('.save').addEventListener('click', () => saveDirection(d.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteDirection(d.id));
    attachDrag(tr, saveDirectionsOrder);
    return tr;
}

async function saveDirectionsOrder() {
    const ids = [...directionsTbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(DIRECTIONS_API + '/reorder', {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadDirections();
}

async function addDirection() {
    const body = {
        name: document.getElementById('new-dir-name').value,
        badges: document.getElementById('new-dir-badges').value,
        content: document.getElementById('new-dir-content').value
    };
    const res = await fetch(DIRECTIONS_API, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    ['new-dir-name', 'new-dir-badges', 'new-dir-content'].forEach(id => document.getElementById(id).value = '');
    await loadDirections();
}

async function saveDirection(id, tr) {
    const body = {
        name: tr.querySelector('.f-name').value,
        badges: tr.querySelector('.f-badges').value,
        content: tr.querySelector('.f-content').value
    };
    const res = await fetch(DIRECTIONS_API + '/' + id, {
        method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadDirections();
}

async function deleteDirection(id) {
    if (!confirm('Удалить направление #' + id + '?')) return;
    const res = await fetch(DIRECTIONS_API + '/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadDirections();
}

const VACANCIES_API = '/api/vacancies';
const vacanciesTbody = document.getElementById('vacancies-items');
enableDropZone(vacanciesTbody);

async function loadVacancies() {
    const res = await fetch(VACANCIES_API);
    const items = await res.json();
    vacanciesTbody.innerHTML = '';
    for (const v of items) {
        vacanciesTbody.appendChild(buildVacancyRow(v));
    }
}

function buildVacancyRow(v) {
    const tr = document.createElement('tr');
    tr.dataset.id = v.id;
    tr.innerHTML = `
        <td class="drag-handle">≡</td>
        <td><input class="f-title" value="${escapeHtml(v.title)}"></td>
        <td><input class="f-address" value="${escapeHtml(v.address)}"></td>
        <td><input class="f-url" value="${escapeHtml(v.url)}"></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;
    tr.querySelector('.save').addEventListener('click', () => saveVacancy(v.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteVacancy(v.id));
    attachDrag(tr, saveVacanciesOrder);
    return tr;
}

async function saveVacanciesOrder() {
    const ids = [...vacanciesTbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(VACANCIES_API + '/reorder', {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadVacancies();
}

async function addVacancy() {
    const body = {
        title: document.getElementById('new-vac-title').value,
        address: document.getElementById('new-vac-address').value,
        url: document.getElementById('new-vac-url').value
    };
    const res = await fetch(VACANCIES_API, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    ['new-vac-title', 'new-vac-address', 'new-vac-url'].forEach(id => document.getElementById(id).value = '');
    await loadVacancies();
}

async function saveVacancy(id, tr) {
    const body = {
        title: tr.querySelector('.f-title').value,
        address: tr.querySelector('.f-address').value,
        url: tr.querySelector('.f-url').value
    };
    const res = await fetch(VACANCIES_API + '/' + id, {
        method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadVacancies();
}

async function deleteVacancy(id) {
    if (!confirm('Удалить вакансию #' + id + '?')) return;
    const res = await fetch(VACANCIES_API + '/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadVacancies();
}

async function uploadFile(fileInput, targetInput) {
    const file = fileInput.files[0];
    if (!file) { alert('Сначала выберите файл'); return; }
    const form = new FormData();
    form.append('file', file);
    const res = await fetch('/api/upload', { method: 'POST', body: form });
    if (!res.ok) { alert('Ошибка загрузки: ' + (await res.text())); return; }
    const data = await res.json();
    targetInput.value = data.url;
    targetInput.dispatchEvent(new Event('input'));
    fileInput.value = '';
}

const GALLERY_API = '/api/gallery';
const galleryTbody = document.getElementById('gallery-items');
enableDropZone(galleryTbody);

async function loadGallery() {
    const res = await fetch(GALLERY_API);
    const items = await res.json();
    galleryTbody.innerHTML = '';
    for (const g of items) {
        galleryTbody.appendChild(buildGalleryRow(g));
    }
}

function buildGalleryRow(g) {
    const tr = document.createElement('tr');
    tr.dataset.id = g.id;
    tr.innerHTML = `
        <td class="drag-handle">≡</td>
        <td>
            <select class="f-type">
                <option value="img">Изображение</option>
                <option value="video">Видео</option>
            </select>
        </td>
        <td>
            <input class="f-url" value="${escapeHtml(g.imageUrl)}">
            <div class="upload-row">
                <input type="file" class="f-file" accept="image/*,video/*">
                <button type="button" class="upload">Загрузить</button>
            </div>
        </td>
        <td><input class="f-caption" value="${escapeHtml(g.caption)}"></td>
        <td class="row-actions">
            <button class="save">Сохранить</button>
            <button class="danger del">Удалить</button>
        </td>`;
    tr.querySelector('.f-type').value = g.type;
    tr.querySelector('.upload').addEventListener('click', () => uploadFile(tr.querySelector('.f-file'), tr.querySelector('.f-url')));
    tr.querySelector('.save').addEventListener('click', () => saveGallery(g.id, tr));
    tr.querySelector('.del').addEventListener('click', () => deleteGallery(g.id));
    attachDrag(tr, saveGalleryOrder);
    return tr;
}

async function saveGalleryOrder() {
    const ids = [...galleryTbody.querySelectorAll('tr')].map(tr => Number(tr.dataset.id));
    const res = await fetch(GALLERY_API + '/reorder', {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(ids)
    });
    if (!res.ok) { alert('Не удалось сохранить порядок'); }
    await loadGallery();
}

async function addGallery() {
    const body = {
        type: document.getElementById('new-gal-type').value,
        imageUrl: document.getElementById('new-gal-url').value,
        caption: document.getElementById('new-gal-caption').value
    };
    const res = await fetch(GALLERY_API, {
        method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    ['new-gal-url', 'new-gal-caption'].forEach(id => document.getElementById(id).value = '');
    await loadGallery();
}

async function saveGallery(id, tr) {
    const body = {
        type: tr.querySelector('.f-type').value,
        imageUrl: tr.querySelector('.f-url').value,
        caption: tr.querySelector('.f-caption').value
    };
    const res = await fetch(GALLERY_API + '/' + id, {
        method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body)
    });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadGallery();
}

async function deleteGallery(id) {
    if (!confirm('Удалить фото #' + id + '?')) return;
    const res = await fetch(GALLERY_API + '/' + id, { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadGallery();
}

async function logout() {
    await fetch('/api/auth/logout', { method: 'POST' });
    window.location.href = '/login.html';
}

function showTab(name) {
    document.querySelectorAll('.tab').forEach(t => t.classList.remove('tab--active'));
    document.getElementById('tab-' + name).classList.add('tab--active');
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.toggle('tab-btn--active', b.dataset.tab === name));
    if (name === 'media') loadMedia(mediaPath);
    if (name === 'visibility') loadVisibility();
}

const BLOCKS_API = '/api/blocks';

async function loadVisibility() {
    const res = await fetch(BLOCKS_API);
    const blocks = await res.json();
    const list = document.getElementById('visibility-list');
    list.innerHTML = '';
    for (const b of blocks) {
        const row = document.createElement('label');
        row.className = 'vis-row';
        const cb = document.createElement('input');
        cb.type = 'checkbox';
        cb.checked = b.visible;
        cb.addEventListener('change', () => setVisibility(b.key, cb.checked));
        const span = document.createElement('span');
        span.textContent = b.title;
        row.appendChild(cb);
        row.appendChild(span);
        list.appendChild(row);
    }
}

async function setVisibility(key, visible) {
    const res = await fetch(BLOCKS_API + '/' + key, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ visible })
    });
    if (!res.ok) { alert('Не удалось сохранить'); }
}

const MEDIA_API = '/api/media';
let mediaPath = '';

async function loadMedia(path) {
    mediaPath = path;
    const res = await fetch(MEDIA_API + '/list?path=' + encodeURIComponent(path));
    if (!res.ok) { alert('Не удалось открыть папку'); return; }
    const data = await res.json();
    renderBreadcrumb(data.path);

    const list = document.getElementById('media-list');
    list.innerHTML = '';

    for (const folder of data.folders) {
        const div = document.createElement('div');
        div.className = 'media-item media-folder';
        div.innerHTML = `<div class="media-icon">📁</div><div class="media-name">${escapeHtml(folder)}</div>`;
        div.addEventListener('click', () => loadMedia(joinPath(path, folder)));
        list.appendChild(div);
    }

    for (const file of data.files) {
        const div = document.createElement('div');
        div.className = 'media-item';
        const preview = file.isImage
            ? `<img src="${escapeHtml(file.url)}" alt="">`
            : `<div class="media-icon">📄</div>`;
        div.innerHTML = `${preview}<div class="media-name">${escapeHtml(file.name)}</div>`;
        const del = document.createElement('button');
        del.className = 'danger';
        del.textContent = 'Удалить';
        del.addEventListener('click', () => deleteMedia(joinPath(path, file.name)));
        div.appendChild(del);
        list.appendChild(div);
    }

    if (data.folders.length === 0 && data.files.length === 0) {
        list.innerHTML = '<p class="hint">Папка пуста.</p>';
    }
}

function joinPath(base, name) {
    return base ? base + '/' + name : name;
}

function renderBreadcrumb(path) {
    const bc = document.getElementById('media-breadcrumb');
    bc.innerHTML = '';
    const root = document.createElement('a');
    root.textContent = 'media';
    root.addEventListener('click', () => loadMedia(''));
    bc.appendChild(root);

    let acc = '';
    for (const part of (path ? path.split('/') : [])) {
        acc = acc ? acc + '/' + part : part;
        bc.appendChild(document.createTextNode(' / '));
        const a = document.createElement('a');
        a.textContent = part;
        const target = acc;
        a.addEventListener('click', () => loadMedia(target));
        bc.appendChild(a);
    }
}

async function uploadMedia() {
    const input = document.getElementById('media-file');
    const file = input.files[0];
    if (!file) { alert('Сначала выберите файл'); return; }
    const form = new FormData();
    form.append('file', file);
    form.append('path', mediaPath);
    const res = await fetch(MEDIA_API + '/upload', { method: 'POST', body: form });
    if (!res.ok) { alert('Ошибка загрузки: ' + (await res.text())); return; }
    input.value = '';
    await loadMedia(mediaPath);
}

async function deleteMedia(relPath) {
    if (!confirm('Удалить файл ' + relPath + '?')) return;
    const res = await fetch(MEDIA_API + '?path=' + encodeURIComponent(relPath), { method: 'DELETE' });
    if (!res.ok) { alert('Ошибка: ' + (await res.text())); return; }
    await loadMedia(mediaPath);
}

loadHero();
loadTeam();
loadPlatform();
loadBrands();
loadDirections();
loadVacancies();
loadGallery();
