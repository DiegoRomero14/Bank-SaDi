function toggleMenu() {
    const contenedor = document.querySelector('.contenedor');
    contenedor.classList.toggle('menu-abierto');
}

function toggleUserMenu() {
    const userMenu = document.getElementById('userMenu');
    const overlay = document.getElementById('overlay');
    const isVisible = userMenu.style.display === 'block';

    userMenu.style.display = isVisible ? 'none' : 'block';
    overlay.style.display = isVisible ? 'none' : 'block';
}

function logout() {
    // Agrega aquí la lógica de cierre de sesión
    alert("Cerrando sesión...");
    toggleUserMenu(); // Cierra el menú después de hacer clic
}

function mostrarCuentas(button) {
    var tablaCuentas = document.querySelector('.tabla-cuentas');
    tablaCuentas.style.display = (tablaCuentas.style.display === "none" || tablaCuentas.style.display === "") ? "table" : "none";
}


function mostrarCuentas(button) {
    const row = button.closest('tr');
    let nextRow = row.nextElementSibling;

    while (nextRow && nextRow.classList.contains('account-row')) {
        nextRow.style.display = nextRow.style.display === 'none' ? '' : 'none';
        nextRow = nextRow.nextElementSibling;
    }
}

// JavaScript para manejar el modal y cargar los datos
const historyButton = document.querySelector('#btn-history');
const modalHistory = document.querySelector('#modal-History');
const modalBody = document.querySelector('.modalH-body');
const closeModalButton = document.querySelector('.close-btn');

if (historyButton && modalHistory && closeModalButton) {
    historyButton.addEventListener('click', async () => {
        modalHistory.classList.add('show');
        modalHistory.style.display = 'flex';
        await loadChangesHistory();
    });

    closeModalButton.addEventListener('click', () => {
        closeModal();
    });
}

function closeModal() {
    modalHistory.style.display = 'none';
    modalHistory.classList.remove('show');
}

async function loadChangesHistory() {
    try {
        const response = await fetch('/CRUD/GetChangesHistory');
        const data = await response.json();

        if (data.error) {
            modalBody.innerHTML = `<p class="error">${data.error}</p>`;
            return;
        }

        const table = `
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Usuario</th>
                        <th>Fecha</th>
                        <th>Tipo de Cambio</th>
                        <th>Tabla Afectada</th>
                        <th>ID Registro</th>
                        <th>Descripcion</th>
                    </tr>
                </thead>
                <tbody>
                    ${data.map(change => `
                        <tr>
                            <td>${change.changesId}</td>
                            <td>${change.userName}</td>
                            <td>${change.changeDate}</td>
                            <td>${change.changeType}</td>
                            <td>${change.tableAffected}</td>
                            <td>${change.recordId}</td>
                            <td>${change.description}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;

        modalBody.innerHTML = table;
    } catch (error) {
        modalBody.innerHTML = '<p class="error">Error al cargar el historial de cambios</p>';
    }
}



function openEditUserModal(userId, firstName, lastName, email, nationalid) {
    // Llenar los campos del formulario con los datos del usuario
    document.getElementById('editUserId').value = userId;
    document.getElementById('editFirstName').value = firstName;
    document.getElementById('editLastName').value = lastName;
    document.getElementById('editEmail').value = email;
    document.getElementById('editNational').value = nationalid;

    // Mostrar el modal
    document.getElementById('editUserModal').style.display = 'flex';
}

function closeModal2() {
    document.getElementById('editUserModal').style.display = 'none';
}