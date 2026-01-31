document.addEventListener('DOMContentLoaded', function () {
    initInscripcionesModule();
});

function initInscripcionesModule() {
    initDropdownValidation();
    initTableAnimations();
    initTooltips();
    initStatusBadge();
}

function initDropdownValidation() {
    const estudianteSelect = document.getElementById('estudianteSelect');
    const cursoSelect = document.getElementById('cursoSelect');
    const form = document.getElementById('inscripcionForm');

    if (form && estudianteSelect && cursoSelect) {
        form.addEventListener('submit', function (e) {
            let isValid = true;

            if (!estudianteSelect.value) {
                estudianteSelect.classList.add('is-invalid');
                isValid = false;
            } else {
                estudianteSelect.classList.remove('is-invalid');
            }

            if (!cursoSelect.value) {
                cursoSelect.classList.add('is-invalid');
                isValid = false;
            } else {
                cursoSelect.classList.remove('is-invalid');
            }

            if (!isValid) {
                e.preventDefault();
                showToast('Por favor seleccione un estudiante y un curso', 'error');
            }
        });


        [estudianteSelect, cursoSelect].forEach(select => {
            select.addEventListener('change', function () {
                if (this.value) {
                    this.classList.remove('is-invalid');
                }
            });
        });
    }
}

function initStatusBadge() {
    const estadoSelect = document.getElementById('Estado');
    
    if (estadoSelect) {
        estadoSelect.addEventListener('change', function () {

            this.classList.remove('border-success', 'border-warning', 'border-danger', 'border-primary', 'border-secondary');
            

            switch (this.value) {
                case 'Activo':
                    this.classList.add('border-success');
                    break;
                case 'Pendiente':
                    this.classList.add('border-warning');
                    break;
                case 'Completado':
                    this.classList.add('border-primary');
                    break;
                case 'Cancelado':
                    this.classList.add('border-danger');
                    break;
                default:
                    this.classList.add('border-secondary');
            }
        });


        estadoSelect.dispatchEvent(new Event('change'));
    }
}

function initTableAnimations() {
    const table = document.getElementById('inscripcionesTable');
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

function confirmDelete() {
    return confirm('¿Está seguro de eliminar esta inscripción?\nEsta acción no se puede deshacer.');
}
