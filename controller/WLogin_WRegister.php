<!DOCTYPE html>
<html>
<head>
    <title>assignment controller</title>
</head>
<body>
	<?php
	//use
	use PHPMailer\PHPMailer\PHPMailer;
	use PHPMailer\PHPMailer\Exception;
	//includes
	//agregar la funcion para conectarse a la base
	require 'vendor/autoload.php';
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
		$mail = new PHPMailer(true);
		try{
			//configuraciones cambiar las configuraciones despues :b
			$mail->isSMTP();
			$mail->Host = 'smtp.gmail.com';
			$mail->SMTPAuth = true;
			$mail->Username = 'arayacastilloj@gmail.com';
			$mail->Password = 'ejoe yccy pndu fjzo';
			$mail->SMTPSecure = PHPMailer::ENCRYPTION_STARTTLS;
			$mail->Port = 587;

			//recipients?
			$mail->setFrom('arayacastilloj@gmail.com','prueba de correo');
			$mail->addAddress($email);

			//contenido del correo
			$mail->isHTML(true);
			$mail->Subject = 'Prueba de enviar correo con PHP/'.phpversion();
			$mail->Body = 'Esto es una prueba del cuerpo del correo <b>bold text(nose xd)</b>';
			$mail->AltBody = 'algo para correos sin html';

			$mail->send();
			echo "se envio correo";
		} catch (Exception $e){
			echo "no se envio correo :(. error: {$mail->ErrorInfo}";
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

	?>
</body>
</html>