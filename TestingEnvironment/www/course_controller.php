<?php

include_once 'course_model.php';
include_once 'db_controller.php';

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

