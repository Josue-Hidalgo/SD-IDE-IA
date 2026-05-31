

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


?>
