// ===============================
// Main JavaScript for the Professor Dashboard frontend
// ===============================


//
const API_BASE = ".";


// Variables globales 
let selectedCourse = null;
let professorId = localStorage.getItem("professorId") || "";
let loggedProfessorEmail = localStorage.getItem("professorEmail") || "";
let editingAssignmentName = null;


// Funciones auxiliares
function showAlert(containerId, message, type) {
  const container = document.getElementById(containerId);
  if (!container) return;

  container.innerHTML = `
    <div class="alert alert-${type}">
      ${escapeHtml(message)}
    </div>
  `;
}

// Limpia el contenido de un contenedor de alertas
function clearAlert(containerId) {
  const container = document.getElementById(containerId);
  if (container) container.innerHTML = "";
}

// maneja caracteres especiales para evitar problemas con HTML
function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

// Intenta parsear la respuesta del backend como JSON, pero maneja casos donde 
// el backend devuelve texto plano o tiene caracteres antes del JSON

function parseBackendResponse(text) {
  const cleanText = (text || "").trim();

  if (cleanText === "") return null;

  try {
    return JSON.parse(cleanText);
  } catch (error) {
    const jsonStart = cleanText.search(/[\[{]/);
    if (jsonStart !== -1) {
      try {
        return JSON.parse(cleanText.slice(jsonStart));
      } catch (ignored) { }
    }

    if (cleanText.includes("false")) return false;
    if (cleanText.includes("true")) return true;

    return cleanText;
  }
}

// Función genérica para hacer solicitudes al backend
async function requestBackend(controller, options = {}) {
  const method = options.method || "GET";
  const params = options.params || null;
  const body = options.body || null;

  let url = `${controller}`;

  if (params) {
    const queryParams = new URLSearchParams(params);
    url += `?${queryParams.toString()}`;
  }

  const fetchOptions = {
    method,
    credentials: "include",
    headers: {}
  };

  if (body) {
    fetchOptions.headers["Content-Type"] = "application/json";
    fetchOptions.body = JSON.stringify(body);
  }

  const response = await fetch(url, fetchOptions);
  const text = await response.text();
  const data = parseBackendResponse(text);

  return {
    ok: response.ok,
    status: response.status,
    data,
    rawText: text
  };
}

// Funciones para manejar las estructuras de datos del backend, 
// adaptándose a posibles inconsistencias en los nombres de campos

function getCourseCode(course) {
  return course.code_course ?? course.course_code ?? course.code ?? "";
}

function getCourseName(course) {
  return course.name_course ?? course.name ?? "Curso sin nombre";
}

function getCourseDescription(course) {
  return course.description_course ?? course.description ?? "Sin descripción";
}

function getAssignmentName(assignment) {
  return assignment.name_assignment ?? assignment.assign_name ?? assignment.name ?? "Tarea sin nombre";
}

function getAssignmentDescription(assignment) {
  return assignment.description_assignment ?? assignment.assign_desc ?? assignment.description ?? "Sin descripción";
}

function getAssignmentDeadline(assignment) {
  return assignment.deadline ?? assignment.assign_deadline ?? "";
}

// El backend puede usar diferentes campos o formatos para indicar 
// si la entrega tardía está permitida
function getAssignmentAllowed(assignment) {
  const value = assignment.is_allowed_after_deadline ?? assignment.allowed ?? false;
  return value === true || value === 1 || value === "1" || value === "true";
}

function formatDateTimeForBackend(value) {
  if (!value) return "";
  return value.includes("T") ? value.replace("T", " ") + ":00" : value;
}

function formatDateTimeForInput(value) {
  if (!value) return "";
  return String(value).replace(" ", "T").slice(0, 16);
}


// Funciones para manejar la interfaz y la lógica de la aplicación
function showLogin() {
  document.getElementById("loginSection").classList.remove("d-none");
  document.getElementById("registerSection").classList.add("d-none");
  document.getElementById("dashboardSection").classList.add("d-none");
}

function showRegister() {
  document.getElementById("loginSection").classList.add("d-none");
  document.getElementById("registerSection").classList.remove("d-none");
  document.getElementById("dashboardSection").classList.add("d-none");
}

function showDashboard(name) {
  document.getElementById("loginSection").classList.add("d-none");
  document.getElementById("registerSection").classList.add("d-none");
  document.getElementById("dashboardSection").classList.remove("d-none");
  document.getElementById("navbarUser").classList.remove("d-none");
  document.getElementById("loggedUserName").textContent = name || "Profesor";

  loadCourses();
}

// Limpia la sesión y vuelve a la pantalla de login
function logout() {
  selectedCourse = null;
  editingAssignmentName = null;
  loggedProfessorEmail = "";
  localStorage.removeItem("professorEmail");

  document.getElementById("navbarUser").classList.add("d-none");
  document.getElementById("loginEmail").value = "";
  document.getElementById("loginPassword").value = "";

  clearCourses();
  clearAssignments();
  showLogin();
}

// Maneja el proceso de login, valida campos y comunica con el backend
async function login() {
  clearAlert("loginMessage");

  const email = document.getElementById("loginEmail").value.trim();
  const password = document.getElementById("loginPassword").value.trim();


  if (email === "" || password === "") {
    showAlert("loginMessage", "Debe ingresar correo y contraseña.", "warning");
    return;
  }


  try {
    const response = await requestBackend("api.php", {
      method: "GET",
      params: {
        action: "log_prof",
        email,
        password
      }
    });

    if (!response.ok || response.data?.success !== true) {
      showAlert(
        "loginMessage",
        response.data?.message || "Correo o contraseña incorrectos.",
        "danger"
      );
      return;
    }

    loggedProfessorEmail = response.data.email || email;

    localStorage.setItem("professorEmail", loggedProfessorEmail);

    showDashboard(response.data.name || loggedProfessorEmail);
  } catch (error) {
    showAlert("loginMessage", "Error de conexión.", "danger");
  }
}

// Maneja el proceso de registro, valida campos y comunica con el backend
async function register() {
  clearAlert("registerMessage");

  const name = document.getElementById("registerName").value.trim();
  const lastname = document.getElementById("registerLastname").value.trim();
  const email = document.getElementById("registerEmail").value.trim();
  const password = document.getElementById("registerPassword").value.trim();

  if (name === "" || lastname === "" || email === "" || password === "") {
    showAlert("registerMessage", "Todos los campos son obligatorios.", "warning");
    return;
  }

  try {
    const response = await requestBackend("api.php", {
      method: "POST",
      body: {
        action: "create_prof",
        username: name,
        userLast: lastname,
        email,
        password
      }
    });

    if (!response.ok || response.data?.success === false) {
      showAlert("registerMessage", response.data?.message || "No se pudo registrar el profesor.", "danger");
      return;
    }

    showAlert("registerMessage", "Profesor registrado correctamente. Ahora puede iniciar sesión.", "success");
    document.getElementById("registerName").value = "";
    document.getElementById("registerLastname").value = "";
    document.getElementById("registerEmail").value = "";
    document.getElementById("registerPassword").value = "";
  } catch (error) {
    showAlert("registerMessage", "Error de conexión con el registro del backend.", "danger");
  }
}

// Funciones para manejar cursos y tareas, incluyendo creación, carga y edición
function showCreateCourseForm() {
  document.getElementById("createCourseSection").classList.remove("d-none");
}

function hideCreateCourseForm() {
  document.getElementById("createCourseSection").classList.add("d-none");
  clearAlert("courseMessage");
}

function clearCourses() {
  const container = document.getElementById("coursesContainer");
  if (container) container.innerHTML = "";
}

function clearAssignments() {
  selectedCourse = null;
  const section = document.getElementById("assignmentSection");
  const container = document.getElementById("assignmentsContainer");

  if (section) section.classList.add("d-none");
  if (container) container.innerHTML = "";
}

// Maneja la creación de un nuevo curso, valida campos y se comunica con el backend
async function createCourse() {
  clearAlert("courseMessage");

  const code = document.getElementById("courseCode").value.trim();
  const name = document.getElementById("courseName").value.trim();
  const description = document.getElementById("courseDescription").value.trim();

  if (code === "" || name === "" || description === "") {
    showAlert("courseMessage", "Todos los campos del curso son obligatorios.", "warning");
    return;
  }

  try {
    const response = await requestBackend("api.php", {
      method: "POST",
      body: {
        action: "create_course",
        course_code: code,
        name_course: name,
        description
      }
    });

    if (!response.ok || response.data?.success === false) {
      showAlert("courseMessage", response.data?.message || "No se pudo crear el curso.", "danger");
      return;
    }

    document.getElementById("courseCode").value = "";
    document.getElementById("courseName").value = "";
    document.getElementById("courseDescription").value = "";

    showAlert("courseMessage", "Curso guardado en el backend correctamente.", "success");

    await loadCourses();


  } catch (error) {
    showAlert("courseMessage", "Error de conexion al crear el curso.", "danger");
  }
}
// Carga los cursos del profesor desde el backend y los muestra en la interfaz
async function loadCourses() {
  const container = document.getElementById("coursesContainer");

  container.innerHTML = `
    <div class="col-12">
      <div class="alert alert-secondary">Cargando cursos</div>
    </div>
  `;

  try {
    const response = await requestBackend("api.php", {
      method: "GET",
      params: {
        action: "get_all_courses",
        prof_id: professorId,
      }
    });

    if (!response.ok) {
      container.innerHTML = `
        <div class="col-12">
          <div class="alert alert-danger">No se pudieron cargar los cursos.</div>
        </div>
      `;
      return;
    }

    renderCourses(Array.isArray(response.data) ? response.data : []);
  } catch (error) {
    container.innerHTML = `
      <div class="col-12">
        <div class="alert alert-danger">Error de conexión al cargar cursos.</div>
      </div>
    `;
  }
}
// Muestra los cursos en la interfaz, manejando casos donde no hay cursos 
// o el formato de datos es inconsistente
function renderCourses(courses) {
  const container = document.getElementById("coursesContainer");
  container.innerHTML = "";

  if (!courses || courses.length === 0) {
    container.innerHTML = `
      <div class="col-12">
        <div class="alert alert-info">No hay cursos registrados todavía.</div>
      </div>
    `;
    return;
  }

  courses.forEach((course, index) => {
    const code = getCourseCode(course);
    const name = getCourseName(course);
    const description = getCourseDescription(course);

    container.innerHTML += `
      <div class="col-md-4">
        <div class="course-card" onclick="openCourse(${index})">
          <div class="course-code">${escapeHtml(code)}</div>
          <div class="course-name">${escapeHtml(name)}</div>
          <div class="course-description">${escapeHtml(description)}</div>
        </div>
      </div>
    `;
  });

  window.loadedCourses = courses;
}

async function openCourse(courseIndex) {
  const courses = window.loadedCourses || [];
  selectedCourse = courses[courseIndex];

  if (!selectedCourse) return;

  document.getElementById("assignmentSection").classList.remove("d-none");
  document.getElementById("assignmentCourseTitle").textContent =
    "Asignar tarea - " + getCourseName(selectedCourse);

  resetAssignmentForm();
  await loadAssignments();
}
// Maneja la creación o modificación de una tarea, valida campos y comunicacon el backend
async function createAssignment() {
  clearAlert("assignmentMessage");

  if (selectedCourse === null) {
    showAlert("assignmentMessage", "Debe seleccionar un curso primero.", "warning");
    return;
  }

  const name = document.getElementById("assignmentName").value.trim();
  const description = document.getElementById("assignmentDescription").value.trim();
  const deadline = document.getElementById("assignmentDeadline").value;
  const allowed = document.getElementById("assignmentAllowed").checked;

  if (name === "" || description === "" || deadline === "") {
    showAlert("assignmentMessage", "Todos los campos de la tarea son obligatorios.", "warning");
    return;
  }

  const action = editingAssignmentName ? "modify_assign" : "create_assign";

  try {
    const response = await requestBackend("api.php", {
      method: "POST",
      body: {
        action,
        assign_name: editingAssignmentName || name,
        course_code: getCourseCode(selectedCourse),
        assign_desc: description,
        assign_deadline: formatDateTimeForBackend(deadline),
        allowed
      }
    });

    if (!response.ok || response.data?.success === false) {
      showAlert("assignmentMessage", response.data?.message || "No se pudo guardar la tarea.", "danger");
      return;
    }

    showAlert(
      "assignmentMessage",
      editingAssignmentName ? "Tarea modificada en el backend correctamente." : "Tarea guardada en el backend correctamente.",
      "success"
    );

    resetAssignmentForm();
    await loadAssignments();
  } catch (error) {
    showAlert("assignmentMessage", "Error de conexión al guardar la tarea.", "danger");
  }
}

async function loadAssignments() {
  const container = document.getElementById("assignmentsContainer");
  container.innerHTML = `
    <div class="alert alert-secondary">Cargando tareas...</div>
  `;

  if (!selectedCourse) return;

  try {
    const response = await requestBackend("api.php", {
      method: "GET",
      params: {
        action: "get_assign_by_course",
        code_course: getCourseCode(selectedCourse)
      }
    });

    if (!response.ok) {
      container.innerHTML = `<div class="alert alert-danger">No se pudieron cargar las tareas.</div>`;
      return;
    }

    renderAssignments(Array.isArray(response.data) ? response.data : []);
  } catch (error) {
    container.innerHTML = `<div class="alert alert-danger">Error de conexión al cargar tareas.</div>`;
  }
}

function renderAssignments(assignments) {
  const container = document.getElementById("assignmentsContainer");
  container.innerHTML = "";

  if (!assignments || assignments.length === 0) {
    container.innerHTML = `<div class="alert alert-info">Este curso todavía no tiene tareas.</div>`;
    return;
  }

  assignments.forEach((assignment, index) => {
    const name = getAssignmentName(assignment);
    const description = getAssignmentDescription(assignment);
    const deadline = getAssignmentDeadline(assignment);
    const allowed = getAssignmentAllowed(assignment);

    container.innerHTML += `
      <div class="assignment-card">
        <div class="assignment-title">${escapeHtml(name)}</div>
        <p>${escapeHtml(description)}</p>
        <div class="assignment-date">Fecha límite: ${escapeHtml(deadline)}</div>
        <div>Entrega tardía: ${allowed ? "Permitida" : "No permitida"}</div>
        <button class="btn btn-outline-primary btn-sm mt-2" onclick="editAssignment(${index})">
          Modificar
        </button>
      </div>
    `;
  });

  window.loadedAssignments = assignments;
}

// Maneja la edición de una tarea, cargando sus datos en el formulario para modificarla
function editAssignment(index) {
  const assignments = window.loadedAssignments || [];
  const assignment = assignments[index];

  if (!assignment) return;

  editingAssignmentName = getAssignmentName(assignment);

  document.getElementById("assignmentName").value = editingAssignmentName;
  document.getElementById("assignmentName").disabled = true;
  document.getElementById("assignmentDescription").value = getAssignmentDescription(assignment);
  document.getElementById("assignmentDeadline").value = formatDateTimeForInput(getAssignmentDeadline(assignment));
  document.getElementById("assignmentAllowed").checked = getAssignmentAllowed(assignment);
  document.getElementById("assignmentSubmitButton").textContent = "Guardar cambios";
  document.getElementById("cancelEditAssignmentButton").classList.remove("d-none");
}

function resetAssignmentForm() {
  editingAssignmentName = null;
  document.getElementById("assignmentName").value = "";
  document.getElementById("assignmentName").disabled = false;
  document.getElementById("assignmentDescription").value = "";
  document.getElementById("assignmentDeadline").value = "";
  document.getElementById("assignmentAllowed").checked = false;
  document.getElementById("assignmentSubmitButton").textContent = "Guardar tarea";
  document.getElementById("cancelEditAssignmentButton").classList.add("d-none");
}

window.addEventListener("DOMContentLoaded", () => {
  showLogin();
});
