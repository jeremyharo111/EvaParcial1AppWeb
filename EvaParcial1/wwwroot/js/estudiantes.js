document.addEventListener('DOMContentLoaded', function () {
    initEstudiantesModule();
});

function initEstudiantesModule() {
    initBirthDateValidation();
    initEmailValidation();
    initTableAnimations();
    initTooltips();
}

function initBirthDateValidation() {
    const fechaNacimiento = document.getElementById('fechaNacimiento');
    
    if (fechaNacimiento) {

        const today = new Date().toISOString().split('T')[0];
        fechaNacimiento.max = today;
        

        const minDate = new Date();
        minDate.setFullYear(minDate.getFullYear() - 120);
        fechaNacimiento.min = minDate.toISOString().split('T')[0];

        fechaNacimiento.addEventListener('change', function () {
            if (this.value) {
                const birthDate = new Date(this.value);
                const today = new Date();
                
                if (birthDate > today) {
                    showToast('La fecha de nacimiento no puede ser futura', 'error');
                    this.value = '';
                } else {

                    const age = calculateAge(birthDate);
                    if (age < 5) {
                        showToast(`Edad calculada: ${age} años. ¿Es correcta esta fecha?`, 'warning');
                    }
                }
            }
        });
    }
}

function calculateAge(birthDate) {
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }
    
    return age;
}

function initEmailValidation() {
    const emailInput = document.getElementById('Email');
    
    if (emailInput) {
        emailInput.addEventListener('blur', function () {
            const email = this.value.trim();
            if (email && !isValidEmail(email)) {
                this.classList.add('is-invalid');
                showToast('Por favor ingrese un correo electrónico válido', 'warning');
            } else {
                this.classList.remove('is-invalid');
            }
        });

        emailInput.addEventListener('input', function () {
            if (this.classList.contains('is-invalid') && isValidEmail(this.value.trim())) {
                this.classList.remove('is-invalid');
            }
        });
    }
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function initTableAnimations() {
    const table = document.getElementById('estudiantesTable');
    if (table) {
        const rows = table.querySelectorAll('tbody tr');
        rows.forEach((row, index) => {
            row.style.opacity = '0';
            row.style.transform = 'translateX(-20px)';
            row.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
            
            setTimeout(() => {
                row.style.opacity = '1';
                row.style.transform = 'translateX(0)';
            }, index * 50);
        });
    }
}

function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[title]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

function showToast(message, type = 'info') {
    const toastContainer = document.getElementById('toast-container') || createToastContainer();
    
    const toastHtml = `
        <div class="toast align-items-center text-bg-${type === 'error' ? 'danger' : type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi bi-${type === 'error' ? 'exclamation-triangle' : type === 'warning' ? 'exclamation-circle' : 'info-circle'} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    
    const toastElement = document.createElement('div');
    toastElement.innerHTML = toastHtml;
    const toast = toastElement.firstElementChild;
    toastContainer.appendChild(toast);
    
    const bsToast = new bootstrap.Toast(toast, { delay: 5000 });
    bsToast.show();
    
    toast.addEventListener('hidden.bs.toast', () => toast.remove());
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toast-container';
    container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    container.style.zIndex = '1100';
    document.body.appendChild(container);
    return container;
}

function confirmDelete(studentName) {
    return confirm(`¿Está seguro de eliminar al estudiante "${studentName}"?\nEsta acción eliminará también todas sus inscripciones.`);
}
