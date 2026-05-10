<!DOCTYPE html>
<html>
<head>
    <title>prueba php</title>
</head>
<body>
	<?php
		//header("Access-Control-Allow-Origin: *");
		//header("Access-Control-Allow-Headers: *");
		include 'nuevapruebaIDEA.php';
		$DataArray = ['id_user' => 01,
	'email' => "algoMail",
	'password' => "algoPass",
	'name_user' => "jose",
	'lastname_user' => "algoLast",
	'id_professor' =>  01];

	$profTest = new Professor(...$DataArray);
	print_r($profTest);

	?>
</body>
</html>