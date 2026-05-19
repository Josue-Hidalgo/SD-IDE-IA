let selectedCourse = null;

/*
  OJOOOOOOOOOOO!!!!!!!!!!!!!:
  Esta variable es solo para que el frontend pueda probarse sin backend.
  Cuando el backend esté conectado, estas listas se pueden eliminar.
*/

let localCourses = [];
let localAssignments = [];

function showAlert(containerId, message, type) {
  document.getElementById(containerId).innerHTML = `
    <div class="alert alert-${type}">
      ${message}
    </div>
  `;
}

function clearAlert(containerId) {
  document.getElementById(containerId).innerHTML = "";
}

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

  document.getElementById("loggedUserName").textContent = name;

  renderCourses();
}

function logout() {
  selectedCourse = null;

  document.getElementById("navbarUser").classList.add("d-none");
  document.getElementById("loginEmail").value = "";
  document.getElementById("loginPassword").value = "";

  showLogin();
}

function login() {
  clearAlert("loginMessage");

  const email = document.getElementById("loginEmail").value.trim();
  const password = document.getElementById("loginPassword").value.trim();

  if (email === "" || password === "") {
    showAlert("loginMessage", "Debe ingresar correo y contraseña.", "warning");
    return;
  }

  /*
    FRONTEND ÚNICAMENTE:
    Aquí solo validamos visualmente el formulario.

    Cuando el backend esté listo, esta función se cambia para usar fetch,
    pero esa parte depende del backend.
  */

  showDashboard(email);
}

function register() {
  clearAlert("registerMessage");

  const name = document.getElementById("registerName").value.trim();
  const lastname = document.getElementById("registerLastname").value.trim();
  const email = document.getElementById("registerEmail").value.trim();
  const password = document.getElementById("registerPassword").value.trim();

  if (name === "" || lastname === "" || email === "" || password === "") {
    showAlert("registerMessage", "Todos los campos son obligatorios.", "warning");
    return;
  }

  showAlert(
    "registerMessage",
    "Registro validado desde el frontend. Pendiente conexión con backend.",
    "success"
  );
}

function showCreateCourseForm() {
  document.getElementById("createCourseSection").classList.remove("d-none");
}

function hideCreateCourseForm() {
  document.getElementById("createCourseSection").classList.add("d-none");
}

function createCourse() {
  clearAlert("courseMessage");

  const code = document.getElementById("courseCode").value.trim();
  const name = document.getElementById("courseName").value.trim();
  const description = document.getElementById("courseDescription").value.trim();

  if (code === "" || name === "" || description === "") {
    showAlert("courseMessage", "Todos los campos del curso son obligatorios.", "warning");
    return;
  }

  const course = {
    code: code,
    name: name,
    description: description
  };

  localCourses.push(course);

  document.getElementById("courseCode").value = "";
  document.getElementById("courseName").value = "";
  document.getElementById("courseDescription").value = "";

  showAlert("courseMessage", "Curso agregado visualmente en el frontend.", "success");

  renderCourses();
}

function renderCourses() {
  const container = document.getElementById("coursesContainer");

  container.innerHTML = "";

  if (localCourses.length === 0) {
    container.innerHTML = `
      <div class="col-12">
        <div class="alert alert-info">
          No hay cursos registrados todavía.
        </div>
      </div>
    `;
    return;
  }

  localCourses.forEach(course => {
    container.innerHTML += `
      <div class="col-md-4">
        <div class="course-card" onclick="openCourse('${course.code}')">
          <div class="course-code">${course.code}</div>
          <div class="course-name">${course.name}</div>
          <div class="course-description">${course.description}</div>
        </div>
      </div>
    `;
  });
}

function openCourse(courseCode) {
  selectedCourse = localCourses.find(course => course.code === courseCode);

  document.getElementById("assignmentSection").classList.remove("d-none");
  document.getElementById("assignmentCourseTitle").textContent =
    "Asignar tarea - " + selectedCourse.name;

  renderAssignments();
}

function createAssignment() {
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

  const assignment = {
    courseCode: selectedCourse.code,
    name: name,
    description: description,
    deadline: deadline,
    allowed: allowed
  };

  localAssignments.push(assignment);

  document.getElementById("assignmentName").value = "";
  document.getElementById("assignmentDescription").value = "";
  document.getElementById("assignmentDeadline").value = "";
  document.getElementById("assignmentAllowed").checked = false;

  showAlert("assignmentMessage", "Tarea agregada visualmente en el frontend.", "success");

  renderAssignments();
}

function renderAssignments() {
  const container = document.getElementById("assignmentsContainer");

  container.innerHTML = "";

  const courseAssignments = localAssignments.filter(
    assignment => assignment.courseCode === selectedCourse.code
  );

  if (courseAssignments.length === 0) {
    container.innerHTML = `
      <div class="alert alert-info">
        Este curso todavía no tiene tareas.
      </div>
    `;
    return;
  }

  courseAssignments.forEach(assignment => {
    container.innerHTML += `
      <div class="assignment-card">
        <div class="assignment-title">${assignment.name}</div>
        <p>${assignment.description}</p>
        <div class="assignment-date">
          Fecha límite: ${assignment.deadline}
        </div>
        <div>
          Entrega tardía: ${assignment.allowed ? "Permitida" : "No permitida"}
        </div>
      </div>
    `;
  });
}