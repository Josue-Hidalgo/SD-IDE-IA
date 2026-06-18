<?php
include_once 'db_controller.php';

function enroll_student(int $id_stud, string $course_code){
	$value = enroll_stud($id_stud, $course_code);
	if ($value) {
		return $value;
	}else {
		return FALSE;
	}
}

function get_enroll_courses(int $id_stud){
	return get_all_stud_courses($id_stud);
}
