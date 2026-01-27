// FamilyShow Web - Site JavaScript

// Global state
let selectedPersonId = null;
let familyListExpanded = false;
let currentView = 'list'; // Default to list view
let allPeopleData = []; // Cache of all people for filtering

// View switching
function switchView(viewName) {
    console.log('switchView called with:', viewName);

    // Hide all views
    document.querySelectorAll('.content-view').forEach(view => {
        view.style.display = 'none';
        console.log('Hiding view:', view.id);
    });

    // Remove active class from all buttons
    document.querySelectorAll('.view-btn').forEach(btn => {
        btn.classList.remove('active');
    });

    // Show selected view
    currentView = viewName;
    if (viewName === 'list') {
        const listView = document.getElementById('listView');
        if (listView) {
            listView.style.display = 'block';
            console.log('List view shown');
        } else {
            console.error('listView element not found!');
        }
        const listBtn = document.getElementById('listViewBtn');
        if (listBtn) {
            listBtn.classList.add('active');
        }
    } else if (viewName === 'tree') {
        const treeView = document.getElementById('treeView');
        if (treeView) {
            treeView.style.display = 'block';
            console.log('Tree view shown');
        } else {
            console.error('treeView element not found!');
        }
        const treeBtn = document.getElementById('treeViewBtn');
        if (treeBtn) {
            treeBtn.classList.add('active');
        }
    } else if (viewName === 'details') {
        const detailsView = document.getElementById('personDetailsView');
        if (detailsView) {
            detailsView.style.display = 'block';
            console.log('Details view shown');
        } else {
            console.error('personDetailsView element not found!');
        }
        const detailsBtn = document.getElementById('detailsViewBtn');
        if (detailsBtn) {
            detailsBtn.classList.add('active');
        }
    }
    console.log('switchView complete. Current view:', currentView);
}

// Populate list view table
function populateListView(peopleCollection) {
    console.log('populateListView called with:', peopleCollection ? peopleCollection.length : 0, 'people');
    const tbody = document.getElementById('familyListTableBody');
    const stats = document.getElementById('list-stats');

    console.log('tbody element exists:', !!tbody);
    console.log('stats element exists:', !!stats);

    if (!peopleCollection || peopleCollection.length === 0) {
        console.warn('No people to populate');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="8" style="text-align: center; padding: 2rem; color: var(--alternate-font-color);">No family members found</td></tr>';
        }
        if (stats) {
            stats.textContent = 'No people';
        }
        return;
    }

    allPeopleData = peopleCollection; // Cache for filtering
    if (stats) {
        stats.textContent = `${peopleCollection.length} ${peopleCollection.length === 1 ? 'person' : 'people'}`;
    }

    if (!tbody) {
        console.error('Cannot populate list - tbody element not found!');
        return;
    }

    tbody.innerHTML = '';
    console.log('Starting to populate', peopleCollection.length, 'people');

    peopleCollection.forEach((person, index) => {
        if (index < 5) {
            console.log('Processing person', index, ':', person);
        }

        const tr = document.createElement('tr');
        tr.className = 'family-row';
        tr.onclick = () => {
            updatePersonDetails(person);
        };

        // Calculate age
        let age = '-';
        if (person.BirthDate) {
            const birthDate = new Date(person.BirthDate);
            const endDate = person.DeathDate ? new Date(person.DeathDate) : new Date();
            const ageYears = Math.floor((endDate - birthDate) / (365.25 * 24 * 60 * 60 * 1000));
            if (ageYears >= 0) {
                age = ageYears.toString();
            }
        }

        // Format dates
        const formatDate = (dateStr) => {
            if (!dateStr) return '-';
            const date = new Date(dateStr);
            // Check for default/null dates
            if (date.getFullYear() < 1800) return '-';
            return date.toLocaleDateString();
        };

                const isLiving = !person.DeathDate || new Date(person.DeathDate).getFullYear() < 1800;

                // Photo icon
                const photoIcon = person.PhotoCount > 0 ? '📷' : '';

                tr.innerHTML = `
                    <td style="text-align: center;">${photoIcon}</td>
                    <td>${person.FirstName || '-'}</td>
                    <td>${person.LastName || '-'}</td>
                    <td>${age}</td>
                    <td><input type="checkbox" ${isLiving ? 'checked' : ''} disabled></td>
                    <td>${formatDate(person.BirthDate)}</td>
                    <td>${person.BirthPlace || '-'}</td>
                    <td>${formatDate(person.DeathDate)}</td>
                    <td>${person.DeathPlace || '-'}</td>
                `;

                tbody.appendChild(tr);
            });

            console.log('List view population complete');
        }

        // Sort list view
        let currentSortColumn = 'firstName';
        let currentSortDirection = 'asc';

        function sortList(column) {
            console.log('Sorting by:', column, 'Current direction:', currentSortDirection);

            // Toggle direction if same column
            if (currentSortColumn === column) {
                currentSortDirection = currentSortDirection === 'asc' ? 'desc' : 'asc';
            } else {
                currentSortColumn = column;
                currentSortDirection = 'asc';
            }

            // Sort the data
            const sortedData = [...allPeopleData].sort((a, b) => {
                let aVal, bVal;

                switch(column) {
                    case 'firstName':
                        aVal = (a.FirstName || '').toLowerCase();
                        bVal = (b.FirstName || '').toLowerCase();
                        break;
                    case 'lastName':
                        aVal = (a.LastName || '').toLowerCase();
                        bVal = (b.LastName || '').toLowerCase();
                        break;
                    case 'age':
                        // Calculate age for sorting
                        aVal = calculateAge(a.BirthDate, a.DeathDate);
                        bVal = calculateAge(b.BirthDate, b.DeathDate);
                        break;
                    case 'birthDate':
                        aVal = a.BirthDate ? new Date(a.BirthDate).getTime() : 0;
                        bVal = b.BirthDate ? new Date(b.BirthDate).getTime() : 0;
                        break;
                    case 'photos':
                        aVal = a.PhotoCount || 0;
                        bVal = b.PhotoCount || 0;
                        break;
                    default:
                        return 0;
                }

                if (currentSortDirection === 'asc') {
                    return aVal < bVal ? -1 : aVal > bVal ? 1 : 0;
                } else {
                    return aVal > bVal ? -1 : aVal < bVal ? 1 : 0;
                }
            });

            // Update sort indicators
            document.querySelectorAll('.sort-indicator').forEach(indicator => {
                indicator.textContent = '';
            });
            const headerCell = event.target.closest('th');
            if (headerCell) {
                const indicator = headerCell.querySelector('.sort-indicator');
                if (indicator) {
                    indicator.textContent = currentSortDirection === 'asc' ? ' ▲' : ' ▼';
                }
            }

            // Repopulate the list
            populateListView(sortedData);
        }

        function calculateAge(birthDate, deathDate) {
            if (!birthDate) return -1;
            const birth = new Date(birthDate);
            const end = deathDate ? new Date(deathDate) : new Date();
            return Math.floor((end - birth) / (365.25 * 24 * 60 * 60 * 1000));
        }

// Filter list view
function filterList() {
    const filter = document.getElementById('listFilter').value.toLowerCase();
    const rows = document.querySelectorAll('.family-row');
    let visibleCount = 0;

    rows.forEach(row => {
        const cells = row.querySelectorAll('td');
        const firstName = cells[0].textContent.toLowerCase();
        const lastName = cells[1].textContent.toLowerCase();

        if (firstName.includes(filter) || lastName.includes(filter)) {
            row.style.display = '';
            visibleCount++;
        } else {
            row.style.display = 'none';
        }
    });

    const stats = document.getElementById('list-stats');
    if (filter) {
        stats.textContent = `${visibleCount} of ${allPeopleData.length} ${allPeopleData.length === 1 ? 'person' : 'people'}`;
    } else {
        stats.textContent = `${allPeopleData.length} ${allPeopleData.length === 1 ? 'person' : 'people'}`;
    }
}

// Toggle details panel
function toggleDetailsPanel() {
    const panel = document.getElementById('detailsPanel');
    const treeArea = document.getElementById('treeViewArea');

    if (panel && treeArea) {
        panel.classList.toggle('open');
        treeArea.classList.toggle('panel-open');
    }
}

// Show details panel (if not already open)
function showDetailsPanel() {
    const panel = document.getElementById('detailsPanel');
    const treeArea = document.getElementById('treeViewArea');

    if (panel && !panel.classList.contains('open')) {
        panel.classList.add('open');
        treeArea.classList.add('panel-open');
    }
}

// Hide details panel
function hideDetailsPanel() {
    const panel = document.getElementById('detailsPanel');
    const treeArea = document.getElementById('treeViewArea');

    if (panel && panel.classList.contains('open')) {
        panel.classList.remove('open');
        treeArea.classList.remove('panel-open');
    }
}

// Show tab in details panel
function showTab(tabName) {
    // Hide all tabs
    document.querySelectorAll('.tab-content').forEach(tab => {
        tab.classList.remove('active');
    });
    document.querySelectorAll('.detail-tab').forEach(btn => {
        btn.classList.remove('active');
    });

    // Show selected tab
    const tabContent = document.getElementById('tab-' + tabName);
    if (tabContent) {
        tabContent.classList.add('active');
    }

    // Activate button
    event.target.classList.add('active');
}

// Update person details in panel
function updatePersonDetails(person) {
    if (!person) return;

    selectedPersonId = person.Id;

    // Update header
    document.querySelector('#panelPersonName span:last-child').textContent = person.FullName || `${person.FirstName} ${person.LastName}`;

    // Update photo section
    document.getElementById('personNameLarge').textContent = person.FullName || `${person.FirstName} ${person.LastName}`;

    let datesText = '';
    if (person.BirthDate) {
        datesText = `Born: ${new Date(person.BirthDate).toLocaleDateString()}`;
    }
    if (person.DeathDate) {
        datesText += ` - Died: ${new Date(person.DeathDate).toLocaleDateString()}`;
    }
    document.getElementById('personDates').textContent = datesText;

    // Update details tab
    document.getElementById('detailFirstName').textContent = person.FirstName || '-';
    document.getElementById('detailLastName').textContent = person.LastName || '-';
    document.getElementById('detailBirthDate').textContent = person.BirthDate ? new Date(person.BirthDate).toLocaleDateString() : '-';
    document.getElementById('detailBirthPlace').textContent = person.BirthPlace || '-';
    document.getElementById('detailDeathDate').textContent = person.DeathDate ? new Date(person.DeathDate).toLocaleDateString() : '-';
    document.getElementById('detailDeathPlace').textContent = person.DeathPlace || '-';

    // Update relationships tab
    updateRelationshipsList(person);

    // Show the panel
    showDetailsPanel();
}

// Update relationships list
function updateRelationshipsList(person) {
    const relationshipsList = document.getElementById('relationshipsList');
    relationshipsList.innerHTML = '';

    if (!person.Relatives || person.Relatives.length === 0) {
        relationshipsList.innerHTML = '<li style="color: var(--alternate-font-color); text-align: center; padding: 2rem;">No relationships found</li>';
        return;
    }

    person.Relatives.forEach(relative => {
        const li = document.createElement('li');
        li.className = 'relationship-item';
        li.onclick = () => {
            focusOnPerson(relative.PersonId);
            if (window.familyData) {
                const relPerson = window.familyData.PeopleCollection.find(p => p.Id === relative.PersonId);
                if (relPerson) {
                    updatePersonDetails(relPerson);
                }
            }
        };

        li.innerHTML = `
            <div class="relationship-type">${relative.RelationType}</div>
            <div class="relationship-name">${relative.PersonName}</div>
        `;
        relationshipsList.appendChild(li);
    });
}

// Populate family list
function populateFamilyList(peopleCollection) {
    const familyListContent = document.getElementById('familyListContent');
    const familyCount = document.getElementById('familyCount');

    if (!peopleCollection || peopleCollection.length === 0) {
        familyListContent.innerHTML = '<div style="color: var(--alternate-font-color); text-align: center; padding: 1rem;">No family members</div>';
        familyCount.textContent = '0';
        return;
    }

    familyCount.textContent = peopleCollection.length;

    familyListContent.innerHTML = '';
    peopleCollection.forEach(person => {
        const div = document.createElement('div');
        div.className = 'family-member-item';
        div.onclick = () => {
            focusOnPerson(person.Id);
            updatePersonDetails(person);
        };

        let dates = '';
        if (person.BirthDate) {
            const birthYear = new Date(person.BirthDate).getFullYear();
            dates = birthYear.toString();
            if (person.DeathDate) {
                const deathYear = new Date(person.DeathDate).getFullYear();
                dates += ` - ${deathYear}`;
            }
        }

        div.innerHTML = `
            <span class="family-member-name">${person.FullName || `${person.FirstName} ${person.LastName}`}</span>
            <span class="family-member-dates">${dates}</span>
        `;
        familyListContent.appendChild(div);
    });
}

// Toggle family list expansion
function toggleFamilyList() {
    const content = document.getElementById('familyListContent');
    const toggle = event.target;

    familyListExpanded = !familyListExpanded;

    if (familyListExpanded) {
        content.style.maxHeight = '400px';
        toggle.textContent = 'Collapse';
    } else {
        content.style.maxHeight = '0';
        toggle.textContent = 'Expand';
    }
}

// Filter family list
function filterFamilyList() {
    const filter = document.getElementById('familyFilter').value.toLowerCase();
    const items = document.querySelectorAll('.family-member-item');

    items.forEach(item => {
        const name = item.querySelector('.family-member-name').textContent.toLowerCase();
        if (name.includes(filter)) {
            item.style.display = 'flex';
        } else {
            item.style.display = 'none';
        }
    });
}

// Edit current person
function editCurrentPerson() {
    if (selectedPersonId) {
        alert('Edit person functionality coming soon!');
        // TODO: Implement edit dialog
    }
}

// Show add person dialog
function showAddPersonDialog() {
    alert('Add person functionality coming soon!');
    // TODO: Implement add person dialog
}

// Show add relationship dialog
function showAddRelationshipDialog() {
    if (selectedPersonId) {
        alert('Add relationship functionality coming soon!');
        // TODO: Implement add relationship dialog
    } else {
        alert('Please select a person first');
    }
}
