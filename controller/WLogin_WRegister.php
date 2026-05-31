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
function Login_web(string $email, string $password){
	//revisar en la base
	$value = login_user_web($email, $password);
	if ($value) {
		createProf($value["prof_id"],$value["email"],$value["password"],$value["name"],$value["lastname"]);
	}else {
		return FALSE;
	}
}

function Login_desk(string $email, string $password){
	//revisar en la base
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
			$mail->Username = 'ideia12026@gmail.com';//cambiar por correo de la app
			$mail->Password = 'nxkl gbcbmgci awbe';// contraseña app password de gmail
			$mail->SMTPSecure = PHPMailer::ENCRYPTION_STARTTLS;
			$mail->Port = 587;

			//recipients
			$mail->setFrom('ideia12026@gmail.com','IDEIA-ACADEMIC');
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
		Return FALSE;
	}
	
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
	$json = file_get_contents('php://input');
	$data = json_decode($json);
	header('Content-type: application/json; charset=utf-8');
	switch($data->action){
		case 'insert_student':
			$st_name = $data->username;
			$st_email =$data->email;
			$st_pass =$data->password;
			$st_last = $data->userLast;
			Register_stud($st_email, $st_pass, $st_name, $st_last);
			http_response_code(201);
			$responseData =[
				'success' => true,
				'message' => 'Date received successfully',
			];
			echo json_encode($responseData);
			break;
		case 'create_prof':
			$prof_name = $data->username;
			$prof_email =$data->email;
			$prof_pass =$data->password;
			$prof_last = $data->userLast;
			Register_prof($prof_email, $prof_pass, $prof_name, $prof_last);
			http_response_code(201);
			$responseData =[
				'success' => true,
				'message' => 'Date received successfully',
			];
			echo json_encode($responseData);
			break;
	}

}

if($_GET['action'] === 'log_student'){
	$email = $_GET['email'];
	$password = $_GET['password'];
	$data = Login_desk($email,$password);
	header('Content-type: application/json');
	echo json_encode($data);
}
if($_GET['action'] === 'log_prof'){
	$email = $_GET['email'];
	$password = $_GET['password'];
	$data = Login_web($email,$password);
}

?>
