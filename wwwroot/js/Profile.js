function togglePassword() {
    const passwordField = document.getElementById('passwordField');
    if (passwordField.type === 'password') {
        passwordField.type = 'text';
    } else {
        passwordField.type = 'password';
    }
}


// Selecciona todos los botones de "Update"
const updateButtons = document.querySelectorAll('.update-btn');

// Modal y sus elementos
const modal = document.getElementById('updateModal');
const confirmUpdate = document.getElementById('confirmUpdate');
const cancelUpdate = document.getElementById('cancelUpdate');
const updateInput = document.getElementById('actuallyInformation');
const updateField = document.getElementById('fieldToUpdate'); // Campo oculto para el nombre del campo
let currentField = null; // Variable para rastrear el campo actual

// Agrega el evento de clic a cada botón de "Update"
updateButtons.forEach((button) => {
    button.addEventListener('click', () => {
        modal.classList.add('show');
        modal.style.display = 'flex';

        // Guarda el campo actual a actualizar
        const dataCell = button.parentElement.previousElementSibling;
        currentField = dataCell;

        // Llena el input con el valor actual
        updateInput.value = dataCell.textContent.trim();

        // Establece el campo de nombre en el campo oculto "fieldToUpdate"
        const fieldName = dataCell.parentElement.firstElementChild.textContent.trim().replace(" ", "");
        updateField.value = fieldName;
        document.getElementById('fieldName').textContent = dataCell.parentElement.firstElementChild.textContent.trim();
    });
});

// Botón para cerrar la modal
cancelUpdate.addEventListener('click', closeModal);

// Cierra el modal al hacer clic fuera de él
window.addEventListener('click', (event) => {
    if (event.target === modal) {
        closeModal();
    }
});

function closeModal() {
    modal.classList.remove('show');
    setTimeout(() => {
        modal.style.display = 'none';
        updateInput.value = ''; // Limpia el valor del input
        currentField = null; // Restablece el campo actual
    }, 300); // Tiempo de espera para la animación
}
