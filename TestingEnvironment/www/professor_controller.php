<?php
session_start();
?>
<?php
include 'professor_model.php';

function createProf(int $prof_id,string $email, string $password, string $username, string $userLast){
	$_SESSION['prof_id'] = $prof_id;
	$_SESSION['email'] = $email;
	$_SESSION['password'] = $password;
	$_SESSION['username'] = $username;
	$_SESSION['userLast'] = $userLast;
}

function getProfName(){
	return $_SESSION['username'];
}

function getProfEmail(){
	return $_SESSION['email'];
}

function getProfLName(){
	return $_SESSION['userLast'];
}
function getProfId(){
	return $_SESSION['prof_id'];
}


?>
