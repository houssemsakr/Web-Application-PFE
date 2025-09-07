// Fonction utilitaire pour limiter la fréquence d'exécution
function debounce(func, wait) {
    let timeout;
    return function () {
        const context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(context, args), wait);
    };
}

// Fonctions pour gérer les statuts
function getStatusClass(statut) {
    switch (statut) {
        case 'Validée': return 'status-validated';
        case 'En attente de Validation': return 'status-pending';
        case 'Non Validée': return 'status-not-validated';
        default: return '';
    }
}

function getStatusColor(statut) {
    switch (statut) {
        case 'Validée': return 'green';
        case 'En attente de Validation': return 'orange';
        case 'Non Validée': return 'red';
        default: return 'black';
    }
}

// Fonction pour ajouter une ligne RFQ dans le tableau
function addRFQRow(tableBody, data, versionNumber) {
    const row = document.createElement('tr');

    // Désactiver le bouton d'édition pour la version 0 (RFQ originale)
    const editButton = versionNumber === 0
        ? '<button type="button" class="custom-button" disabled><i class="bi bi-pencil-square"></i></button>'
        : `<button type="button" class="custom-button" onclick="editVersion(${data.id || data.rfqId}, ${versionNumber})"><i class="bi bi-pencil-square"></i></button>`;

    row.innerHTML = `
        <td>
            <div class="document-card">
                <div class="document-card-header">
                    <div class="document-header-content">
                        <div class="document-icon-wrapper">
                            <img src="/images/RFQ.png" alt="Logo" class="w-8 h-11">
                        </div>
                        <div>
                            <span class="document-number">RFQ N°0${data.rfqId} ___ V${versionNumber}</span>
                            <div class="document-description">Format RFQ</div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="custom-card" style="animation-delay: 0ms;">
                <div class="custom-actions">
                    ${editButton}
                </div>
            </div>
        </td>
        <td>     
            <span class="status-badge ${getStatusClass(data.statut)}" style="color: ${getStatusColor(data.statut)};">
                ${data.statut || 'N/A'}
            </span>
        </td>
        <td>
            ${new Date(data.dateCreation).toLocaleDateString()} 
        </td>
    `;
    tableBody.appendChild(row);
}

// Fonction pour récupérer une RFQ avec ses versions
function fetchRFQWithVersions(rfqId) {
    fetch(`/Versions/GetRFQById?id=${rfqId}`, {
        headers: {
            'Accept': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
        .then(response => {
            if (!response.ok) throw new Error('RFQ non trouvée');
            return response.json();
        })
        .then(data => {
            if (!data) throw new Error('Données invalides');

            const tableBody = document.querySelector('#rfqTable tbody');
            tableBody.innerHTML = '';

            // RFQ originale (V0)
            addRFQRow(tableBody, {
                id: data.id,
                rfqId: data.rfqId,
                dateCreation: data.dateCreation,
                statut: data.statut
            }, 0);

            // Versions (V1, V2...)
            data.versions.forEach((version, index) => {
                addRFQRow(tableBody, {
                    id: version.id,
                    rfqId: version.rfqId,
                    dateCreation: version.dateCreation,
                    statut: version.statut
                }, index + 1);
            });
        })
        .catch(error => {
            console.error('Erreur:', error);
            const tbody = document.querySelector('#rfqTable tbody');
            if (tbody) tbody.innerHTML = '<tr><td colspan="3">Aucune RFQ trouvée avec cet ID</td></tr>';
        });
}

// Fonction pour éditer une version
function editVersion(id, versionNumber) {
    if (!id || isNaN(id)) {
        console.error('ID de version invalide');
        return;
    }
    window.location.href = `/Versions/View_Version?id=${id}&version=${versionNumber}`;
}

// Initialisation dynamique pour "Consulter les versions"
function initializeDynamicContent() {
    // Initialisation spécifique à "Consulter les versions"
    if (document.getElementById('searchInputConsulter')) {
        const searchInput = document.getElementById('searchInputConsulter');

        // Restaurer la dernière recherche depuis localStorage
        const lastSearchedRFQ = localStorage.getItem('lastSearchedRFQ');
        if (lastSearchedRFQ) {
            searchInput.value = lastSearchedRFQ;
            fetchRFQWithVersions(lastSearchedRFQ);
        }

        // Gestionnaire d'événements pour la recherche
        searchInput.addEventListener('input', debounce(function () {
            const rfqId = this.value.trim();
            if (rfqId) {
                // Sauvegarder dans localStorage
                localStorage.setItem('lastSearchedRFQ', rfqId);
                fetchRFQWithVersions(rfqId);
            } else {
                const tbody = document.querySelector('#rfqTable tbody');
                if (tbody) tbody.innerHTML = '<tr><td colspan="3">Veuillez entrer un ID de RFQ.</td></tr>';
            }
        }, 300));
    }
}

// Initialisation au chargement de la page
document.addEventListener("DOMContentLoaded", function () {
    initializeDynamicContent();

    // Gestion du bouton retour
    window.addEventListener('popstate', function () {
        // Recharger les données si nécessaire
        const lastSearchedRFQ = localStorage.getItem('lastSearchedRFQ');
        if (lastSearchedRFQ) {
            fetchRFQWithVersions(lastSearchedRFQ);
        }
    });
});