<?php
session_start();
?>

<?php
//use // instalar composer y luego usar -> sudo composer require phpmailer/phpmailer
use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;
//includes
//agregar la funcion para conectarse a la base
require 'vendor/autoload.php';
include 'professor_controller.php';
include 'db_controller.php';

//functions
function Login(string $email, string $password){
	//revisar en la base
	$value = login_user_web($email, $password);
	if ($value) {
		create_prof($value["prof_id"],$value["email"],$value["password"],$value["name"],$value["lastname"]);
	}else {
		return "The given email or password is incorrect.";//cambiar despues
	}
}

function Register(string $email, string $password, string $username, string $userLast){
	if(!check_email($email)){
		create_prof($email, $password, $username, $userLast);
	}else{
		echo "The email is already in use.";//cambiar despues
	}
	
}

function RememberPassword(string $email){
	//revisar el correo con la base
	if(check_email($email)){
		$mail = new PHPMailer(true);
		try{
			//configuraciones cambiar las configuraciones despues :b
			$conn = create_db_conn();
			$result = $conn->query("SELECT password_user from User where email_user = \"$email\"");
			$row = $result->fetch_assoc();
			$user_pass = $row["password_user"]; 
			$conn->close();
			$mail->isSMTP();
			$mail->Host = 'smtp.gmail.com';
			$mail->SMTPAuth = true;
			$mail->Username = 'arayacastilloj@gmail.com';//cambiar por correo de la app
			$mail->Password = 'ejoe yccy pndu fjzo';// contraseña app password de gmail
			$mail->SMTPSecure = PHPMailer::ENCRYPTION_STARTTLS;
			$mail->Port = 587;

			//recipients
			$mail->setFrom('arayacastilloj@gmail.com','prueba de correo');
			$mail->addAddress($email);

			//contenido del correo
			$mail->isHTML(true);
			$mail->Subject = 'Remember Password';
			$mail->Body = "Your account password is: <b>$user_pass</b>";
			$mail->AltBody = 'Your account password is: '.$user_pass;

			$mail->send();
		} catch (Exception $e){
			echo "no se envio correo :(. error: {$mail->ErrorInfo}";
		}
	}else{
		echo "email not found";//cambiar por un error correcto
	}
	
}
//Register("arayacastilloj@gmail.com", "correoprueba", "jose", "araya");
//Login("arayacastilloj@gmail.com", "correoprueba");
//RememberPassword("arayacastilloj@gmail.com");//true
//RememberPassword("fallo@fallo.com");//false

?>
