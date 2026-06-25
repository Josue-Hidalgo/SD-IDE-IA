// ===============================
// Main JavaScript for the Professor Dashboard frontend
// ===============================

const API_BASE = ".";

// Ícono lápiz (trazado del PNG provisto)
const ICON_EDIT = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" fill="none"
  stroke="currentColor" stroke-width="28" stroke-linecap="round" stroke-linejoin="round"
  style="width:14px;height:14px;vertical-align:middle;display:inline-block;">
  <!-- punta -->
  <polygon points="34,478 76,388 124,436"/>
  <!-- cuerpo izquierdo -->
  <line x1="76" y1="388" x2="308" y2="156"/>
  <!-- cuerpo derecho -->
  <line x1="124" y1="436" x2="356" y2="204"/>
  <!-- base ferrule -->
  <line x1="308" y1="156" x2="356" y2="204"/>
  <!-- separador sección superior -->
  <line x1="308" y1="156" x2="268" y2="196"/>
  <line x1="356" y1="204" x2="316" y2="244"/>
  <line x1="268" y1="196" x2="316" y2="244"/>
  <!-- goma / cap -->
  <path d="M356 204 L394 166 Q430 110 468 80 Q492 60 488 44 Q480 24 458 36 Q424 58 388 96 L350 134 Z"/>
  <!-- oval ranura goma -->
  <rect x="412" y="94" width="34" height="62" rx="17" ry="17" transform="rotate(-45 429 125)"/>
  <!-- divisor ferrule -->
  <line x1="388" y1="96" x2="350" y2="134"/>
  <line x1="420" y1="64" x2="382" y2="102"/>
</svg>`;

// Variables globales
let selectedCourse = null;
let professorId = localStorage.getItem("professorId") || "";
let loggedProfessorEmail = localStorage.getItem("professorEmail") || "";
let editingAssignmentName = null;

// ─────────────────────────────────────────
// UTILIDADES GENERALES
// ─────────────────────────────────────────

function showAlert(containerId, message, type) {
  const container = document.getElementById(containerId);
  if (!container) return;
  container.innerHTML = `<div class="alert alert-${type}">${escapeHtml(message)}</div>`;
}

function clearAlert(containerId) {
  const container = document.getElementById(containerId);
  if (container) container.innerHTML = "";
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function parseBackendResponse(text) {
  const cleanText = (text || "").trim();
  if (cleanText === "") return null;
  try {
    return JSON.parse(cleanText);
  } catch {
    const jsonStart = cleanText.search(/[\[{]/);
    if (jsonStart !== -1) {
      try { return JSON.parse(cleanText.slice(jsonStart)); } catch { }
    }
    if (cleanText.includes("false")) return false;
    if (cleanText.includes("true")) return true;
    return cleanText;
  }
}

async function requestBackend(controller, options = {}) {
  const method = options.method || "GET";
  const params = options.params || null;
  const body   = options.body   || null;

  let url = `${controller}`;
  if (params) url += `?${new URLSearchParams(params).toString()}`;

  const fetchOptions = { method, credentials: "include", headers: {} };
  if (body) {
    fetchOptions.headers["Content-Type"] = "application/json";
    fetchOptions.body = JSON.stringify(body);
  }

  const response = await fetch(url, fetchOptions);
  const text = await response.text();
  const data = parseBackendResponse(text);
  return { ok: response.ok, status: response.status, data, rawText: text };
}

// ─────────────────────────────────────────
// HELPERS DE CAMPOS
// ─────────────────────────────────────────

function getCourseCode(c)        { return c.code_course ?? c.course_code ?? c.code ?? ""; }
function getCourseName(c)        { return c.name_course ?? c.name ?? "Curso sin nombre"; }
function getCourseDescription(c) { return c.description_course ?? c.description ?? "Sin descripción"; }
function getAssignmentName(a)    { return a.name_assignment ?? a.assign_name ?? a.name ?? "Tarea sin nombre"; }
function getAssignmentDescription(a) { return a.description_assignment ?? a.assign_desc ?? a.description ?? "Sin descripción"; }
function getAssignmentDeadline(a) { return a.deadline ?? a.assign_deadline ?? ""; }
function getAssignmentAllowed(a) {
  const v = a.is_allowed_after_deadline ?? a.allowed ?? false;
  return v === true || v === 1 || v === "1" || v === "true";
}

function formatDateTimeForBackend(value) {
  if (!value) return "";
  return value.includes("T") ? value.replace("T", " ") + ":00" : value;
}
function formatDateTimeForInput(value) {
  if (!value) return "";
  return String(value).replace(" ", "T").slice(0, 16);
}
function formatDateDisplay(value) {
  if (!value) return "—";
  const d = new Date(value);
  if (isNaN(d)) return value;
  return d.toLocaleDateString("es-CR", { day: "2-digit", month: "2-digit", year: "numeric" });
}

// ─────────────────────────────────────────
// NAVEGACIÓN ENTRE SECCIONES
// ─────────────────────────────────────────

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

function logout() {
  selectedCourse = null;
  editingAssignmentName = null;
  loggedProfessorEmail = "";
  localStorage.removeItem("professorEmail");
  localStorage.removeItem("professorId");
  document.getElementById("navbarUser").classList.add("d-none");
  document.getElementById("loginEmail").value = "";
  document.getElementById("loginPassword").value = "";
  clearCourses();
  clearAssignments();
  closeReviewPanel();
  showLogin();
}

// ─────────────────────────────────────────
// AUTH
// ─────────────────────────────────────────

async function login() {
  clearAlert("loginMessage");
  const email    = document.getElementById("loginEmail").value.trim();
  const password = document.getElementById("loginPassword").value.trim();
  if (!email || !password) {
    showAlert("loginMessage", "Debe ingresar correo y contraseña.", "warning");
    return;
  }
  try {
    const res = await requestBackend("api.php", {
      method: "GET",
      params: { action: "log_prof", email, password }
    });
    if (!res.ok || res.data?.success !== true) {
      showAlert("loginMessage", res.data?.message || "Correo o contraseña incorrectos.", "danger");
      return;
    }
    loggedProfessorEmail = res.data.email || email;
    professorId = res.data.prof_id || null;
    localStorage.setItem("professorId", professorId);
    localStorage.setItem("professorEmail", loggedProfessorEmail);
    showDashboard(res.data.name || loggedProfessorEmail);
  } catch {
    showAlert("loginMessage", "Error de conexión.", "danger");
  }
}

async function register() {
  clearAlert("registerMessage");
  const name     = document.getElementById("registerName").value.trim();
  const lastname = document.getElementById("registerLastname").value.trim();
  const email    = document.getElementById("registerEmail").value.trim();
  const password = document.getElementById("registerPassword").value.trim();
  if (!name || !lastname || !email || !password) {
    showAlert("registerMessage", "Todos los campos son obligatorios.", "warning");
    return;
  }
  try {
    const res = await requestBackend("api.php", {
      method: "POST",
      body: { action: "create_prof", username: name, userLast: lastname, email, password }
    });
    if (!res.ok || res.data?.success === false) {
      showAlert("registerMessage", res.data?.message || "No se pudo registrar el profesor.", "danger");
      return;
    }
    showAlert("registerMessage", "Profesor registrado correctamente. Ahora puede iniciar sesión.", "success");
    ["registerName","registerLastname","registerEmail","registerPassword"].forEach(id => document.getElementById(id).value = "");
  } catch {
    showAlert("registerMessage", "Error de conexión con el registro.", "danger");
  }
}

// ─────────────────────────────────────────
// CURSOS
// ─────────────────────────────────────────

function showCreateCourseForm() { document.getElementById("createCourseSection").classList.remove("d-none"); }
function hideCreateCourseForm() { document.getElementById("createCourseSection").classList.add("d-none"); clearAlert("courseMessage"); }
function clearCourses() { const c = document.getElementById("coursesContainer"); if (c) c.innerHTML = ""; }
function clearAssignments() {
  selectedCourse = null;
  document.getElementById("assignmentSection")?.classList.add("d-none");
  const c = document.getElementById("assignmentsContainer"); if (c) c.innerHTML = "";
}

async function createCourse() {
  clearAlert("courseMessage");
  const code = document.getElementById("courseCode").value.trim();
  const name = document.getElementById("courseName").value.trim();
  const description = document.getElementById("courseDescription").value.trim();
  if (!code || !name || !description) {
    showAlert("courseMessage", "Todos los campos del curso son obligatorios.", "warning");
    return;
  }
  try {
    const res = await requestBackend("api.php", {
      method: "POST",
      body: { action: "create_course", course_code: code, name_course: name, description }
    });
    if (!res.ok || res.data?.success === false) {
      showAlert("courseMessage", res.data?.message || "No se pudo crear el curso.", "danger");
      return;
    }
    ["courseCode","courseName","courseDescription"].forEach(id => document.getElementById(id).value = "");
    showAlert("courseMessage", "Curso creado correctamente.", "success");
    await loadCourses();
  } catch {
    showAlert("courseMessage", "Error de conexión al crear el curso.", "danger");
  }
}

async function loadCourses() {
  const container = document.getElementById("coursesContainer");
  container.innerHTML = `<div class="col-12"><div class="alert alert-secondary">Cargando cursos…</div></div>`;
  try {
    const res = await requestBackend("api.php", {
      method: "GET",
      params: { action: "get_all_courses", prof_id: professorId }
    });
    if (!res.ok) {
      container.innerHTML = `<div class="col-12"><div class="alert alert-danger">No se pudieron cargar los cursos.</div></div>`;
      return;
    }
    renderCourses(Array.isArray(res.data) ? res.data : []);
  } catch {
    container.innerHTML = `<div class="col-12"><div class="alert alert-danger">Error de conexión al cargar cursos.</div></div>`;
  }
}

function renderCourses(courses) {
  const container = document.getElementById("coursesContainer");
  container.innerHTML = "";
  if (!courses || courses.length === 0) {
    container.innerHTML = `<div class="col-12"><div class="alert alert-info">No hay cursos registrados todavía.</div></div>`;
    return;
  }
  courses.forEach((course, index) => {
    container.innerHTML += `
      <div class="col-md-4">
        <div class="course-card" onclick="openCourse(${index})">
          <div class="course-code">${escapeHtml(getCourseCode(course))}</div>
          <div class="course-name">${escapeHtml(getCourseName(course))}</div>
          <div class="course-description">${escapeHtml(getCourseDescription(course))}</div>
        </div>
      </div>`;
  });
  window.loadedCourses = courses;
}

async function openCourse(courseIndex) {
  const courses = window.loadedCourses || [];
  selectedCourse = courses[courseIndex];
  if (!selectedCourse) return;

  closeReviewPanel();
  document.getElementById("assignmentSection").classList.remove("d-none");
  document.getElementById("assignmentCourseTitle").textContent = "Asignar tarea — " + getCourseName(selectedCourse);
  resetAssignmentForm();
  await loadAssignments();
}

// ─────────────────────────────────────────
// TAREAS
// ─────────────────────────────────────────

async function createAssignment() {
  clearAlert("assignmentMessage");
  if (!selectedCourse) { showAlert("assignmentMessage", "Debe seleccionar un curso primero.", "warning"); return; }

  const name        = document.getElementById("assignmentName").value.trim();
  const description = document.getElementById("assignmentDescription").value.trim();
  const deadline    = document.getElementById("assignmentDeadline").value;
  const allowed     = document.getElementById("assignmentAllowed").checked;

  if (!name || !description || !deadline) {
    showAlert("assignmentMessage", "Todos los campos de la tarea son obligatorios.", "warning");
    return;
  }

  const action = editingAssignmentName ? "modify_assign" : "create_assign";
  try {
    const res = await requestBackend("api.php", {
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
    if (!res.ok || res.data?.success === false) {
      showAlert("assignmentMessage", res.data?.message || "No se pudo guardar la tarea.", "danger");
      return;
    }
    showAlert("assignmentMessage", editingAssignmentName ? "Tarea modificada correctamente." : "Tarea creada correctamente.", "success");
    resetAssignmentForm();
    await loadAssignments();
  } catch {
    showAlert("assignmentMessage", "Error de conexión al guardar la tarea.", "danger");
  }
}

async function loadAssignments() {
  const container = document.getElementById("assignmentsContainer");
  container.innerHTML = `<div class="alert alert-secondary">Cargando tareas…</div>`;
  if (!selectedCourse) return;
  try {
    const res = await requestBackend("api.php", {
      method: "GET",
      params: { action: "get_assign_by_course", code_course: getCourseCode(selectedCourse) }
    });
    if (!res.ok) {
      container.innerHTML = `<div class="alert alert-danger">No se pudieron cargar las tareas.</div>`;
      return;
    }
    const assignments = Array.isArray(res.data) ? res.data : [];
    window.loadedAssignments = assignments;
    renderAssignments(assignments);
  } catch {
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
  assignments.forEach((a, index) => {
    container.innerHTML += `
      <div class="assignment-card" id="acard-${index}">
        <div class="d-flex justify-content-between align-items-start">
          <div style="flex:1; cursor:pointer;" onclick="openReviewPanel(${index})">
            <div class="assignment-title">${escapeHtml(getAssignmentName(a))}</div>
            <div class="assignment-date">Fecha límite: ${formatDateDisplay(getAssignmentDeadline(a))}</div>
            <div class="assignment-submissions-count mt-1" id="acount-${index}">
              <span class="badge-submissions">Cargando entregas…</span>
            </div>
          </div>
          <div class="d-flex gap-2 ms-2">
            <button class="btn-icon-edit" onclick="editAssignment(${index})" title="Editar tarea">${ICON_EDIT}</button>
            <button class="btn btn-sm btn-review" onclick="openReviewPanel(${index})">Ver entregas</button>
          </div>
        </div>
      </div>`;
    // cargar conteo de entregas en paralelo
    loadSubmissionCount(index, a.id_assignment);
  });
  window.loadedAssignments = assignments;
}

async function loadSubmissionCount(index, id_assignment) {
  try {
    const res = await requestBackend("api.php", {
      method: "GET",
      params: { action: "get_all_submits", id_assign: id_assignment }
    });
    const el = document.getElementById(`acount-${index}`);
    if (!el) return;
    const subs = Array.isArray(res.data) ? res.data : [];
    const reviewed = subs.filter(s => s.grade !== null && s.grade !== undefined && s.grade !== "").length;
    const total    = subs.length;
    el.innerHTML = `<span class="badge-submissions">${total} entrega${total !== 1 ? "s" : ""} · ${reviewed} revisada${reviewed !== 1 ? "s" : ""}</span>`;
  } catch {
    // silencioso
  }
}

function editAssignment(index) {
  const assignment = (window.loadedAssignments || [])[index];
  if (!assignment) return;
  editingAssignmentName = getAssignmentName(assignment);
  document.getElementById("assignmentName").value = editingAssignmentName;
  document.getElementById("assignmentName").disabled = true;
  document.getElementById("assignmentDescription").value = getAssignmentDescription(assignment);
  document.getElementById("assignmentDeadline").value = formatDateTimeForInput(getAssignmentDeadline(assignment));
  document.getElementById("assignmentAllowed").checked = getAssignmentAllowed(assignment);
  document.getElementById("assignmentSubmitButton").textContent = "Guardar cambios";
  document.getElementById("cancelEditAssignmentButton").classList.remove("d-none");
  document.getElementById("assignmentName").scrollIntoView({ behavior: "smooth" });
}

function resetAssignmentForm() {
  editingAssignmentName = null;
  ["assignmentName","assignmentDescription","assignmentDeadline"].forEach(id => document.getElementById(id).value = "");
  document.getElementById("assignmentName").disabled = false;
  document.getElementById("assignmentAllowed").checked = false;
  document.getElementById("assignmentSubmitButton").textContent = "Guardar tarea";
  document.getElementById("cancelEditAssignmentButton").classList.add("d-none");
}

// ─────────────────────────────────────────
// PANEL DE REVISIÓN DE ENTREGAS
// ─────────────────────────────────────────

let currentReviewAssignment = null;
let currentReviewSubmissions = [];

function closeReviewPanel() {
  document.getElementById("reviewPanel").classList.add("d-none");
  document.getElementById("studentDetailPanel").classList.add("d-none");
  currentReviewAssignment = null;
  currentReviewSubmissions = [];
}

async function openReviewPanel(index) {
  const assignment = (window.loadedAssignments || [])[index];
  if (!assignment) return;

  currentReviewAssignment = assignment;
  document.getElementById("studentDetailPanel").classList.add("d-none");

  const panel = document.getElementById("reviewPanel");
  panel.classList.remove("d-none");

  document.getElementById("reviewAssignmentTitle").textContent = getAssignmentName(assignment);
  document.getElementById("reviewAssignmentDesc").textContent  = getAssignmentDescription(assignment);
  document.getElementById("reviewAssignmentDate").textContent  = "Fecha límite: " + formatDateDisplay(getAssignmentDeadline(assignment));
  document.getElementById("reviewStudentsList").innerHTML = `<div class="alert alert-secondary">Cargando entregas…</div>`;

  // scroll al panel
  panel.scrollIntoView({ behavior: "smooth", block: "start" });

  try {
    const res = await requestBackend("api.php", {
      method: "GET",
      params: { action: "get_all_submits", id_assign: assignment.id_assignment }
    });
    currentReviewSubmissions = Array.isArray(res.data) ? res.data : [];
    renderStudentsList(currentReviewSubmissions);
    // refrescar conteo en la tarjeta
    loadSubmissionCount(index, assignment.id_assignment);
  } catch {
    document.getElementById("reviewStudentsList").innerHTML = `<div class="alert alert-danger">Error al cargar entregas.</div>`;
  }
}

// Determina el estado de una entrega
function getSubmissionStatus(sub) {
  if (!sub || sub.project_name === undefined) return "no_entregada"; // no entregó
  if (sub.grade !== null && sub.grade !== undefined && sub.grade !== "") return "revisada";
  return "pendiente";
}

function statusBadge(status) {
  switch (status) {
    case "revisada":     return `<span class="status-badge status-revisada">Revisada</span>`;
    case "pendiente":    return `<span class="status-badge status-pendiente">Pendiente</span>`;
    case "no_entregada": return `<span class="status-badge status-no-entregada">No entregada</span>`;
    default:             return `<span class="status-badge status-pendiente">Pendiente</span>`;
  }
}

function renderStudentsList(submissions) {
  const list = document.getElementById("reviewStudentsList");
  list.innerHTML = "";

  if (!submissions || submissions.length === 0) {
    list.innerHTML = `<div class="alert alert-info">No hay entregas registradas para esta tarea.</div>`;
    return;
  }

  // ordenar: revisadas al final, no entregadas al principio
  const order = { no_entregada: 0, pendiente: 1, revisada: 2 };
  const sorted = [...submissions].sort((a, b) => order[getSubmissionStatus(a)] - order[getSubmissionStatus(b)]);

  sorted.forEach((sub, i) => {
    const status = getSubmissionStatus(sub);
    const name   = escapeHtml((sub.name_user ?? "") + " " + (sub.lastname_user ?? "")).trim() || "Estudiante";
    const gradeText = (status === "revisada") ? `${sub.grade} pts` : "";
    const originalIndex = submissions.indexOf(sub);

    list.innerHTML += `
      <div class="student-review-card status-border-${status}" onclick="openStudentDetail(${originalIndex})">
        <div class="student-review-info">
          <span class="student-review-name">${name}</span>
          ${gradeText ? `<span class="student-review-grade">${escapeHtml(gradeText)}</span>` : ""}
        </div>
        <div>${statusBadge(status)}</div>
      </div>`;
  });
}

// ─────────────────────────────────────────
// DETALLE DE ESTUDIANTE / CALIFICAR
// ─────────────────────────────────────────

function openStudentDetail(subIndex) {
  const sub = currentReviewSubmissions[subIndex];
  if (!sub) return;

  const status = getSubmissionStatus(sub);
  const name   = ((sub.name_user ?? "") + " " + (sub.lastname_user ?? "")).trim() || "Estudiante";

  const panel = document.getElementById("studentDetailPanel");
  panel.classList.remove("d-none");
  panel.scrollIntoView({ behavior: "smooth", block: "start" });

  document.getElementById("detailStudentName").textContent = name;
  document.getElementById("detailStatusBadge").innerHTML   = statusBadge(status);

  const bodyEl = document.getElementById("detailBody");

  if (status === "no_entregada") {
    bodyEl.innerHTML = `<div class="alert alert-secondary mt-3">Este estudiante no ha realizado ninguna entrega.</div>`;
    return;
  }

  let decodedCode = "";
  if (sub.project_data) {
    try { decodedCode = atob(sub.project_data); } catch { decodedCode = sub.project_data; }
  }

  const fileSection = sub.project_name
    ? `<div class="detail-row">
         <span class="detail-label">Archivo:</span>
         <span class="detail-value">${escapeHtml(sub.project_name)}</span>
       </div>
       <div class="student-code-viewer mt-3">
         <div class="student-code-header">
           <span class="student-code-label">&#128196; Código del estudiante</span>
           <button class="btn-run-student" id="runStudentBtn" onclick="runStudentCode(${subIndex})">&#9654; Ejecutar</button>
         </div>
         <textarea class="student-code-editor" readonly id="studentCodeBox">${escapeHtml(decodedCode)}</textarea>
         <div class="student-code-output" id="studentOutput">La salida aparecerá aquí...</div>
       </div>`
    : `<div class="text-muted fst-italic mt-2">Sin archivo adjunto</div>`;

  const submittedAt = sub.submitted_at ? `<div class="detail-row">
    <span class="detail-label">Entregado:</span>
    <span class="detail-value">${formatDateDisplay(sub.submitted_at)}</span>
  </div>` : "";

  bodyEl.innerHTML = `
    ${submittedAt}
    ${fileSection}

    <div class="grade-form mt-4">
      <div class="mb-3">
        <label class="form-label">Calificación</label>
        <input type="number" id="detailGrade" class="form-control grade-input"
               min="0" max="100" step="0.5"
               value="${escapeHtml(String(sub.grade ?? ""))}">
      </div>
      <div class="mb-3">
        <label class="form-label">Comentarios</label>
        <textarea id="detailFeedback" class="form-control" rows="3"
                  placeholder="Escribe retroalimentación…">${escapeHtml(sub.feedback ?? "")}</textarea>
      </div>
      <div id="detailMessage" class="mb-2"></div>
      <button class="btn btn-success w-100" onclick="saveGrade(${subIndex})">
        Guardar evaluación
      </button>
    </div>`;
}

async function saveGrade(subIndex) {
  const sub      = currentReviewSubmissions[subIndex];
  const grade    = parseFloat(document.getElementById("detailGrade").value);
  const feedback = document.getElementById("detailFeedback").value.trim();

  if (isNaN(grade) || grade < 0 || grade > 100) {
    showAlert("detailMessage", "La calificación debe ser un número entre 0 y 100.", "warning");
    return;
  }

  try {
    const res = await requestBackend("api.php", {
      method: "POST",
      body: {
        action:    "grade_submission",
        id_stud:   sub.id_student,
        id_assign: currentReviewAssignment.id_assignment,
        grade,
        feedback
      }
    });
    if (!res.ok || res.data?.success === false) {
      showAlert("detailMessage", res.data?.message || "No se pudo guardar la calificación.", "danger");
      return;
    }
    // actualizar localmente
    currentReviewSubmissions[subIndex].grade    = grade;
    currentReviewSubmissions[subIndex].feedback = feedback;
    showAlert("detailMessage", "Evaluación guardada correctamente.", "success");
    renderStudentsList(currentReviewSubmissions);
  } catch {
    showAlert("detailMessage", "Error de conexión al guardar.", "danger");
  }
}

function downloadSubmission(subIndex) {
  const sub = currentReviewSubmissions[subIndex];
  if (!sub || !sub.project_data) {
    alert("No hay archivo disponible para descargar.");
    return;
  }
  // Si el backend devuelve base64, decodificar y descargar
  try {
    const byteStr = atob(sub.project_data);
    const ab = new ArrayBuffer(byteStr.length);
    const ia = new Uint8Array(ab);
    for (let i = 0; i < byteStr.length; i++) ia[i] = byteStr.charCodeAt(i);
    const blob = new Blob([ab], { type: "application/octet-stream" });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement("a");
    a.href = url; a.download = sub.project_name || "archivo";
    document.body.appendChild(a); a.click();
    document.body.removeChild(a); URL.revokeObjectURL(url);
  } catch {
    alert("No se pudo procesar el archivo. Verifique que el backend devuelva project_data en base64.");
  }
}

// ─────────────────────────────────────────
// EJECUTOR DE PYTHON (nice-to-have)
// ─────────────────────────────────────────

async function runPythonCode() {
  const code   = document.getElementById("pythonCode").value;
  const output = document.getElementById("pythonOutput");
  if (!code.trim()) return;

  output.textContent = "Ejecutando…";
  const filename = "temp_" + Date.now() + ".py";

  try {
    const createRes = await requestBackend("api.php", {
      method: "GET",
      params: { action: "create_temp_file", name: filename, data: code }
    });
    if (!createRes.data) { output.textContent = "Error: no se pudo crear el archivo."; return; }

    const execRes = await requestBackend("api.php", {
      method: "GET",
      params: { action: "execute_temp_file", name: filename }
    });
    output.textContent = execRes.data ?? "(sin salida)";

    await requestBackend("api.php", {
      method: "GET",
      params: { action: "delete_temp_file", name: filename }
    });
  } catch (e) {
    output.textContent = "Error: " + e.message;
  }
}

// ─────────────────────────────────────────
// EJECUTAR CÓDIGO DEL ESTUDIANTE
// ─────────────────────────────────────────

async function runStudentCode(subIndex) {
    const sub = currentReviewSubmissions[subIndex];
    console.log(currentReviewSubmissions);
    console.log(sub);
    const code = sub["project_blob"]; 

    console.log(code);

    if (!code) {
        alert("No se encontró código en esta entrega.");
        return;
    }

    const outputEl = document.getElementById("pythonOutput");
    const btn = document.getElementById("runStudentBtn");

    try {
        const cleanCode = code.replace(/\\"/g, '"').replace(/\\'/g, "'");

        const filename = "stud_" + (Date.now()) + ".py";

        const createRes = await fetch("api.php?action=create_temp_file_post&name=" + encodeURIComponent(filename), {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: "data=" + encodeURIComponent(cleanCode)
        });

        const createResult = await createRes.text();
        
        const execRes = await requestBackend("api.php", {
            method: "GET",
            params: { action: "execute_temp_file", name: filename }
        });

        if (outputEl) {
            outputEl.textContent = execRes.data ?? "(sin salida o error)";
        }

    } catch (e) {
        console.error("Error al ejecutar:", e);
        if (outputEl) outputEl.textContent = "Error: " + e.message;
    }
}