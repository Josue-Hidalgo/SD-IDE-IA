<?php //session_start()?>

<?php
include 'assignment_model.php';
include 'db_controller.php';

function create_assign(string $code_course, string $assign_name, string $desc, string $deadline, bool $is_allowed){
	create_assignment($code_course, $assign_name, $desc, $is_allowed);//agregar la fecha despues
}

function get_all_assignment_by_course(string $code_course){
	$assignments = get_assignments_by_course($code_course);
	//cambiar esto despues
	$size = count($assignments);
	for($i = 0;$i < $size; $i++){
		print_r($assignments[$i])."<br>";
	}
	//return $assinments;
}

?>
