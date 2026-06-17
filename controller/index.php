<?php
session_start();
?>
<?php  
include 'professor_controller.php';
include 'student_controller.php';
include 'course_controller.php';
include 'assignment_controller.php';
include 'WLogin_WRegister.php';

if($_GET['action']){
	$action = $_GET['action'];
	switch($action){
		case 'get_id':
			$data = getProfId();
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'get_name':
			$data = getProfName();
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'get_LName':
			$data = getProfLName();
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'get_email':
			$data = getProfEmail();
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'log_student':
			$email = $_GET['email'];
			$password = $_GET['password'];
			$data = Login_desk($email,$password);
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'log_prof':
			$email = $_GET['email'];
			$password = $_GET['password'];
			$data = Login_web($email,$password);
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'get_enroll_courses':
			$id = $_GET['id_stud'];
			$data = get_enroll_courses($id);
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'get_all_courses':
			$id = $_GET['prof_id'];
			$data = get_all_courses($id);
			header('Content-type: application/json');
			echo json_encode($data);
			break;

		case 'get_assign_by_course':
			$code = $_GET['code_course'];
			$data = get_all_assignment_by_course($code);
			header('Content-type: application/json');
			echo json_encode($data);
			break;
	}
	
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
	$json = file_get_contents('php://input');
	$data = json_decode($json);
	header('Content-type: application/json; charset=utf-8');
	switch($data->action){
		case 'create_student':
			$st_name = $data->username;
			$st_email =$data->email;
			$st_pass =$data->password;
			$st_last = $data->userLast;
			$success = Register_stud($st_email, $st_pass, $st_name, $st_last);
			if ($success) {
				http_response_code(201);
				$responseData =[
					'success' => true,
					'message' => 'Data received successfully',
				];
				echo json_encode($responseData);
			} else {
				http_response_code(400);
				$responseData =[
					'success' => FALSE,
					'message' => 'User already exist.',
				];
				echo json_encode($responseData);

			}
			
			
			break;

		case 'create_prof':
			$prof_name = $data->username;
			$prof_email =$data->email;
			$prof_pass =$data->password;
			$prof_last = $data->userLast;
			$success = Register_prof($prof_email, $prof_pass, $prof_name, $prof_last);
			if ($success) {
				http_response_code(201);
				$responseData =[
					'success' => true,
					'message' => 'Data received successfully',
				];
				echo json_encode($responseData);
			} else {
				http_response_code(400);
				$responseData =[
					'success' => FALSE,
					'message' => 'User already exist.',
				];
				echo json_encode($responseData);

			}
			break;

		case 'enroll_student':
			$st_id = $data->id_stud;
			$cr_code = $data->course_code;

			$success = enroll_student($st_id, $cr_code);
			if ($success) {
				http_response_code(201);
				$responseData =[
					'success' => true,
					'message' => 'Data received successfully',
				];
				echo json_encode($responseData);
			} else {
				http_response_code(400);
				$responseData =[
					'success' => FALSE,
					'message' => 'User already exist.',
				];
				echo json_encode($responseData);
			}
			break;

		case 'create_course':
			$cr_code = $data->course_code;
			$name = $data->name_course;
			$desc = $data->description;

			$success = create_course_c($cr_code, $name, $desc);
			if ($success) {
				http_response_code(201);
				$responseData =[
					'success' => true,
					'message' => 'Date received successfully',
				];
				echo json_encode($responseData);
			} else {
				http_response_code(400);
				$responseData =[
					'success' => FALSE,
					'message' => 'User already exist.',
				];
				echo json_encode($responseData);
			}
			break;

		case 'create_assign':
			$as_name = $data->assign_name;
			$cr_code =$data->course_code;
			$as_desc =$data->assign_desc;
			$as_deadline = $data->assign_deadline;
			$as_is_allowed = $data->allowed;
			$success = create_assignment($cr_code, $as_name, $as_desc, $as_deadline, $as_is_allowed);
			if ($success) {
				http_response_code(201);
				$responseData =[
					'success' => true,
					'message' => 'Date received successfully',
				];
				echo json_encode($responseData);
			} else {
				http_response_code(400);
				$responseData =[
					'success' => FALSE,
					'message' => 'User already exist.',
				];
				echo json_encode($responseData);

			}
			
			
			break;
		case 'modify_assign':
			$as_name = $data->assign_name;
			$cr_code =$data->course_code;
			$as_desc =$data->assign_desc;
			$as_deadline = $data->assign_deadline;
			$as_is_allowed = $data->allowed;
			$success = modify_assignment($as_name, $cr_code, $as_desc, $as_deadline, $as_is_allowed);
			if ($success) {
				http_response_code(201);
				$responseData =[
					'success' => true,
					'message' => 'Date received successfully',
				];
				echo json_encode($responseData);
			} else {
				http_response_code(400);
				$responseData =[
					'success' => FALSE,
					'message' => 'User already exist.',
				];
				echo json_encode($responseData);

			}
			break;
	}
}

?>