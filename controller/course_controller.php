<!DOCTYPE html>
<html>
<head>
    <title>course controller</title>
</head>
<body>
	<?php
		include 'course_model.php';
		$DataArray = ['id_course' => 03,'name_course' => "Diseno"// array para pruebas
	];

	$courseTest = new Course(...$DataArray);

	function getCourseName(int $course_id){
		//cambiar luego con la logica de mysql
		echo "id del curso: $course_id".'<br>';
		global $courseTest;
		return $courseTest->name_course;
	}
	function setCourseName(int $course_id, string $nCourseName){
		//cambiar luego con la logica de mysql
		echo "id del curso: $course_id y nombre: $nCourseName".'<br>';
		global $courseTest;
		$courseTest->name_course = $nCourseName;
		echo "nuevo nombre del curso: $courseTest->name_course".'<br>';
	}

	echo getCourseName(03);
	echo "<br>";
	setCourseName(03, "algoritmos");
	?>
</body>
</html>