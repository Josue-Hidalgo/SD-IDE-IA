<?php
include 'assignment_model.php';
include_once 'db_controller.php';
require 'vendor/autoload.php';

use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;

function create_assign(string $code_course, string $assign_name, string $desc, string $deadline, bool $is_allowed){
	$value = create_assignment($code_course, $assign_name, $desc, $deadline, $is_allowed);
	if($value){
		notify_students($code_course, $assign_name);
		return $value;
	}else{
		return FALSE;
	}
}

function get_all_assignment_by_course(string $code_course){
	return get_assignments_by_course($code_course);
}

function modify_assignment(string $assign_name, string $code_course, string $desc, string $deadline, bool $is_allowed){
	return modify_assign($assign_name, $code_course, $desc, $deadline, $is_allowed);
}

function notify_students(string $code_course, string $assign_title){//

	$students = get_all_stud_mail_in_course($code_course);

	$mail = new PHPMailer(true);
	try{
		$mail->isSMTP();
		$mail->Host = 'smtp.gmail.com';
		$mail->SMTPAuth = true;
		$mail->Username = 'ideia12026@gmail.com';
		$mail->Password = 'nxkl gbcbmgci awbe';
		$mail->SMTPSecure = PHPMailer::ENCRYPTION_STARTTLS;
		$mail->Port = 587;

		//recipients
		$mail->setFrom('ideia12026@gmail.com','IDEIA-ACADEMIC');
		foreach ($students as $stud) {

			$mail->addAddress($stud["email_user"]);
		}

		//contenido del correo
		$mail->isHTML(true);
		$mail->Subject = "New Assignment from course $code_course";
		$mail->Body = "The new assignment $assign_title is now available";
		$mail->AltBody = "The new assignment $assign_title is now available";

		$mail->send();
	} catch (Exception $e){
		echo "no se envio correo :(. error: {$mail->ErrorInfo}";
	}

}

function create_submission(int $id_stud, int $id_assign, string $project_name, string $project_data){
	return create_submit($id_stud, $id_assign, $project_name, $project_data);
}

function grade_submission(int $id_stud, int $id_assign, float $grade, string $feedback){
	return grade_submit($id_stud, $id_assign, $grade, $feedback);
}

function get_all_submissions_by_assignment(int $id_assign){
	return get_submit_by_assign($id_assign);
}

?>
