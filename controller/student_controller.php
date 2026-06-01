<?php
include 'db_controller.php';

function enroll_student(int $id_stud, string $course_code){
	$value = enroll_stud($id_stud, $course_code);
	if ($value) {
		return $value;
	}else {
		return FALSE;
	}
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
	$json = file_get_contents('php://input');
	$data = json_decode($json);
	header('Content-type: application/json; charset=utf-8');
	$st_id = $data->id_stud;
	$cr_code = $data->course_code;

	$success = enroll_student($st_id, $cr_code);
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