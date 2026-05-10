<!DOCTYPE html>
<html>
<head>
    <title>professor controller</title>
</head>
<body>
	<?php
		include 'professor_model.php';
		$DataArray = ['id_user' => 01,'email' => "algoMail",// array para pruebas
	'password' => "algoPass",'name_user' => "jose",
	'lastname_user' => "algoLast",'id_professor' =>  01];

	$profTest = new Professor(...$DataArray);

	function getProfName(int $professor_id){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id".'<br>';
		global $profTest;
		return $profTest->name_user;
	}
	function setProfName(int $professor_id, string $nProfName){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id y nombre: $nProfName".'<br>';
		global $profTest;
		$profTest->name_user = $nProfName;
		echo "nuevo nombre del profesor: $profTest->name_user".'<br>';
	}

	function getProfEmail(int $professor_id){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id".'<br>';
		global $profTest;
		return $profTest->email;
	}
	function setProfEmail(int $professor_id, string $nProfEmail){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id y correo: $nProfEmail".'<br>';
		global $profTest;
		$profTest->email = $nProfEmail;
		echo "nuevo correo del profesor: $profTest->email".'<br>';
	}

	function getProfPass(int $professor_id){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id".'<br>';
		global $profTest;
		return $profTest->password;
	}
	function setProfPass(int $professor_id, string $nProfPass){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id y contrasena: $nProfPass".'<br>';
		global $profTest;
		$profTest->password = $nProfPass;
		echo "nuevo contrasena del profesor: $profTest->password".'<br>';
	}

	function getProfLName(int $professor_id){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id".'<br>';
		global $profTest;
		return $profTest->lastname_user;
	}
	function setProfLName(int $professor_id, string $nProfLName){
		//cambiar luego con la logica de mysql
		echo "id del professor: $professor_id y apellido: $nProfLName".'<br>';
		global $profTest;
		$profTest->lastname_user = $nProfLName;
		echo "nuevo apellido del profesor: $profTest->lastname_user".'<br>';
	}

	echo getProfName(01);
	echo "<br>";
	echo getProfEmail(01);
	echo "<br>";
	echo getProfPass(01);
	echo "<br>";
	echo getProfLName(01);
	echo "<br>";

	setProfName(01,"luis");
	setProfPass(01,"nueva_contra");
	setProfEmail(01,"nuevoalgo@fahhh.com");
	setProfLName(01,"fahhhhh");
	

	?>
</body>
</html>