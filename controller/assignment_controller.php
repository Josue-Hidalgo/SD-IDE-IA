<!DOCTYPE html>
<html>
<head>
    <title>assignment controller</title>
</head>
<body>
	<?php
		include 'assignment_model.php';
		$DataArray = ['id_assignment' => 02,'name_assignment' => "tarea final",// array para pruebas
		'description_assignment'=>"para ayer", 'deadline'=>date('l jS \of F Y h:i:s A'), 
		'is_allowed_after_deadline'=>true
	];

	$assignmentTest = new Assignment(...$DataArray);

	function getAssignmentName(int $assignment_id){
		//cambiar luego con la logica de mysql
		echo "id de la asignacion: $assignment_id".'<br>';
		global $assignmentTest;
		return $assignmentTest->name_assignment;
	}
	function setAssignmentName(int $assignment_id, string $nAssignmentName){
		//cambiar luego con la logica de mysql
		echo "id del asignacion: $assignment_id y nombre: $nAssignmentName".'<br>';
		global $assignmentTest;
		$assignmentTest->name_assignment = $nAssignmentName;
		echo "nuevo nombre del asignacion: $assignmentTest->name_assignment".'<br>';
	}

	function getAssignmentDesc(int $assignment_id){
		//cambiar luego con la logica de mysql
		echo "id de la asignacion: $assignment_id".'<br>';
		global $assignmentTest;
		return $assignmentTest->description_assignment;
	}
	function setAssignmentDesc(int $assignment_id, string $nAssignmentDesc){
		//cambiar luego con la logica de mysql
		echo "id del asignacion: $assignment_id y descripcion: $nAssignmentDesc".'<br>';
		global $assignmentTest;
		$assignmentTest->description_assignment = $nAssignmentDesc;
		echo "nuevo desc del asignacion: $assignmentTest->description_assignment".'<br>';
	}

	function getAssignmentDeadline(int $assignment_id){
		//cambiar luego con la logica de mysql
		echo "id de la asignacion: $assignment_id".'<br>';
		global $assignmentTest;
		return $assignmentTest->deadline;
	}
	function setAssignmentDeadline(int $assignment_id, string $nAssignmentDeadline){
		//cambiar luego con la logica de mysql
		echo "id del asignacion: $assignment_id y deadline: $nAssignmentDeadline".'<br>';
		global $assignmentTest;
		$assignmentTest->deadline = $nAssignmentDeadline;
		echo "nuevo deadline del asignacion: $assignmentTest->deadline".'<br>';
	}

	function getAssignmentIsAllowed(int $assignment_id){
		//cambiar luego con la logica de mysql
		echo "id de la asignacion: $assignment_id".'<br>';
		global $assignmentTest;
		return $assignmentTest->is_allowed_after_deadline;
	}
	function setAssignmentIsAllowed(int $assignment_id, bool $nIsAllowed){
		//cambiar luego con la logica de mysql
		echo "id del asignacion: $assignment_id y IsAllowed: $nIsAllowed".'<br>';
		global $assignmentTest;
		$assignmentTest->is_allowed_after_deadline = $nIsAllowed;
		echo "nuevo bool del asignacion: $assignmentTest->is_allowed_after_deadline".'<br>';
	}


	echo getAssignmentName(03);
	echo "<br>";
	setAssignmentName(03, "otra tarea");
	echo getAssignmentDesc(03);
	echo "<br>";
	setAssignmentDesc(03, "ya no hay tiempo");
	echo getAssignmentDeadline(03);
	echo "<br>";
	setAssignmentDeadline(03, date('l'));
	echo getAssignmentIsAllowed(03);
	echo "<br>";
	setAssignmentIsAllowed(03, false);
	
	?>
</body>
</html>