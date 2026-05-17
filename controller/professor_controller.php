<!DOCTYPE html>
<html>
<head>
    <title>professor controller</title>
</head>
<body>
	<?php
	include 'professor_model.php';

	function createProf(int $prof_id,string $email, string $password, string $username, string $userLast){
		return new Professor($prof_id,$email,$password, $username, $userLast);
	}

	function getProfName(Professor $prof){
		return $prof->name_user;
	}

	function getProfEmail(Professor $prof){
		return $prof->email;
	}

	function getProfLName(Professor $prof){
		return $prof->lastname_user;
	}
	function getProfId(Professor $prof){
		return $prof->id_professor;
	}

	?>
</body>
</html>