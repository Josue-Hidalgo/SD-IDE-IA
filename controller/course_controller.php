<?php session_start(); ?>

<?php

include 'course_model.php';
include 'db_controller.php';

function create_course_c(string $code_course, string $name_course, string $desc){
	if(!check_course($code_course)){
		$prof_id = $_SESSION['prof_id'];
		create_course($code_course,$prof_id,$name_course,$desc);
	}else{
		echo "Course already exist.";
	}
}

function get_all_courses(int $prof_id){
	$courses = get_all_prof_courses($prof_id);
	//cambiar esto despues
	$size = count($courses);
	for($i = 0;$i < $size; $i++){
		print_r($courses[$i])."<br>";
	}
	//return $courses;
}

//get_all_courses(7);

?>
