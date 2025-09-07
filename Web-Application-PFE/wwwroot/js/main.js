const RFQViewManager = {
    loadRFQDetails: function (rfqId) {
        const url = `/AddRFQ/_Details?id=${rfqId}`;
        ViewLoader.loadView(url);
    }
};
    document.addEventListener("DOMContentLoaded", function () {
            // Initialisation des icônes
            if (feather) feather.replace();
    if (lucide) lucide.createIcons();

    // Charger la vue par défaut au démarrage
   
             // Gestion du clic sur les éléments du menu
            document.querySelectorAll('.menu-item').forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();

            // Supprimer la classe active de tous les éléments
            document.querySelectorAll('.menu-item').forEach(el => {
                el.classList.remove('active');
            });

            // Ajouter la classe active à l'élément cliqué
            this.classList.add('active');

            // Récupérer l'action à effectuer
            // loadPartialView(this.getAttribute('onclick').match(/'(.*?)'/)[1]);

            const action = this.getAttribute('data-action');

            // Charger le contenu dynamique
            loadContent(action);
        });
            });

         
        });

    function loadPartialView(url) {
        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(html => {
                document.getElementById('dynamicContent').innerHTML = html;
                // Réinitialiser les icônes
                if (feather) feather.replace();
                if (lucide) lucide.createIcons();

                // Si c'est la vue de consultation, initialiser la recherche
                if (url.includes('PartialView2')) {
                    initSearchFunctionality();
                }
            })
            .catch(error => {
                console.error('Error loading partial view:', error);
                document.getElementById('dynamicContent').innerHTML =
                    '<div class="error">Erreur de chargement du contenu</div>';
            });
        }

    function initSearchFunctionality() {
            const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const searchText = this.value.toLowerCase();
            const rows = document.querySelectorAll('#rfqTable tbody tr');

            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(searchText) ? '' : 'noney
            });
        });
            }
        }
    function loadDetailsView(url) {
        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(html => {
                document.getElementById('dynamicContent').innerHTML = html;
                // Réinitialiser les icônes
                if (feather) feather.replace();
                if (lucide) lucide.createIcons();
            })
            .catch(error => {
                console.error('Error loading details view:', error);
                document.getElementById('dynamicContent').innerHTML =
                    '<div class="error">Erreur de chargement des détails</div>';
            });
        }