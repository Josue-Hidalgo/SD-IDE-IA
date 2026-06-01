

<?php
include 'assignment_model.php';
include 'db_controller.php';

function create_assign(string $code_course, string $assign_name, string $desc, string $deadline, bool $is_allowed){
	return create_assignment($code_course, $assign_name, $desc, $deadline, $is_allowed);
}

function get_all_assignment_by_course(string $code_course){
	return get_assignments_by_course($code_course);
}

function modify_assignment(string $assign_name, string $code_course, string $desc, string $deadline, bool $is_allowed){
	return modify_assign($assign_name, $code_course, $desc, $deadline, $is_allowed);
}

if($_GET['code_course']){
	$code = $_GET['code_course'];
	$data = get_all_assignment_by_course($code);
	header('Content-type: application/json');
	echo json_encode($data);
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
	$json = file_get_contents('php://input');
	$data = json_decode($json);
	header('Content-type: application/json; charset=utf-8');
	switch($data->action){
		case 'create':
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
		case 'modify':
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
