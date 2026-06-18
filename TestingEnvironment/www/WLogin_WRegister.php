<?php
//use // instalar composer y luego usar -> sudo composer require phpmailer/phpmailer
use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;
//includes
require_once 'vendor/autoload.php';
include_once 'professor_controller.php';
include_once 'db_controller.php';

//functions
function Login_web(string $email, string $password){
	$value = login_user_web($email, $password);
	if ($value) {
		createProf($value["prof_id"],$value["email"],$value["password"],$value["name"],$value["lastname"]);
		return $value;
	}else {
		return FALSE;
	}
}

function Login_desk(string $email, string $password){
	$value = login_user_desk($email, $password);
	if ($value) {
		return $value;
	}else {
		return FALSE;
	}
}

function Register_prof(string $email, string $password, string $username, string $userLast){
	if(!check_email($email)){
		create_prof($email, $password, $username, $userLast);
		Return TRUE;
	}else{
		Return FALSE;
	}
}

function Register_stud(string $email, string $password, string $username, string $userLast){
	if(!check_email($email)){
		create_stud($email, $password, $username, $userLast);
		Return TRUE;
	}else{
		Return FALSE;
	}
}

function RememberPassword(string $email){
	if(check_email($email)){
		$mail = new PHPMailer(true);
		try{
			$conn = create_db_conn();
			$result = $conn->query("SELECT password_user from User where email_user = \"$email\"");
			$row = $result->fetch_assoc();
			$user_pass = $row["password_user"]; 
			$conn->close();
			$mail->isSMTP();
			$mail->Host = 'smtp.gmail.com';
			$mail->SMTPAuth = true;
			$mail->Username = 'ideia12026@gmail.com';
			$mail->Password = 'nxkl gbcbmgci awbe';
			$mail->SMTPSecure = PHPMailer::ENCRYPTION_STARTTLS;
			$mail->Port = 587;
			$mail->setFrom('ideia12026@gmail.com','IDEIA-ACADEMIC');
			$mail->addAddress($email);
			$mail->isHTML(true);
			$mail->Subject = 'Remember Password';
			$mail->Body = "Your account password is: <b>$user_pass</b>";
			$mail->AltBody = 'Your account password is: '.$user_pass;
			$mail->send();
		} catch (Exception $e){
			echo "no se envio correo :(. error: {$mail->ErrorInfo}";
		}
	}else{
		Return FALSE;
	}
}
