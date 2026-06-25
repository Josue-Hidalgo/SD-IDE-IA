<?php 
 function create_db_conn(){// sudo apt-get install php8.4-mysql
 	$servername = "localhost";
	$username = "root";//"AcademycIDEIA";//
	$password = "Qwertys123.";//"#IDEIA#";//
	$dbname = "prueba";//"IDEA";//

	$conn =  new mysqli($servername, $username, $password, $dbname);

	if ($conn->connect_error) {
		die("connection failed: ".mysqli_connect_error());
	}
	return $conn;
 }

 //crear profesor

 function create_prof(string $email, string $password, string $name_user, string $lastname_user){
 	$conn = create_db_conn();

 	$sql = "CALL create_user(\"$email\", \"$password\", \"$name_user\", \"$lastname_user\")";
 	if ($conn->query($sql) === TRUE) {
 		if ($conn->query("CALL create_professor(LAST_INSERT_ID())") === TRUE) {
 			$conn->close();
 			return TRUE;
 		}else{
 			$conn->close();
 			return FALSE;
 		}
 	}else{
 		$conn->close();
 		return FALSE;
 		}
 }

 function create_stud(string $email, string $password, string $name_user, string $lastname_user){
 	$conn = create_db_conn();

 	$sql = "CALL create_user(\"$email\", \"$password\", \"$name_user\", \"$lastname_user\")";
 	if ($conn->query($sql) === TRUE) {
 		if ($conn->query("CALL create_student(LAST_INSERT_ID())") === TRUE) {
 			$conn->close();
 			return TRUE;
 		}else{
 			$conn->close();
 			return FALSE;
 		}
 	}else{
 		$conn->close();
 		return FALSE;
 		}
 	
 }


 function create_course(string $code_course, int $prof_id, string $name_course, string $desc){
 	$conn = create_db_conn();

 	$sql = "CALL create_course(\"$code_course\", $prof_id, \"$name_course\", \"$desc\")";
 	if ($conn->query($sql) === TRUE) {
 		$conn->close();
 		return TRUE;
 	}else{
 		$conn->close();
 		return FALSE;
 		}
 }

function create_assignment(string $code_course, string $assi_name, string $desc,string $deadline, bool $is_allowed){
	$conn = create_db_conn();

 	$sql = "CALL create_assignment(\"$code_course\", \"$assi_name\", \"$desc\", \"$deadline\", ".((int)$is_allowed).")";

 	if ($conn->query($sql) === TRUE) {
 		$conn->close();
 		return TRUE;
 	}else{
 		$conn->close();
 		return FALSE;
 		}
 
}

function login_user_web(string $email, string $password){
	$conn = create_db_conn();

 	$sql = "CALL login_user(\"$email\", \"$password\")";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$row = $result->fetch_assoc();
 		$prof_data = [];
 		$prof_data["name"] = $row["name_user"];
 		$prof_data["lastname"] = $row["lastname_user"];
 		$prof_data["email"] = $row["email_user"];
 		$prof_data["password"] = $row["password_user"];
 		$id_user = $row["id_user"];
 		while($conn->more_results()){
 			$conn->next_result();
 			$conn->use_result();
 		}
 		$result2 = $conn->query("SELECT id_professor from Professor where id_user = $id_user");

 		$row2 = $result2->fetch_assoc();
 		$prof_data["prof_id"] = $row2["id_professor"];
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $prof_data;
}

function login_user_desk(string $email, string $password){
	$conn = create_db_conn();

 	$sql = "CALL login_user(\"$email\", \"$password\")";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$row = $result->fetch_assoc();
 		$stud_data = [];
 		$stud_data["name"] = $row["name_user"];
 		$stud_data["lastname"] = $row["lastname_user"];
 		$stud_data["email"] = $row["email_user"];
 		$stud_data["password"] = $row["password_user"];
 		$id_user = $row["id_user"];
 		while($conn->more_results()){
 			$conn->next_result();
 			$conn->use_result();
 		}
 		$result2 = $conn->query("SELECT id_student from Student where id_user = $id_user");

 		$row2 = $result2->fetch_assoc();
 		$stud_data["stud_id"] = $row2["id_student"];
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $stud_data;
}

function check_email(string $email){
	$conn = create_db_conn();

 	$sql = "SELECT name_user from User where email_user = \"$email\"";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$conn->close();
 		return TRUE;
 	}else{
 		$conn->close();
 		return False;
 	}
 	
}

function check_course(string $course_code){
	$conn = create_db_conn();

 	$sql = "SELECT name_course from Course where code_course = \"$course_code\"";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$conn->close();
 		return TRUE;
 	}else{
 		$conn->close();
 		return FALSE;
 	}
}

function get_all_prof_courses(int $prof_id){
	$conn = create_db_conn();

 	$sql = "SELECT * from Course where id_professor = $prof_id";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$courses = [];
 		$count = 0;
 		while($row = $result->fetch_assoc()){
 			$courses[$count] = $row;
 			$count = $count+1;
 		}
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $courses;
}

function get_all_stud_courses(int $stud_id){
	$conn = create_db_conn();

 	$sql = "CALL get_student_courses($stud_id)";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$courses = [];
 		$count = 0;
 		while($row = $result->fetch_assoc()){
 			$courses[$count] = $row;
 			$count = $count+1;
 		}
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $courses;
}

function get_assignments_by_course(string $code_course){
	$conn = create_db_conn();

 	$sql = "CALL get_assignments_by_course(\"$code_course\")";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$assigments = [];
 		$count = 0;
 		while($row = $result->fetch_assoc()){
 			$assigments[$count] = $row;
 			$count = $count+1;
 		}
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $assigments;
}

function  modify_assign(string $assign_name, string $code_course, string $desc, string $deadline, bool $is_allowed){
	$conn = create_db_conn();

	$sql = "UPDATE Assignment SET description_assignment = \"$desc\", deadline = \"$deadline\", is_allowed_after_deadline = ".((int)$is_allowed)." where code_course = \"$code_course\" and name_assignment = \"$assign_name\"";

	if ($conn->query($sql) === TRUE) {
 		$conn->close();
 		return TRUE;
 	}else{
 		$conn->close();
 		return FALSE;
 	}

}

function enroll_stud(int $id_stud, string $course_code){
	$conn = create_db_conn();

 	$sql = "CALL enroll_student($id_stud,\"$course_code\")";
 	if ($conn->query($sql) === TRUE) {
 		$conn->close();
 		return TRUE;
 	}else{
 		$conn->close();
 		return FALSE;
 	}
}

function get_all_stud_mail_in_course(string $code_course){
	$conn = create_db_conn();


	$sql = "SELECT User.email_user FROM Enrollment INNER JOIN Student ON Enrollment.id_student = Student.id_student INNER JOIN User ON Student.id_user = User.id_user WHERE Enrollment.code_course = \"$code_course\"";
	$result = $conn->query($sql);

	if ($result->num_rows >0) {
 		$student_mail = [];
 		while($row = $result->fetch_assoc()){
 			$student_mail[] = $row;
 		}
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $student_mail;
}

function get_stud_mail(int $id_stud){
	$conn = create_db_conn();


	$sql = "SELECT User.email_user FROM Student INNER JOIN User ON Student.id_user = User.id_user WHERE Student.id_user = $id_stud";
	$result = $conn->query($sql);

	if ($result->num_rows >0) {
		$row = $result->fetch_assoc();
 		$student_mail = $row["email_user"];
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $student_mail;
}

function get_stud_info(int $id_stud){
	$conn = create_db_conn();

	$sql = "SELECT User.name_user, User.lastname_user FROM Student INNER JOIN User ON Student.id_user = User.id_user WHERE Student.id_user = $id_stud";
	$result = $conn->query($sql);

	if ($result->num_rows >0) {
		$stud_info = [];
		$row = $result->fetch_assoc();
 		$stud_info["name"] = $row["name_user"];
 		$stud_info["lastname"] = $row["lastname_user"];
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $stud_info;
}

function get_assignment_name(int $id_assign){
	$conn = create_db_conn();

 	$sql = "SELECT name_assignment from Assignment where id_assignment = $id_assign";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$row = $result->fetch_assoc();
 		$name = $row["name_assignment"];
 		$conn->close();
 		return $name;
 	}else{
 		$conn->close();
 		return FALSE;
 	}
}

function create_submit(int $id_stud, int $id_assign, string $project_name, string $project_data){
	$conn = create_db_conn();

	$stmt= $conn->prepare("CALL create_submission(?,?,?,?)");
	$stmt->bind_param("iiss", $id_stud, $id_assign, $project_name, $project_data);

	if ($stmt->execute()) {
		$stmt->close();
		$conn->close();
		return true;
	} else {
		$stmt->close();
		$conn->close();
		return false;
	}
}

function grade_submit(int $id_stud, int $id_assign, float $grade, string $feedback){
	$conn = create_db_conn();

	$escaped_feedback = $conn->real_escape_string($feedback);

	$sql = "CALL grade_submission($id_stud,$id_assign,$grade,\"$escaped_feedback\")";

 	if ($conn->query($sql) === TRUE) {
 		$conn->close();
 		return TRUE;
 	}else{
 		$conn->close();
 		return FALSE;
 	}
}

function get_submit_by_assign(int $id_assign){
	$conn = create_db_conn();

 	$sql = "CALL get_submissions_by_assignment($id_assign)";
 	$result = $conn->query($sql);
 	if ($result->num_rows >0) {
 		$submissions = [];
 		$count = 0;
 		while($row = $result->fetch_assoc()){
 			$submissions[$count] = $row;
 			$count = $count+1;
 		}
 	}else{
 		$conn->close();
 		return FALSE;
 	}
 	$conn->close();
 	return $submissions;
}

function get_assign_grade(int $id_stud, int $id_assign){
	$conn = create_db_conn();

 	$sql = "SELECT grade, project_name, feedback from Submission where id_student = $id_stud and id_assignment = $id_assign";
 	$result = $conn->query($sql);
 	$sub_data = [];
 	if ($result->num_rows >0) {
 		$row = $result->fetch_assoc();
 		$sub_data['grade'] = $row["grade"];
 		$sub_data['name'] = $row["project_name"];
 		$sub_data['feedback'] = $row["feedback"];
 		$conn->close();
 		return $sub_data;
 	}else{
 		$conn->close();
 		return 0;
 	}
}

?>