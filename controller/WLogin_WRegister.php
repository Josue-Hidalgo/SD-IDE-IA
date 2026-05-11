<!DOCTYPE html>
<html>
<head>
    <title>assignment controller</title>
</head>
<body>
	<?php
	//includes
	//agregar la funcion para conectarse a la base
	include 'professor_controller.php';

	//functions
	function Login(string $email, string $password){
		//revisar en la base
		if ($email == "algode@prueba.com") {//puesto en crudo para probar, conectar a la base despues
			return $password == "contraprueba";
		}else {
			return "fallo";
		}
	}

	function Register(string $email, string $password, string $username, string $userLast){
		$nProf = createProf($email,$password, $username, $userLast);
		print_r($nProf);
		//agregar deteccion de errores al conectarlo a la base
	}

	function RememberPassword(string $email){
		//revisar el correo con la base
		if ($email == "arayacastilloj@gmail.com") {
			$subject = 'prueba de enviar correo';
			$message = 'aqui deberia de estar la contraseña del usuario';
			$headers = array(
			'From' => 'arayacastilloj@gmail.com', //es obligatorio el 'From' asi que agregar uno para el proyecto
			'Reply-To' => 'arayacastilloj@gmail.com',
			'X-Mailer' => 'PHP/'.phpversion()
		);
			mail($email, $subject, $message);
		}else{
			echo "correo no existente";
		}
	}

	//pruebas

	print_r(Login("algode@prueba.com", "contraprueba"));//true
	echo "\n";
	print_r(Login("algode@prueba.com", "fallo"));		//false
	echo "<br>";
	print_r(Login("algode@fallo.com", "contraprueba"));//false
	echo "<br>";

	Register("correoprueba@algo.com", "contrasenaPrueba", "pruebanombre", "otraPruebaApellido");
	RememberPassword("arayacastilloj@gmail.com");//true
	RememberPassword("fallo@fallo.com");//false

	?>
</body>
</html>