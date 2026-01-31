document.addEventListener("DOMContentLoaded", function () {
  initCursosModule();
});

function initCursosModule() {
  initDateValidation();
  initTableAnimations();
  initTooltips();
}

function initDateValidation() {
  const fechaInicio = document.getElementById("FechaInicio");
  const fechaFin = document.getElementById("FechaFin");

  if (fechaInicio && fechaFin) {

    fechaInicio.addEventListener("change", function () {
      if (this.value) {
        fechaFin.min = this.value;


        if (fechaFin.value && fechaFin.value < this.value) {
          fechaFin.value = "";
          showToast(
            "La fecha de fin debe ser posterior a la fecha de inicio",
            "warning",
          );
        }
      }
    });


    const form = document.getElementById("cursoForm");
    if (form) {
      form.addEventListener("submit", function (e) {
        if (fechaInicio.value && fechaFin.value) {
          if (new Date(fechaFin.value) < new Date(fechaInicio.value)) {
            e.preventDefault();
            showToast(
              "La fecha de fin debe ser mayor o igual a la fecha de inicio",
              "error",
            );
            fechaFin.focus();
          }
        }
      });
    }
  }
}

function initTableAnimations() {
  const table = document.getElementById("cursosTable");
  if (table) {
    const rows = table.querySelectorAll("tbody tr");
    rows.forEach((row, index) => {
      row.style.opacity = "0";
      row.style.transform = "translateX(-20px)";
      row.style.transition = "opacity 0.3s ease, transform 0.3s ease";

      setTimeout(() => {
        row.style.opacity = "1";
        row.style.transform = "translateX(0)";
      }, index * 50);
    });
  }
}

function initTooltips() {
  const tooltipTriggerList = [].slice.call(
    document.querySelectorAll("[title]"),
  );
  tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });
}

function showToast(message, type = "info") {
  const toastContainer =
    document.getElementById("toast-container") || createToastContainer();

  const toastHtml = `
        <div class="toast align-items-center text-bg-${type === "error" ? "danger" : type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi bi-${type === "error" ? "exclamation-triangle" : type === "warning" ? "exclamation-circle" : "info-circle"} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

  const toastElement = document.createElement("div");
  toastElement.innerHTML = toastHtml;
  const toast = toastElement.firstElementChild;
  toastContainer.appendChild(toast);

  const bsToast = new bootstrap.Toast(toast, { delay: 5000 });
  bsToast.show();

  toast.addEventListener("hidden.bs.toast", () => toast.remove());
}

function createToastContainer() {
  const container = document.createElement("div");
  container.id = "toast-container";
  container.className = "toast-container position-fixed bottom-0 end-0 p-3";
  container.style.zIndex = "1100";
  document.body.appendChild(container);
  return container;
}

function confirmDelete(courseName) {
  return confirm(
    `¿Está seguro de eliminar el curso "${courseName}"?\nEsta acción no se puede deshacer.`,
  );
}
