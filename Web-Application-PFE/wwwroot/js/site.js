function updateLine() {
    const box1 = document.querySelector('.box1');
    const box2 = document.querySelector('.box2');
    const line = document.getElementById('dynamic-line');

    if (box1 && box2 && line) {
        const rect1 = box1.getBoundingClientRect();
        const rect2 = box2.getBoundingClientRect();

        line.setAttribute("x1", rect1.left + rect1.width / 2);
        line.setAttribute("y1", rect1.top + rect1.height / 2);
        line.setAttribute("x2", rect2.left + rect2.width / 2);
        line.setAttribute("y2", rect2.top + rect2.height / 2);
    }
}

window.addEventListener('resize', updateLine);
window.addEventListener('load', updateLine);

function updateTitle(event, button) {
    event.preventDefault(); // Empêche le comportement par défaut

    if (!button) return;

    var title = button.getAttribute('data-title') || "Web_Application_PFE";

    document.getElementById("page-title").innerText = title;
    document.title = title + " - Web_Application_PFE";
}

document.addEventListener("DOMContentLoaded", function () {
    // Initialisation des icônes
    if (typeof feather !== 'undefined') {
        feather.replace();
    }

    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }

    // Gestion des onglets
    const tabs = document.querySelectorAll('.tab-trigger');
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const targetTab = tab.getAttribute('data-tab');
            if (!targetTab) return;

            // Mettre à jour l'onglet actif
            tabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');

            // Afficher le contenu approprié
            document.querySelectorAll('.tab-content').forEach(content => {
                content.classList.remove('active');
                content.classList.add('hidden');
            });

            const activeTabContent = document.getElementById(targetTab);
            if (activeTabContent) {
                activeTabContent.classList.remove('hidden');
                activeTabContent.classList.add('active');
            }
        });
    });

    // Gestion du menu déroulant (dropdown)
    const profileMenu = document.getElementById('profile-menu');
    const dropdownMenu = document.getElementById('dropdown-menu');

    if (profileMenu && dropdownMenu) {
        profileMenu.addEventListener('click', function (event) {
            event.stopPropagation(); // Empêche la propagation de l'événement
            dropdownMenu.classList.toggle('hidden');
        });

        // Fermer le menu déroulant si l'utilisateur clique en dehors
        window.addEventListener('click', function (event) {
            if (!dropdownMenu.contains(event.target) && event.target !== profileMenu) {
                dropdownMenu.classList.add('hidden');
            }
        });
    }

   
   
});
document.addEventListener('DOMContentLoaded', function () {
    // Récupérer les données de route depuis le HTML
    const routeData = document.getElementById('route-data');
    const controller = routeData.getAttribute('data-controller');
    const action = routeData.getAttribute('data-action');
    const area = routeData.getAttribute('data-area');

    console.log(`Route: Area=${area}, Controller=${controller}, Action=${action}`);

    // Mapping basé sur le contrôleur et l'action
    const routeTitles = {
        'Home': {
            'Index': 'Tableau de bord'

        },
        'RFQ': {
            'Rfq': 'RFQ'

        },
        'AddRFQ': {
            'Create': 'Créer nouvelle RFQ',
            'Edit': 'Modifier RFQ',
            'Gestion_RFQ': 'Gestion RFQ'
        },
        'Suivi_RFQ': {
            'SuiviRFQ': 'Suivi RFQ'
        },
        'Versions': {
            'Ajouter_une_version': 'Versions'
        }
    };

    // Définir le titre par défaut
    let title = 'Tableau de bord';

    // Vérifier si une correspondance existe
    if (controller && action && routeTitles[controller] && routeTitles[controller][action]) {
        title = routeTitles[controller][action];
    }

    // Appliquer le titre à l'élément HTML
    const titleElement = document.getElementById('page-title');
    if (titleElement) {
        titleElement.textContent = title;
        console.log('Titre appliqué:', title);
    } else {
        console.error('Element #page-title non trouvé');
    }
});
document.addEventListener("DOMContentLoaded", function () {
    const profileMenu = document.getElementById("profile-menu");
    const dropdownMenu = document.getElementById("dropdown-menu");

    profileMenu.addEventListener("click", function (event) {
        // Empêcher la propagation pour éviter la fermeture instantanée
        event.stopPropagation();

        // Toggle (ouvrir/fermer) le menu
        dropdownMenu.classList.toggle("hidden");
    });

    // Cacher le menu lorsqu'on clique en dehors
    document.addEventListener("click", function (event) {
        if (!profileMenu.contains(event.target) && !dropdownMenu.contains(event.target)) {
            dropdownMenu.classList.add("hidden");
        }
    });
});
