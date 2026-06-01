<?php session_start(); ?>

<?php

include 'course_model.php';
include 'db_controller.php';

function create_course_c(string $code_course, string $name_course, string $desc){
	if(!check_course($code_course)){
		$prof_id = $_SESSION['prof_id'];
		create_course($code_course,$prof_id,$name_course,$desc);
		return TRUE;
	}else{
		return FALSE;
	}
}

function get_all_courses(int $prof_id){ 
	return get_all_prof_courses($prof_id);
}

if($_GET['prof_id']){
	$id = $_GET['prof_id'];
	$data = get_all_courses($id);
	header('Content-type: application/json');
	echo json_encode($data);
}


if ($_SERVER['REQUEST_METHOD'] === 'POST') {
	$json = file_get_contents('php://input');
	$data = json_decode($json);
	header('Content-type: application/json; charset=utf-8');
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
}

?>
